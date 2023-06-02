using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TMPro;
using System.Collections.Generic;

public class TestingLoadCSVData : MonoBehaviour
{
    public TextAsset skeletonCSV;
    public TextAsset muscularCSV;
    public TextAsset nervousCSV;
    [SerializeField]
    private GameObject buttonPrefab;
    public static TestingLoadCSVData Instance;

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


    public TMP_Text tmpText; // Assign the TMP_Text field in the Inspector
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
        UpdateData();
    }

    public void UpdateData()
    {
        if (tmpText != null)
        {
            string selectedText = tmpText.text;
            TextAsset selectedCSV = GetSelectedCSV(selectedText);

            if (selectedCSV != null)
            {
                ClearButtons();

                string[,] grid = SplitCsvGrid(selectedCSV.text);
                string output = "";

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
                        if (output != "")
                        {
                            output += "\n";
                        }

                        GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                        newButton.GetComponentInChildren<TextMeshProUGUI>().text = category;
                        newButton.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold | FontStyles.Underline;

                        lastCategory = category;
                        lastSection = "";
                    }

                    if (!string.IsNullOrEmpty(section) && section != lastSection)
                    {
                        GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                        newButton.GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>().offsetMin += new Vector2(5, 0);
                        newButton.GetComponentInChildren<TextMeshProUGUI>().text = section;
                        newButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 12;

                        lastSection = section;
                    }

                    for (int j = 0; j < items.Count; j++)
                    {
                        GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                        newButton.GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>().offsetMin += new Vector2(5, 0);
                        newButton.GetComponentInChildren<TextMeshProUGUI>().text = items[j];
                        newButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 12;
                    }
                }
            }
            else
            {
                Debug.LogError("No CSV file found for the selected text: " + selectedText);
            }
        }
        else
        {
            Debug.LogError("TMP_Text field not assigned.");
        }
    }

    private TextAsset GetSelectedCSV(string selectedText)
    {
        switch (selectedText)
        {
            case "Skeleton":
                return skeletonCSV;
            case "Muscular":
                return muscularCSV;
            case "Nervous":
                return nervousCSV;
            case "Select System":
                return null; // No CSV file for the initial "Select System" text
            default:
                return null;
        }
    }
    private void ClearButtons()
    {
        // Destroy all existing buttons
        Button[] buttons = buttonParent.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            Destroy(button.gameObject);
        }
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