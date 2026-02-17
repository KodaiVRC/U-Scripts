using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FloatingObjects : UdonSharpBehaviour
{
    [SerializeField] private Transform[] objects; // Array of objects to move
    [SerializeField] private float movementSpeed = 0.2f; // Speed of movement
    [SerializeField] private float movementRange = 0.5f; // Range of movement

    private Vector3[] initialPositions; // Store the initial positions of objects
    private Vector3[] randomOffsets; // Randomized offsets for each object

    void Start()
    {
        // Initialize arrays
        initialPositions = new Vector3[objects.Length];
        randomOffsets = new Vector3[objects.Length];

        // Store initial positions and generate random offsets
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                initialPositions[i] = objects[i].position;
                randomOffsets[i] = new Vector3(
                    Random.Range(0f, 10f),
                    Random.Range(0f, 10f),
                    Random.Range(0f, 10f)
                );
            }
        }
    }

    void Update()
    {
        // Update the position of each object
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                Vector3 newPosition = initialPositions[i] +
                    new Vector3(
                        Mathf.Sin(Time.time * movementSpeed + randomOffsets[i].x) * movementRange,
                        Mathf.Cos(Time.time * movementSpeed + randomOffsets[i].y) * movementRange,
                        Mathf.Sin(Time.time * movementSpeed + randomOffsets[i].z) * movementRange
                    );

                objects[i].position = newPosition;
            }
        }
    }
}
