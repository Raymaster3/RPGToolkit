using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemQuestData))]
public class ItemQuestEditor : Editor
{
    private bool firstFoldout;
    private List<bool> foldoutLevels = new List<bool>();

    SerializedProperty _itemObj;


    private void OnEnable()
    {
        
    }
    public override void OnInspectorGUI()
    {
        //serializedObject.Update();
        base.OnInspectorGUI();
        /*ItemQuestData data = target as ItemQuestData;

        firstFoldout = EditorGUILayout.Foldout(firstFoldout, "Objectives", true);

        if (firstFoldout)
        {
            int i = 0;
            foreach (ObjectiveQuant<Item> objQuant in data.itemObjectives)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (foldoutLevels.Count <= i) foldoutLevels.Add(false);
                foldoutLevels[i] = EditorGUILayout.Foldout(foldoutLevels[i], "Objective " + i, true);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                if (foldoutLevels[i])
                {
                    GUILayout.BeginVertical();
                    objQuant.objective = EditorGUILayout.ObjectField("Item", objQuant.objective, typeof(Item), false) as Item;
                    objQuant.totalQuantity = EditorGUILayout.IntField("Quantity", objQuant.totalQuantity);
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                i++;
            }
        }
        if (GUILayout.Button("Add Objective"))
        {

            //data.itemObjectives.Add(new ItemObjectiveQuant());
            //EditorUtility.SetDirty(data);
            _itemObj = serializedObject.FindProperty("itemObjectives");
            _itemObj.arraySize++;
            var element = _itemObj.GetArrayElementAtIndex(_itemObj.arraySize - 1);
            object value = new ObjectiveQuant<Item>();
            element.objectReferenceValue = (Object) value;
        }
        serializedObject.ApplyModifiedProperties();*/
    }
}
