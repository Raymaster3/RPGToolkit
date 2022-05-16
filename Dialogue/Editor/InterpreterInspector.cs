using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TreeInterpreter))]
public class InterpreterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        var tree = (TreeInterpreter) target;

        DialogueContainer container = (DialogueContainer) EditorGUILayout.ObjectField("Tree", tree._tree, typeof(DialogueContainer), false);

        if (container == null)
        {
            tree._tree = container;
            return;
        }

        if (container != tree._tree)
            tree.GenerateRuntimeContainer(container);

        RuntimeDialogueContainer runtimeContainer = tree.runtimeContainer;

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

        if (GUILayout.Button("TestRun"))
        {
            tree.Run();
        }
    }
}
