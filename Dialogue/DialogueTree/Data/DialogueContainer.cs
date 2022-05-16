using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu()]
public class DialogueContainer : ScriptableObject
{
    public List<NodeLink> NodeLinks = new List<NodeLink>();
    [SerializeReference] public List<NodeData> DialogueNodes = new List<NodeData>();
    [SerializeReference] public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
}

[System.Serializable]
public class RuntimeDialogueContainer
{
    public List<NodeLink> NodeLinks = new List<NodeLink>();
    [SerializeReference] public List<NodeData> DialogueNodes = new List<NodeData>();
    [SerializeReference] public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();

    public PlotStep currentStep;
    public Action onCancelCallback;

}
