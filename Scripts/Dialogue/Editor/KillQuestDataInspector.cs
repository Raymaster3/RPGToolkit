using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KillQuestData))]
public class KillQuestDataInspector : Editor
{
    List<Item> itemObjectives = new List<Item>();
    List<int> itemObjectivesQuant = new List<int>();
    List<CharacterData> killObjectives = new List<CharacterData>();
    List<int> killObjectivesQuant = new List<int>();

    bool itemsFoldout = false, killFoldout = false;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        FillItemList();
        FillKillList();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            loadItemListData();
            loadKillListData();
        }
        if (GUILayout.Button("Save"))
        {

            KillQuestData questData = target as KillQuestData;
            questData.objectives = new List<GenericObjectiveQuant>();

            int index = 0;
            foreach (Item i in itemObjectives)
            {
                questData.objectives.Add(new GenericObjectiveQuant(i, itemObjectivesQuant[index]));
                index++;
            }
            index = 0;
            foreach(CharacterData cd in killObjectives)
            {
                questData.objectives.Add(new GenericObjectiveQuant(cd, killObjectivesQuant[index]));
                index++;
            }
        }
        GUILayout.EndHorizontal();
    }

    private void FillItemList()
    {

        itemsFoldout = EditorGUILayout.Foldout(itemsFoldout, "Item Objectives", true);
        GUILayout.BeginVertical();
        if (itemsFoldout)
        {
            GUILayout.BeginHorizontal();
            // Assign item objectives
            GUILayout.Space(15);
            int newSize = EditorGUILayout.IntField("Size", itemObjectives.Count);
            GUILayout.EndHorizontal();
            int extraElements = newSize - itemObjectives.Count;
            for (int i = 0; i < extraElements; i++)
            {
                if (itemObjectives.Count != 0)
                {
                    itemObjectives.Add(itemObjectives[itemObjectives.Count - 1]);
                    itemObjectivesQuant.Add(itemObjectivesQuant[itemObjectivesQuant.Count - 1]);
                }
                else
                {
                    itemObjectives.Add(null);
                    itemObjectivesQuant.Add(1);
                }
            }
            if (extraElements < 0)
            {
                int size = itemObjectives.Count;
                for (int i = size - 1; i > size - 1 + extraElements; i--)
                {
                    itemObjectives.Remove(itemObjectives[i]);
                    itemObjectivesQuant.RemoveAt(i);
                }
            }

            for (int i = 0; i < itemObjectives.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                itemObjectives[i] = EditorGUILayout.ObjectField("Item " + i, itemObjectives[i], typeof(Item), false) as Item;
                itemObjectivesQuant[i] = EditorGUILayout.IntField(itemObjectivesQuant[i]);
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
    }
    private void FillKillList()
    {
        killFoldout = EditorGUILayout.Foldout(killFoldout, "Kill Objectives", true);
        GUILayout.BeginVertical();
        if (killFoldout)
        {
            GUILayout.BeginHorizontal();
            // Assign kill objectives
            GUILayout.Space(15);
            int newSize = EditorGUILayout.IntField("Size", killObjectives.Count);
            GUILayout.EndHorizontal();
            int extraElements = newSize - killObjectives.Count;
            for (int i = 0; i < extraElements; i++)
            {
                if (killObjectives.Count != 0)
                {
                    killObjectives.Add(killObjectives[killObjectives.Count - 1]);
                    killObjectivesQuant.Add(killObjectivesQuant[killObjectivesQuant.Count - 1]);
                }
                else
                {
                    killObjectives.Add(null);
                    killObjectivesQuant.Add(1);
                }
            }
            if (extraElements < 0)
            {
                int size = killObjectives.Count;
                for (int i = size - 1; i > size - 1 + extraElements; i--)
                {
                    killObjectives.Remove(killObjectives[i]);
                    killObjectivesQuant.RemoveAt(i);
                }
            }

            for (int i = 0; i < killObjectives.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                killObjectives[i] = EditorGUILayout.ObjectField("Character " + i, killObjectives[i], typeof(CharacterData), false) as CharacterData;
                killObjectivesQuant[i] = EditorGUILayout.IntField(killObjectivesQuant[i]);
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
    }

    private void loadItemListData()
    {
        KillQuestData data = target as KillQuestData;

        itemObjectives = new List<Item>();
        itemObjectivesQuant = new List<int>(); // Reset lists

        if (data.objectives == null) data.objectives = new List<GenericObjectiveQuant>();

        foreach (GenericObjectiveQuant obj in data.objectives) // And then initialize them with the values
        {
            if (obj.objective as Item != null)
            {
                itemObjectives.Add(obj.objective as Item);
                itemObjectivesQuant.Add(obj.totalQuantity);
            }
        }
    }
    private void loadKillListData()
    {
        KillQuestData data = target as KillQuestData;

        killObjectives = new List<CharacterData>();
        killObjectivesQuant = new List<int>(); // Reset lists

        foreach (GenericObjectiveQuant obj in data.objectives) // And then initialize them with the values
        {
            if (obj.objective as CharacterData != null)
            {
                killObjectives.Add(obj.objective as CharacterData);
                killObjectivesQuant.Add(obj.totalQuantity);
            }
        }
    }
}
