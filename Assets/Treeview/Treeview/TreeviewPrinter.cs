using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Tree drawing tools.
/// </summary>
public static class TreeviewPrinter
{
    #region Displaying messages
    /// <summary>
    /// Displays a tree event message.
    /// </summary>
    public static void DisplayMessage(string text, Color textColor)
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = textColor;
        style.fontSize = 24;
        style.fontStyle = FontStyle.Bold;

        GUILayout.Label(text, style);
    }

    /// <summary>
    /// Displays a tree warning message.
    /// </summary>
    public static void DisplayWarningMessage(string text)
    {
        DisplayMessage(text, new Color(1, 0.6f, 0, 1));
    }
    
    /// <summary>
    /// Displays a tree error message.
    /// </summary>
    public static void DisplayErrorMessage(string text)
    {
        DisplayMessage(text, Color.red);
    }
    #endregion

    /// <summary>
    /// Sets the name of the texture used in the tree.<br/>
    /// The name has a special prefix to count all the textures in the tree.
    /// </summary>
    public static void SetName(this Texture2D background, string name)
    {
        background.name = Treeview.NamePrefix + name;
    }

    /// <summary>
    /// Returns the tuned GUI style.
    /// </summary>
    /// <returns>The tuned GUI style.</returns>
    public static GUIStyle TunedCopy(this GUIStyle style)
    {
        GUIStyle tunedCopy = new GUIStyle(style);
        tunedCopy.margin.left = 0;
        tunedCopy.margin.right = 0;

        return tunedCopy;
    }

    /// <summary>
    /// Calculates the width of a word given the size and font.
    /// </summary>
    /// <returns>The width of a word given the size and font.</returns>
    public static GUILayoutOption GUIWidth(this string text, Font font, int fontSize)
    {
        CharacterInfo ci;
        int textLength = 0;
        text = string.Concat(text, "  ");

        for (int i = 0; i < text.Length; i++)
        {
            font.RequestCharactersInTexture(text, fontSize);
            font.GetCharacterInfo(text[i], out ci, fontSize);
            textLength += ci.advance;
        }

        return GUILayout.MaxWidth(textLength);
    }
    
    #region Inspector
    private static GUIStyle headerStyle;
    private static GUIStyle HeaderStyle
    {
        get
        {
            if (headerStyle == null)
            {
                headerStyle = GUI.skin.label.TunedCopy();
                headerStyle.fontStyle = FontStyle.Bold;
            }

            return headerStyle;
        }
    }

    /// <summary>
    /// Displays a label with HeaderStyle.
    /// </summary>
    public static void InspectorHeader(string text, int spaceTop = 15)
    {
        GUILayout.Space(spaceTop);
        GUILayout.Label(text, HeaderStyle);
    }

    /// <summary>
    /// Displays a button and returns true when the button is clicked.
    /// </summary>
    /// <returns>true when the button is clicked.</returns>
    public static bool InspectorButton(this Treeview treeview, string text)
    {
        return GUILayout.Button(text, treeview.DefaultButtonStyle, GUILayout.MaxWidth(150));
    }

    /// <summary>
    /// Displays the button to add a new child node to the selected node.<br/>
    /// Adds a new child node to the selected node and returns true when the button is clicked.
    /// </summary>
    /// <returns>true when the button is clicked.</returns>
    public static bool InspectorButton_AddChild(this Treeview treeview, string text)
    {
        bool result = treeview.SelectedNode != null && treeview.InspectorButton(text);

        if (result)
        {
            Node child = new Node("ID", treeview.SelectedNode);
            child.Text += child.Id.ToString("D3");
            treeview.SelectedNode.Children.Add(child);
        }

        return result;
    }

    /// <summary>
    /// Displays the button to empty the selected node of its descendants.<br/>
    /// Deletes all descendants of the selected node and returns true when the button is clicked.
    /// </summary>
    /// <returns>true when the button is clicked.</returns>
    public static bool InspectorButton_Clear(this Treeview treeview, string text)
    {
        bool result = treeview.SelectedNode != null
            && treeview.SelectedNode.Children.Any()
            && treeview.InspectorButton(text);

        if (result)
        {
            treeview.SelectedNode.Children.Clear();
            treeview.SelectedNode.IsExpanded = false;
        }

        return result;
    }

    /// <summary>
    /// Displays the button to delete the selected node.<br/>
    /// Deletes the selected node with all its descendants and returns true when the button is clicked.
    /// </summary>
    /// <returns>true when the button is clicked.</returns>
    public static bool InspectorButton_Delete(this Treeview treeview, string text)
    {
        bool result = treeview.SelectedNode != null
            && treeview.SelectedNode != treeview.Root
            && treeview.InspectorButton(text);

        if (result)
        {
            treeview.SelectedNode.Parent.Children.Remove(treeview.SelectedNode);
            treeview.SelectedNode = null;
        }

        return result;
    }

    /// <summary>
    /// Displays the button to move the selected child node up.<br/>
    /// Moves the node up and returns true when the button is clicked.
    /// </summary>
    /// <returns>true when the button is clicked.</returns>
    public static bool InspectorButton_MoveUp(this Treeview treeview, string text)
    {
        bool result = treeview.SelectedNode != null
            && treeview.SelectedNode != treeview.Root
            && treeview.SelectedNode.Parent.Children.Count > 1
            && treeview.InspectorButton(text);

        if (result)
        {
            Node movingNode = treeview.SelectedNode;
            int oldIndex = treeview.SelectedNode.Parent.Children.IndexOf(treeview.SelectedNode);
            int newIndex = -1 + (oldIndex == 0 ? treeview.SelectedNode.Parent.Children.Count : oldIndex);

            treeview.SelectedNode.Parent.Children.Remove(movingNode);
            treeview.SelectedNode.Parent.Children.Insert(newIndex, movingNode);
            treeview.SelectedNode = movingNode;
        }

        return result;
    }

    /// <summary>
    /// Displays the button to move the selected child node down.<br/>
    /// Moves the node down and returns true when the button is clicked.
    /// </summary>
    /// <returns>true when the button is clicked.</returns>
    public static bool InspectorButton_MoveDown(this Treeview treeview, string text)
    {
        bool result = treeview.SelectedNode != null
            && treeview.SelectedNode != treeview.Root
            && treeview.SelectedNode.Parent.Children.Count > 1
            && treeview.InspectorButton(text);

        if (result)
        {
            Node movingNode = treeview.SelectedNode;
            int oldIndex = treeview.SelectedNode.Parent.Children.IndexOf(treeview.SelectedNode);
            int newIndex = oldIndex == treeview.SelectedNode.Parent.Children.Count - 1 ? 0 : (oldIndex + 1);

            treeview.SelectedNode.Parent.Children.Remove(movingNode);
            treeview.SelectedNode.Parent.Children.Insert(newIndex, movingNode);
            treeview.SelectedNode = movingNode;
        }
        
        return result;
    }
    #endregion

    /// <summary>
    /// Displays the node as a button and returns true if it is selected, i.e. the button has been pressed.<br/>
    /// The style of the button corresponds to the state of the node.
    /// </summary>
    /// <param name="node">The node to display.</param>
    /// <returns>true if the node is selected, i.e. the button has been pressed.</returns>
    public static bool Display_IsSelected(this Node node)
    {
        GUILayoutOption[] options = null;
        GUIStyle style = new GUIStyle();
        ENodeView env = node.IsSelected ? node.Treeview.SelectedENodeView : node.Treeview.NormalENodeView;
        
        style.normal.textColor = env.EColor.Normal;
        style.hover.textColor = env.EColor.Hover;
        style.active.textColor = env.EColor.Active;
        style.normal.background = env.ETexture2D.Normal == null ? node.Treeview.DefaultBackground : env.ETexture2D.Normal;
        style.hover.background = env.ETexture2D.Hover == null ? node.Treeview.DefaultBackground : env.ETexture2D.Hover;
        style.active.background = env.ETexture2D.Active == null ? node.Treeview.DefaultBackground : env.ETexture2D.Active;
        style.margin = node.Treeview.NodeMargin;
        style.padding = node.Treeview.NodePadding;
        style.font = node.Treeview.NodeFont;
        style.fontSize = node.Treeview.NodeFontSize;
        style.alignment = node.Treeview.NodeTextAnchor;

        switch (env.Size)
        {
            case NodeViewSize.Stretch:
                options = new GUILayoutOption[1];
                options[0] = GUILayout.MaxWidth(node.Treeview.Width);
                break;

            case NodeViewSize.FitText:
                options = new GUILayoutOption[1];
                options[0] = node.Text.GUIWidth(style.font, style.fontSize);
                //options[0] = GUILayout.ExpandWidth(true);
                break;

            case NodeViewSize.Individual:
                options = new GUILayoutOption[2];
                options[0] = GUILayout.MaxWidth(node.Width);
                options[1] = GUILayout.MaxHeight(node.Height);
                break;

            case NodeViewSize.NormalTexture:
                options = new GUILayoutOption[2];
                options[0] = GUILayout.MaxWidth(style.normal.background.width);
                options[1] = GUILayout.MaxHeight(style.normal.background.height);
                break;

            case NodeViewSize.HoverTexture:
                options = new GUILayoutOption[2];
                options[0] = GUILayout.MaxWidth(style.hover.background.width);
                options[1] = GUILayout.MaxHeight(style.hover.background.height);
                break;

            case NodeViewSize.ActiveTexture:
                options = new GUILayoutOption[2];
                options[0] = GUILayout.MaxWidth(style.active.background.width);
                options[1] = GUILayout.MaxHeight(style.active.background.height);
                break;
        }

        if (node.SizeApplied)
        {
            options = new GUILayoutOption[2];
            options[0] = GUILayout.MaxWidth(node.Width);
            options[1] = GUILayout.MaxHeight(node.Height);
        }

        return GUILayout.Button(node.Text, style, options);
    }

    /// <summary>
    /// Generates a string of glyphs.<br/>
    /// The last glyph of the sequence represents the node.<br/>
    /// Example:<br/><br/>
    /// <code>
    /// ││├■
    /// </code>
    /// </summary>
    /// <param name="node">The node in front of which the glyph sequence will be drawn.</param>
    public static void GenGlyphString(this Node node)
    {
        EGlyph eg = node.IsSelected ? node.Treeview.SelectedEGlyph : node.Treeview.NormalEGlyph;
        
        if (eg.Font == null)
        {
            if (node.Treeview.NodeFont == null)
            {
                return;
            }

            eg.Font = node.Treeview.NodeFont;
        }

        #region Node type symbol in pseudographic line
        GUIStyle nodeTypeGlyphStyle = new GUIStyle();
        nodeTypeGlyphStyle.font = eg.Font;
        nodeTypeGlyphStyle.fontSize = eg.FontSize;
        nodeTypeGlyphStyle.margin = eg.Margin;
        nodeTypeGlyphStyle.padding = eg.Padding;

        GUILayoutOption[] nodeTypeGlyphOptions = { GUILayout.MaxWidth(eg.Width), GUILayout.MaxHeight(eg.Height) };

        char nodeTypeGlyph;

        if (!node.Children.Any())
        {
            nodeTypeGlyph = eg.Empty;
            nodeTypeGlyphStyle.normal.background = eg.EmptyETexture2D.Normal == null ? node.Treeview.DefaultBackground : eg.EmptyETexture2D.Normal;
            nodeTypeGlyphStyle.hover.background = eg.EmptyETexture2D.Hover == null ? node.Treeview.DefaultBackground : eg.EmptyETexture2D.Hover;
            nodeTypeGlyphStyle.active.background = eg.EmptyETexture2D.Active == null ? node.Treeview.DefaultBackground : eg.EmptyETexture2D.Active;
            nodeTypeGlyphStyle.normal.textColor = eg.EmptyEColor.Normal;
            nodeTypeGlyphStyle.hover.textColor = eg.EmptyEColor.Hover;
            nodeTypeGlyphStyle.active.textColor = eg.EmptyEColor.Active;
        }
        else if (node.IsExpanded)
        {
            nodeTypeGlyph = eg.Expanded;
            nodeTypeGlyphStyle.normal.background = eg.ExpandedETexture2D.Normal == null ? node.Treeview.DefaultBackground : eg.ExpandedETexture2D.Normal;
            nodeTypeGlyphStyle.hover.background = eg.ExpandedETexture2D.Hover == null ? node.Treeview.DefaultBackground : eg.ExpandedETexture2D.Hover;
            nodeTypeGlyphStyle.active.background = eg.ExpandedETexture2D.Active == null ? node.Treeview.DefaultBackground : eg.ExpandedETexture2D.Active;
            nodeTypeGlyphStyle.normal.textColor = eg.ExpandedEColor.Normal;
            nodeTypeGlyphStyle.hover.textColor = eg.ExpandedEColor.Hover;
            nodeTypeGlyphStyle.active.textColor = eg.ExpandedEColor.Active;
        }
        else
        {
            nodeTypeGlyph = eg.Collapsed;
            nodeTypeGlyphStyle.normal.background = eg.CollapsedETexture2D.Normal == null ? node.Treeview.DefaultBackground : eg.CollapsedETexture2D.Normal;
            nodeTypeGlyphStyle.hover.background = eg.CollapsedETexture2D.Hover == null ? node.Treeview.DefaultBackground : eg.CollapsedETexture2D.Hover;
            nodeTypeGlyphStyle.active.background = eg.CollapsedETexture2D.Active == null ? node.Treeview.DefaultBackground : eg.CollapsedETexture2D.Active;
            nodeTypeGlyphStyle.normal.textColor = eg.CollapsedEColor.Normal;
            nodeTypeGlyphStyle.hover.textColor = eg.CollapsedEColor.Hover;
            nodeTypeGlyphStyle.active.textColor = eg.CollapsedEColor.Active;
        }
        #endregion

        #region Branch in pseudographic line
        char lastGlyph = eg.Last;
        GUILayoutOption[] lastOptions = { GUILayout.MaxWidth(eg.Width), GUILayout.MaxHeight(eg.Height) };
        GUIStyle lastStyle = new GUIStyle();
        lastStyle.font = eg.Font;
        lastStyle.fontSize = eg.FontSize;
        lastStyle.margin = eg.Margin;
        lastStyle.padding = eg.Padding;
        lastStyle.normal.background = eg.LastTexture2D == null ? node.Treeview.DefaultBackground : eg.LastTexture2D;
        lastStyle.normal.textColor = eg.LastColor;

        char notLastGlyph = eg.NotLast;
        GUIStyle notLastStyle = new GUIStyle();
        GUILayoutOption[] notLastOptions = { GUILayout.MaxWidth(eg.Width), GUILayout.MaxHeight(eg.Height) };
        notLastStyle.font = eg.Font;
        notLastStyle.fontSize = eg.FontSize;
        notLastStyle.margin = eg.Margin;
        notLastStyle.padding = eg.Padding;
        notLastStyle.normal.background = eg.NotLastTexture2D == null ? node.Treeview.DefaultBackground : eg.NotLastTexture2D;
        notLastStyle.normal.textColor = eg.NotLastColor;

        char parentIsLastGlyph = eg.ParentIsLast;
        GUIStyle parentIsLastStyle = new GUIStyle();
        GUILayoutOption[] parentIsLastOptions = { GUILayout.MaxWidth(eg.Width), GUILayout.MaxHeight(eg.Height) };
        parentIsLastStyle.font = eg.Font;
        parentIsLastStyle.fontSize = eg.FontSize;
        parentIsLastStyle.margin = eg.Margin;
        parentIsLastStyle.padding = eg.Padding;
        parentIsLastStyle.normal.background = eg.ParentIsLastTexture2D == null ? node.Treeview.DefaultBackground : eg.ParentIsLastTexture2D;
        parentIsLastStyle.normal.textColor = eg.ParentIsLastColor;

        char parentIsNotLastGlyph = eg.ParentIsNotLast;
        GUIStyle parentIsNotLastStyle = new GUIStyle();
        GUILayoutOption[] parentIsNotLastOptions = { GUILayout.MaxWidth(eg.Width), GUILayout.MaxHeight(eg.Height) };
        parentIsNotLastStyle.font = eg.Font;
        parentIsNotLastStyle.fontSize = eg.FontSize;
        parentIsNotLastStyle.margin = eg.Margin;
        parentIsNotLastStyle.padding = eg.Padding;
        parentIsNotLastStyle.normal.background = eg.ParentIsNotLastTexture2D == null ? node.Treeview.DefaultBackground : eg.ParentIsNotLastTexture2D;
        parentIsNotLastStyle.normal.textColor = eg.ParentIsNotLastColor;
        #endregion

        #region Display pseudographic line
        List<PseudographicGlyphView> reversePseudographicLine = new List<PseudographicGlyphView>();

        if (node.Level > 0)
        {
            if (node.Parent.Children.Count == 1 || node.Parent.Children.LastOrDefault() == node)
            {
                reversePseudographicLine.Add(new PseudographicGlyphView(lastGlyph, lastStyle, lastOptions));
            }
            else
            {
                reversePseudographicLine.Add(new PseudographicGlyphView(notLastGlyph, notLastStyle, notLastOptions));
            }
        }

        Node parent = node.Parent;

        while (node.Level > 1 && parent != null && parent.Level > 0)
        {
            if (parent.Parent.Children.Count == 1
                || parent.Parent.Children.LastOrDefault() == parent)
            {
                reversePseudographicLine.Add(new PseudographicGlyphView(parentIsLastGlyph, parentIsLastStyle, parentIsLastOptions));
            }
            else
            {
                reversePseudographicLine.Add(new PseudographicGlyphView(parentIsNotLastGlyph, parentIsNotLastStyle, parentIsNotLastOptions));
            }

            parent = parent.Parent;
        }

        for (int i = reversePseudographicLine.Count - 1; i >= 0; i--)
        {
            GUILayout.Label(reversePseudographicLine[i].View, reversePseudographicLine[i].Style, reversePseudographicLine[i].Options);
        }

        if (GUILayout.Button(nodeTypeGlyph.ToString(), nodeTypeGlyphStyle, nodeTypeGlyphOptions))
        {
            if (node.Children.Any())
            {
                node.IsExpanded = !node.IsExpanded;
            }
        }
        #endregion
    }
}
