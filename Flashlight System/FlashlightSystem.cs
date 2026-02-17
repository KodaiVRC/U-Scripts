using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class FlashlightSystem : UdonSharpBehaviour
{
    [Header("Flashlight")]
    public Light flashlight;

    [Header("Audio")]
    public AudioSource toggleAudioSource;
    public AudioClip onSound;
    public AudioClip offSound;

    [Header("Sway")]
    public float swaySpeed = 2f;
    public float swayAngle = 2f;

    [Header("Input")]
    public float doubleTapTime = 0.35f;

    [Header("Battery")]
    public float maxBattery = 100f;
    public float startBattery = 100f;
    public float batteryDrainRate = 5f;     // per second
    public float lowBatteryThreshold = 25f; // percent-based threshold
    public float flickerInterval = 0.12f;

    [Header("HUD")]
    public RedRoomsHUDManager hudManager;

    private float currentBattery;
    private bool isOn;
    private float swayTimer;
    private float flickerTimer;

    private float lastTriggerTime = -1f;
    private bool triggerPreviouslyDown;

    private bool batteryLowSent; // 🔴 prevents spam
    private VRCPlayerApi localPlayer;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
        currentBattery = Mathf.Clamp(startBattery, 0f, maxBattery);

        if (flashlight != null)
            flashlight.enabled = false;

        UpdateBatteryHUD();
    }

    void Update()
    {
        if (!Utilities.IsValid(localPlayer)) return;

        HandleToggleInput();

        if (isOn)
            DrainBattery();

        UpdateTransform();
    }

    /* ================= INPUT ================= */

    private void HandleToggleInput()
    {
        bool toggle =
            (!localPlayer.IsUserInVR() && Input.GetKeyDown(KeyCode.F)) ||
            CheckDoubleTriggerVR();

        if (!toggle || flashlight == null) return;

        // Prevent turning on with dead battery
        if (!isOn && currentBattery <= 0f)
            return;

        isOn = !isOn;
        flashlight.enabled = isOn;
        PlayToggleSound();
    }

    /* ================= BATTERY ================= */

    private void DrainBattery()
    {
        if (currentBattery <= 0f)
        {
            ForceShutoff();
            return;
        }

        currentBattery -= batteryDrainRate * Time.deltaTime;
        currentBattery = Mathf.Max(0f, currentBattery);

        // Flicker when low battery
        if (currentBattery <= lowBatteryThreshold)
        {
            flickerTimer += Time.deltaTime;
            if (flickerTimer >= flickerInterval)
            {
                flashlight.enabled = !flashlight.enabled;
                flickerTimer = 0f;
            }
        }
        else
        {
            flashlight.enabled = true;
        }

        UpdateBatteryHUD();
    }

    private void ForceShutoff()
    {
        currentBattery = 0f;
        isOn = false;

        if (flashlight != null)
            flashlight.enabled = false;

        PlayToggleSound();
        UpdateBatteryHUD();
    }

    public void AddBatteryOverTime(float amount)
    {
        if (amount <= 0f) return;

        float space = maxBattery - currentBattery;
        if (space <= 0f) return;

        currentBattery += Mathf.Min(amount, space);
        UpdateBatteryHUD();
    }

    /* ================= HUD ================= */

    private void UpdateBatteryHUD()
    {
        if (hudManager == null) return;

        // Update bar
        hudManager.UpdateBatteryUI(currentBattery, maxBattery);

        // 🔴 LOW BATTERY ICON LOGIC
        float percent = (currentBattery / maxBattery) * 100f;
        bool isLow = percent <= 25f;

        if (isLow != batteryLowSent)
        {
            batteryLowSent = isLow;
            hudManager.SetBatteryLow(isLow);
        }
    }

    /* ================= TRANSFORM ================= */

    private void UpdateTransform()
    {
        VRCPlayerApi.TrackingData head =
            localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

        transform.position =
            head.position + head.rotation * new Vector3(0f, 0f, 0.2f);

        swayTimer += Time.deltaTime * swaySpeed;

        float swayX = Mathf.Sin(swayTimer) * swayAngle;
        float swayY = Mathf.Cos(swayTimer * 0.8f) * swayAngle * 0.5f;

        transform.rotation =
            head.rotation * Quaternion.Euler(swayX, swayY, 0f);
    }

    /* ================= VR INPUT ================= */

    private bool CheckDoubleTriggerVR()
    {
        if (!localPlayer.IsUserInVR()) return false;

        float triggerValue =
            Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger");

        bool triggerDown = triggerValue > 0.85f;

        if (triggerDown && !triggerPreviouslyDown)
        {
            float now = Time.time;
            if (now - lastTriggerTime <= doubleTapTime)
            {
                lastTriggerTime = -1f;
                triggerPreviouslyDown = true;
                return true;
            }
            lastTriggerTime = now;
        }

        triggerPreviouslyDown = triggerDown;
        return false;
    }

    private void PlayToggleSound()
    {
        if (toggleAudioSource == null) return;

        AudioClip clip = isOn ? onSound : offSound;
        if (clip != null)
            toggleAudioSource.PlayOneShot(clip);
    }
}
