// InventoryItem.cs — ScriptableObject untuk tiap item
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public string  itemName;
    public Sprite  icon;
    [TextArea] public string description;
}