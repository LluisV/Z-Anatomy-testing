using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Script : MonoBehaviour    
{

    public UIDocument uiDoc;
    private Button playBtn;
    private Button leftArw;
    private Button rightArw;
    private Button backBtn;

    // Start is called before the first frame update
    void Start()
    {
        playBtn = uiDoc.rootVisualElement.Q<Button>("Play_btn");
        playBtn.RegisterCallback<ClickEvent>(playBtn_CE);
        playBtn.clicked += UIButton_Clicked;

        leftArw = uiDoc.rootVisualElement.Q<Button>("left_arrow");
        leftArw.RegisterCallback<ClickEvent>(leftArw_CE);
        leftArw.clicked += UIButton_Clicked;

        rightArw = uiDoc.rootVisualElement.Q<Button>("right_arrow");
        rightArw.RegisterCallback<ClickEvent>(rightArw_CE);
        rightArw.clicked += UIButton_Clicked;

        backBtn = uiDoc.rootVisualElement.Q<Button>("back_btn");
        backBtn.RegisterCallback<ClickEvent>(backBtn_CE);
        backBtn.clicked += UIButton_Clicked;


    }

    
    private void UIButton_Clicked()
    {
        Debug.Log("The Clicked Event Happened");
    }

    private void playBtn_CE(ClickEvent ClickEv)
    {
        Debug.Log("Play Button clicked");
    }
    private void leftArw_CE(ClickEvent ClickEv)
    {
        Debug.Log("Left Button clicked");
    }
    private void rightArw_CE(ClickEvent ClickEv)
    {
        Debug.Log("Right Button clicked");
    }
    private void backBtn_CE(ClickEvent ClickEv)
    {
        Debug.Log("Back Button clicked");
    }
    

}
