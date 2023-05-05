using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using TMPro;
using System.Collections.Generic;

public class TestingLoadCSVData : MonoBehaviour
{
    public TextAsset csvFile; // Assign the CSV file to this variable in the Inspector
    public TextMeshProUGUI outputText;
    // Assign the Text field to this variable in the Inspector

    //public int categoryFontSize = 24;
    //public int sectionFontSize = 20;
    //public int itemsFontSize = 18;
    public GameObject buttonPrefab;
    public GameObject buttonParent;
    
    

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
                newButton.GetComponentInChildren<Text>().text = category;
                output = "\n\n";

                lastCategory = category;
                lastSection = "";
            }

           

            if (!string.IsNullOrEmpty(section) && section != lastSection)

            {
                
                GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                newButton.GetComponentInChildren<Text>().text = section;
                output =  "\n\n";
                lastSection = section;
            }

            for (int j = 0; j < items.Count; j++)
            {
                
                GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
                newButton.GetComponentInChildren<Text>().text = items[j];
              
            }
        }

        outputText.text = output;
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


