using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.PackageManager;
using static UnityEngine.GraphicsBuffer;
using System.Linq;
using UnityEditor;
using System;

public class TangibleLevelManager : MonoBehaviour
{
    public static TangibleLevelManager Instance;

    public GameObject container;
    public int deadline;
    public float hintTime;

    private Slider time_slider;
    private Dictionary<string, List<TangibleBodyPart>> groupedTargets = new Dictionary<string, List<TangibleBodyPart>>();
    private Tuple<string, List<TangibleBodyPart>> current_target;
    private int indexOfTarget = 0;
    private Coroutine target_change_coroutine;
    private int errors = 0;
    public Light hint_light;

    public GameObject cardPrefab;
    public Transform scrollView;

    private List<GameObject> cards = new List<GameObject>();

    bool playing = false;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        GlobalVariables.Instance.globalParent = container;
        GlobalVariables.Instance.GetScripts();
        GetTargets();
        yield return new WaitUntil(() => GlobalVariables.Instance.allBodyParts.All(it => !it.enabled || it.initialized));
        CameraController.instance.SetTarget(container);
        CameraController.instance.CenterImmediate();
        CenterCamera();

        yield return new WaitForSeconds(deadline);

        // Get first target
        FindNewTarget();
    }

    private void CenterCamera()
    {
        CameraController camScript = Camera.main.GetComponent<CameraController>();
        camScript.SetTarget(container);
        camScript.CenterImmediate();
    }

    private void GetTargets()
    {
        // For each target
        foreach (var collider in ModelScene.Instance.targets)
        {
            if (!groupedTargets.ContainsKey(collider.Key.RemoveSuffix()))
                groupedTargets[collider.Key.RemoveSuffix()] = new List<TangibleBodyPart>();

            // Click any of its childs will be correct, so add them all
            foreach (var script in collider.Value.GetComponentsInChildren<TangibleBodyPart>())
                groupedTargets[collider.Key.RemoveSuffix()].Add(script);
        }

        //Order by name
        groupedTargets.OrderBy(it => it.Key);

        //Instantiate a label card for each target
        foreach (var target in groupedTargets.Keys)
        {
            GameObject newCard = Instantiate(cardPrefab, scrollView);
            newCard.GetComponentInChildren<TextMeshProUGUI>().text = target;
            //newCard.GetComponentInChildren<Slider>().gameObject.SetActive(false);
            cards.Add(newCard);
        }

    }

    private void Update()
    {
        if (!playing)
            return;

        time_slider.value -= Time.deltaTime;

        //Show the hint light
        if (time_slider.value <= hintTime)
        {
            // Disabled
            return;
            //TODO: Show two lights if needed
            hint_light.transform.position = current_target.Item2[0].transform.position;
            float t = 1 - time_slider.value / hintTime; // 0 to 1
            float hintSpeed = 10f * t;
            float hintIntensity = 0.004f;
            float intensity = Mathf.Abs(Mathf.Sin(t * Mathf.PI * 2 * hintSpeed)) * hintIntensity;
            hint_light.intensity = intensity;
        }
        else
            hint_light.intensity = 0f;

        // If the slider reached 0
        if (time_slider.value <= 0f)
        {
            HandleError();
            FindNewTarget();
        }
        UpdateSliderColor();
    }

    private void UpdateSliderColor()
    {
        // Map the slider value to a color value between green and red
        float hue = Mathf.Lerp(0f, 120f, time_slider.value / deadline);
        Color color = Color.HSVToRGB(hue / 360f, 1f, 1f);
        time_slider.fillRect.GetComponent<Image>().color = color;
    }

    public void Clicked(TangibleBodyPart clicked)
    {
        if (IsCorrect(clicked, clicked.transform.parent))
        {
            //Get a new target
            FindNewTarget();
            time_slider.value = deadline;
        }
        else
            HandleError();
    }

    // This is a recursive function that checks if we have clicked the current target of his children
    private bool IsCorrect(TangibleBodyPart clicked, Transform parent)
    {
        // If we have clicked the current target
        if (clicked != null && clicked.name.RemoveSuffix() == current_target.Item1.RemoveSuffix())
            return true;

        // If it does not have a parent, then its null
        if (parent == null)
            return false;

        // Then we check if the parent is the current target
        if (clicked != null && parent.name.RemoveSuffix() == current_target.Item1.RemoveSuffix())
            return true;


        return IsCorrect(clicked, parent.parent);
    }

    private void FindNewTarget()
    {
        if(current_target != null)
            groupedTargets.Remove(current_target.Item1.RemoveSuffix());

        // If the game is not over yet
        if (groupedTargets.Count > 0)
        {
            if (target_change_coroutine != null)
                StopCoroutine(target_change_coroutine);

            // Rotate the cards (TODO: MAKE AN ANIMATION)
            if (indexOfTarget != 0)
                cards[indexOfTarget - 1].transform.SetSiblingIndex(cards.Count - 1);

            // The target is the top one
            current_target = Tuple.Create(groupedTargets.First().Key, groupedTargets.First().Value);
            // Get the slider of the target's card
            time_slider = cards[indexOfTarget].GetComponentInChildren<Slider>();
            indexOfTarget++;

            target_change_coroutine = StartCoroutine(ChangeTargetPeriodically());
        }
        else
        {
            if (target_change_coroutine != null)
                StopCoroutine(target_change_coroutine);
            playing = false;
        }

        // Set slider values
        time_slider.maxValue = deadline;
        time_slider.value = deadline;
    }

    private void HandleError()
    {
        errors++;
    }

    private IEnumerator ChangeTargetPeriodically()
    {
        playing = true;
        yield return new WaitForSeconds(deadline);
        FindNewTarget();
    }
}
