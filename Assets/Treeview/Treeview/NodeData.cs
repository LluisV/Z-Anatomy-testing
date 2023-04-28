using System;

/// <summary>
/// The serializable part of Node.
/// </summary>
[Serializable]
public struct NodeData
{
    public int Id;
    public int ParentId;
    public int Level;
    public string Text;
    public string Description;
    public float Width;
    public float Height;
    public bool SizeApplied;

    public NodeData(Node node)
    {
        Id = node.Id;
        ParentId = node.Parent == null ? -1 : node.Parent.Id;
        Level = node.Level;
        Text = node.Text;
        Description = node.Description;
        Width = node.Width;
        Height = node.Height;
        SizeApplied = node.SizeApplied;
    }
}
