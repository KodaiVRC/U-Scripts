using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Keycard : UdonSharpBehaviour
{
    [Tooltip("Unique ID for this keycard.")]
    public string keyID = "AlphaKey";
}
