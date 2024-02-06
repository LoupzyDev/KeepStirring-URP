using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CraftingDataObjectEditorWindow : ExtendedEditorWindow
{
    public static void Open(CraftingManager dataObject)
    {
        CraftingDataObjectEditorWindow window = GetWindow<CraftingDataObjectEditorWindow>("Crafting Data Object Editor");
        window.serializedObject = new SerializedObject(dataObject);
    }

    private void OnGUI()
    {
        // Craftables
        craftablesProperty = serializedObject.FindProperty("craftables");

        EditorGUILayout.BeginHorizontal();

        // Sidebar
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
        DrawSidebar(craftablesProperty);
        // Add button
        if (GUILayout.Button("Add Craftable"))
        {
            // Call the method in the scriptable object
            ((CraftingManager)serializedObject.targetObject).AddCraftable();
            // Refresh the UI
            serializedObject.Update();
        }
        EditorGUILayout.EndVertical();

        // Main area
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
        if (selectedProperty != null)
        {
            DrawProperties(selectedProperty, true);
        }
        else
        {
            EditorGUILayout.LabelField("Select an item from the sidebar to edit");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        Apply();
    }


}
