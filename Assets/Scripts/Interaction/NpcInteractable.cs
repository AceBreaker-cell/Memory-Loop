using UnityEngine;

public class NpcInteractable : MonoBehaviour, IInteractable
{
    public enum NpcRole { IbuOutside, IbuKitchen }
    public NpcRole role;

    public string GetHintText() => "[E] Bicara";

    public void Interact()
    {
        switch (role)
        {
            case NpcRole.IbuOutside:
                // Final Loop hanya punya TriggerOutsideDialogue
                if      (FinalLoopManager.Instance != null) FinalLoopManager.Instance.TriggerOutsideDialogue();
                else if (Loop3Manager.Instance     != null) Loop3Manager.Instance.TriggerOutsideDialogue();
                else if (Loop2Manager.Instance     != null) Loop2Manager.Instance.TriggerOutsideDialogue();
                else if (Loop1Manager.Instance     != null) Loop1Manager.Instance.TriggerOutsideDialogue();
                else                                        GameFlowManager.Instance?.TriggerOutsideDialogue();
                break;

            case NpcRole.IbuKitchen:
                // Final Loop TIDAK punya TriggerKitchenDialogue — skip
                if      (Loop3Manager.Instance != null) Loop3Manager.Instance.TriggerKitchenDialogue();
                else if (Loop2Manager.Instance != null) Loop2Manager.Instance.TriggerKitchenDialogue();
                else if (Loop1Manager.Instance != null) Loop1Manager.Instance.TriggerKitchenDialogue();
                else                                    GameFlowManager.Instance?.TriggerKitchenDialogue();
                break;
        }
    }
}