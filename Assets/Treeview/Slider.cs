using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows examples of trees.
/// </summary>
public class Slider : MonoBehaviour
{
    /// <summary>
    /// Game objects with Treeview and TreeviewExample components.
    /// </summary>
    public List<GameObject> treeviews;

    private int index = 0;
    private int maxIndex = 0;
    private Button nextButton;
    private Button previousButton;
    private Text treeviewName;

    /// <summary>
    /// Last event message.
    /// </summary>
    private Text log;

    /// <summary>
    /// Initializes all components and shows the first tree.
    /// </summary>
    private void Awake()
    {
        Debug.ClearDeveloperConsole();

        treeviewName = gameObject.transform.Find("DemoName").GetComponent<Text>();
        treeviewName.text = treeviews.Any() ? treeviews[0].name : "";

        previousButton = gameObject.transform.Find("Previous").GetComponent<Button>();
        previousButton.onClick.AddListener(PreviousButtonClick);

        nextButton = gameObject.transform.Find("Next").GetComponent<Button>();
        nextButton.onClick.AddListener(NextButtonClick);
        
        log = gameObject.transform.Find("Log").GetComponent<Text>();
        log.text = "";
        
        maxIndex = treeviews.Count - 1;
        
        if (treeviews.Count > 1)
        {
            for (int i = 1; i < treeviews.Count; i++)
            {
                treeviews[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Shows the next tree, displays its name and clears the log.
    /// </summary>
    private void NextButtonClick()
    {
        if (!treeviews.Any())
        {
            return;
        }

        log.text = "";
        treeviews[index].SetActive(false);
        index = index < maxIndex ? index + 1 : 0;
        treeviews[index].SetActive(true);
        treeviewName.text = treeviews[index].name;
    }

    /// <summary>
    /// Shows the previous tree, displays its name and clears the log.
    /// </summary>
    private void PreviousButtonClick()
    {
        if (!treeviews.Any())
        {
            return;
        }

        log.text = "";
        treeviews[index].SetActive(false);
        index = index == 0 ? maxIndex : index - 1;
        treeviews[index].SetActive(true);
        treeviewName.text = treeviews[index].name;
    }
}
