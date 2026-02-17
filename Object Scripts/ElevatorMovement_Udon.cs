using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class ElevatorMovement_Udon : UdonSharpBehaviour
{
    [Header("Floors")]
    public int floorCount = 5;
    public float floorHeight = 3f;

    [Header("Movement")]
    public float travelTimePerFloor = 2f;
    public float stopDelay = 2f;

    [Header("Easing")]
    public float easeStrength = 2f;

    private int currentFloor = 0;
    private int targetFloor = 0;

    private Vector3 basePosition;
    private Vector3 startPos;
    private Vector3 targetPos;

    private float moveTimer = 0f;
    private float moveDuration = 0f;
    private float stopTimer = 0f;

    private bool isMoving = false;

    void Start()
    {
        basePosition = transform.position;
        PickNextFloor();
    }

    void Update()
    {
        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(moveTimer / moveDuration);
            float easedT = EaseInOut(t);

            transform.position = Vector3.Lerp(startPos, targetPos, easedT);

            if (t >= 1f)
            {
                transform.position = targetPos;
                currentFloor = targetFloor;
                isMoving = false;
                stopTimer = 0f;
            }
        }
        else
        {
            stopTimer += Time.deltaTime;
            if (stopTimer >= stopDelay)
            {
                PickNextFloor();
            }
        }
    }

    void PickNextFloor()
    {
        int next;
        do
        {
            next = Random.Range(0, floorCount);
        }
        while (next == currentFloor);

        targetFloor = next;

        startPos = transform.position;
        targetPos = basePosition + Vector3.up * (targetFloor * floorHeight);

        float floorsTraveled = Mathf.Abs(targetFloor - currentFloor);
        moveDuration = travelTimePerFloor * floorsTraveled;

        moveTimer = 0f;
        isMoving = true;
    }

    float EaseInOut(float t)
    {
        return Mathf.Pow(t, easeStrength) /
               (Mathf.Pow(t, easeStrength) + Mathf.Pow(1f - t, easeStrength));
    }
}
