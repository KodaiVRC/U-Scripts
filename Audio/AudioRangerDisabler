using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Complex Interactive/Audio/Audio Range Disabler")]
public class AudioRangeDisabler : UdonSharpBehaviour
{
    [Header("Audio Source (Auto Assigned)")]
    [Tooltip("Cached AudioSource on this GameObject. This script enables/disables it based on player distance.")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Distance Check Settings")]
    [Tooltip("How often (in seconds) the distance check runs. Lower = more responsive, Higher = more performance friendly.")]
    [Range(0.05f, 2f)]
    public float checkInterval = 0.2f;

    [Header("Debug Options")]
    [Tooltip("If enabled, logs distance checks and state changes to the console.")]
    public bool enableDebugLogs = false;

    private float nextCheckTime = 0f;

    void Start()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("[AudioRangeDisabler] No AudioSource found on this GameObject.");
            return;
        }

        if (enableDebugLogs)
            Debug.Log("[AudioRangeDisabler] Initialized successfully.");
    }

    void Update()
    {
        if (Time.time < nextCheckTime)
            return;

        nextCheckTime = Time.time + checkInterval;

        if (_audioSource == null)
            return;

        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null)
            return;

        float distance = Vector3.Distance(localPlayer.GetPosition(), transform.position);

        bool shouldEnable = distance <= _audioSource.maxDistance;

        if (_audioSource.enabled != shouldEnable)
        {
            _audioSource.enabled = shouldEnable;

            if (enableDebugLogs)
            {
                Debug.Log(
                    $"[AudioRangeDisabler] Audio {(shouldEnable ? "Enabled" : "Disabled")} | Distance: {distance:F2} | Max: {_audioSource.maxDistance}"
                );
            }
        }
    }
}
