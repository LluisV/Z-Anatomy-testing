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

public class LabelLevelManager : MonoBehaviour
{
    public static LabelLevelManager Instance;

    public GameObject tangibleBodyPart;
    public int deadline;
    public float hintTime;
    public Transform container;

    private Slider time_slider;
    private Dictionary<string, List<LabelCollider>> groupedTargets = new Dictionary<string, List<LabelCollider>>();
    private List<LabelCollider> current_target;
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
        string bodypart = PlayerPrefs.GetString("LevelName");
        GameObject go = Resources.Load<GameObject>("Prefabs/Label Prefabs/" + bodypart);

        // It is null, let's try adding .l
        if (go == null)
            go = Resources.Load<GameObject>("Prefabs/Label Prefabs/" + bodypart + ".l");
        // It is null, let's try adding .r
        if (go == null)
            go = Resources.Load<GameObject>("Prefabs/Label Prefabs/" + bodypart + ".r");
        // It is null
        if (go == null)
            throw new Exception("Something went wrong loading " + bodypart + ". Check the names");

        GameObject model = Instantiate(go, container.transform, true);
        model.name = go.gameObject.name;

        //Destroy all its childrens (labels, lines, etc)
        /*foreach (Transform child in model.transform)
        {
            if (!child.gameObject.IsLabel() && !child.gameObject.IsLine())
                Destroy(child.gameObject);
        }*/

        tangibleBodyPart = model.GetComponent<TangibleBodyPart>().gameObject;

        // Get all colliders
        var allColliders = tangibleBodyPart.GetComponentsInChildren<LabelCollider>();
        // Group them by name
        foreach (var collider in allColliders)
        {
            if(!groupedTargets.ContainsKey(collider.name.RemoveSuffix()))
                groupedTargets[collider.name.RemoveSuffix()] = new List<LabelCollider>();
            groupedTargets[collider.name.RemoveSuffix()].Add(collider);
        }

        //Order by name
        groupedTargets.OrderBy(it => it.Key);

        //Instantiate a label card for each target
        foreach (var target in groupedTargets)
        {
            GameObject newCard = Instantiate(cardPrefab, scrollView);
            newCard.GetComponentInChildren<TextMeshProUGUI>().text = target.Value[0].name.RemoveSuffix();
            //newCard.GetComponentInChildren<Slider>().gameObject.SetActive(false);
            cards.Add(newCard);
        }
        
    }

    private IEnumerator Start()
    {
        GlobalVariables.Instance.globalParent = tangibleBodyPart;
        GlobalVariables.Instance.GetScripts();
        yield return new WaitUntil(() => tangibleBodyPart.GetComponent<TangibleBodyPart>().initialized);
        CameraController.instance.SetTarget(tangibleBodyPart);
        CameraController.instance.CenterImmediate();
        // Disable all the label meshes
        foreach (var target in groupedTargets)
        {
            foreach (var collider in target.Value)
                collider.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(deadline);

        // Disable all the labels
        foreach (var target in groupedTargets)
        {
            foreach (var collider in target.Value)
            {
                collider.label.Hide();
                collider.gameObject.SetActive(false);
            }
        }

        // Get first target
        FindNewTarget();
    }

    private void Update()
    {
        if (!playing)
            return;

        time_slider.value -= Time.deltaTime;

        //Show the hint light
        if (time_slider.value <= hintTime)
        {
            //TODO: Show two lights if needed
            hint_light.transform.position = current_target[0].transform.position + current_target[0].label.line.outDir * 0.055f;
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

    public void Clicked(LabelCollider target)
    {
        // If correct
        if (target != null && target.name.RemoveSuffix() == current_target[0].name.RemoveSuffix())
        {
            //Show its label
            current_target[0].label.Show();
            //Get a new target
            FindNewTarget();
            time_slider.value = deadline;
        }
        else
            HandleError();     
    }

    private void FindNewTarget()
    {
        if (current_target != null)
        {
            // Hide the current target
            foreach (var target in current_target)
                target.gameObject.SetActive(false);
            //Remove it from the list of targets
            groupedTargets.Remove(current_target[0].name.RemoveSuffix());
        }

        // If the game is not over yet
        if (groupedTargets.Count > 0)
        {
            if (target_change_coroutine != null)
                StopCoroutine(target_change_coroutine);

            // Rotate the cards (TODO: MAKE AN ANIMATION)
            if(indexOfTarget != 0)
                cards[indexOfTarget-1].transform.SetSiblingIndex(cards.Count - 1);
            // The target is the top one
            current_target = groupedTargets.First().Value;
            // Get the slider of the target's card
            time_slider = cards[indexOfTarget].GetComponentInChildren<Slider>();
            indexOfTarget++;

            // Show the current target
            foreach (var target in current_target)
                target.gameObject.SetActive(true);

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
