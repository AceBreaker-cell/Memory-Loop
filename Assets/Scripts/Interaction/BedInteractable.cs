using UnityEngine;

public class BedInteractable : MonoBehaviour, IInteractable
{
    public string GetHintText() => "[E] Tidur";

    public void Interact()
    {
        if      (FinalLoopManager.Instance != null) FinalLoopManager.Instance.TriggerSleep();
        else if (Loop3Manager.Instance     != null) Loop3Manager.Instance.TriggerSleep();
        else if (Loop2Manager.Instance     != null) Loop2Manager.Instance.TriggerSleep();
        else if (Loop1Manager.Instance     != null) Loop1Manager.Instance.TriggerSleep();
        else                                        GameFlowManager.Instance?.TriggerSleep();
    }
}