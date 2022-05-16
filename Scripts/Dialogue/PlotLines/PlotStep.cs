using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlotStep : MonoBehaviour, IPlotSteps
{
    [HideInInspector] public DialogueContainer dialogue;
    [HideInInspector] public DialogueContainer defaultDialogue;
    [HideInInspector] public DialogueContainer rewardDialogue;

    public QuestState[] requirements;

    [HideInInspector] public bool completed = false;
    [HideInInspector] public bool questFinnished = false;
    [HideInInspector] public bool rewardClaimed = false;

    /// <summary>
    /// Set in GiveMissionNode, nullable
    /// </summary>
    [HideInInspector] public Quest currentQuest = null;

    [HideInInspector] public RuntimeDialogueContainer runtimeDialogue;
    [HideInInspector] public RuntimeDialogueContainer runtimeDefaultDialogue;
    [HideInInspector] public RuntimeDialogueContainer runtimeRewardDialogue;

    private void Start()
    {
        foreach (QuestState qs in requirements)
        {
            qs.quest = qs.data.GetRuntimeQuest();
        }
    }

    public void UpdateRequirements(Quest q)
    {
        foreach (QuestState quest in requirements)
        {
            if (quest.quest.name == q.name) {
                quest.requirementMet = true;
                return;
            }
        }
    }
    public bool CheckRequirements()
    {
        foreach (QuestState q in requirements) {
            if (!q.requirementMet) return false;
        }
        return true;
    }
    public void StartInteraction(QuestStepState state)
    {
        switch (state)
        {
            case QuestStepState.rewardDialogue:
                GenerateRewardRuntimeContainer(rewardDialogue);
                updateNodesData(2);
                runtimeRewardDialogue.currentStep = this;
                runtimeRewardDialogue.onCancelCallback = () => { }; // Reset
                runtimeRewardDialogue.DialogueNodes.Find(x => x.nodeGUID == runtimeRewardDialogue.NodeLinks[0].TargetNodeGUID).Run(runtimeRewardDialogue);
                break;
            case QuestStepState.defaultDialogue:
                GenerateRuntimeContainer(defaultDialogue, true);
                updateNodesData(1);
                runtimeDefaultDialogue.currentStep = this;
                runtimeDefaultDialogue.onCancelCallback = () => { }; // Reset
                runtimeDefaultDialogue.DialogueNodes.Find(x => x.nodeGUID == runtimeDefaultDialogue.NodeLinks[0].TargetNodeGUID).Run(runtimeDefaultDialogue);
                break;
            default:
                GenerateRuntimeContainer(dialogue);
                updateNodesData(0);
                runtimeDialogue.currentStep = this;
                runtimeDialogue.onCancelCallback = () => { }; // Reset
                runtimeDialogue.DialogueNodes.Find(x => x.nodeGUID == runtimeDialogue.NodeLinks[0].TargetNodeGUID).Run(runtimeDialogue);
                break;
        }


        /*if (completed || !CheckRequirements())
        {
            // Run default tree
            runtimeDefaultDialogue.currentStep = this;
            runtimeDefaultDialogue.DialogueNodes.Find(x => x.nodeGUID == runtimeDefaultDialogue.NodeLinks[0].TargetNodeGUID).Run(runtimeDefaultDialogue);
            return;
        }
        // Run the dialogue tree
        runtimeDialogue.currentStep = this;
        runtimeDialogue.DialogueNodes.Find(x => x.nodeGUID == runtimeDialogue.NodeLinks[0].TargetNodeGUID).Run(runtimeDialogue);*/
    }

    public void GenerateRuntimeContainer(DialogueContainer container, bool defaultDial = false)
    {
        RuntimeDialogueContainer runtimeContainer = runtimeDialogue;
        List<ExposedProperty> tempList;

        // Aqui cambiaremos las exposedProperties del runtimeContainer
        if (defaultDial)
        {
            defaultDialogue = container;
            tempList = runtimeDefaultDialogue.ExposedProperties;
            runtimeDefaultDialogue = new RuntimeDialogueContainer();
            runtimeContainer = runtimeDefaultDialogue;
        }else
        {
            dialogue = container;
            tempList = runtimeDialogue.ExposedProperties;
            runtimeDialogue = new RuntimeDialogueContainer();
            runtimeContainer = runtimeDialogue;
        }

        container.DialogueNodes.ForEach(x => runtimeContainer.DialogueNodes.Add(x.createCopy()));
        container.ExposedProperties.ForEach(x => runtimeContainer.ExposedProperties.Add(x.createDataCopy()));

        foreach (ExposedProperty property in tempList)
        {
            ExposedProperty tmp = runtimeContainer.ExposedProperties.Find(x => x.PropertyName == property.PropertyName);
            if (tmp == null)
                continue;
            tmp.setValue(property.getValue());
        }

        runtimeContainer.NodeLinks.AddRange(container.NodeLinks);
    }
    private void updateNodesData(int tree)
    {
        ExposedProperty[] properties;
        RuntimeDialogueContainer target = null;
        switch (tree)
        {
            case 0:     // Normal
                target = runtimeDialogue;
                break;
            case 1:     // Default
                target = runtimeDefaultDialogue;
                break;
            default:    // Reward
                target = runtimeRewardDialogue;
                break;
        }
        properties = target.ExposedProperties.ToArray();

        foreach (ExposedProperty property in properties)
        {
            target.DialogueNodes.FindAll(x =>
            {
                PropertyNode node = x as PropertyNode;
                if (node == null) return false;
                return node.property.PropertyName == property.PropertyName;
            }).ForEach(x => {
                List<NodeLink> connections = target.NodeLinks.FindAll(y => y.BaseNodeGUID == x.nodeGUID);
                foreach (NodeLink link in connections)
                {
                    target.DialogueNodes.Find(z => z.nodeGUID == link.TargetNodeGUID).setInputData(property.getValue());
                }
            });
        }
    }
    public void GenerateRewardRuntimeContainer(DialogueContainer container)
    {
        RuntimeDialogueContainer runtimeContainer = runtimeRewardDialogue;
        List<ExposedProperty> tempList;

        rewardDialogue = container;
        tempList = runtimeRewardDialogue.ExposedProperties;
        runtimeRewardDialogue = new RuntimeDialogueContainer();
        runtimeContainer = runtimeRewardDialogue;

        container.DialogueNodes.ForEach(x => runtimeContainer.DialogueNodes.Add(x.createCopy()));
        container.ExposedProperties.ForEach(x => runtimeContainer.ExposedProperties.Add(x.createDataCopy()));

        foreach (ExposedProperty property in tempList)
        {
            ExposedProperty tmp = runtimeContainer.ExposedProperties.Find(x => x.PropertyName == property.PropertyName);
            if (tmp == null)
                continue;
            tmp.setValue(property.getValue());
        }

        //runtimeContainer.ExposedProperties.AddRange(_tree.ExposedProperties);
        runtimeContainer.NodeLinks.AddRange(container.NodeLinks);
    }
}
[System.Serializable]
public class QuestState
{
    public QuestData data;
    [HideInInspector] public Quest quest;
    [HideInInspector] public bool requirementMet = false;
}
