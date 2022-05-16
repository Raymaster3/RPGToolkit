using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestsManager : MonoBehaviour
{
    public static QuestsManager instance;

    public Action<Quest> onQuestFinnish;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void QuestFinnished(Quest q)
    {
        onQuestFinnish?.Invoke(q);
    }
}
