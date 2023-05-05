using System;
using UnityEngine;

/// <summary>
/// Variable glyph for states "Normal", "Hover", "Active".
/// </summary>
[Serializable]
public class EGlyph
{
    #region Pseudographic line
    [Header("Pseudographic line")]
    public char Collapsed = '►';
    public char Expanded = '▼';
    public char Empty = '■';
    public char Last = '└';
    public char NotLast = '├';
    public char ParentIsLast = ' ';
    public char ParentIsNotLast = '│';
    public Font Font;
    public int FontSize = 24;
    public int Width = 16;
    public int Height = 20;
    public RectOffset Margin = new RectOffset();
    public RectOffset Padding = new RectOffset();
    #endregion

    #region Colors
    [Header("Colors")]
    public EColor CollapsedEColor;
    public EColor ExpandedEColor;
    public EColor EmptyEColor;
    public Color LastColor;
    public Color NotLastColor;
    public Color ParentIsLastColor;
    public Color ParentIsNotLastColor;
    #endregion

    #region Textures
    [Header("Textures")]
    public ETexture2D CollapsedETexture2D;
    public ETexture2D ExpandedETexture2D;
    public ETexture2D EmptyETexture2D;
    public Texture2D LastTexture2D;
    public Texture2D NotLastTexture2D;
    public Texture2D ParentIsLastTexture2D;
    public Texture2D ParentIsNotLastTexture2D;
    #endregion

    public EGlyph()
    {
    }

    public EGlyph(Color common)
    {
        CollapsedEColor = new EColor(common);
        ExpandedEColor = new EColor(common);
        EmptyEColor = new EColor(common);
        LastColor = common;
        NotLastColor = common;
        ParentIsLastColor = common;
        ParentIsNotLastColor = common;
    }

    public EGlyph(Color normal, Color hover, Color active)
    {
        CollapsedEColor = new EColor(normal, hover, active);
        ExpandedEColor = new EColor(normal, hover, active);
        EmptyEColor = new EColor(normal, hover, active);
        LastColor = normal;
        NotLastColor = normal;
        ParentIsLastColor = normal;
        ParentIsNotLastColor = normal;
    }
}
