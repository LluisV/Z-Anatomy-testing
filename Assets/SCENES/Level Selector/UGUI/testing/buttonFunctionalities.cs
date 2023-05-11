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
            // Load the "modelscene" scene
            SceneManager.LoadScene("ModelScene");
        }
        else
        {
            Debug.Log(buttonText);
        }
    }
}
