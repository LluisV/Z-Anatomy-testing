using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.Rendering.DebugUI.Table;

public class ReadCSVLevels : MonoBehaviour
{
    enum System
    {
        Skeletal
    }

    public static ReadCSVLevels Instance;
    public TextAsset skeletonCSV;

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

    [HideInInspector]
    public int flag = 0;
    [HideInInspector]
    public Button lastSelectedButton;

    System currentSystem = System.Skeletal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        buttonParent.GetComponent<VerticalLayoutGroup>().spacing = verticalSpacing;
    }

    void Start()
    {
        CreateButtons();
    }

    private void CreateButtons()
    {

        List<List<string>> rows = GetRows();

        // For each row
        foreach (List<string> row in rows)
        {
            // For each column, create 
            for(int i = 0; i <  row.Count; i++)
            {
                // The first column is the title
                bool isTitle = i == 0;
                CreateButton(isTitle, row[i]);
            }

            // Add space between sections
            AddSpacing();
        }
    }

    private List<List<string>> GetRows()
    {
        TextAsset csv;

        // Select the csv corresponding to the current system
        switch (currentSystem)
        {
            case System.Skeletal:
                csv = skeletonCSV;
                break;
            default:
                csv = skeletonCSV;
                break;
        }

        // Get rows
        string csvText = csv.text;
        return ParseCSV(csvText);
    }

    // Reads the CSV as a string and returs a list bidimensional matrix of individual strings
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

    // Instantiates a button
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

    }

    // Adds a vertical space to the level list
    private void AddSpacing()
    {
        GameObject spacing = new GameObject();
        spacing.transform.parent = buttonParent.transform;
        spacing.AddComponent<RectTransform>().SetHeight(spaceBetweenSections);
    }


    // Get the list of the 
    public List<string> GetSelectedParts(string title)
    {
        List<List<string>> rows = GetRows();
        List<string> selectedParts = new List<string>();

        // Iterate over the rows
        for (int i = 0; i < rows.Count; i++)
        {
            // Check if the title column matches the given title
            if (rows[i].Count > 0 && rows[i][0] == title)
            {
                // Add the remaining columns to the selectedParts list
                for (int j = 1; j < rows[i].Count; j++)
                {
                    selectedParts.Add(rows[i][j]);
                }

                break; // Exit the loop since we found the matching title
            }
        }

        return selectedParts;
    }

}
