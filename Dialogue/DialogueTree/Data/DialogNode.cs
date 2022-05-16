using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogNode : NodeData
{
    public string DialogueText;
    public bool overritten;

    public override string getType()
    {
        return "Dialogue Node";
    }
    public override void setInputData(object data)
    {
        DialogueText = data as string;
    }
    public override NodeData createCopy()
    {
        return new DialogNode
        {
            nodeGUID = nodeGUID,
            Position = Position,
            DialogueText = DialogueText,
            overritten = overritten
        };
    }
    public override void Run(RuntimeDialogueContainer context)
    {
        List<NodeLink> connections = context.NodeLinks.FindAll(x => x.BaseNodeGUID == nodeGUID);

        // We populate a view with the message of the dialogue
        UIManager.instance.ShowDialogueWindow(this, context);

        // For testing
        /*if (connections.Count == 0) return;
        context.DialogueNodes.Find(x => x.nodeGUID == connections[0].TargetNodeGUID).Run(context); */
    }
}
