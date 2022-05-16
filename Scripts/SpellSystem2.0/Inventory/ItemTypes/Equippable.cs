using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equippable", menuName = "RPGToolkit/Items/Equippable", order = 1)]
public class Equippable : Item
{
    [SerializeField] private EquipPosition equipPosition;
    [SerializeField] private StatValueEffect[] statsToModify; // Stats this item alter 
    [SerializeField] private Mesh visual;
    [SerializeField] private Material material;               // To define its color
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private bool showOnBothHands;

    [SerializeField] private AnimationState[] animationsOverride;
    

    public EquipPosition Position { get => equipPosition; }

    public override void RightClickAction(Character caster, ushort position)
    {
        // Equip
        caster.getEquipment().placeItem(this, caster, position);
    }
    public StatValueEffect[] getStats() { return statsToModify; }
    public Mesh getVisual() { return visual; }
    public Material getMat() { return material; }
    public AnimationState[] getAnimationsOverrides() { return animationsOverride; }
    public GameObject getPrefab() { return weaponPrefab; }
    public bool getShowOnBothHands() { return showOnBothHands; }
}

[System.Serializable]
public class AnimationState
{
    public string stateName;
    public AnimationClip _clip;

    public AnimationState(string sn, AnimationClip cl)
    {
        stateName = sn;
        _clip = cl;
    }
}
