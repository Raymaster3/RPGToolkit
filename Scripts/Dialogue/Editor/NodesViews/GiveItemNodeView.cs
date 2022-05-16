using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GiveItemNodeView : NodeView
{
    private Item inputData;
    private int quantity;

    public override NodeData GetNewObject()
    {
        NodeData newObj = new GiveItemNode
        {
            nodeGUID = GUID,
            item = inputData,
            quantity = quantity,
            Position = GetPosition().position
        };
        return newObj;
    }
    public override NodeView createNodeVisual(NodeData data)
    {
        title = "Give Item";
        GUID = data.nodeGUID;

        var inputPort = GeneratePort(typeof(float), Direction.Input, Port.Capacity.Multi);
        var inputDataPort = GeneratePort(typeof(Item), Direction.Input, Port.Capacity.Single);
        var inputQuantPort = GeneratePort(typeof(int), Direction.Input, Port.Capacity.Single);
        var outputPort = GeneratePort(typeof(float), Direction.Output, Port.Capacity.Single);
        var outputFailPort = GeneratePort(typeof(float), Direction.Output, Port.Capacity.Single);

        inputDataPort.portColor = Utils.typeColors.Find(x => x.Key == typeof(Item)).Value;
        inputQuantPort.portName = "Amount";
        outputPort.portName = "Next";
        outputFailPort.portName = "Fail";

        inputContainer.Add(inputPort);
        inputContainer.Add(inputDataPort);
        inputContainer.Add(inputQuantPort);
        outputContainer.Add(outputPort);
        outputContainer.Add(outputFailPort);

        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(data.Position, new Vector2(150, 200)));

        return this;
    }
    public override void SetInputValue(Port port, object value)
    {
        if (value as Item == null)
        {
            quantity = (int)value;
        }
        else inputData = value as Item;
    }
    public override void SetInputValue(int portIndex, object value)
    {
        if (portIndex == 1)
            inputData = value as Item;
        else
        {
            quantity = (int) value;
        }
    }
    public override void UnSetInput(int portIndex)
    {
        if (portIndex == 1)
            inputData = null;
        else if (portIndex == 2)
        {
            quantity = 1;
        }
    }
    public override void UnSetInput(Port port)
    {

        for (int i = 0; i < inputContainer.childCount; i++)
        {
            if (inputContainer[i].Q<Port>().portName == port.portName)
            {
                if (i == 1)
                {
                    inputData = null;
                }else if (i == 2)
                {
                    quantity = 1;
                }
                return;
            }
        }
    }
}
