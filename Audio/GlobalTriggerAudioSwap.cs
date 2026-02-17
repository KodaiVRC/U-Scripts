using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GlobalTriggerAudioSwap : UdonSharpBehaviour
{
    [UdonSynced(UdonSyncMode.None)]
    private bool isInside = false;

    public AudioSource audioA; // Plays when NO ONE is inside
    public AudioSource audioB; // Plays when SOMEONE is inside

    private int localTriggerCount = 0;

    private void Start()
    {
        UpdateAudio();
    }

    private void OnTriggerEnter(Collider other)
    {
#if !UNITY_EDITOR
        if (other == null || other.name != "VRCPlayer_Local") return;
#endif
        localTriggerCount++;

        if (localTriggerCount == 1)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            isInside = true;
            RequestSerialization();
            UpdateAudio();
        }
    }

    private void OnTriggerExit(Collider other)
    {
#if !UNITY_EDITOR
        if (other == null || other.name != "VRCPlayer_Local") return;
#endif
        localTriggerCount--;

        if (localTriggerCount <= 0)
        {
            localTriggerCount = 0;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            isInside = false;
            RequestSerialization();
            UpdateAudio();
        }
    }

    public override void OnDeserialization()
    {
        UpdateAudio();
    }

    private void UpdateAudio()
    {
        if (audioA == null || audioB == null)
        {
            UnityEngine.Debug.LogWarning("[GlobalAudioTriggerSwitch] AudioSources not assigned.");
            return;
        }

        if (isInside)
        {
            if (audioA.isPlaying) audioA.Stop();
            if (!audioB.isPlaying) audioB.Play();
            UnityEngine.Debug.Log("[GlobalAudioTriggerSwitch] Playing Audio B (Inside).");
        }
        else
        {
            if (audioB.isPlaying) audioB.Stop();
            if (!audioA.isPlaying) audioA.Play();
            UnityEngine.Debug.Log("[GlobalAudioTriggerSwitch] Playing Audio A (Outside).");
        }
    }
}
