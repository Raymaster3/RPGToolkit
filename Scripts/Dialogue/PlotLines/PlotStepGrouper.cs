using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotStepGrouper : MonoBehaviour, IPlotSteps
{
    private PlotStep[] steps;


    public void StartInteraction(QuestStepState state)
    {
        //foreach (PlotStep step in steps) step.StartInteraction();
    }

    public void UpdateRequirements(Quest q)
    {
        foreach (PlotStep step in steps) step.UpdateRequirements(q);
    }

    // Start is called before the first frame update
    void Start()
    {
        steps = GetComponents<PlotStep>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
