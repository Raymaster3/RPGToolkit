using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeData
{
    public string nodeGUID;
    public Vector2 Position;
    
    public virtual string getType()
    {
        return "Basic";
    }
    public virtual NodeData createCopy()
    {
        return new NodeData();
    }
    public virtual void setInputData(object data)
    {

    }
    public virtual void Run(RuntimeDialogueContainer context)
    {
        NodeLink nextNodeLink = context.NodeLinks.Find(x => x.BaseNodeGUID == nodeGUID);
        NodeData nextNode = context.DialogueNodes.Find(x => x.nodeGUID == nextNodeLink.TargetNodeGUID);
        nextNode.Run(context);
    }
}
