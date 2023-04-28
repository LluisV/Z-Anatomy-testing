using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Tree model and component.
/// </summary>
[Serializable]
public class Treeview : MonoBehaviour//, ISerializationCallbackReceiver
{
    /// <summary>
    /// The prefix of entity names in the context of the tree.<br/>
    /// Used to count all entities of the tree.
    /// </summary>
    public const string NamePrefix = "Treeview";

    #region Inspector buttons style
    private GUIStyle defaultButtonStyle;

    /// <summary>
    /// Used to prevent the custom style of buttons in the tree from being applied to control buttons in the inspector.
    /// </summary>
    public GUIStyle DefaultButtonStyle
    {
        get
        {
            if (defaultButtonStyle == null)
            {
                throw new Exception("DefaultButtonStyle must be set by calling SaveDefaultButtonStyle() in On...GUI(), not within Treeview.cs.");
            }

            return defaultButtonStyle;
        }
    }

    /// <summary>
    /// If true, the custom button style in the tree will not be applied to buttons in the inspector.
    /// </summary>
    public bool SavedDefaultButtonStyle => defaultButtonStyle != null;

    /// <summary>
    /// Retains the default style for inspector buttons.<br/>
    /// Called only in On...GUI() before the tree is drawn.
    /// </summary>
    public void SaveDefaultButtonStyle()
    {
        if (defaultButtonStyle == null)
        {
            defaultButtonStyle = GUI.skin.button.TunedCopy();
        }
    }
    #endregion

    #region Background
    private Texture2D background;
    private Texture2D defaultBackground;
    private GUIStyle backgroundStyle;
    private Rect backgroundRect = new Rect();

    public Texture2D Background
    {
        get
        {
            if (background == null)
            {
                background = new Texture2D(1, 1);
                background.SetName("Background");
            }

            background.SetPixel(1, 1, BackgroundColor);
            background.Apply();

            return background;
        }
    }
    public Texture2D DefaultBackground
    {
        get
        {
            if (defaultBackground == null)
            {
                defaultBackground = new Texture2D(1, 1, TextureFormat.Alpha8, false);
                defaultBackground.SetPixel(1, 1, new Color32(0, 0, 0, 0));
                defaultBackground.Apply();
                defaultBackground.SetName("DefaultBackground");
            }

            return defaultBackground;
        }
    }
    public GUIStyle BackgroundStyle
    {
        get
        {
            if (backgroundStyle == null)
            {
                backgroundStyle = new GUIStyle();
            }

            backgroundStyle.padding = Padding;
            backgroundStyle.normal.background = Background;

            return backgroundStyle;
        }
    }
    public Rect BackgroundRect
    {
        get
        {
            backgroundRect.x = 50;
            backgroundRect.y = Y;
            backgroundRect.width = Width;
            backgroundRect.height = Height;

            return backgroundRect;
        }
    }

    /// <summary>
    /// Destroys all textures of the component.
    /// </summary>
    public void DestroyTextures()
    {
        Destroy(background);
        Destroy(defaultBackground);

        Debug.Log("Treeview textures destroyed.");
    }
    #endregion

    #region Nodes
    private Node root;
    public Node Root
    {
        get
        {
            if (root == null)
            {
                root = new Node("Select Your Level", this);
            }

            return root;
        }
    }
    public Node SelectedNode;
    #endregion

    #region Controls
    [Header("Display")]
    public bool DisplayInInspector = false;
    public bool DisplayInGame = true;
    public bool DisplayInScene = true;

    [Header("Treeview")]
    public int X = 5;
    public int Y = 5;
    public int Width = 450;
    public int Height = 350;
    public RectOffset Padding = new RectOffset();
    public int LevelDistance = 0;
    public Color BackgroundColor = new Color(0, 0, 0, 0.65f);

    [Header("Any Node")]
    public RectOffset NodeMargin = new RectOffset();
    public RectOffset NodePadding = new RectOffset();
    public Font NodeFont;
    public int NodeFontSize = 20;
    public TextAnchor NodeTextAnchor = TextAnchor.MiddleLeft;

    [Header("Nodes")]
    public ENodeView NormalENodeView = new ENodeView(new Color(0.8f, 0.8f, 0.8f), Color.white, Color.red);
    public ENodeView SelectedENodeView = new ENodeView(Color.yellow, Color.white, Color.red);

    [Header("Glyphs")]
    public EGlyph NormalEGlyph = new EGlyph(new Color(0.8f, 0.8f, 0.8f), Color.white, Color.red);
    public EGlyph SelectedEGlyph = new EGlyph(Color.yellow, Color.white, Color.red);
    #endregion

    #region Serialization
    /// <summary>
    /// Id of the last created node.
    /// </summary>
    [Readonly] public int LastNodeId;

    /// <summary>
    /// Serializable part of a tree.
    /// </summary>
    private List<NodeData> nodeDatas = new List<NodeData>();

    /// <summary>
    /// Serializes a tree.
    /// </summary>
    /// <param name="root">The root of the tree to be serialized.</param>
    private void Serialize(Node root)
    {
        if (root == null)
        {
            root = new Node("Select Your Level", this);
        }

        nodeDatas.Add(new NodeData(root));
        root.Children.ForEach(c => Serialize(c));

        Debug.Log("Treeview serialized.");
    }

    /// <summary>
    /// Deserializes a tree.
    /// </summary>
    /// <param name="root">The root of the tree to be restored.</param>
    private void Deserialize(Node root)
    {
        // Controls the uniqueness of any Id.
        Dictionary<int, Node> nd = nodeDatas.ToDictionary(k => k.Id, v => new Node(v, this));

        foreach (Node node in nd.Values)
        {
            Node altParent;
            if (node.Parent != null && nd.TryGetValue(node.Parent.Id, out altParent))
            {
                node.Parent = altParent;
                altParent.Children.Add(node);
            }
        }

        root = nd.Values.First(x => x.Parent == null);

        Debug.Log("Treeview deserialized.");
    }

    /// <summary>
    /// Serializes the tree.
    /// </summary>
    public void OnBeforeSerialize()
    {
        nodeDatas.Clear();
        Serialize(root);
    }

    /// <summary>
    /// Deserializes the tree.
    /// </summary>
    public void OnAfterDeserialize()
    {
        Deserialize(root);
    }
    #endregion

    /// <summary>
    /// The position to use display the tree container.
    /// </summary>
    private Vector2 scrollPosition;

    /// <summary>
    /// Displays the tree container and the root with all its descendants.
    /// </summary>
    public void Display()
    {
        if (NodeFont == null)
        {
            TreeviewPrinter.DisplayWarningMessage("Node font not set.");
            return;
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, BackgroundStyle, GUILayout.MaxWidth(Width), GUILayout.MaxHeight(Height));
        GUILayout.BeginVertical();

        Root.Display();

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// Displays the tree when DisplayInGame and SavedDefaultButtonStyle are set to true.
    /// </summary>
    private void OnGUI()
    {
        if (DisplayInGame && SavedDefaultButtonStyle)
        {
            GUILayout.BeginArea(BackgroundRect);

            Display();

            GUILayout.EndArea();
        }
    }

    /// <summary>
    /// Destroys the component and counts all resources with type Texture2D (for performance analysis).
    /// </summary>
    private void OnDestroy()
    {
        Texture2D[] ts = Resources.FindObjectsOfTypeAll<Texture2D>();
        int tsMaxId = ts.Max(t => t.GetInstanceID());
        int tsCount = ts.Length;

        Debug.Log($"textures: {{ max Id: {tsMaxId}, count: {tsCount} }}");
    }

    /// <summary>
    /// Disables the component and deletes all of its textures.
    /// </summary>
    private void OnDisable()
    {
        DestroyTextures();
    }

    /// <summary>
    /// Resets the component to its default state, deletes SelectedNode and all descendants of the root.
    /// </summary>
    private void Reset()
    {
        SelectedNode = null;
        Root?.Children.Clear();
    }
}
