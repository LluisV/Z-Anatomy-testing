using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// An example of tree data management logic.
public static class NodeExtensions
{
    public static Node FindChild(this Node node, string text)
    {
        foreach (Node child in node.Children)
        {
            if (child.Text == text)
            {
                return child;
            }
        }
        return null;
    }
}

public class TreeviewDataExample : MonoBehaviour
{
    private const string treeviewComponentNotFound = "Treeview component not found.";
    private const string treeviewDisplayingByEditorDisabled = "Treeview displaying has been disabled in component \"Treeview\".";

    private Treeview treeview;

    /// <summary>
    /// Last event message.
    /// </summary>
    public Text Log;

    /// <summary>
    /// Gets the tree component, sets node event handlers, creates the root descendants.
    /// </summary>
    private void Awake()
    {
        if (!gameObject.TryGetComponent<Treeview>(out treeview))
        {
            Debug.LogError(treeviewComponentNotFound);
            return;
        }
        treeview.DisplayInGame = true;

        if (Log != null)
        {
            // Inherited by all descendants.
            treeview.Root.SelectHandler = new Node.NodeEventHandler((s, e) =>
                Log.text = $"Selected: {{Id: {e.Node.Id}, Text: \"{e.Node.Text}\"}}");
        }

        // Read data from CSV file
        string path = "Assets/Level Selector/UGUI/vertebralsheet.csv";
        List<string[]> data = new List<string[]>();
        using (StreamReader reader = new StreamReader(path))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');
                data.Add(values);
            }
        }

        // Create tree nodes from CSV data
        Node rootNode = treeview.Root;

        foreach (string[] values in data)
        {
            Node parentNode = rootNode;
            Node node = null;

            for (int i = 0; i < values.Length; i++)
            {
                node = parentNode.FindChild(values[i]);
                if (node == null)
                {
                    node = parentNode.AddChild(values[i]);
                }
                parentNode = node;
            }
        }
    }

    /// <summary>
    /// Displays the tree.
    /// </summary>
    private void OnGUI()
    {
        treeview.SaveDefaultButtonStyle();

        if (treeview == null)
        {
            Debug.LogError(treeviewComponentNotFound);
            return;
        }

        if (treeview.DisplayInGame)
        {
            Debug.Log(treeviewDisplayingByEditorDisabled);
            treeview.DisplayInGame = false;
        }

        treeview.X = (Screen.width - treeview.Width) / 2;

        GUILayout.BeginArea(treeview.BackgroundRect);

        treeview.Display();

        GUILayout.EndArea();
    }
}
