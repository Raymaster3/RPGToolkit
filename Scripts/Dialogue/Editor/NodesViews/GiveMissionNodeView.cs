using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GiveMissionNodeView : NodeView
{
    private QuestData inputData;

    public override NodeData GetNewObject()
    {
        NodeData newObj = new GiveMissionNode
        {
            nodeGUID = GUID,
            quest = inputData,
            Position = GetPosition().position
        };
        return newObj;
    }
    public override NodeView createNodeVisual(NodeData data)
    {
        title = "Give Quest";
        GUID = data.nodeGUID;

        var inputPort = GeneratePort(typeof(float), Direction.Input, Port.Capacity.Multi);
        var inputDataPort = GeneratePort(typeof(QuestData), Direction.Input, Port.Capacity.Single);
        var outputPort = GeneratePort(typeof(float), Direction.Output, Port.Capacity.Single);
        var outputFailPort = GeneratePort(typeof(float), Direction.Output, Port.Capacity.Single);


        inputDataPort.portColor = Utils.typeColors.Find(x => x.Key == typeof(QuestData)).Value;
        outputPort.portName = "Next";
        outputFailPort.portName = "Failed";

        inputContainer.Add(inputPort);
        inputContainer.Add(inputDataPort);
        outputContainer.Add(outputPort);
        outputContainer.Add(outputFailPort);

        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(data.Position, new Vector2(150, 200)));

        return this;
    }
    public override void SetInputValue(Port port, object value)
    {
        inputData = value as QuestData;
    }
    public override void SetInputValue(int portIndex, object value)
    {
        inputData = value as QuestData;
    }
    public override void UnSetInput(int portIndex)
    {
        inputData = null;
    }
    public override void UnSetInput(Port port)
    {
        inputData = null;
    }
}
