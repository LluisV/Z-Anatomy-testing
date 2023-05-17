using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TMPro;
using System.Collections.Generic;


    public class TestingLoadCSVData : MonoBehaviour
    {
        public TextAsset csvFile; // Assign the CSV file to this variable in the Inspector

        // Assign the Text field to this variable in the Inspector

        public static TestingLoadCSVData Instance;
        public GameObject buttonPrefab;
        public GameObject buttonParent;

        public int flag = 0;
        public Button lastSelectedButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    void Start()
        {


            string[,] grid = SplitCsvGrid(csvFile.text);
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
                    newButton.GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>().offsetMin += new Vector2(8, 0);
                    newButton.GetComponentInChildren<TextMeshProUGUI>().text = items[j];
                    newButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 12;
                    
                }
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
