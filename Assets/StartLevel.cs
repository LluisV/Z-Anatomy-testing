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
    SerializableList<string> bodyPartList = new SerializableList<string>();

    // Start is called before the first frame update
    public void StartLevelClick()
    {
        TextMeshProUGUI txtScript = ReadCSVLevels.Instance.lastSelectedButton.GetComponentInChildren<TextMeshProUGUI>();
        // Get the level data
        string levelName = txtScript.text;
        bodyPartList.list.Clear();
        foreach (var part in ReadCSVLevels.Instance.GetSelectedParts(levelName)) {
            bodyPartList.list.Add(part);
        }

        if(bodyPartList.list.Count > 0)
        {

            // Convert the list to a JSON string (so we can send it as a string)
            string jsonString = JsonUtility.ToJson(bodyPartList);
            // Send the list
            PlayerPrefs.SetString("BodypartList", jsonString);
            // Send the level name
            PlayerPrefs.SetString("LevelName", levelName);
            // Load the "modelscene" scene
            SceneManager.LoadScene("ModelScene");
        }
        else
        {
            // TODO: Tangible level
        }

    }

}
