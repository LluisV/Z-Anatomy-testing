using System;
using UnityEngine;

/// <summary>
/// Variable color for states "Normal", "Hover", "Active".
/// </summary>
[Serializable]
public class EColor
{
    /// <summary>
    /// Used when the node is displayed normally.
    /// </summary>
    public Color Normal;

    /// <summary>
    /// Used when the mouse is hovering over the node.
    /// </summary>
    public Color Hover;

    /// <summary>
    /// Used when the node is pressed down.
    /// </summary>
    public Color Active;

    public EColor()
    {
    }

    public EColor(Color common)
    {
        Normal = common;
        Hover = common;
        Active = common;
    }

    public EColor(Color normal, Color hover, Color active)
    {
        Normal = normal;
        Hover = hover;
        Active = active;
    }
}
