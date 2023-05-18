using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TMPro;
using System.Collections.Generic;


public class TestingLoadCSVData : MonoBehaviour
{
    public static TestingLoadCSVData Instance;

    [SerializeField]
    private TextAsset csvFile; // Assign the CSV file to this variable in the Inspector
    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private GameObject buttonParent;
    [Range(5, 15)]
    [SerializeField]
    private float textSize = 10;
    [Range(5, 15)]
    [SerializeField]
    private float categoryTextSize = 15;
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

        string[,] grid = SplitCsvGrid(csvFile.text);

        string lastCategory = "";
        string lastSection = "";
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            string category = grid[i, 0];
            string section = grid[i, 1];
            List<string> items = new List<string>();
            for (int j = 2; j < grid.GetLength(1); j++)
            {
                if (!string.IsNullOrEmpty(grid[i, j]))
                {
                    items.Add(grid[i, j]);
                }
            }

            if (category != lastCategory)
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                newButton.GetComponent<RectTransform>().SetHeight(categoryTextSize);
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = category;
                newButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = categoryTextSize;
                newButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold | FontStyles.Underline;

                lastCategory = category;
                lastSection = "";
            }



            if (!string.IsNullOrEmpty(section) && section != lastSection)
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                newButton.GetComponent<RectTransform>().SetHeight(textSize);
                newButton.GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>().offsetMin += new Vector2(5, 0);
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = section;
                newButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = textSize;

                lastSection = section;
            }

            for (int j = 0; j < items.Count; j++)
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                newButton.GetComponent<RectTransform>().SetHeight(textSize);
                newButton.GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>().offsetMin += new Vector2(5 + horizontalSpacing, 0);
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = items[j];
                newButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = textSize;  
            }

            if (!string.IsNullOrEmpty(section))
                AddSpacing();
        }
    }

    private void AddSpacing()
    {
        GameObject newSpacing = new GameObject();
        newSpacing.name = "Spacing";
        newSpacing.transform.SetParent(buttonParent.transform);
        newSpacing.AddComponent<RectTransform>().SetHeight(spaceBetweenSections);
    }

    static public string[,] SplitCsvGrid(string csvText)
    {
        string[] lines = csvText.Split("\n"[0]);

        int numRows = lines.Length;

        // Determine the number of columns in the CSV sheet
        int numCols = 0;
        for (int i = 0; i < numRows; i++)
        {
            string[] line = lines[i].Split(',');
            numCols = Mathf.Max(numCols, line.Length);
        }

        string[,] grid = new string[numRows, numCols];
        for (int r = 0; r < numRows; r++)
        {
            string[] line = lines[r].Split(',');
            for (int c = 0; c < numCols; c++)
            {
                if (c < line.Length)
                {
                    grid[r, c] = line[c];
                }
                else
                {
                    grid[r, c] = "";
                }
            }
        }
        return grid;
    }
}
