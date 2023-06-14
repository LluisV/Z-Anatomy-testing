using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleSelectionItem : MonoBehaviour
{
    public void Select()
    {
        SingleSelectionScrollView scrollView = GetComponentInParent<SingleSelectionScrollView>();
        scrollView.Select(this.GetComponent<Button>());
    }
}
