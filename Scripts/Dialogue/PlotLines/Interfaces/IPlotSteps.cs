using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlotSteps
{
    void UpdateRequirements(Quest q);
    //bool CheckRequirements();
    void StartInteraction(QuestStepState state);
}
