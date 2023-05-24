using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TMP_TextEventTrigger : MonoBehaviour
{
    private TMP_Text tmpText;
    private string previousText;

    private void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        previousText = tmpText.text;
    }

    private void Update()
    {
        if (tmpText.text != previousText)
        {
            TestingLoadCSVData.Instance.UpdateData();
            previousText = tmpText.text;
        }
    }
}
