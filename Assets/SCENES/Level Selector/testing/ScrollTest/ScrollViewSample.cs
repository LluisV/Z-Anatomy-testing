using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ScrollViewSample : MonoBehaviour
{
    [ Serialized ] private RectTransform _content;
    [ Serialized ] private GameObject _prefabListItem;

    [ Space(10) ]
    [ Header ("Scroll View Events") ]

    [ Serialized ] private ItemButtonEvent _eventItemClicked;
    [ Serialized ] private ItemButtonEvent _eventItemOnSelect;
    [ Serialized ] private ItemButtonEvent _eventItemOnSubmit;

    [ Space(10) ]
    [ Header("Default Selcted Index") ]
    [ SerializeField ] private int _defaultSelectedIndex = 0;


    [ Space(10) ]
    [ Header("For Testing Only!") ]
    [ SerializeField ] private int _testButtonCount = 1;

    void Start()
    {
        if( _testButtonCount > 0)
        {
            TestCreateItems(_testButtonCount);
            UpdateAllButtonNavigationalReferences();
        }
        StartCorotuine( DelayedSelectChild(_defaultSelectedIndex));
    }

    public void SelectChild(int index)
    {
        int childCount = _content.transform.childCount;
        if (index >= childCount)
        {
            return; //Out of Range
        }

        GameObject childObject = _content.transform.GetChild(index).gameObject;
        ItemButton item = childObject.GetComponent<ItemButton>();
        item.ObtainSelectionFocus();
    }

    public IEnumerator DelayedSelectChild( int index)
    {
        yield return new WaitForSeconds(1f) ; //artifical delay
        SelectChild(index);
    }

    private void UpdateAllButtonNavigationalRefrences()
    {
        ItemButton[] children = _content.tranform.GetComponentsInChildren< ItemButton >();
        if( children.Length < 2)
        {
            return; //must have atleast 2 for navigation to work correctly
        }

        ItemButton item;
        Navigation navigation;

        for( int i=0; i< children.Length; i++)
        {
            item = children[i];
            navigation = item.gameObject.GetComponent<Button>().navigation;
            navigation.selectOnUp = GetNavigationUp(i, children.Length);
            navigation.selectOnDown = GetNavigationDown(i, children.Length);

            item.gameObject.GetComponent<Button>().navigation = navigation;
        }
    }


    private Selectable GetNavigationDown(int indexCurrent, int totalEntries)
    {
        ItemButton item;
        if (indexCurrent == totalEntries -1)
        {
            item = _content.transform.GetChild(0).GetComponent<ItemButton>();
        }
        else
        {
            item = _content.transform.GetChild(indexCurrent + 1).GetComponent<ItemButton>();
        }
        return item.GetComponent<Selectable>();
    }

    private Selectable GetNavigationUp( int indexCurrent, int totalEntries)
    {
        ItemButton item;
        if ( indexCurrent==0 ) {
            item = _content.transform.GetChild(totalEntries - 1).GetComponent<ItemButton>();
        }
        else
        {
            item = _content.transform.GetChild(indexCurrent - 1).GetComponent<ItemButton>();
        }

        return item.GetComponent < Selectable>(); 
    }

    private void TestCreateItems( int count)
    {
        for ( int i=0; i<count; i++)
        {
            CreateItem("Player_" + i);
        }
    }

    private ItemButton CreateItem( string strName)
    {
        GameObject gObj;
        ItemButton item;

        gObj = Instantiate(_prefabListItem, Vector3.zero, Quaternion.identity);
        gObj.transform.SetParent(_content.transform);                               //this is 1st
        gObj.transform.localScale = new Vector3(1f, 1f, 1f);
        gObj.transform.localPosition = new Vector3();
        gObj.transform.localRotation = Quaternion.Euler(new Vector3());
        gObj.name = strName;
        //Set parameters
        item                    = gObj.GetComponent<ItemButton>();
        item.ItemNameValue      = strName;

        item.OnSelectEvent.AddListener((ItemButton) => { HandleEventItemOnSelect(item); });
        item.OnClcikEvent.AddListener((ItemButton) => { HandleEventItemOnClick(item); });
        item.OnSubmitEvent.AddListener((ItemButton) => { HandleEventItemOnSubmit(item); });

        return item;
    }

    private void HandleEventItemOnSubmit( ItemButton item)
    {
        _eventItemOnSubmit.Invoke(item);
    }


    private void HandleEventItemOnClick(ItemButton item)
    {
        _eventItemClicked.Invoke(item);
    }

    private void HandleEventItemOnSelect(ItemButton item)
    {
        ScrollViewAutoScroll scrollViewAutoScroll = GetComponent<ScrollViewAutoScroll>();
        scrollViewAutoScroll.HandleOnSelectChange(item.gameObject);
        _eventItemOnSelect.Invoke(item);
    }
