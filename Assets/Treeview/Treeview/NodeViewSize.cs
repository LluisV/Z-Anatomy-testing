/// <summary>
/// Specifies the size of the node view.
/// </summary>
public enum NodeViewSize
{
    /// <summary>
    /// Occupies all the space between the glyph string and the right edge of the tree container.<br/>
    /// Example:<br/><br/>
    /// <code>
    /// |  └■[   lorem ipsum   ]|
    /// |   └■[      dolor     ]|
    /// </code>
    /// </summary>
    Stretch,

    /// <summary>
    /// Has a width calculated from the length of the word.<br/>
    /// Example:<br/><br/>
    /// <code>
    /// |  └■[lorem ipsum]      |
    /// |   └■[dolor]           |
    /// </code>
    /// </summary>
    FitText,

    /// <summary>
    /// Has the size set by the node's Width and Height parameters.
    /// </summary>
    Individual,

    /// <summary>
    /// Has the node texture size associated to the "Normal" state.
    /// </summary>
    NormalTexture,

    /// <summary>
    /// Has the node texture size associated to the "Hover" state.
    /// </summary>
    HoverTexture,

    /// <summary>
    /// Has the node texture size associated to the "Active" state.
    /// </summary>
    ActiveTexture
}
