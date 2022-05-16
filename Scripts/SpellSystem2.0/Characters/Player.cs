using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public static Player instance;
    public Item forQuestTest;

    [SerializeReference] public List<Quest> activeQuests = new List<Quest>();
    [SerializeReference] public List<Quest> completedQuests = new List<Quest>();

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;

        //activeQuests.Add(new ItemQuest(forQuestTest, 10));
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 1; i <= abilities.Count; i++)
        {
            if (Raycaster.instance.keyNumbers[i])
            {
                abilities[i-1]?.Cast(this, SpellsManager.getMouseWorldPos());
            }
        }
    }
    
    public override void BlockMovement()
    {
        GetComponentInChildren<CharMovement>().BlockMovement();
    }
    public override void StopMoving()
    {
        GetComponentInChildren<CharMovement>().StopMoving();
    }
    public override void UnBlockMovement()
    {
        GetComponentInChildren<CharMovement>().UnBlockMovement();
    }
    public override void UpdateMovingAnimator()
    {
        GetComponentInChildren<CharMovement>().UpdateAnimation();
    }
    public void updateQuests(object objective, int quant)
    {
        foreach(Quest q in activeQuests)
        {
            q.UpdateObjectives(objective, quant);
        }
        UIManager.instance.UpdateQuestsTab();
    }
    public void AcceptQuest(Quest q)
    {
        SoundManager.instance.PlayAcceptQuestSound();
        if (activeQuests == null) activeQuests = new List<Quest>();
        activeQuests.Add(q);
        q.CheckObjectives();
        UIManager.instance.UpdateQuestsTab();
    }
    public void CompleteQuest(Quest q)
    {
        activeQuests.Remove(q);
        completedQuests.Add(q);
        UIManager.instance.UpdateQuestsTab();
    } 
    public Quest[] getActiveQuests()
    {
        return activeQuests.ToArray();
    }
}
