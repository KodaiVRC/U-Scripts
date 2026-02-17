using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class FlashLightBatteryHUD : UdonSharpBehaviour
{
    [Header("🔋 HUD Setup")]
    [Tooltip("Reference to the flashlight system.")]
    public FlashlightSystem flashlightSystem;

    [Tooltip("UI Slider representing battery life.")]
    public Slider batterySlider;

    [Tooltip("Optional text to display battery percentage.")]
    public Text batteryText;

    private VRCPlayerApi localPlayer;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;

        if (batterySlider != null)
        {
            batterySlider.minValue = 0f;
            batterySlider.maxValue = 1f;
        }
    }

    void Update()
    {
        if (flashlightSystem == null || !flashlightSystem.useBattery || flashlightSystem.maxBatteryLife <= 0f)
        {
            if (batterySlider != null) batterySlider.gameObject.SetActive(false);
            return;
        }

        if (batterySlider != null)
        {
            batterySlider.gameObject.SetActive(true);
            batterySlider.value = flashlightSystem.batteryLife / flashlightSystem.maxBatteryLife;
        }

        if (batteryText != null)
        {
            float percent = (flashlightSystem.batteryLife / flashlightSystem.maxBatteryLife) * 100f;
            batteryText.text = Mathf.RoundToInt(percent).ToString() + "%";
        }
    }
}
