using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
    public class buttonFunctionalities : MonoBehaviour
    {
        
        public void onButtonClick(GameObject buttonGameObject)
        {
            string buttonText = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
            Debug.Log(buttonText);

            // If a button was previously selected, reset its color to its original color
            if (TestingLoadCSVData.Instance.flag == 1)
            {
                TestingLoadCSVData.Instance.lastSelectedButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }

            // Set the color of the current button to yellow
            Button currentButton = buttonGameObject.GetComponent<Button>();

            currentButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.yellow;

            // Store the selected button's text in PlayerPrefs
            PlayerPrefs.SetString("SelectedButton", buttonText);

            // Save a reference to the last selected button
            TestingLoadCSVData.Instance.lastSelectedButton = currentButton;
            TestingLoadCSVData.Instance.flag = 1;

        }

        public void onPlayButtonClick()
        {
            // Load the "modelscene" scene
            SceneManager.LoadScene("ModelScene");
        }
    }