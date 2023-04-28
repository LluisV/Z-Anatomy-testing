using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Tree node model. Contains the minimum of data and methods that are required to realize hierarchical relationships.
/// </summary>
public class Node
{
    public delegate void NodeEventHandler(object sender, NodeEventArgs args);

    public const int TextMinLength = 1;
    public const int TextMaxLength = 30;
    public const int DescriptionMaxLength = 50;

    /// <summary>
    /// The generation number starting at 0.<br/>
    /// <code>
    /// root - 0
    /// root.child - 1
    /// root.child.child - 2
    /// </code>
    /// </summary>
    public int Level;

    public int Id;
    public string Text;
    public string Description;
    public Treeview Treeview;
    public Node Parent;
    public List<Node> Children = new List<Node>();
    public NodeEventHandler SelectHandler;
    public float Width = 100;
    public float Height = 20;
    public bool SizeApplied = false;
    public bool IsExpanded = true;
    public bool IsParent => Children.Any();
    public bool IsRoot => Parent == null;
    public bool IsSelected => this == Treeview.SelectedNode;

    #region Specialized constructors
    /// <summary>
    /// Creates a node during deserialization.
    /// </summary>
    private Node(int id, Treeview treeview, int level = 0)
    {
        Id = id;
        Treeview = treeview;
        Level = level;
    }

    /// <summary>
    /// Creates a root with Id 1.
    /// </summary>
    public Node(string text, Treeview treeview)
    {
        Treeview = treeview;
        Id = System.Threading.Interlocked.Exchange(ref Treeview.LastNodeId, 1);
        Level = 0;
        Text = text;
    }

    /// <summary>
    /// Creates a child node.
    /// </summary>
    /// <param name="parent">The node that will be the parent of the new node and from which the new node will inherit the event handler.</param>
    public Node(string text, Node parent)
    {
        Treeview = parent.Treeview;
        Id = System.Threading.Interlocked.Increment(ref Treeview.LastNodeId);
        Text = text;
        Parent = parent;
        Level = parent.Level + 1;
        SelectHandler = parent.SelectHandler;
    }

    /// <summary>
    /// Creates a new node during deserialization.<br/>
    /// After creating Treeview.LastNodeId becomes equal to the Id of the created node.
    /// </summary>
    public Node(NodeData nodeData, Treeview treeview)
    {
        Id = nodeData.Id;
        Level = nodeData.Level;
        Text = nodeData.Text;
        Description = nodeData.Description;
        Width = nodeData.Width;
        Height = nodeData.Height;
        SizeApplied = nodeData.SizeApplied;
        Treeview = treeview;
        Treeview.LastNodeId = System.Threading.Interlocked.Exchange(ref Treeview.LastNodeId, Id);

        if (nodeData.ParentId > 0)
        {
            Parent = new Node(nodeData.ParentId, treeview);
        }
    }
    #endregion

    /// <summary>
    /// Draws the node and all its descendants.
    /// </summary>
    public void Display()
    {
        GUILayout.BeginHorizontal();
        
        this.GenGlyphString();
        bool selected = this.Display_IsSelected();

        GUILayout.EndHorizontal();
        GUILayout.Space(Treeview.LevelDistance);

        if (selected)
        {
            Treeview.SelectedNode = this;
            SelectHandler?.Invoke(this, new NodeEventArgs(Treeview.SelectedNode));
        }

        if (IsParent && IsExpanded)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Display();
            }
        }
    }

    /// <summary>
    /// Adds a new node.<br/>
    /// The whole tree can be created by a sequence of calls to this method.<br/>
    /// The next method in the sequence will be called for the node defined by the second argument.<br/>
    /// The first method of the sequence is called for the root node, which always exists in the tree (the smallest tree).
    /// </summary>
    /// <param name="returnedNode">Defines the node to be returned.</param>
    /// <returns>Root, parent (default) or newly created. Defined via returnedNode.</returns>
    public Node AddChild(string text, ReturnedNode returnedNode = ReturnedNode.Parent)
    {
        Node child = new Node(text, this);
        this.Children.Add(child);

        switch (returnedNode)
        {
            case ReturnedNode.Created:
                return child;
            case ReturnedNode.Root:
                return Treeview.Root;
            case ReturnedNode.Parent:
            default:
                return this;
        }
    }
}
