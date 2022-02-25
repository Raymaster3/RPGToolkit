using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGObject : ScriptableObject
{
    [SerializeField] protected string objectName;
    [SerializeField] protected string description;
    [SerializeField] protected Sprite icon;

    public string Name { get => objectName; }
    public string Description { get => description; }
    public Sprite Icon { get => icon; }
}
