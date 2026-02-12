using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;
    public PlayerMove playerMove;
    public float smoothSpeed = 5f;

    [Header("カメラオフセット")]
    public float baseOffset = 2f;
    public float extraOffsetAtMax = 2f;

    [Header("画面揺れ")]
    public float defaultShakeDuration = 0.2f;
    public float defaultShakeMagnitude = 0.15f;
    public float shakeFrequency = 25f;

    private float fixedY;
    private Vector3 followPosition;
    private float shakeTimer;
    private float shakeMagnitude;
    private float shakeSeed;

    void Start()
    {
        fixedY = transform.position.y;
        followPosition = transform.position;
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

        followPosition = Vector3.Lerp(followPosition, desiredPosition, smoothSpeed * Time.deltaTime);

        Vector3 shakeOffset = Vector3.zero;
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            float t = (Time.time + shakeSeed) * shakeFrequency;
            float x = (Mathf.PerlinNoise(t, 0f) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(0f, t) - 0.5f) * 2f;
            shakeOffset = new Vector3(x, y, 0f) * shakeMagnitude;
        }

        transform.position = followPosition + shakeOffset;
    }

    public void Shake()
    {
        Shake(defaultShakeDuration, defaultShakeMagnitude);
    }

    public void Shake(float duration, float magnitude)
    {
        if (duration <= 0f || magnitude <= 0f) return;

        shakeTimer = duration;
        shakeMagnitude = magnitude;
        shakeSeed = Random.Range(0f, 1000f);
    }
}
