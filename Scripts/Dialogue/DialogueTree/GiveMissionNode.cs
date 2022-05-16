using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveMissionNode : NodeData
{
    public QuestData quest;

    public override string getType()
    {
        return "Give Mission Node";
    }
    public override void setInputData(object data)
    {
        quest = data as QuestData;
    }
    public override NodeData createCopy()
    {
        return new GiveMissionNode
        {
            nodeGUID = nodeGUID,
            Position = Position,
            quest = quest
        };
    }
    public override void Run(RuntimeDialogueContainer context)
    {
        Quest q = quest.GetRuntimeQuest();
        q.questGiver = context.currentStep;
        Player.instance.AcceptQuest(q);

        context.currentStep.currentQuest = q;

        context.DialogueNodes.Find(y => y.nodeGUID == context.NodeLinks.Find(x => x.BaseNodeGUID == nodeGUID).TargetNodeGUID).Run(context);
    }
}
