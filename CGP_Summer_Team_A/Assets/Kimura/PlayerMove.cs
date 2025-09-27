using UnityEngine;
using TMPro;

public class PlayerMove : MonoBehaviour
{
    [Header("速度設定")]
    public float startSpeed = 10f;   // 初期速度
    public float maxSpeed = 15f;     // 最大速度（通常時）
    public float accel = 10f;        // 加速の強さ
    public float decel = 10f;        // 減速の強さ
    public float smooth = 5f;        // 通常時の滑らかさ
    public float baseSpeed = 10f;    // キーを離したとき戻る速度

    [Header("カウントダウン設定")]
    public float countdownTime = 3f;
    public TMP_Text countdownText;

    [Header("サウンド設定")]
    public AudioSource raceBGM; // BGM
    public AudioSource CountdownSE; // BGM

    public AudioClip countdownSE;

    public AudioClip itemGetSE;
    private AudioSource audioSource;

    private bool hasplayed = false;
    private float targetSpeed;       // 入力による目標速度
    private float currentSpeed;      // 実際の速度
    private float remainingCountdownTime;
    private bool isCountdownActive = true;
    public bool IsCountdownActive => isCountdownActive;

    // --- Obstacle関連 ---
    private bool isOnObstacle = false;   // Obstacle中か
    private float obstacleMaxSpeed = 2f; // Obstacle時の最大速度
    private float obstacleMinSpeed = 0f; // Obstacle時の最小速度
    private float obstacleSmooth = 20f;  // Obstacle時の減速スピード

    void Start()
    {
        currentSpeed = startSpeed;
        targetSpeed = startSpeed;
        remainingCountdownTime = countdownTime;

        if (countdownText != null)
        {
            countdownText.text = Mathf.Ceil(remainingCountdownTime).ToString();
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // --- カウントダウン処理 ---
        if (isCountdownActive)
        {
            remainingCountdownTime -= Time.deltaTime;
            if (countdownText != null)
                countdownText.text = Mathf.Ceil(remainingCountdownTime).ToString();

            if (remainingCountdownTime >= 2.1 && remainingCountdownTime <= 2.98 && !hasplayed)
            {
                CountdownSE.PlayOneShot(countdownSE);
                hasplayed = true;
            }
            
            if (remainingCountdownTime <= 0)
                {
                    isCountdownActive = false;
                    if (countdownText != null)
                    {
                        countdownText.text = "Go!";
                        Invoke("HideCountdownText", 1f);
                    }
                    Debug.Log("レース開始！！");

                    if (raceBGM != null)
                    {
                        raceBGM.Play();
                    }
                }
            return;
        }

        // --- 入力処理 ---
        bool isKeyPressed = false;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            targetSpeed += accel * Time.deltaTime;
            isKeyPressed = true;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            targetSpeed -= decel * Time.deltaTime;
            isKeyPressed = true;
        }

        // 入力なし → baseSpeed に滑らかに戻す（Obstacle中は除外）
        if (!isKeyPressed && !isOnObstacle)
        {
            targetSpeed = Mathf.MoveTowards(targetSpeed, baseSpeed, smooth * Time.deltaTime);
        }

        // --- 速度制限 & 減速処理 ---
        if (isOnObstacle)
        {
            // Obstacle中は速度を0〜2fに制限
            targetSpeed = Mathf.Clamp(targetSpeed, obstacleMinSpeed, obstacleMaxSpeed);

            // 急速に減速する（obstacleSmoothを使用）
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, obstacleSmooth * Time.deltaTime);
        }
        else
        {
            // 通常は0〜maxSpeedに制限
            targetSpeed = Mathf.Clamp(targetSpeed, 0f, maxSpeed);

            // 通常の滑らかさ
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, smooth * Time.deltaTime);
        }

        // --- 移動 ---
        transform.Translate(Vector2.right * currentSpeed * Time.deltaTime);
    }

    void HideCountdownText()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    // 現在の速度を取得するメソッド
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    // --- Obstacleに入ったとき ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (currentSpeed <= 11f) // 速度11以下のときのみ制限適用
            {
                Debug.Log("Obstacleに衝突 → 速度制限(0〜2f)");
                isOnObstacle = true;
            }
        }
    }

    // --- Obstacleから出たとき ---
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (isOnObstacle)
            {
                Debug.Log("Obstacleから離脱 → 通常速度に戻す");
                isOnObstacle = false;
            }
        }
    }
}
