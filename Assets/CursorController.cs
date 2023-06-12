using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D cursor;
    public Texture2D cursorClickedL;
    public Texture2D cursorClickedM;
    public Texture2D cursorClickedR;

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
        controls.Mouse.LClick.started   += _ => StartedClickL();
        controls.Mouse.MClick.started   += _ => StartedClickM();
        controls.Mouse.RClick.started   += _ => StartedClickR();
        controls.Mouse.LClick.performed += _ => EndedClick();
        controls.Mouse.MClick.performed += _ => EndedClick();
        controls.Mouse.RClick.performed += _ => EndedClick();
    }

    private void StartedClickL()
    {
        ChangeCursor(cursorClickedL);
    }

    private void StartedClickM()
    {
        ChangeCursor(cursorClickedM);
    }

    private void StartedClickR()
    {
        ChangeCursor(cursorClickedR);
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
