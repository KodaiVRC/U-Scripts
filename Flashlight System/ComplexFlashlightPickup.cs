using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ComplexFlashlightPickup : UdonSharpBehaviour
{
    [Tooltip("Reference to the flashlight system to enable when picked up.")]
    public GameObject flashlightSystem;

    [Tooltip("Sound that plays when the flashlight is picked up.")]
    public AudioSource pickupSound;

    private VRCPlayerApi localPlayer;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
    }

    public override void Interact()
    {
        if (localPlayer == null) return;

        if (flashlightSystem != null)
            flashlightSystem.SetActive(true); // enable the player's head flashlight

        if (pickupSound != null)
            pickupSound.Play();

        gameObject.SetActive(false); // remove world pickup mesh
    }
}
