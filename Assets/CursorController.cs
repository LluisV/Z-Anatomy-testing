using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D cursor;
    public Texture2D cursorClicked;

    private CursorControls controls;

    private void Awake()
    {
        controls = new CursorControls();
        ChangeCursor(cursor);
        //Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnEnable()
{
    controls.Enable();
}

private void OnDisable()
{
    controls.Disable();
}

private void Start()
{
    controls.Mouse.Click.started += _ => StartedClick();
    controls.Mouse.Click.performed += _ => EndedClick();
}

private void StartedClick()
{
    ChangeCursor(cursorClicked);
}

private void EndedClick()
{
    ChangeCursor(cursor);
}


private void ChangeCursor(Texture2D cursorType)
    {
        Cursor.SetCursor(cursorType, new Vector2(11, 4), CursorMode.Auto);
    }

}
