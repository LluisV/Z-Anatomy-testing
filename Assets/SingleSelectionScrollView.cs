using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleSelectionScrollView : MonoBehaviour
{
    [HideInInspector]
    public Button selectedButton;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color disabledColor = Color.gray;

    public void Select(Button btn)
    {
        if (btn.GetComponentInChildren<TextMeshProUGUI>().color == disabledColor)
            return;

        if (selectedButton != null)
            Deselect(selectedButton);

        selectedButton = btn;
        selectedButton.GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
    }

    public void Deselect(Button btn)
    {
        btn.GetComponentInChildren<TextMeshProUGUI>().color = normalColor;
    }
}
