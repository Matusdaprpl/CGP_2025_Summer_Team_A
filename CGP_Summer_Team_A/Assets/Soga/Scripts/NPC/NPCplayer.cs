using Unity.VisualScripting;
using UnityEngine;
using TMPro;

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

    [Header("カウントダウン設定")]
    [Tooltip("カウントダウンの時間（秒）")]
    public float countdownTime = 3f;

    [Tooltip("カウントダウンのUIテキスト")]
    public TMP_Text countdownText;

    private Rigidbody2D rb;
    private float currentSpeed;
    private float timeSinceLastChange;
    private float remainingCountdownTime;
    private bool isCountdownActive = true;

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
        remainingCountdownTime = countdownTime;
        if (countdownText != null)
        {
            countdownText.text = Mathf.Ceil(remainingCountdownTime).ToString();

        }
    }

    void Update()
    {
        if (isCountdownActive)
        {
            remainingCountdownTime -= Time.deltaTime;

            if (countdownText != null)
            {
                countdownText.text = Mathf.Ceil(remainingCountdownTime).ToString();
            }

            if (remainingCountdownTime <= 0)
            {
                isCountdownActive = false;
                if (countdownText != null)
                {
                    countdownText.text = "Go!";
                    Invoke("HideCountdownText", 1f);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!isCountdownActive && rb != null)
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

    void HideCountdownText()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }
}

