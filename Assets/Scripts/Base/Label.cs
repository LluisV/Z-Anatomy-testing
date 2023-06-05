using GK;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

/// <summary>
/// Represents a label in 3D space. 
/// Updates its color based on its position and handles its selection.
/// </summary>
public class Label : MonoBehaviour
{
    private Camera cam;
    private TextMeshPro text;
    private float parentScale;
    private MaterialPropertyBlock _propBlock;
    private Renderer _renderer;
    private Transform originPoint;
    private BoxCollider boxCollider;
    RectTransform rect;
    [HideInInspector]
    public Material labelMaterial;
    [HideInInspector]
    public Line line;
    private NameAndDescription nameScript;
    private BodyPartVisibility visibilityScript;
    private bool hasLine;
    private Color color;
    private float fontSize;
    [HideInInspector]
    public LabelCollider lc;
    [HideInInspector]
    public LabelCollider mirroredLc;

    [HideInInspector]
    public TangibleBodyPart parent;

    [HideInInspector]
    public Vector3 lineDirection;

    //Collider stuff
    [HideInInspector]
    public bool debug = false;
    [HideInInspector]
    public float diameter = 0.25f;
    [HideInInspector]
    public float offset = 0;
    [HideInInspector]
    public bool angleThresholdEnabled = false;
    [HideInInspector]
    public bool invertAngle = false;
    [HideInInspector]
    public float angleThresholdMultiplier = 1;
    [HideInInspector]
    public bool enablePlane = false;
    [HideInInspector]
    public bool invertPlane = false;
    [HideInInspector]
    public bool mirrored = false;
    [HideInInspector]
    public float planeOffset = -0.025f;
    [HideInInspector]
    public float inflate = 0.01f;
    private Transform clickableLabelsParent;
    private GameObject colliderGo;
    private GameObject mirroredColliderGo;


    private void Awake()
    {
        nameScript = GetComponent<NameAndDescription>();
        visibilityScript = GetComponent<BodyPartVisibility>();
        text = GetComponent<TextMeshPro>();
        boxCollider = gameObject.AddComponent<BoxCollider>();
        rect = GetComponent<RectTransform>();
        cam = Camera.main;
        color = Color.white;
        parent = GetComponentInParent<TangibleBodyPart>();
    }

    private void Start()
    {
        if(!hasLine)
            GetLine();
        Initialize();
    }

    private void GetLine()
    {
        try
        {
            var lineObj = transform.parent.Find(new StringBuilder().Append(nameScript.originalName.RemoveSuffix()).Append(".j").ToString());
            if(lineObj == null)
                lineObj = transform.parent.Find(new StringBuilder().Append(name.RemoveSuffix()).Append(".j").ToString());
            if(lineObj == null)
                lineObj = transform.parent.Find(new StringBuilder().Append(nameScript.originalName.RemoveSuffix()).Append(".i").ToString());
            if (lineObj == null)
                lineObj = transform.parent.Find(new StringBuilder().Append(name.RemoveSuffix()).Append(".i").ToString());

            if (lineObj != null)
                line = lineObj.GetComponent<Line>();
            else
                line = transform.parent.Find(new StringBuilder().Append(nameScript.originalName.Replace(".t", "").Replace(".s", "")).Append(".i").ToString()).GetComponent<Line>();
            line.gameObject.SetActive(true);
            originPoint = line.transform.Find("maxPoint");
            hasLine = true;
        }
        catch (System.Exception)
        {
            hasLine = false;
        }


        if (line != null && line.minPoint != null && line.maxPoint != null)
            lineDirection = line.maxPoint.position - line.minPoint.position;
        else
            lineDirection = parent.transform.position - transform.position;
    }



    /// <summary>
    /// Initializes the Label component, setting its properties and text value.
    /// </summary>
    public void Initialize()
    {
        // Set the box collider center to the center of the text mesh.
        boxCollider.center = Vector3.zero;
        // Store the parent scale value for later use.
        parentScale = transform.parent.lossyScale.x;

        // Set text properties, such as margins, alignment, font size, material, and so on.
        text.margin = new Vector4(8f, 2f, 8f, 2f);
        text.alignment = TextAlignmentOptions.Center;
        fontSize = GlobalVariables.Instance.labelFontSize * 0.15f;
        text.fontSize = fontSize * Mathf.Clamp(cam.orthographicSize, 0.075f, 1.5f);
        text.material = labelMaterial;

        // Set the scale of the rect transform to account for the parent scale.
        rect.localScale = new Vector3(1 / parentScale, 1 / parentScale, 1 / parentScale);

        // Create a new material property block for the label.
        _propBlock = new MaterialPropertyBlock();
        _renderer = text.renderer;

        // Set the label tex
        SetText(gameObject.name.Replace(".j", "").Replace(".i", "").Replace(".t", "").Replace(".s",""));
    }

    // Update is called once per frame
    void Update()
    {
        boxCollider.size = text.textBounds.size;
        transform.rotation = cam.transform.rotation;
        text.fontSize = fontSize * Mathf.Clamp(cam.orthographicSize, 0.075f, 1.5f);

        if (hasLine)
            UpdateColor();
    }

    /// <summary>
    /// Updates the color of the object based on the angle between the origin point and camera direction.
    /// </summary>
    /// <remarks>
    /// This function calculates the angle between the origin point and camera direction, and then uses the angle to calculate the new color of the object.
    /// The color is then set to the object and also to the line renderer attached to the object. 
    /// If the object is selected, the line color is set to the new color.
    /// </remarks>
    /// <returns>
    void UpdateColor()
    {
        float angle = Vector3.Angle(originPoint.position - transform.position, -cam.transform.forward);
        float a = angle * angle * angle * angle * 0.000000025f;
        _renderer.enabled = a > .075f;
        line._renderer.enabled = _renderer.enabled;

        if (a > 1)
            a = 1;

        Color newColor = new Color(color.r, color.g, color.b, a);

        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_FaceColor", newColor);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);

        if(visibilityScript.isSelected)
            line.SetColor(newColor);
        else
            line.SetColor(a * 0.5f);
    }

    /// <summary>
    /// Sets the text of the label.
    /// </summary>
    /// <remarks>
    /// If the 'name' parameter matches the substring obtained from the parent object's name, the font style is set to 'Bold' and the font size is set to a scaled value.
    /// </remarks>
    public void SetText(string name)
    {
        if(text != null)
        {
            text.text = name;
            string substring = transform.parent.name.Replace("(R)", "").Replace("(L)", "").Trim();
            if (substring.Contains("."))
            {
                int indexOfPoint = transform.parent.name.IndexOf('.');
                substring = transform.parent.name.Substring(0, indexOfPoint);
            }
            if (name.Equals(substring))
            {
                text.fontStyle = FontStyles.Bold;
                fontSize = GlobalVariables.Instance.titleLabelFontSize * 0.15f;
            }
        }
    }


    /// <summary>
    /// Handles the click event of the label and updates the UI.
    /// </summary>
    /// <remarks>
    /// This function checks if the game object is not already selected. 
    /// If not, it deselects all other children of the parent object and selects the game object.
    /// Then updates the hierarchy bar, expands the lexicon and sets the description. 
    /// If the game object is already selected, it deselects the object. 
    /// </remarks>
    public void Click()
    {
        if (!SelectedObjectsManagement.Instance.selectedObjects.Contains(gameObject))
        {
            SelectedObjectsManagement.Instance.DeselectAllChildren(gameObject.transform.parent);
            SelectedObjectsManagement.Instance.SelectObject(gameObject);

            nameScript.SetDescription();
        }
        else
        {
            SelectedObjectsManagement.Instance.DeselectObject(gameObject);
            GetComponent<BodyPartVisibility>().isVisible = false;
        }
        ActionControl.Instance.UpdateButtons();
    }

    /// <summary>
    /// Sets the color of the label and line to the highlight color.
    /// </summary>
    public void Select()
    {
        color = GlobalVariables.HighligthColor;
        if(!hasLine && _renderer != null)
        {
            // Get the current value of the material properties in the renderer.
            _renderer.GetPropertyBlock(_propBlock);
            // Assign our new value.
            _propBlock.SetColor("_FaceColor", color);
            // Apply the edited values to the renderer.
            _renderer.SetPropertyBlock(_propBlock);
        }
    }

    /// <summary>
    /// Sets the color of the label and line to the default color.
    /// </summary>
    public void Deselect()
    {
        color = Color.white;
        if (!hasLine && _renderer != null)
        {
            // Get the current value of the material properties in the renderer.
            _renderer.GetPropertyBlock(_propBlock);
            // Assign our new value.
            _propBlock.SetColor("_FaceColor", color);
            // Apply the edited values to the renderer.
            _renderer.SetPropertyBlock(_propBlock);
        }
    }

    /// <summary>
    /// Shows the label in the 3D space
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        visibilityScript.isVisible = true;
        if (line != null)
            line.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the label in the 3D space
    /// </summary>
    public void Hide()
    {
        visibilityScript.isVisible = false;
        if (line != null)
            line.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void UpdatePos()
    {
        if(line != null)
            line.UpdatePos();
    }

    //------------MESH STUFF-----------

    public void BuildMeshClick()
    {
        if (!hasLine)
        {
            if (parent == null)
                parent = GetComponentInParent<TangibleBodyPart>();
            if (nameScript == null)
                nameScript = GetComponent<NameAndDescription>();
            GetLine();
        }
        if (!hasLine)
        {
            Debug.Log("Error in " + name + ": mesh was not built. <color=#FF0000>Line not found.</color> Check that the names match.");
            return;
        }

        EditorApplication.delayCall += () =>
        {
            if (colliderGo != null)
                DestroyImmediate(colliderGo);
            if (mirroredColliderGo != null)
                DestroyImmediate(mirroredColliderGo);
            BuildMesh();
        };
    }

    public void DestroyMeshClick()
    {
        if (clickableLabelsParent == null)
            clickableLabelsParent = parent.transform.Find("---------LABEL MESHES---------");
        if(clickableLabelsParent != null)
        {
            if (colliderGo == null)
                colliderGo = clickableLabelsParent.Find(name + ".col")?.gameObject;
            if (colliderGo != null)
            {
                EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(colliderGo);
                    AssetDatabase.DeleteAsset(lc.savePath);
                };
            }
            else
            {
                Debug.Log("Collider not found");
            }
            if (mirroredColliderGo == null)
                mirroredColliderGo = clickableLabelsParent.Find(name + ".colm")?.gameObject;
            if (mirroredColliderGo != null)
            {
                EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(mirroredColliderGo);
                    AssetDatabase.DeleteAsset(mirroredLc.savePath);
                };
            }
        }

    }

    public void ResetValuesClick()
    {
        diameter = 0.25f;
        offset = 0;
        angleThresholdEnabled = false;
        invertAngle = false;
        angleThresholdMultiplier = 1;
        enablePlane = false;
        planeOffset = -0.025f;
        invertPlane = false;
        inflate = 0.01f;
        mirrored = false;

        EditorApplication.delayCall += () =>
        {
            if (colliderGo != null)
            {
                if(mirroredColliderGo != null)
                    DestroyImmediate(mirroredColliderGo);
                DestroyImmediate(colliderGo);
                BuildMesh();
            }
        };
    }

    public void DebugModeClick()
    {
        if (lc != null)
        {
            UpdateMeshVariables(lc);
            lc.UpdateMaterial();
        }
        if(mirroredLc != null)
        {
            UpdateMeshVariables(mirroredLc);
            mirroredLc.UpdateMaterial();
        }
    }

    public void UpdateMeshVariables(LabelCollider c)
    {
        if(c != null)
        {
            c.debug = debug;
            c.diameter = diameter;
            c.offset = offset;
            c.angleThresholdEnabled = angleThresholdEnabled;
            c.invertAngle = invertAngle;
            c.angleThresholdMultiplier = angleThresholdMultiplier;
            c.enablePlane = enablePlane;
            c.planeOffset = planeOffset;
            c.invertPlane = invertPlane;
            c.inflate = inflate;
        }
    }
    private void BuildMesh()
    {
        // Check if 'Clickable label meshes' object exists

        parent = GetComponentInParent<TangibleBodyPart>();
        clickableLabelsParent = parent.transform.Find("---------LABEL MESHES---------");
        if (clickableLabelsParent == null)
        {
            clickableLabelsParent = new GameObject("---------LABEL MESHES---------").transform;
            clickableLabelsParent.SetParent(parent.transform);
            clickableLabelsParent.SetSiblingIndex(0);
        }
        clickableLabelsParent.transform.position = Vector3.zero;
        clickableLabelsParent.transform.rotation = Quaternion.identity;
        colliderGo = new GameObject(name + ".col");
        lc = colliderGo.AddComponent<LabelCollider>();
        lc.label = this;
        lc.parent = clickableLabelsParent;
        UpdateMeshVariables(lc);
        lc.Build();

        if(mirrored)
        {
            mirroredColliderGo = new GameObject(name + ".colm");
            mirroredLc = mirroredColliderGo.AddComponent<LabelCollider>();
            mirroredLc.label = this;
            mirroredLc.parent = clickableLabelsParent;
            UpdateMeshVariables(mirroredLc);
            mirroredLc.Build();
            mirroredColliderGo.transform.localScale = new Vector3(-1f, 1f, 1f); // Mirror on the x-axis
            mirroredColliderGo.transform.position = new Vector3(-colliderGo.transform.position.x, colliderGo.transform.position.y, colliderGo.transform.position.z);
            mirroredColliderGo.transform.localRotation = Quaternion.identity;
        }
    }
}


//TODO: Adapt the code so that editing multiple labels at the same time works correctly.
[CustomEditor(typeof(Label)), CanEditMultipleObjects]
public class customLabelInspector : Editor
{
    private bool changes = true;
    public override void OnInspectorGUI()
    {
        Label label = (Label)target;

        if(targets.Length == 1)
        {
            GUILayout.Space(10);
            // Display the default Inspector GUI
            DrawDefaultInspector();
            string meshStatus = label.lc != null ? "<b><color=#00ff00>Mesh Built</color></b>" : "<b><color=#ff0000>Mesh Not Built</color></b>";
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontStyle = FontStyle.Bold;
            style.richText = true;
            EditorGUILayout.LabelField(meshStatus, style);
            // Add custom GUI elements
            GUILayout.Label("Mesh Parameters", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            label.diameter = EditorGUILayout.Slider("Mesh size", label.diameter, 0.01f, 2f);
            label.offset = EditorGUILayout.Slider("Position offset", label.offset, -0.1f, 0.1f);
            label.inflate = EditorGUILayout.Slider("Inflate mesh", label.inflate, -0.1f, 0.1f);

            GUILayout.Space(10);
            label.angleThresholdEnabled = EditorGUILayout.ToggleLeft("Delete heavily rotated vertices (for thin surfaces)", label.angleThresholdEnabled);
            if (label.angleThresholdEnabled)
            {
                label.invertAngle = EditorGUILayout.ToggleLeft("Invert angle threshold", label.invertAngle);
                label.angleThresholdMultiplier = EditorGUILayout.Slider("Angle threshold multiplier", label.angleThresholdMultiplier, 0f, 4f);
            }
            GUILayout.Space(10);
            label.enablePlane = EditorGUILayout.ToggleLeft("Limiting plane", label.enablePlane);
            if (label.enablePlane)
            {
                label.invertPlane = EditorGUILayout.ToggleLeft("Invert limiting plane", label.invertPlane);
                label.planeOffset = EditorGUILayout.Slider("Plane offset", label.planeOffset, -0.25f, 0.25f);
            }
            GUILayout.Space(10);
            label.mirrored = EditorGUILayout.ToggleLeft("Mirror", label.mirrored);
            GUILayout.Space(10);
            changes |= EditorGUI.EndChangeCheck();
            if (changes)
            {
                label.UpdateMeshVariables(label.lc);
                label.UpdateMeshVariables(label.mirroredLc);
            }
        }
        
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Mesh Actions", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        GUIStyle buildStyle = new GUIStyle(GUI.skin.button);
        GUI.enabled = changes || label.lc == null;
        buildStyle.normal.textColor = GUI.enabled ? Color.green : Color.white;
        buildStyle.fontStyle = GUI.enabled ? FontStyle.Bold : FontStyle.Normal;
        if (GUILayout.Button(label.lc == null ? "Build Mesh" : "Apply Changes", buildStyle))
        {
            foreach (var target in targets)
            {
                Label l = target as Label;
                l.BuildMeshClick();
            }
            changes = false;
        }

        GUIStyle destroyStyle = new GUIStyle(GUI.skin.button);
        GUI.enabled = label.lc != null;
        destroyStyle.normal.textColor = GUI.enabled ? Color.red : Color.white;
        if (GUILayout.Button("Destroy Mesh", destroyStyle))
        {
            foreach (var target in targets)
            {
                Label l = target as Label;
                l.DestroyMeshClick();
            }
            changes = false;
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        EditorGUILayout.BeginVertical();
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        if (GUILayout.Button("Reset parameters"))
        {
            foreach (var target in targets)
            {
                Label l = target as Label;
                l.ResetValuesClick();
            }
        }

        GUIStyle debugStyle = new GUIStyle(GUI.skin.button);
        debugStyle.normal.textColor = label.debug ? Color.green : Color.white;

        if (GUILayout.Button(label.debug ? "Debug Mode: ON" : "Debug Mode: OFF", debugStyle))
        {
            bool d_v = (target as Label).debug;
            foreach (var target in targets)
            {
                Label l = target as Label;
                l.debug = !d_v;
                l.DebugModeClick();
            }
        }
        EditorGUILayout.EndVertical();
    }
}

