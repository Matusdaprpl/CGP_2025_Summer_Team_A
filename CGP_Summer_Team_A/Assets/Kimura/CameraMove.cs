using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;
    public PlayerMove playerMove;
    public float smoothSpeed = 5f;

    [Header("カメラオフセット")]
    public float baseOffset = 2f;
    public float extraOffsetAtMax = 2f;

    private float fixedY;

    void Start()
    {
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        if (player == null || playerMove == null) return;

        float speedRate = playerMove.GetCurrentSpeed() / playerMove.maxSpeed;
        float targetOffset = baseOffset + Mathf.Lerp(0f, extraOffsetAtMax, speedRate);

        Vector3 desiredPosition = new Vector3(
            player.position.x + targetOffset,
            fixedY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
