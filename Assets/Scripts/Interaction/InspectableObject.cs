using UnityEngine;

public class InspectableObject : MonoBehaviour, IInteractable
{
    [Header("Info")]
    public string objectName = "Foto Lama";
    public string hintText   = "[E] Lihat";

    [Header("Inspect Lines")]
    [TextArea(2, 4)]
    public string[] lines;

    [Header("Inspect Image (opsional)")]
    public Sprite inspectSprite;          // gambar yang ditampilkan di kertas

    [Header("Add to Inventory?")]
    public bool addToInventory = false;
    public InventoryItem inventoryItem;

    public void Interact()
    {
        InspectManager.Instance?.OpenInspect(this);
    }

    public string GetHintText() => hintText;
}