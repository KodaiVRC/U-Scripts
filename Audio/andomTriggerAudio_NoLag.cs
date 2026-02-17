using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class RandomTriggerAudio_NoLag : UdonSharpBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clip;

    [Header("Chance")]
    [Range(0f, 1f)]
    public float playChance = 0.3f;

    [Header("Fade Settings")]
    public float fadeInTime = 1.5f;
    public float maxFadeDistance = 15f;

    private bool localInside = false;
    private bool isPlaying = false;
    private bool rngPassed = false;

    private float fadeInSpeed;
    private VRCPlayerApi localPlayer;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;

        if (audioSource == null || clip == null) return;

        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.clip = clip;
        audioSource.volume = 0f;

        fadeInSpeed = 1f / Mathf.Max(0.01f, fadeInTime);

        // Pre-decode
        audioSource.Play();
        audioSource.Stop();
    }

    void Update()
    {
        if (!isPlaying || localPlayer == null) return;

        if (localInside)
        {
            // Fade IN while inside
            audioSource.volume = Mathf.MoveTowards(
                audioSource.volume,
                1f,
                fadeInSpeed * Time.deltaTime
            );
        }
        else
        {
            // Distance-based fade OUT
            float distance = Vector3.Distance(
                localPlayer.GetPosition(),
                transform.position
            );

            float t = Mathf.Clamp01(distance / maxFadeDistance);
            audioSource.volume = 1f - t;

            if (audioSource.volume <= 0.01f)
            {
                audioSource.Stop();
                audioSource.volume = 0f;
                isPlaying = false;
                rngPassed = false;
            }
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        if (isPlaying) return;

        localInside = true;

        if (Random.value > playChance) return;

        rngPassed = true;
        isPlaying = true;

        audioSource.volume = 0f;
        audioSource.Play();
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        localInside = false;
    }
}
