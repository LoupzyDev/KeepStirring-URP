using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class AssetHandler
{
    [OnOpenAsset()]
    public static bool OpenEditor(int instanceID, int line)
    {
        CraftingManager dataObject = EditorUtility.InstanceIDToObject(instanceID) as CraftingManager;
        if (dataObject != null)
        {
            CraftingDataObjectEditorWindow.Open(dataObject);
            return true;
        }
        return false;
    }
}

[CustomEditor(typeof(CraftingManager))]
public class CraftingDataObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Crafting Editor"))
        {
            CraftingDataObjectEditorWindow.Open((CraftingManager)target);
        }

        base.OnInspectorGUI();
    }

}
