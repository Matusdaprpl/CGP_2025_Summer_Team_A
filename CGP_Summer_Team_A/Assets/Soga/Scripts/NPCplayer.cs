using Unity.VisualScripting;
using UnityEngine;

public class NPCplayer : MonoBehaviour
{
    [Header("速度設定")]
    [Tooltip("NPCの最低速度")]
    public float minSpeed = 2f;

    [Tooltip("NPCの最高速度")]
    public float maxSpeed = 5f;

    [Header("挙動判定")]
    [Tooltip("速度を変更する間隔（秒）")]
    public float speedChangeInterval = 3f;

    private Rigidbody2D rb;
    private float currentSpeed;
    private float timeSinceLastChange;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2Dコンポーネントが見つかりません。");
        }
    }

    void Start()
    {
        UpdateSpeed();
        timeSinceLastChange = 0f;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(currentSpeed, 0);
        }

        timeSinceLastChange += Time.fixedDeltaTime;

        if (timeSinceLastChange >= speedChangeInterval)
        {
            UpdateSpeed();
            timeSinceLastChange = 0f;
        }
    }

    void UpdateSpeed()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
    }
}
