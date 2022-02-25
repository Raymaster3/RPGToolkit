using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "RPGToolkit/Stats/Basic", order = 1)]
public class Stat : ScriptableObject
{
    [SerializeField]
    private string statName;
    [SerializeField]
    private string description;
    [SerializeField, ColorUsage(true, true)]
    private Color tooltipColor;

    public Color getColor()
    {
        return tooltipColor;
    }
    public string getName() { return statName; }
}
