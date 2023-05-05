using System;

/// <summary>
/// Event arguments with the node it happened to.
/// </summary>
public class NodeEventArgs : EventArgs
{
    /// <summary>
    /// The node that the event happened to.
    /// </summary>
    public readonly Node Node;

    public NodeEventArgs(Node node)
    {
        this.Node = node;
    }
}