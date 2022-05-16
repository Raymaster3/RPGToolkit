using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndNode : NodeData
{
    public override string getType()
    {
        return "End Node";
    }
    public override NodeData createCopy()
    {
        return new EndNode
        {
            nodeGUID = nodeGUID,
            Position = Position,
        };
    }
    public override void Run(RuntimeDialogueContainer context)
    {
        UIManager.instance.HideDialogueWindow();
    }
}
