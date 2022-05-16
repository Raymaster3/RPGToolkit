using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlotStep))]
public class PlotStepInspector : Editor
{
    bool foldout;
    bool dialog1, dialog2, dialog3;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlotStep plotStep = target as PlotStep;
      
        foldout = EditorGUILayout.Foldout(foldout, "Dialogues", true);

        if (foldout)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            dialog1 = EditorGUILayout.Foldout(dialog1, "Dialogue Tree", true);
            GUILayout.EndHorizontal();

            if (dialog1)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GenerateDialogueUI(plotStep, plotStep.dialogue);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            dialog2 = EditorGUILayout.Foldout(dialog2, "Default Dialogue Tree", true);
            GUILayout.EndHorizontal();

            if (dialog2)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GenerateDialogueUI(plotStep, plotStep.defaultDialogue, true);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            dialog3 = EditorGUILayout.Foldout(dialog3, "Reward Dialogue Tree", true);
            GUILayout.EndHorizontal();

            if (dialog3)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GenerateRewardDialogueUI(plotStep);
                GUILayout.EndHorizontal();
            }
        }


    }

    private void GenerateDialogueUI(PlotStep target, DialogueContainer dialogue, bool defaultDial = false)
    {
        GUILayout.BeginVertical();
        DialogueContainer container = (DialogueContainer)EditorGUILayout.ObjectField("Tree", dialogue, typeof(DialogueContainer), false);

        if (container == null)
        {
            if (defaultDial)
                target.defaultDialogue = container;
            else target.dialogue = container;
            GUILayout.EndVertical();
            return;
        }

        //if (container != dialogue)
            target.GenerateRuntimeContainer(container, defaultDial);

        RuntimeDialogueContainer runtimeContainer;
        if (!defaultDial) runtimeContainer = target.runtimeDialogue;
        else runtimeContainer = target.runtimeDefaultDialogue;

        GUILayout.Space(5);
        GUILayout.Label("Exposed Properties", EditorStyles.boldLabel);
        GUILayout.Space(5);

        // Get all exposed properties and show them
        foreach (ExposedProperty prop in runtimeContainer.ExposedProperties)
        {
            if (!prop.exposed) continue;
            GUILayout.BeginHorizontal();
            GUILayout.Label(prop.PropertyName);
            runtimeContainer.DialogueNodes.FindAll(x => {
                // Find all nodes referencing this property
                PropertyNode node = x as PropertyNode;
                if (node == null) return false;
                return node.property.PropertyName == prop.PropertyName;
            }).ForEach(x => {
                // For each node update the input value of all the connected nodes
                List<NodeLink> connections = runtimeContainer.NodeLinks.FindAll(y => y.BaseNodeGUID == x.nodeGUID);
                object data = prop.createInputMethod(prop.getValue());
                prop.setValue(data);
                foreach (NodeLink link in connections)
                {
                    runtimeContainer.DialogueNodes.Find(z => z.nodeGUID == link.TargetNodeGUID).setInputData(data);
                }
            });
            GUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(target);
        GUILayout.EndVertical();
    }
    private void GenerateRewardDialogueUI(PlotStep target)
    {
        GUILayout.BeginVertical();
        DialogueContainer container = (DialogueContainer)EditorGUILayout.ObjectField("Tree", target.rewardDialogue, typeof(DialogueContainer), false);

        if (container == null)
        {
            target.rewardDialogue = container;
            GUILayout.EndVertical();
            return;
        }

        //if (container != target.rewardDialogue)
            target.GenerateRewardRuntimeContainer(container);

        RuntimeDialogueContainer runtimeContainer;
        runtimeContainer = target.runtimeRewardDialogue;

        GUILayout.Space(5);
        GUILayout.Label("Exposed Properties", EditorStyles.boldLabel);
        GUILayout.Space(5);

        // Get all exposed properties and show them
        foreach (ExposedProperty prop in runtimeContainer.ExposedProperties)
        {
            if (!prop.exposed) continue;
            GUILayout.BeginHorizontal();
            GUILayout.Label(prop.PropertyName);
            runtimeContainer.DialogueNodes.FindAll(x => {
                // Find all nodes referencing this property
                PropertyNode node = x as PropertyNode;
                if (node == null) return false;
                return node.property.PropertyName == prop.PropertyName;
            }).ForEach(x => {
                // For each node update the input value of all the connected nodes
                List<NodeLink> connections = runtimeContainer.NodeLinks.FindAll(y => y.BaseNodeGUID == x.nodeGUID);
                object data = prop.createInputMethod(prop.getValue());
                prop.setValue(data);
                foreach (NodeLink link in connections)
                {
                    runtimeContainer.DialogueNodes.Find(z => z.nodeGUID == link.TargetNodeGUID).setInputData(data);
                }
            });
            GUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(target);
        GUILayout.EndVertical();
    }
}
