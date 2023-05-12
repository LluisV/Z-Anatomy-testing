using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class buttonFunctionalities : MonoBehaviour
{
    public void onButtonClick(GameObject b)
    {
        string buttonText = b.GetComponentInChildren<Text>().text;
        if (buttonText == "APPENDICULAR SKELETON")
        {
            // Change the color of the button text to yellow
            b.GetComponentInChildren<Text>().color = Color.yellow;
        }
        else
        {
            Debug.Log(buttonText);
        }
    }

    public void onPlayButtonClick()
    {
        // Load the "modelscene" scene
        SceneManager.LoadScene("ModelScene");
    }

}
