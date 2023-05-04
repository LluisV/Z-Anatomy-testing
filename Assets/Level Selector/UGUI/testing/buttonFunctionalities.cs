using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonFunctionalities : MonoBehaviour
{
    public void onButtonClick(GameObject b)
    {
        Debug.Log(b.GetComponentInChildren<Text>().text);
    }
}
