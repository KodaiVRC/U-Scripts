using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AutoDoorSlide : UdonSharpBehaviour
{
    [Header("Door Settings")]
    public GameObject[] doors;
    public Vector3[] doorsClosedPosition;
    public Vector3[] doorsOpenPosition;

    [Header("Door Timing")]
    public float doorSpeed = 3f;
    public float doorOpenDuration = 3f;

    [Header("Audio")]
    public AudioClip soundOpenDoor;
    public AudioClip soundCloseDoor;
    private AudioSource doorSound;

    private float doorCloseTime;
    private bool openDoors;
    private bool playedCloseSound;

    // Track players inside trigger
    private VRCPlayerApi[] playersInZone = new VRCPlayerApi[64]; // supports up to 64 players
    private int playerCount = 0;

    void Start()
    {
        playedCloseSound = true;
        doorSound = GetComponent<AudioSource>();

        if (doorsClosedPosition == null || doorsClosedPosition.Length != doors.Length)
            doorsClosedPosition = new Vector3[doors.Length];

        for (int i = 0; i < doors.Length; i++)
            doorsClosedPosition[i] = doors[i].transform.localPosition;
    }

    void Update()
    {
        if (openDoors)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                Vector3 vecDistanceLeft = doors[i].transform.localPosition - doorsOpenPosition[i];
                float doorSpeedModifier = vecDistanceLeft.magnitude > 0 ? 1 / vecDistanceLeft.magnitude : 1f;
                doors[i].transform.localPosition = Vector3.Lerp(
                    doors[i].transform.localPosition,
                    doorsOpenPosition[i],
                    Time.deltaTime * doorSpeed * doorSpeedModifier
                );
            }
        }
        else if (doorCloseTime <= Time.time)
        {
            if (!playedCloseSound)
            {
                playedCloseSound = true;
                if (soundCloseDoor != null && doorSound != null)
                {
                    doorSound.clip = soundCloseDoor;
                    doorSound.Play();
                }
            }

            for (int i = 0; i < doors.Length; i++)
            {
                Vector3 vecDistanceLeft = doors[i].transform.localPosition - doorsClosedPosition[i];
                float doorSpeedModifier = vecDistanceLeft.magnitude > 0 ? 1 / vecDistanceLeft.magnitude : 1f;
                doors[i].transform.localPosition = Vector3.Lerp(
                    doors[i].transform.localPosition,
                    doorsClosedPosition[i],
                    Time.deltaTime * doorSpeed * doorSpeedModifier
                );
            }
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!Utilities.IsValid(player)) return;

        if (!IsPlayerInside(player))
        {
            AddPlayer(player);

            if (playerCount == 1) // first player
            {
                openDoors = true;
                if (soundOpenDoor != null && doorSound != null)
                {
                    doorSound.clip = soundOpenDoor;
                    doorSound.Play();
                }
            }
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!Utilities.IsValid(player)) return;

        if (IsPlayerInside(player))
        {
            RemovePlayer(player);

            if (playerCount == 0) // last player left
            {
                doorCloseTime = Time.time + doorOpenDuration;
                openDoors = false;
                playedCloseSound = false;
            }
        }
    }

    private bool IsPlayerInside(VRCPlayerApi player)
    {
        for (int i = 0; i < playerCount; i++)
        {
            if (playersInZone[i] == player)
                return true;
        }
        return false;
    }

    private void AddPlayer(VRCPlayerApi player)
    {
        if (playerCount < playersInZone.Length)
        {
            playersInZone[playerCount] = player;
            playerCount++;
        }
    }

    private void RemovePlayer(VRCPlayerApi player)
    {
        for (int i = 0; i < playerCount; i++)
        {
            if (playersInZone[i] == player)
            {
                playersInZone[i] = playersInZone[playerCount - 1]; // swap last
                playersInZone[playerCount - 1] = null;
                playerCount--;
                return;
            }
        }
    }
}

