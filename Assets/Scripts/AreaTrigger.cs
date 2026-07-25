using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public enum TriggerType { Kitchen, Bed }
    public TriggerType triggerType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (triggerType)
        {
            case TriggerType.Kitchen:
                GameManager.Instance?.TriggerKitchenDialogue();
                break;
            case TriggerType.Bed:
                // Bed bisa juga di-interact manual via InspectableObject
                break;
        }
    }
}