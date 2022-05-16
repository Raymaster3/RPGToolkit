using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour, IEventsHandler
{
    List<PlotStep> rewards = new List<PlotStep>();
    List<PlotStep> defaults = new List<PlotStep>();
    List<PlotStep> quests = new List<PlotStep>();

    public void onMouseClick(int button)
    {
        if (button == 0) return;
        rewards = new List<PlotStep>();
        defaults = new List<PlotStep>();
        quests = new List<PlotStep>();
        // Populate the lists
        foreach (PlotStep step in GetComponentsInChildren<PlotStep>())
        {
            if (step.rewardClaimed) continue;
            if (step.completed)
            {
                if (step.questFinnished)
                    // Reward dialogue
                    rewards.Add(step);
                else
                    // Default dialogue
                    defaults.Add(step);
            }
            else
                // Mission dialogue
                if (step.CheckRequirements())
                quests.Add(step);
        }
        if (rewards.Count > 0)
        {
            rewards[0].StartInteraction(QuestStepState.rewardDialogue);
            return;
        }
        if (defaults.Count > 0)
        {
            defaults[0].StartInteraction(QuestStepState.defaultDialogue);
            return;
        }
        if (quests.Count > 0)
        {
            quests[0].StartInteraction(QuestStepState.questDialogue);
        }
    }

    public void onMouseEnter()
    {
    }

    public void onMouseExit()
    {
    }
}
public enum QuestStepState
{
    rewardDialogue,
    defaultDialogue,
    questDialogue
}
