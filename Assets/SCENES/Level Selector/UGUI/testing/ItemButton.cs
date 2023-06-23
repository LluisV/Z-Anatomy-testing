using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class ItemButton : MonoBehaviour, ISelectHandler, IPointerClickHandler, ISubmitHandler
{
    [SerializeField] private TMP_Text _itemName;

    [SerializeField] private ItemButtonEvent _onSelectEvent;
    [SerializeField] private ItemButtonEvent _onSubmitEvent;
    [SerializeField] private ItemButtonEvent _onClickEvent;

    public ItemButtonEvent OnSelectEvent { get => _onSelectEvent; set => _onSelectEvent = value; }
    public ItemButtonEvent OnSubmitEvent { get => _onSubmitEvent; set => _onSubmitEvent = value; }
    public ItemButtonEvent OnClickEvent { get => _onClickEvent; set => _onClickEvent = value; }
    public string ItemNameValue { get => _itemName.text; set => _itemName.text = value; }

    public void OnPointerClick(PointerEventData eventData) { _onClickEvent.Invoke(this); }
    public void OnSelect(BaseEventData eventData) { _onSelectEvent.Invoke(this); }
    public void OnSubmit(BaseEventData eventData) { _onSubmitEvent.Invoke(this); }

    public void ObtainSelectionFocus()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
        _onSelectEvent.Invoke(this);
    }
}

[System.Serializable]
public class ItemButtonEvent : UnityEvent<ItemButton> { }
