using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ComplexTitles : UdonSharpBehaviour
{
    [Header("Setup")]
    [SerializeField] private Image titleIcon;
    [SerializeField] private Text titleText;
    [SerializeField] private float height = 0.8f;

    private VRCPlayerApi followPlayer;

    [Header("Bind Settings")]
    [SerializeField] private string playerName;
    [SerializeField] private string playerTitle;
    [SerializeField] private Sprite titleIconSprite;

    [Header("🔁 Rotation (Optional)")]
    public bool enableRotation = false;
    public Vector3 rotationSpeed = new Vector3(0, 30, 0);
    public Transform rotatorObject;

    [Header("🌫 Hover Effect (Optional)")]
    public bool enableHover = false;
    public float hoverAmplitude = 0.05f;
    public float hoverSpeed = 2f;

    private float hoverTimeOffset;

    private void Start()
    {
        titleText.text = playerTitle;

        if (titleIconSprite != null)
        {
            titleIcon.sprite = titleIconSprite;
        }

        // Randomize hover start time for desync effect
        hoverTimeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void LateUpdate()
    {
        if (followPlayer != null && followPlayer.IsValid())
        {
            Vector3 headPos = followPlayer.GetBonePosition(HumanBodyBones.Head);
            float yOffset = height;

            if (enableHover)
            {
                float hover = Mathf.Sin(Time.time * hoverSpeed + hoverTimeOffset) * hoverAmplitude;
                yOffset += hover;
            }

            transform.position = headPos + Vector3.up * yOffset;

            // Face local player
            if (followPlayer.playerId == Networking.LocalPlayer.playerId)
            {
                transform.rotation = followPlayer.GetRotation();
                transform.RotateAround(transform.position, Vector3.up, 180);
            }
            else
            {
                Vector3 playerPos = Networking.LocalPlayer.GetPosition();
                playerPos.y = transform.position.y;
                transform.LookAt(playerPos, Vector3.up);
            }

            // Optional rotation
            if (enableRotation && rotatorObject != null)
            {
                rotatorObject.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
            }
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.displayName == playerName)
        {
            followPlayer = player;
            transform.localScale = Vector3.one;
        }
    }
}
