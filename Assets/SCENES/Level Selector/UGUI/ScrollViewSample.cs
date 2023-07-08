using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Text;
using System;
using CsvHelper;

public class ScrollViewSample : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private GameObject _prefabListItem;

    [Space(10)]
    [Header("Scroll View Events")]

    [SerializeField] private ItemButtonEvent _eventItemClicked;
    [SerializeField] private ItemButtonEvent _eventItemOnSelect;
    [SerializeField] private ItemButtonEvent _eventItemOnSubmit;

    [Space(10)]
    [Header("Default Selcted Index")]
    [SerializeField] private int _defaultSelectedIndex = 0;


    [Space(10)]
    [Header("For Testing Only!")]
    [SerializeField] private int _testButtonCount = 1;



    public static ScrollViewSample Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
    }


    void Start()
    {
        if (_testButtonCount > 0)
        {
            TestCreateItems(_testButtonCount);
            UpdateAllButtonNavigationalReferences();
        }
        ReadCSVLevels.Instance.LoadLevelOverviews();
        Debug.Log("loaded LoadLevelOverviews()");
        ReadCSVLevels.Instance.CreateButtons();

        //StartCoroutine(DelayedSelectChild(_defaultSelectedIndex));
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

    public void UpdateAllButtonNavigationalReferences()
    {
        ItemButton[] children = _content.transform.GetComponentsInChildren<ItemButton>();
        if (children.Length < 2)
        {
            return; //must have atleast 2 for navigation to work correctly
        }

        ItemButton item;
        Navigation navigation;

        for (int i = 0; i < children.Length; i++)
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
        if (indexCurrent == totalEntries - 1)
        {
            item = _content.transform.GetChild(0).GetComponent<ItemButton>();
        }
        else
        {
            item = _content.transform.GetChild(indexCurrent + 1).GetComponent<ItemButton>();
        }
        return item.GetComponent<Selectable>();
    }

    private Selectable GetNavigationUp(int indexCurrent, int totalEntries)
    {
        ItemButton item;
        if (indexCurrent == 0)
        {
            item = _content.transform.GetChild(totalEntries - 1).GetComponent<ItemButton>();
        }
        else
        {
            item = _content.transform.GetChild(indexCurrent - 1).GetComponent<ItemButton>();
        }

        return item.GetComponent<Selectable>();
    }

    private void TestCreateItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateItem("Player_" + i);
        }
    }

    public ItemButton CreateItem(string strName)
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
        item = gObj.GetComponent<ItemButton>();
        item.ItemNameValue = strName;

        item.OnSelectEvent.AddListener((ItemButton) => { HandleEventItemOnSelect(item); });
        item.OnClickEvent.AddListener((ItemButton) => { HandleEventItemOnClick(item); });
        item.OnSubmitEvent.AddListener((ItemButton) => { HandleEventItemOnSubmit(item); });

        return item;
    }

    public ItemButton CreateItemHeading(string strName)
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
        item = gObj.GetComponent<ItemButton>();
        item.ItemNameValue = strName;
        gObj.GetComponentInChildren<TextMeshProUGUI>().fontSize = 15;
        item.OnSelectEvent.AddListener((ItemButton) => { HandleEventItemOnSelect(item); });
        item.OnClickEvent.AddListener((ItemButton) => { HandleEventItemOnClick(item); });
        item.OnSubmitEvent.AddListener((ItemButton) => { HandleEventItemOnSubmit(item); });
        
        return item;
    }

    private void HandleEventItemOnSubmit(ItemButton item)
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


    //////////////// 

}