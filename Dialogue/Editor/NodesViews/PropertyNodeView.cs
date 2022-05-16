using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class PropertyNodeView : NodeView
{
    [SerializeReference] ExposedProperty property;

    public void setProperty(ExposedProperty p) { property = p; }
    public ExposedProperty getProperty() { return property; }

    public override NodeView createNodeVisual(NodeData data)
    {
        PropertyNode nodeData = (PropertyNode) data;

        title = nodeData.property.PropertyName;
        GUID = Guid.NewGuid().ToString();
        property = nodeData.property;

        var outputPort = GeneratePort(nodeData.property.getType(), Direction.Output, Port.Capacity.Multi);

        KeyValuePair<Type, Color> keyValue = Utils.typeColors.Find(x => x.Key == nodeData.property.getType());
        if (keyValue.Key != null) outputPort.portColor = keyValue.Value; 
        outputContainer.Add(outputPort);

        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(nodeData.Position, new Vector2(150, 200)));

        return this;
    }
    public override NodeData GetNewObject()
    {
        NodeData data = new PropertyNode
        {
            nodeGUID = GUID,
            Position = GetPosition().position,
            property = property
            
        };
        return data;
    }


}
