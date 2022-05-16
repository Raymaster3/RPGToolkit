using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Rarity", menuName = "RPGToolkit/Items/Rarities/Basic", order = 1)]
public class ItemRarity : ScriptableObject
{
    [SerializeField] private string rarityName;
    [ColorUsage(true, true)]
    [SerializeField] private Color color;

    public Color getColor() { return color; }
}
