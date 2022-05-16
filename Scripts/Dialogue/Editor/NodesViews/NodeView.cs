using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeView : Node
{
    public string GUID;
    public bool EntryPoint = false;

    public virtual NodeData GetNewObject()
    {
        return new NodeData();
    }
    public virtual NodeView createNodeVisual(NodeData data)
    {
        return new NodeView();
    }
    public virtual Port GeneratePort(Type type, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        Port port = InstantiatePort(Orientation.Horizontal, portDirection, capacity, type);
        return port;
    }
    public virtual void AddPort(string overrittenName = "")
    {

    }
    public virtual void SetInputValue(int portIndex, object value)
    {

    }
    public virtual void SetInputValue(Port port, object value)
    {

    }
    public virtual void UnSetInput(int portIndex)
    {

    }
    public virtual void UnSetInput(Port port)
    {

    }
}
