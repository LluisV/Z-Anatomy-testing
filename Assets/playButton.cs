using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playButton : MonoBehaviour
{
    public buttonFunctionalities buttonFunc;

    public void onButtonClick()
    {
        buttonFunc.onPlayButtonClick();
    }
}
