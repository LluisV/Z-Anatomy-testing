using System;
using UnityEngine;

/// <summary>
/// Variable texture for states "Normal", "Hover", "Active".
/// </summary>
[Serializable]
public class ETexture2D
{
    /// <summary>
    /// Used when the node is displayed normally.
    /// </summary>
    public Texture2D Normal;

    /// <summary>
    /// Used when the mouse is hovering over the node.
    /// </summary>
    public Texture2D Hover;

    /// <summary>
    /// Used when the node is pressed down.
    /// </summary>
    public Texture2D Active;

    public ETexture2D()
    {
    }
    
    public ETexture2D(Texture2D normal, Texture2D hover, Texture2D active)
    {
        Normal = normal;
        Hover = hover;
        Active = active;
    }
}
