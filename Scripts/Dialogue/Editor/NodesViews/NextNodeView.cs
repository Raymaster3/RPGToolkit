using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NextNodeView : NodeView
{
    public override NodeData GetNewObject()
    {
        NodeData newObj = new NextNode
        {
            nodeGUID = GUID,
            Position = GetPosition().position
        };
        return newObj;
    }
    public override NodeView createNodeVisual(NodeData data)
    {
        title = "Next";
        GUID = data.nodeGUID;

        var inputPort = GeneratePort(typeof(float), Direction.Input, Port.Capacity.Multi);
        inputContainer.Add(inputPort);

        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(data.Position, new Vector2(150, 200)));
        return this;
    }
}
