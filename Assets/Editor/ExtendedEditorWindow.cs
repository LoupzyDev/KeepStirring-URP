using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ExtendedEditorWindow : EditorWindow
{
    protected SerializedObject serializedObject;
    protected SerializedProperty craftablesProperty;

    private string selectedPropertyPath;
    protected SerializedProperty selectedProperty;

    protected void DrawProperties(SerializedProperty prop, bool drawChildren)
    {
        GUILayout.Label(prop.displayName, EditorStyles.boldLabel);
        GUILayout.Space(10);
        string lastPropPath = string.Empty;
        foreach (SerializedProperty p in prop)
        {
            if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                EditorGUILayout.EndHorizontal();
                if (p.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(p, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }
    }

    protected void DrawSidebar(SerializedProperty prop)
    {
        GUILayout.Label("Craftables", EditorStyles.boldLabel);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

        foreach (SerializedProperty p in prop)
        {
            if (GUILayout.Button(p.displayName))
            {
                selectedPropertyPath = p.propertyPath;

                EditorGUIUtility.editingTextField = false;
            }
        }

        if (!string.IsNullOrEmpty(selectedPropertyPath))
        {
            selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
        }
    }

    protected void DrawField(string propertyName, bool relative = false)
    {
        if (relative && craftablesProperty != null)
        {
            EditorGUILayout.PropertyField(craftablesProperty.FindPropertyRelative(propertyName), true);
        }
        else if (serializedObject != null)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), true);
        }
    }

    protected void Apply()
    {
        serializedObject.ApplyModifiedProperties();
    }
}
