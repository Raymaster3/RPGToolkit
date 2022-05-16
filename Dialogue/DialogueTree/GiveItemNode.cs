using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveItemNode : NodeData
{
    public Item item;
    public int quantity;

    public override string getType()
    {
        return "Give Item Node";
    }
    public override void setInputData(object data)
    {
        if (data as Item != null)
            item = data as Item;
        else quantity = (int) data;
    }
    public override NodeData createCopy()
    {
        return new GiveItemNode
        {
            nodeGUID = nodeGUID,
            Position = Position,
            item = item,
            quantity = quantity
        };
    }
    public override void Run(RuntimeDialogueContainer context)
    {
        List<NodeLink> connections = context.NodeLinks.FindAll(x => x.BaseNodeGUID == nodeGUID); // We have 2 connections
        NodeData next = null;                                                                       
        NodeData fail = null;                                                                    
        
        if (connections.Count < 2) // User left one port unplugged
        {
            if (connections.Count != 0)
                context.DialogueNodes.Find(x => x.nodeGUID == connections[0].TargetNodeGUID).Run(context);
            return;
        } 


        foreach (NodeLink connection in connections)
        {
            if (connection.originPort == 0) next = context.DialogueNodes.Find(x => x.nodeGUID == connection.TargetNodeGUID);
            else fail = context.DialogueNodes.Find(x => x.nodeGUID == connection.TargetNodeGUID);
        }

        bool somePlaced = false;
        if (Player.instance.getInventory().addItem(item, out somePlaced, (ushort) quantity) > 0)
        {
            // Fails
            if (somePlaced)
                Player.instance.getInventory().removeItem(item);
            context.onCancelCallback?.Invoke();
            fail.Run(context);
        }
        else
        {
            context.onCancelCallback += () => { Player.instance.getInventory().removeItem(item); };
            next.Run(context);
        }
  }
}
