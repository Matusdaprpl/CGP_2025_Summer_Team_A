using UnityEngine;
using TMPro;

public class PlayerMove : MonoBehaviour
{
    [Header("速度設定")]
    public float startSpeed = 10f;
    public float maxSpeed = 10f;
    public float maxSpeedCharged = 15f;
    public float accel = 10f;
    public float decel = 10f;
    public float smooth = 5f;
    public float baseSpeed = 10f;

    [Header("チャージ設定")]
    public float requiredChargeTime = 2f;
    private float chargeTime = 0f;

    [Header("カウントダウン設定")]
    public float countdownTime = 3f;
    public TMP_Text countdownText;

    [Header("サウンド設定")]
    public AudioSource raceBGM;
    public AudioSource CountdownSE;
    public AudioClip countdownSE;
    public AudioClip itemGetSE;
    private AudioSource audioSource;

    [Header("ブースト設定")]
    public float boostDuration = 3f; // ブースト時間
    public float boostSpeed = 20f;   // ブースト最大速度
    private bool isBoostAvailable = false;
    private bool isBoostActive = false;
    private float boostTimer = 0f;

    [Header("光る設定")]
    public ParticleSystem boostParticles;
    private bool hasplayed = false;

    private float targetSpeed;
    private float currentSpeed;
    private float remainingCountdownTime;
    private bool isCountdownActive = true;
    public bool IsCountdownActive => isCountdownActive;

    // --- Obstacle関連 ---
    private bool isOnObstacle = false;
    private float obstacleMaxSpeed = 2f;
    private float obstacleSmooth = 20f;

    [Header("色変化設定")]
    public SpriteRenderer playerRenderer;  // PlayerのSpriteRenderer
    public float colorChangeSpeed = 1f;    // 基本色変化速度

    private Color[] rainbowColors = new Color[]
    {
        Color.red,
        Color.yellow,
        Color.green,
        Color.blue,
        new Color(0.5f, 0f, 0.5f) // 紫
    };
    private int currentColorIndex = 0;
    private float colorLerpT = 0f;

    void Start()
    {
        currentSpeed = startSpeed;
        targetSpeed = startSpeed;
        remainingCountdownTime = countdownTime;

        if (countdownText != null)
            countdownText.text = Mathf.Ceil(remainingCountdownTime).ToString();

        audioSource = GetComponent<AudioSource>();

        if (boostParticles == null)
            boostParticles = GetComponentInChildren<ParticleSystem>();

        if (boostParticles != null)
        {
            var renderer = boostParticles.GetComponent<ParticleSystemRenderer>();
            renderer.sortingLayerName = "Default";
            renderer.sortingOrder = 5;

            var main = boostParticles.main;
            main.startColor = Color.yellow;
        }

        if (playerRenderer == null)
            playerRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // --- カウントダウン ---
        if (isCountdownActive)
        {
            remainingCountdownTime -= Time.deltaTime;
            if (countdownText != null)
                countdownText.text = Mathf.Ceil(remainingCountdownTime).ToString();

            if (remainingCountdownTime >= 2.1f && remainingCountdownTime <= 2.98f && !hasplayed)
            {
                CountdownSE.PlayOneShot(countdownSE);
                hasplayed = true;
            }

            if (remainingCountdownTime <= 0f)
            {
                isCountdownActive = false;
                if (countdownText != null)
                {
                    countdownText.text = "Go!";
                    Invoke("HideCountdownText", 1f);
                }
                if (raceBGM != null) raceBGM.Play();
            }
            return;
        }

        bool isAccelerateKey = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool isBackwardKey = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);

        // --- チャージ処理 ---
        if (!isBoostActive)
        {
            chargeTime += Time.deltaTime;
            if (chargeTime >= requiredChargeTime)
                isBoostAvailable = true;
        }

        // --- 速度10以上で加速キーを離した場合チャージリセット ---
        if (currentSpeed > 10f && !isAccelerateKey)
        {
            chargeTime = 0f;
            isBoostAvailable = false;
        }

        // --- ブースト開始 ---
        if (isBoostAvailable && !isBoostActive && isAccelerateKey)
        {
            isBoostActive = true;
            boostTimer = 0f;
            isBoostAvailable = false;
        }

        // --- ブースト処理 ---
        if (isBoostActive)
        {
            boostTimer += Time.deltaTime;
            targetSpeed = Mathf.MoveTowards(
                targetSpeed,
                boostSpeed,
                (boostSpeed - baseSpeed) / boostDuration * Time.deltaTime
            );

            if (boostTimer >= boostDuration || !isAccelerateKey)
            {
                isBoostActive = false;
                boostTimer = 0f;
                targetSpeed = baseSpeed;
            }
        }
        else
        {
            // 通常加速
            bool canAccelerate = true;
            if (currentSpeed >= 10f && chargeTime < requiredChargeTime)
                canAccelerate = false;

            if (isAccelerateKey && canAccelerate)
                targetSpeed += accel * Time.deltaTime;
            if (isBackwardKey)
                targetSpeed -= decel * Time.deltaTime;
            if (!isAccelerateKey && !isBackwardKey && !isOnObstacle)
                targetSpeed = Mathf.MoveTowards(targetSpeed, baseSpeed, smooth * Time.deltaTime);
        }

        // 最大速度制限
        float currentMaxSpeed = isBoostActive ? boostSpeed :
            ((chargeTime >= requiredChargeTime) ? maxSpeedCharged : maxSpeed);
        targetSpeed = Mathf.Clamp(targetSpeed, 0f, currentMaxSpeed);

        // 障害物制限
        if (isOnObstacle)
        {
            targetSpeed = Mathf.Clamp(targetSpeed, 0f, obstacleMaxSpeed);
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, obstacleSmooth * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, smooth * Time.deltaTime);
        }

        // 移動
        transform.Translate(Vector2.right * currentSpeed * Time.deltaTime);

        // パーティクル制御
        if (boostParticles != null)
        {
            boostParticles.transform.position = transform.position;
            if (isBoostAvailable || isBoostActive)
            {
                if (!boostParticles.isPlaying) boostParticles.Play();
            }
            else
            {
                if (boostParticles.isPlaying) boostParticles.Stop();
            }
        }

        // --- 色変化処理 ---
        if (playerRenderer != null)
        {
            if (currentSpeed > 10f)
            {
                // 速度10超え → 激しい虹グラデーション
                colorLerpT += Time.deltaTime * 3f; // 激しめ
                if (colorLerpT >= 1f)
                {
                    colorLerpT = 0f;
                    currentColorIndex = (currentColorIndex + 1) % rainbowColors.Length;
                }
                int nextColorIndex = (currentColorIndex + 1) % rainbowColors.Length;
                playerRenderer.color = Color.Lerp(
                    rainbowColors[currentColorIndex],
                    rainbowColors[nextColorIndex],
                    colorLerpT
                );
            }
            else if (chargeTime >= requiredChargeTime && currentSpeed <= 10f)
            {
                // チャージ溜まって速度10以下 → 赤→黄のゆるめグラデーション
                colorLerpT += Time.deltaTime * 1f; // ゆるめ
                if (colorLerpT >= 1f) colorLerpT = 0f;
                playerRenderer.color = Color.Lerp(Color.red, Color.yellow, colorLerpT);
            }
            else
            {
                // 通常色に戻す
                playerRenderer.color = Color.white;
                colorLerpT = 0f;
                currentColorIndex = 0;
            }
        }
    }

    void HideCountdownText()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && currentSpeed <= 11f)
            isOnObstacle = true;
        {
            Debug.Log($"OnTriggerEnter2D called with:{other.gameObject.name},Tag:{other.tag}");
            if(other.CompareTag("Goal"))
            {
                Debug.Log("ゴール！！");
                var gameManager2 = FindFirstObjectByType<GameManager2>(); // 追加: GameManager2を取得
                if (gameManager2 != null)
                {
                    gameManager2.OnGoal("Player");
                }
                else
                {
                    Debug.LogError("GameManager2が見つかりません。");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && isOnObstacle)
            isOnObstacle = false;
    }
}
