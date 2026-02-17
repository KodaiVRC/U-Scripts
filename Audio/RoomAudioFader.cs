using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RoomAudioFader : UdonSharpBehaviour
{
    public AudioSource roomAudio;
    public float fadeDuration = 2f;

    [Range(0f, 1f)]
    public float targetVolume = 1f; // Set this in the Inspector to limit max volume

    private bool fadingIn = false;
    private bool fadingOut = false;

    private float fadeTimer = 0f;
    private float fadeStartVolume = 0f;
    private float fadeEndVolume = 1f;

    private void Start()
    {
        if (roomAudio != null)
        {
            roomAudio.volume = 0f;
            roomAudio.loop = true;
            roomAudio.Play(); // Always playing
        }
    }

    private void Update()
    {
        if (!fadingIn && !fadingOut) return;

        fadeTimer += Time.deltaTime;
        float t = Mathf.Clamp01(fadeTimer / fadeDuration);
        roomAudio.volume = Mathf.Lerp(fadeStartVolume, fadeEndVolume, t);

        if (t >= 1f)
        {
            fadingIn = false;
            fadingOut = false;
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        fadeTimer = 0f;
        fadeStartVolume = roomAudio.volume;
        fadeEndVolume = targetVolume; // Fade to set max volume

        fadingOut = false;
        fadingIn = true;
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        fadeTimer = 0f;
        fadeStartVolume = roomAudio.volume;
        fadeEndVolume = 0f; // Fade to silence

        fadingIn = false;
        fadingOut = true;
    }
}
