using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SerializableList<T>
{
    public List<T> list = new List<T>();
}


public class StartLevel : MonoBehaviour
{
    private SerializableList<string> bodyPartList = new SerializableList<string>();
    private SingleSelectionScrollView scrollView;
    [HideInInspector]
    private string selectedLevelName;

    private void Awake()
    {
        scrollView = FindObjectOfType<SingleSelectionScrollView>();
    }

    // Start is called before the first frame update
    public void StartLevelClick()
    {
        selectedLevelName = scrollView.selectedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        // Get the level data
        bodyPartList.list.Clear();
        foreach (var part in ReadCSVLevels.Instance.GetSelectedParts(selectedLevelName)) {
            bodyPartList.list.Add(part);
        }

        if(bodyPartList.list.Count > 0)
        {

            // Convert the list to a JSON string (so we can send it as a string)
            string jsonString = JsonUtility.ToJson(bodyPartList);
            // Send the list
            PlayerPrefs.SetString("BodypartList", jsonString);
            // Send the level name
            PlayerPrefs.SetString("LevelName", selectedLevelName);
            // Load the "modelscene" scene
            SceneManager.LoadScene("ModelScene");
        }
        else
        {
            PlayerPrefs.SetString("LevelName", selectedLevelName);
            SceneManager.LoadScene("Label level");
        }
    }
}
