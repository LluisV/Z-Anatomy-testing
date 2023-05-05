using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.PackageManager;
using static UnityEngine.GraphicsBuffer;
using System.Linq;

public class LabelLevelManager : MonoBehaviour
{
    public static LabelLevelManager Instance;

    public GameObject tangibleBodyPart;
    public int deadline;
    public float hintTime;
    public Slider time_slider;
    public TextMeshProUGUI target_text;
    public TextMeshProUGUI errors_text;

    private List<LabelCollider> targets = new List<LabelCollider>();
    private LabelCollider current_target;
    private Coroutine target_change_coroutine;
    private int errors = 0;
    public Light hint_light;

    bool playing = false;

    private void Awake()
    {
        Instance = this;
        targets = tangibleBodyPart.GetComponentsInChildren<LabelCollider>().ToList();
    }

    private IEnumerator Start()
    {
        foreach (var target in targets)
            target.gameObject.SetActive(false);
        time_slider.maxValue = deadline;
        time_slider.value = deadline;
        target_text.text = "5";
        yield return new WaitForSeconds(1);
        target_text.text = "4";
        yield return new WaitForSeconds(1);
        target_text.text = "3";
        yield return new WaitForSeconds(1);
        target_text.text = "2";
        yield return new WaitForSeconds(1);
        target_text.text = "1";
        yield return new WaitForSeconds(1);
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

            // Select a target that is not the previous target
            var prev_target = current_target;
            if (targets.Count == 1)
                current_target = targets[0];
            else
            {
                do
                    current_target = targets[Random.Range(0, targets.Count)];
                while (current_target == prev_target);
            }

            current_target.gameObject.SetActive(true);
            target_text.text = current_target.gameObject.name;
            time_slider.value = deadline;
            target_change_coroutine = StartCoroutine(ChangeTargetPeriodically());
        }
        else
        {
            if (target_change_coroutine != null)
                StopCoroutine(target_change_coroutine);
            target_text.text = "Completed!";
            playing = false;
        }
    }

    private void HandleError()
    {
        errors++;
        errors_text.text = "Errors: " + errors;
    }

    private IEnumerator ChangeTargetPeriodically()
    {
        playing = true;
        yield return new WaitForSeconds(deadline);
        FindNewTarget();
    }

}
