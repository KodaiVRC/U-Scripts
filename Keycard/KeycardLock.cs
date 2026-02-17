using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class KeycardLock : UdonSharpBehaviour
{
    [Tooltip("The required key ID for this lock.")]
    public string requiredKeyID = "AlphaKey";

    [Tooltip("Animator that will be triggered on successful keycard insert.")]
    public Animator targetAnimator;

    [Tooltip("Trigger name to call in the Animator.")]
    public string unlockTriggerName = "Unlock";

    [Tooltip("Optional: Audio source to play when the animation is triggered.")]
    public AudioSource unlockAudio;

    private bool isUnlocked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isUnlocked) return;

        Keycard key = other.GetComponent<Keycard>();
        if (key != null && key.keyID == requiredKeyID)
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        isUnlocked = true;

        if (targetAnimator != null && !string.IsNullOrWhiteSpace(unlockTriggerName))
        {
            targetAnimator.SetTrigger(unlockTriggerName);
        }

        if (unlockAudio != null && !unlockAudio.isPlaying)
        {
            unlockAudio.Play();
        }

        UnityEngine.Debug.Log("Keycard accepted. Animation and sound triggered.");
    }
}
