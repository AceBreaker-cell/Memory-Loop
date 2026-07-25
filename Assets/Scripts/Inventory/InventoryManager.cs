// InventoryManager.cs
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private List<InventoryItem> _items = new List<InventoryItem>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(InventoryItem item)
    {
        if (_items.Contains(item)) return;
        _items.Add(item);
        HUDManager.Instance?.RefreshInventoryBadge(_items.Count);
        Debug.Log($"[Inventory] Added: {item.itemName}");
    }

    public List<InventoryItem> GetItems() => _items;
}