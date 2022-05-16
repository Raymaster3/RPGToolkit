using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EndNodeView : NodeView
{
    public override NodeData GetNewObject()
    {
        NodeData newObj = new EndNode
        {
            nodeGUID = GUID,
            Position = GetPosition().position
        };
        return newObj;
    }
    public override NodeView createNodeVisual(NodeData data)
    {
        title = "End";
        GUID = data.nodeGUID;

        var inputPort = GeneratePort(typeof(float), Direction.Input, Port.Capacity.Multi);
        inputContainer.Add(inputPort);

        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(data.Position, new Vector2(150, 200)));
        return this;
    }
}
