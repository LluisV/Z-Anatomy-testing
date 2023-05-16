using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class buttonFunctionalities : MonoBehaviour
{
    // Reference to the last selected button and its original color
    private Button lastSelectedButton;
    private Color lastSelectedButtonColor;

    public void onButtonClick(GameObject buttonGameObject)
    {
        string buttonText = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        Debug.Log(buttonText);

        // If a button was previously selected, reset its color to its original color
        if (lastSelectedButton != null)
        {
            lastSelectedButton.GetComponentInChildren<TextMeshProUGUI>().color = lastSelectedButtonColor;
        }

        // Set the color of the current button to yellow
        Button currentButton = buttonGameObject.GetComponent<Button>();
        lastSelectedButtonColor = currentButton.GetComponentInChildren<TextMeshProUGUI>().color;
        currentButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.yellow;

        // Store the selected button's text in PlayerPrefs
        PlayerPrefs.SetString("SelectedButton", buttonText);

        // Save a reference to the last selected button
        lastSelectedButton = currentButton;
    }

    public void onPlayButtonClick()
    {
        // Load the "modelscene" scene
        SceneManager.LoadScene("ModelScene");
    }
}
