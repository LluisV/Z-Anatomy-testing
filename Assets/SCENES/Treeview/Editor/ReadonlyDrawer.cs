using UnityEditor;
using UnityEngine;

/// <summary>
/// Readonly attribute drawer.
/// </summary>
[CustomPropertyDrawer(typeof(ReadonlyAttribute))]
public class ReadonlyDrawer : PropertyDrawer
{
    /// <summary>
    /// Get the height needed for a PropertyField control.
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    /// <summary>
    /// Draws a public property or field with ReadonlyAttribute when the GUI is disabled.
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;

        EditorGUI.PropertyField(position, property, label, true);

        GUI.enabled = true;
    }
}
