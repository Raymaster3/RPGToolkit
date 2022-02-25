using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Relation
{
    [SerializeField] Faction faction;
    [SerializeField] RelationType relationType;
    public RelationType RelationType { get => relationType; }
    public Faction Faction { get => faction; }
}
