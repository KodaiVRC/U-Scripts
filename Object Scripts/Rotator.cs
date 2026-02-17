using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Rotator : UdonSharpBehaviour
{
    [Tooltip("Rotation speed in degrees per second.")]
    public float speed = 10f;

    [Header("Forward Rotation")]
    public bool ForwardX = false;
    public bool ForwardY = false;
    public bool ForwardZ = false;

    [Header("Reverse Rotation")]
    public bool ReverseX = false;
    public bool ReverseY = false;
    public bool ReverseZ = false;

    void Update()
    {
        Vector3 rotation = Vector3.zero;

        // Forward rotation
        if (ForwardX) rotation.x += speed * Time.deltaTime;
        if (ForwardY) rotation.y += speed * Time.deltaTime;
        if (ForwardZ) rotation.z += speed * Time.deltaTime;

        // Reverse rotation
        if (ReverseX) rotation.x -= speed * Time.deltaTime;
        if (ReverseY) rotation.y -= speed * Time.deltaTime;
        if (ReverseZ) rotation.z -= speed * Time.deltaTime;

        transform.Rotate(rotation, Space.Self);
    }
}
