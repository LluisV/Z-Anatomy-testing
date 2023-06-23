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

    void Start()
    {
        if (_testButtonCount > 0)
        {
            TestCreateItems(_testButtonCount);
            UpdateAllButtonNavigationalReferences();
        }
            LoadLevelOverviews();
        Debug.Log("loaded LoadLevelOverviews()");
            CreateButtons();
        
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
    /*
    public IEnumerator DelayedSelectChild(int index)
    {
        yield return new WaitForSeconds(1f); //artifical delay
        SelectChild(index);
    }
    */
    private void UpdateAllButtonNavigationalReferences()
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

    private ItemButton CreateItem(string strName)
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


    public static ScrollViewSample Instance;
    public TextAsset skeletonCSV;
    public TextAsset levelOverviewCSV;

    public GameObject contentGameObject;

    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private GameObject buttonParent;
    [Range(5, 15)]
    [SerializeField]
    private float titleTextSize = 10;
    [Range(5, 15)]
    [SerializeField]
    private float textSize = 10;
    [Range(0, 10)]
    [SerializeField]
    private float horizontalSpacing = 3;
    [Range(0, 10)]
    [SerializeField]
    private float verticalSpacing = 0;
    [Range(0, 15)]
    [SerializeField]
    private float spaceBetweenSections = 0;

    private Dictionary<string, string> levelOverviews = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        buttonParent.GetComponent<VerticalLayoutGroup>().spacing = verticalSpacing;
    }

  

    private void LoadLevelOverviews()
    {
        List<List<string>> rows = ParseCSV(levelOverviewCSV.text);

        foreach (List<string> row in rows)
        {
            if (row.Count >= 2)
            {
                string levelName = row[0];
                string levelOverview = row[1];
                levelOverviews[levelName] = levelOverview;
            }
        }
    }

    private void CreateButtons()
    {
        List<List<string>> rows = GetRows();

        // For each row
        foreach (List<string> row in rows)
        {
            // For each column, create 
            for (int i = 0; i < row.Count; i++)
            {
                // The first column is the title
                bool isTitle = i == 0;
                CreateItem(row[i]);
                //CreateButton(isTitle, row[i]);

            }

            // Add space between sections
           // AddSpacing();
        }
    }

    private List<List<string>> GetRows()
    {
        TextAsset csv = skeletonCSV;
        string csvText = csv.text;
        return ParseCSV(csvText);
    }

    // Reads the CSV as a string and returns a list of rows with individual strings
    private List<List<string>> ParseCSV(string csvText)
    {
        List<List<string>> rows = new List<List<string>>();
        StringReader reader = new StringReader(csvText);

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            List<string> row = new List<string>(line.Split(','));
            rows.Add(row);
        }

        return rows;
    }

    /* Instantiates a button
    private void CreateButton(bool isTitle, string text)
    {
        GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
        var textScript = newButton.GetComponentInChildren<TextMeshProUGUI>();
        textScript.text = text;

        if (isTitle)
        {
            textScript.fontStyle = FontStyles.Bold | FontStyles.Underline;
            textScript.fontSize = titleTextSize;
        }
        else
        {
            textScript.GetComponent<RectTransform>().offsetMin += new Vector2(horizontalSpacing, 0);
            textScript.fontSize = textSize;
        }

        // Adjust button height to fit the text
        RectTransform buttonRectTransform = newButton.GetComponent<RectTransform>();
        Vector2 txtSize = textScript.GetPreferredValues();
        buttonRectTransform.sizeDelta = new Vector2(buttonRectTransform.sizeDelta.x, txtSize.y);

        // Attach a click listener to the button
        Button buttonComponent = newButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnButtonClick(text));
        }
    }
    */
    public List<string> GetSelectedParts(string title)
    {
        List<List<string>> rows = GetRows();
        List<string> selectedParts = new List<string>();

        // Iterate over the rows
        foreach (List<string> row in rows)
        {
            // Check if the title column matches the given title
            if (row.Count > 0 && row[0] == title)
            {
                // Add the remaining columns to the selectedParts list
                for (int j = 1; j < row.Count; j++)
                {
                    selectedParts.Add(row[j]);
                }

                break; // Exit the loop since we found the matching title
            }
        }

        return selectedParts;
    }

    /* Adds a vertical space to the level list
    private void AddSpacing()
    {
        GameObject spacing = new GameObject();
        spacing.transform.parent = buttonParent.transform;
        spacing.AddComponent<RectTransform>().SetHeight(spaceBetweenSections);
    }
    */
    private void OnButtonClick(string levelName)
    {
        if (levelOverviews.TryGetValue(levelName, out string levelOverview))
        {
            TextMeshProUGUI contentText = contentGameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (contentText != null)
            {
                // Replace the special character with a comma
                levelOverview = levelOverview.Replace("⋅", ",");

                string[] lines = levelOverview.Split('.');
                string formattedContent = "\n• " + string.Join("\n• ", lines).Trim();
                contentText.text = formattedContent;
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found in the children of the assigned GameObject.");
            }
        }
    }


}
