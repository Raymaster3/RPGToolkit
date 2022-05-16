using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotLine : MonoBehaviour
{
    public string plotLineName;
    public PlotStep[] steps;
     

    // Start is called before the first frame update
    void Start()
    {
        QuestsManager.instance.onQuestFinnish += (quest) => 
        { 
            foreach(PlotStep step in steps)
            {
                step.UpdateRequirements(quest);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartInteraction()
    {
        foreach (PlotStep step in steps)
        {
            //step.StartInteraction();
        }
    }
}
