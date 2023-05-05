/// <summary>
/// Defines the node returned by AddChild().
/// </summary>
public enum ReturnedNode
{
    /// <summary>
    /// The newly created node.<br/>
    /// Used in AddChild() to create a chain of generations.
    /// </summary>
    Created,

    /// <summary>
    /// The node that called AddChild().<br/>
    /// Used in AddChild() to increase the number of its children.
    /// </summary>
    Parent,

    /// <summary>
    /// The root node.<br/>
    /// Used in AddChild() to increase the number of root children.
    /// </summary>
    Root
}
