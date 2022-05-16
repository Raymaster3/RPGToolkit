using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MyIEdgeConnector : IEdgeConnectorListener
{
    private DialogueTreeView _graphview;

    public MyIEdgeConnector(DialogueTreeView graphview)
    {
        _graphview = graphview;
    }
    public void OnDrop(GraphView graphView, Edge edge)
    {
        
    }

    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
        NodeCreationContext context = new NodeCreationContext();
        context.screenMousePosition = position;
        _graphview.nodeCreationRequest.Invoke(context);
    }
}
