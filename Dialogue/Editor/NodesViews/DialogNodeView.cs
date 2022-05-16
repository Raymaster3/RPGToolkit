using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class DialogNodeView : NodeView
{
    public string DialogueText;
    public string overrittenText;

    private TextField textField;
    private bool overritten = false;

    public override NodeData GetNewObject()
    {
        NodeData newObj = new DialogNode
        {
            nodeGUID = GUID,
            DialogueText = DialogueText,
            overritten = overritten,
            Position = GetPosition().position
        };
        return newObj;
    }
    public override NodeView createNodeVisual(NodeData data)
    {
        DialogNode nodeData = (DialogNode) data;

        title = nodeData.DialogueText;
        DialogueText = nodeData.DialogueText;
        GUID = data.nodeGUID;
        overritten = nodeData.overritten;

        var inputPort = GeneratePort(typeof(float), Direction.Input, Port.Capacity.Multi);
        var overrideInputPort = GeneratePort(typeof(string), Direction.Input, Port.Capacity.Single);
        overrideInputPort.portName = "Override Text";

        inputContainer.Add(inputPort);
        inputContainer.Add(overrideInputPort);

        var button = new Button(() => { AddChoicePort(); });
        button.text = "New Choice";
        titleContainer.Add(button);

        textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt => {
            DialogueText = evt.newValue;
            title = evt.newValue;
        });
        textField.SetValueWithoutNotify(title);
        mainContainer.Add(textField);

        textField.SetEnabled(!overritten);

        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(data.Position, new Vector2(150, 200)));

        return this;
    }
    private void AddChoicePort(string overritenPortName = "")
    {
        var generatedPort = GeneratePort(typeof(float), Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = outputContainer.Query("connector").ToList().Count;
        var outputPortName = $"Choice {outputPortCount}";

        var portName = string.IsNullOrEmpty(overritenPortName) ? $"Choice{outputPortCount + 1}" : overritenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = portName
        };

        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemovePort(generatedPort)) { text = "X" };
        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName = portName;
        //oldLabel.text = "  ";

        generatedPort.AddManipulator(new EdgeConnector<Edge>(new MyIEdgeConnector(DialogueTreeView.GetInstance())));
        outputContainer.Add(generatedPort);
        RefreshPorts();
        RefreshExpandedState();
    }
    public override void AddPort(string overrittenName = "")
    {
        AddChoicePort(overrittenName);
    }

    private void RemovePort(Port generatedPort)
    {
        
        var targetEdge = DialogueTreeView.GetInstance().edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            DialogueTreeView.GetInstance().RemoveElement(targetEdge.First());
        }
        outputContainer.Remove(generatedPort);
        RefreshPorts();
        RefreshExpandedState();
    }
    public override void SetInputValue(int portIndex, object value)
    {
        overritten = true;
        textField.SetEnabled(false);
        overrittenText = value as string;
        DialogueText = overrittenText;
        title = overrittenText;
        textField.SetValueWithoutNotify(overrittenText);
        RefreshExpandedState();
    }
    public override void SetInputValue(Port port, object value)
    {
        overritten = true;
        textField.SetEnabled(false);
        overrittenText = value as string;
        DialogueText = overrittenText;
        title = overrittenText;
        textField.SetValueWithoutNotify(overrittenText);
        RefreshExpandedState();
    }
    public override void UnSetInput(int portIndex)
    {
        overritten = false;
        textField.SetEnabled(true);
    }
    public override void UnSetInput(Port port)
    {
        overritten = false;
        textField.SetEnabled(true);
    }
}
