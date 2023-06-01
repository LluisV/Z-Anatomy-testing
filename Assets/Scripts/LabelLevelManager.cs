using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.PackageManager;
using static UnityEngine.GraphicsBuffer;
using System.Linq;
using UnityEditor;

public class LabelLevelManager : MonoBehaviour
{
    public static LabelLevelManager Instance;

    public GameObject tangibleBodyPart;
    public int deadline;
    public float hintTime;

    private Slider time_slider;
    private List<LabelCollider> targets = new List<LabelCollider>();
    private LabelCollider current_target;
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
        targets = tangibleBodyPart.GetComponentsInChildren<LabelCollider>().ToList();
        //Instantiate a label card for each target
        foreach (var target in targets)
        {
            GameObject newCard = Instantiate(cardPrefab, scrollView);
            newCard.GetComponentInChildren<TextMeshProUGUI>().text = target.name;
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
        foreach (var target in targets)
            target.gameObject.SetActive(false);
        yield return new WaitForSeconds(deadline);
        foreach (var target in targets)
        {
            target.label.Hide();
            target.gameObject.SetActive(false);
        }
        FindNewTarget();
    }

    private void Update()
    {
        if (!playing)
            return;

        time_slider.value -= Time.deltaTime;

        if (time_slider.value <= hintTime)
        {
            hint_light.transform.position = current_target.transform.position + current_target.label.line.outDir * 0.055f;
            float t = 1 - time_slider.value / hintTime; // 0 to 1
            float hintSpeed = 10f * t;
            float hintIntensity = 0.004f;
            float intensity = Mathf.Abs(Mathf.Sin(t * Mathf.PI * 2 * hintSpeed)) * hintIntensity;
            hint_light.intensity = intensity;
        }
        else
            hint_light.intensity = 0f;


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
        if (target != null && target == current_target)
        {
            current_target.label.Show();
            targets.Remove(target);
            FindNewTarget();
            time_slider.value = deadline;
        }
        else
            HandleError();     
    }

    private void FindNewTarget()
    {
        if(current_target != null)
            current_target.gameObject.SetActive(false);
        if (targets.Count > 0)
        {
            if (target_change_coroutine != null)
                StopCoroutine(target_change_coroutine);

            if(indexOfTarget != 0)
                cards[indexOfTarget-1].transform.SetSiblingIndex(cards.Count - 1);
            current_target = targets[0];
            time_slider = cards[indexOfTarget].GetComponentInChildren<Slider>();
            indexOfTarget++;

            current_target.gameObject.SetActive(true);
            target_change_coroutine = StartCoroutine(ChangeTargetPeriodically());
        }
        else
        {
            if (target_change_coroutine != null)
                StopCoroutine(target_change_coroutine);
            playing = false;
        }

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
