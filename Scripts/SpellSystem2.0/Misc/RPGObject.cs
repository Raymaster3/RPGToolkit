using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RPGObject : ScriptableObject
{
    [SerializeField] protected string objectName;
    [SerializeField] protected string description;
    [SerializeField] protected Sprite icon;
    [HideInInspector] public int index;

    public virtual string Name { get => objectName; }
    public virtual string Description { get => description; }
    public Sprite Icon { get => icon; }

    public virtual string getDescription()
    {
        return description;
    }
    public virtual void Swap(RPGObject with, Character owner = null, ushort ind = 0)
    {

    }
    public virtual void Cast(Character caster, Vector3 pos)
    {

    }
    public virtual string GetMyType()
    {
        return "";
    }
}
