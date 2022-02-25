using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Faction", menuName = "RPGToolkit/Faction", order = 1)]
public class Faction : ScriptableObject
{
    [SerializeField] private string factionName;
    [SerializeField] private Relation[] relations; 

    public string FactionName { get => factionName; }
    public Relation[] Relations { get => relations; }

    public RelationType getRealtionStatus(Faction fact, out Color relColor)
    {
        RelationType rt = getRealtionStatus(fact);
        switch (rt)
        {
            case RelationType.Friendly:
                relColor = Color.blue;
                break;
            case RelationType.Neutral:
                relColor = Color.yellow;
                break;
            case RelationType.Enemy:
                relColor = Color.red;
                break;
            default:
                relColor = Color.gray;
                break;
        }
        return rt;
    }
    public RelationType getRealtionStatus(Faction fact)
    {
        foreach (Relation r in relations)
        {
            if (r.Faction.FactionName == fact.FactionName)
            {
                return r.RelationType;
            }
        }
        return RelationType.Unknown;
    }
}
public enum RelationType
{
    Friendly,
    Neutral,
    Enemy,
    Unknown
}