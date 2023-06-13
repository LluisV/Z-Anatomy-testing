using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class buttonFunctionalities : MonoBehaviour
{

    private List<string> bodyPartList;
    private string levelName;

    public void onButtonClick(GameObject buttonGameObject)
    {
        string buttonText = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        Debug.Log(buttonText);

        // If a button was previously selected, reset its color to its original color
        if (ReadCSVLevels.Instance.flag == 1)
        {
            ReadCSVLevels.Instance.lastSelectedButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }

        // Set the color of the current button to yellow
        Button currentButton = buttonGameObject.GetComponent<Button>();
        TextMeshProUGUI txtScript = currentButton.GetComponentInChildren<TextMeshProUGUI>();
        txtScript.color = Color.yellow;

        // Save a reference to the last selected button
        ReadCSVLevels.Instance.lastSelectedButton = currentButton;
        ReadCSVLevels.Instance.flag = 1;
    }

    public void onPlayButtonClick()
    {
        TextMeshProUGUI txtScript = ReadCSVLevels.Instance.lastSelectedButton.GetComponentInChildren<TextMeshProUGUI>();
        // Get the level data
        levelName = txtScript.text;
        bodyPartList = ReadCSVLevels.Instance.GetSelectedParts(levelName);

        // Convert the list to a JSON string (so we can send it as a string)
        string jsonString = JsonUtility.ToJson(bodyPartList);
        // Send the list
        PlayerPrefs.SetString("BodypartList", jsonString);
        // Send the level name
        PlayerPrefs.SetString("LevelName", levelName);


        // Load the "modelscene" scene
        SceneManager.LoadScene("ModelScene");
    }
}
