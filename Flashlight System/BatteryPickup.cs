using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BatteryPickup : UdonSharpBehaviour
{
    [Header("🔋 Battery Pickup Settings")]
    [Tooltip("Flashlight system to recharge.")]
    public FlashlightSystem flashlightSystem;

    [Tooltip("Amount of battery to restore. Set to -1 to fully recharge.")]
    public float rechargeAmount = -1f;

    [Tooltip("Sound to play when picked up.")]
    public AudioSource pickupSound;

    [Tooltip("Optional visual object to disable after pickup.")]
    public GameObject batteryModel;

    [Tooltip("Seconds before the battery respawns.")]
    public float respawnTime = 30f;

    private bool isAvailable = true;
    private float respawnTimer = 0f;

    void Update()
    {
        if (!isAvailable)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0f)
                ResetPickup();
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!isAvailable || flashlightSystem == null || player != Networking.LocalPlayer)
            return;

        flashlightSystem.RechargeBattery(rechargeAmount);
        PlayPickupSound();

        if (batteryModel != null)
            batteryModel.SetActive(false);

        isAvailable = false;
        respawnTimer = respawnTime;
    }

    private void PlayPickupSound()
    {
        if (pickupSound != null)
            pickupSound.Play();
    }

    private void ResetPickup()
    {
        if (batteryModel != null)
            batteryModel.SetActive(true);

        isAvailable = true;
    }
}
