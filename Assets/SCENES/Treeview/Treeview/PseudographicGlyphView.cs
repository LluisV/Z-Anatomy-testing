using UnityEngine;

/// <summary>
/// The text character that represents the branch or the node.
/// </summary>
public class PseudographicGlyphView
{
    public string View;
    public GUIStyle Style;
    public GUILayoutOption[] Options;

    public PseudographicGlyphView(char view, GUIStyle style, GUILayoutOption[] options)
    {
        View = view.ToString();
        Style = style;
        Options = options;
    }
}
