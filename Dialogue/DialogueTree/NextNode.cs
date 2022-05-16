using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextNode : NodeData
{
    public override string getType()
    {
        return "Next Node";
    }
    public override NodeData createCopy()
    {
        return new NextNode
        {
            nodeGUID = nodeGUID,
            Position = Position,
        };
    }
    public override void Run(RuntimeDialogueContainer context)
    {
        UIManager.instance.HideDialogueWindow();
        PlotStep step = context.currentStep;

        if (step.completed && step.questFinnished)
        {
            // On reward dialogue
            context.currentStep.rewardClaimed = true;
            Player.instance.CompleteQuest(context.currentStep.currentQuest);
            
        } else context.currentStep.completed = true;
    }
}
