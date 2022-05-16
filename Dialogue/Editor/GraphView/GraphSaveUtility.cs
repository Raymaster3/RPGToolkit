using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private DialogueTreeView _targetGraphView;
    private DialogueContainer _container;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<NodeView> Nodes => _targetGraphView.nodes.ToList().Cast<NodeView>().ToList();

    public static GraphSaveUtility GetInstance(DialogueTreeView targeGraphView)
    {
        return new GraphSaveUtility {
            _targetGraphView = targeGraphView
        };
    }
    public void SaveGraph(string fileName)
    {
        bool createNew = false;
        var dialogueContainer = Resources.Load<DialogueContainer>(fileName);
        if (dialogueContainer == null)
        {
            dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
            createNew = true;
        }

        if (!SaveNodes(dialogueContainer)) return;
        SaveExposedProperties(dialogueContainer);    

        // Creates resources foulder if not found
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        EditorUtility.SetDirty(dialogueContainer);
        if (createNew)
            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        
        AssetDatabase.SaveAssets();
    }

    private void SaveExposedProperties(DialogueContainer dialogueContainer)
    {
        dialogueContainer.ExposedProperties = new List<ExposedProperty>();
        dialogueContainer.ExposedProperties.AddRange(_targetGraphView.exposedProperties);
    }

    private bool SaveNodes(DialogueContainer dialogueContainer)
    {
        if (!Edges.Any()) return false;
        dialogueContainer.NodeLinks = new List<NodeLink>();
        dialogueContainer.DialogueNodes = new List<NodeData>();
        
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as NodeView;
            var inputNode = connectedPorts[i].input.node as NodeView;


            dialogueContainer.NodeLinks.Add(new NodeLink
            {
                BaseNodeGUID = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGUID = inputNode.GUID,
                originPort = FindPortIndex(outputNode, connectedPorts[i].output),
                destinyPort = FindPortIndex(inputNode, connectedPorts[i].input, true)
            });
        }
        foreach (var dialogueNode in Nodes.Where(node => !node.EntryPoint))
        {
            dialogueContainer.DialogueNodes.Add(dialogueNode.GetNewObject());
        }
        return true;
    }

    private int FindPortIndex(NodeView node, Port port, bool input = false)
    {
        if (!input)
        {
            for (int i = 0; i < node.outputContainer.childCount; i++) if (node.outputContainer[i].Q<Port>() == port) return i;
        }
        else
            for (int i = 0; i < node.inputContainer.childCount; i++) if (node.inputContainer[i].Q<Port>() == port) return i;

        return 0;
    }

    public void LoadGraph(string fileName)
    {
        _container = Resources.Load<DialogueContainer>(fileName);
        if (_container == null)
        {
            EditorUtility.DisplayDialog("File not found", "Target dialogue graph file does not exists", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
        CreateExposedProperties();
    }

    private void CreateExposedProperties()
    {
        // Clear existing properties
        _targetGraphView.ClearBlackBoard();
        // Add properties to graph
        foreach (var exposedProperty in _container.ExposedProperties)
        {
            _targetGraphView.AddPropertyToBlackBoard(exposedProperty);
        }
    }

    private void ConnectNodes()
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var connections = _container.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[i].GUID).ToList();
            for (var j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGUID;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[connections[j].originPort].Q<Port>(), (Port)targetNode.inputContainer[connections[j].destinyPort]);

                targetNode.SetPosition(new Rect(_container.DialogueNodes.First(x => x.nodeGUID == targetNodeGuid).Position, _targetGraphView.defaultNodeSize));
            }
        }
    }
     
    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };
        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);

        _targetGraphView.Add(tempEdge);
    }

    private void CreateNodes()
    {
        foreach (var node in _container.DialogueNodes)
        {
            //var tempNode = _targetGraphView.CreateDialogNode(node.DialogueText, Vector2.zero);
            var tempNode = _targetGraphView.CreateNewNode(node);
            tempNode.GUID = node.nodeGUID;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _container.NodeLinks.Where(x => x.BaseNodeGUID == node.nodeGUID).ToList();
            nodePorts.ForEach(port => tempNode.AddPort(port.PortName));
            nodePorts.ForEach(port => tempNode.outputContainer[port.originPort].Q<Port>().portName = port.PortName);
        }
    }

    private void ClearGraph()
    {
        // Set new entry point
        if (_container.NodeLinks.Count == 0) return;
        Nodes.Find(x => x.EntryPoint).GUID = _container.NodeLinks[0].BaseNodeGUID;

        foreach (var node in Nodes)
        {
            if (node.EntryPoint) continue;

            // Remove edges connected to this node
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            // Remove the node
            _targetGraphView.RemoveElement(node);

        }
    }
}
