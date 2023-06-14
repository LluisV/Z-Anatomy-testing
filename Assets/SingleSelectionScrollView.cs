using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleSelectionScrollView : MonoBehaviour
{
    [HideInInspector]
    public Button selectedButton;


    public void Select(Button btn)
    {
        if (selectedButton != null)
            Deselect(btn);

        selectedButton = btn;
        selectedButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.blue;
    }

    public void Deselect(Button btn)
    {
        selectedButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }

}
