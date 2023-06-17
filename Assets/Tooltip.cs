using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string message;
    //=======================================For GameObjects==================
    /*private void OnMouseEnter()
    {
        TooltipManager._instance.SetandShowToolTip(message);
    }

    private void OnMouseExit()
    {
        TooltipManager._instance.HideToolTip();
    }*/
    //=======================================For Buttons=========================
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        TooltipManager._instance.SetandShowToolTip(message);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        TooltipManager._instance.HideToolTip();
    }
}
