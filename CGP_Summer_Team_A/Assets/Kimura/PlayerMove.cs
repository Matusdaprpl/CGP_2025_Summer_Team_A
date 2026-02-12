using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; // IEnumerator用に必要
using System.Collections.Generic; // List用に必要
using System.Linq;
using UnityEngine.Rendering; // OrderBy用に必要

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
    public float requiredChargeTime = 7f;
    private float chargeTime = 0f;
    private bool isChargeFull = false;     // チャージが満タンになったか
    private float chargeWaitTimer = 0f;   // 満タン後の1秒待機タイマー
    private const float boostDelayTime = 0.5f;

    [Header("カウントダウン設定")]
    public float countdownTime = 3f;
    public TMP_Text countdownText;

    [Header("サウンド設定")]
    public AudioSource raceBGM;
    public AudioSource CountdownSE;
    public AudioClip countdownSE;
    public AudioClip itemGetSE;
    public AudioClip bulletHitSE;
    private AudioSource audioSource;

    [Header("ブースト設定")]
    public float boostDuration = 5f; // ブースト時間
    public float boostSpeed = 20f;   // ブースト最大速度
    private bool isBoostAvailable = false;
    private bool isBoostActive = false;
    private float boostTimer = 0f;

    [Header("UI設定（ゲージ）")]
    public Slider boostSlider; 
    public Image sliderFillImage;

    [Header("光る設定")]
    public ParticleSystem boostParticles;
    private bool hasplayed = false;

    [Header("被弾設定")]
    public float stopTime = 2f;
    private bool isStopped = false;

    private float targetSpeed;
    private float currentSpeed;
    private float remainingCountdownTime;
    private bool isCountdownActive = true;
    public bool IsCountdownActive => isCountdownActive;
    public bool IsStopped => isStopped;

    // --- Obstacle関連 ---
    private bool isOnObstacle = false;
    private float obstacleMaxSpeed = 2f;
    private float obstacleSmooth = 20f;

    [Header("ゴール距離UI")]
    public TMP_Text goalDistanceText;
    public Transform goalTransform;
    public float worldUnitsToMeters = 1f; // ワールド単位をメートルに変換
    private bool goalReached = false;

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
            // パーティクルをプレイヤーの子にして相対位置を設定
            boostParticles.transform.SetParent(transform);
            boostParticles.transform.localPosition = new Vector3(-0.6f, 0.43f, 0f);
            
            var renderer = boostParticles.GetComponent<ParticleSystemRenderer>();

            var main = boostParticles.main;
            main.startColor = Color.yellow;

            boostParticles.Stop();
        }

        if (playerRenderer == null)
            playerRenderer = GetComponent<SpriteRenderer>();

        if (goalDistanceText != null)
            goalDistanceText.gameObject.SetActive(false);

        if (boostSlider != null)
        {
            boostSlider.minValue = 0f;
            boostSlider.maxValue = 1f;
            boostSlider.value = 0f;
            boostSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if(isStopped) return;
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

                if (goalDistanceText != null)
                {
                    goalDistanceText.gameObject.SetActive(true);
                }
                
                if (boostSlider != null)
                {
                    boostSlider.gameObject.SetActive(true);
                }
                
                if (raceBGM != null) raceBGM.Play();
            }
            return;
        }

        bool isAccelerateKey = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool isBackwardKey = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);

        // --- チャージ処理 ---
        if (!isBoostActive && !isBoostAvailable)
        {
            if (!isChargeFull)
            {
                if (currentSpeed >= 15f)
                {
                    chargeTime += Time.deltaTime;
                    if (chargeTime >= requiredChargeTime)
                    {
                        isChargeFull = true;
                        chargeWaitTimer = 0f; 
                    }
                }
            }
            else
            {
                chargeWaitTimer += Time.deltaTime;
                if (chargeWaitTimer >= boostDelayTime)
                {
                    isBoostAvailable = true;
                }
            }
        }

        // --- ブースト開始 ---
        if (isBoostAvailable && !isBoostActive && isAccelerateKey)
        {
            isBoostActive = true;
            boostTimer = 0f;
            isBoostAvailable = false;
            isChargeFull = false;
            chargeWaitTimer = 0f;
            chargeTime = 0f;
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
                chargeTime = 0f;
            }
        }
        else
        {
            if (isAccelerateKey)
                targetSpeed += accel * Time.deltaTime;
            if (isBackwardKey)
                targetSpeed -= decel * Time.deltaTime;
            if (!isAccelerateKey && !isBackwardKey && !isOnObstacle)
                targetSpeed = Mathf.MoveTowards(targetSpeed, baseSpeed, smooth * Time.deltaTime);
        }

        float currentMaxSpeed = isBoostActive ? boostSpeed :
            ((chargeTime >= requiredChargeTime) ? maxSpeedCharged : maxSpeed);
        targetSpeed = Mathf.Clamp(targetSpeed, 0f, currentMaxSpeed);

        // 障害物制限(ブースト中は無視)
        if (isOnObstacle && !isBoostActive)
        {
            targetSpeed = Mathf.Clamp(targetSpeed, 0f, obstacleMaxSpeed);
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, obstacleSmooth * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, smooth * Time.deltaTime);
        }

        transform.Translate(Vector2.right * currentSpeed * Time.deltaTime);

        // パーティクル制御
        if (boostParticles != null)
        {
            // チャージ中（チャージ完了前）のみパーティクルを表示
            bool isCharging = !isBoostActive && chargeTime > 0f && chargeTime < requiredChargeTime && currentSpeed >= 15f;
            
            if (isCharging)
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
            if (isBoostActive)
            {
                // ブースト中のみ → 激しい虹グラデーション
                colorLerpT += Time.deltaTime * 3f;
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
            else
            {
                // 通常色に戻す
                playerRenderer.color = Color.white;
                colorLerpT = 0f;
                currentColorIndex = 0;
            }
        }

        if (!isCountdownActive)
        {
            UpdateGoalDistanceUI();
            UpdateBoostUI(); 
        }
    }

    void UpdateBoostUI()
    {
        if (boostSlider == null) return;

        if (isBoostActive)
        {
            float remainingRatio = 1f - Mathf.Clamp01(boostTimer / boostDuration);
            boostSlider.value = remainingRatio;
            
            if (sliderFillImage != null) sliderFillImage.color = Color.darkOrange; 
        }
        else if (isBoostAvailable)
        {
            boostSlider.value = 1f;
            // チャージ完了：ゲージをMAXにする
            if (sliderFillImage != null) sliderFillImage.color = Color.yellow;
        }
        else if (isChargeFull)
        {
            boostSlider.value = 1f;
        }
        else
        {
            // チャージ中：チャージ率をゲージで表現（0から1へ増える）
            float chargeRatio = Mathf.Clamp01(chargeTime / requiredChargeTime);
            boostSlider.value = chargeRatio;

            if (sliderFillImage != null) sliderFillImage.color = Color.yellow;
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

    [Header("画面揺れ(点棒)")]
    public float tenbouShakeDuration = 0.2f;
    public float tenbouShakeMagnitude = 0.15f;

    private void OnTriggerEnter2D(Collider2D other)
    {
       ProcessHit(other.gameObject,other.tag);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       ProcessHit(collision.gameObject,collision.gameObject.tag);
    }

    void UpdateGoalDistanceUI()
    {
        if(goalDistanceText==null||goalReached)return;

        if(goalTransform == null)
        {
            var goalObj = GameObject.FindGameObjectWithTag("Goal");
            if(goalObj != null)goalTransform =goalObj.transform;
        }

        if(goalTransform == null)
        {
            goalDistanceText.text ="流局まで: -- m";
            return;
        }

        float dx = goalTransform.position.x-transform.position.x;
        float meters = Mathf.Max(0f,dx * worldUnitsToMeters);
        goalDistanceText.text =$"流局まで: {meters:F0} m";
    }

    private void ProcessHit(GameObject hitObject, string tag)
    {        
        if (tag == "Bullet")
        {
            Bullet2DController bullet = hitObject.GetComponent<Bullet2DController>();
            if (bullet != null && bullet.shooter == Bullet2DController.ShooterType.Player)
            {
                return;
            }

            if (audioSource != null && bulletHitSE != null)
            {
                audioSource.PlayOneShot(bulletHitSE);
            }

            Debug.Log("プレイヤーが点棒に当たりました");
            StartCoroutine(HandleBulletHit());
            TriggerCameraShake();
            Destroy(hitObject);
            return;
        }

        if (tag == "Obstacle" && !isOnObstacle)
        {
            isOnObstacle = true;
        }

        if(tag == "Goal")
        {
            Debug.Log("ゴール");

            goalReached = true;
            if(goalDistanceText != null)
            {
                goalDistanceText.gameObject.SetActive(false);
            }

            var gameManager2 = FindFirstObjectByType<GameManager2>();
            if (gameManager2 != null)
            {
                gameManager2.OnGoal("Player");
            }
        }
    }
    
    private IEnumerator HandleBulletHit()
    {
        isStopped = true;
        currentSpeed = 0f;
        targetSpeed = 0f;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // レーン移動完了待ち
        LaneMove laneMove = GetComponent<LaneMove>();
        if (laneMove != null)
        {
            float waitTimer = 0f;
            while (waitTimer < 2f) // 最大2秒待機
            {
                float targetY = laneMove.GetTargetY();
                if (Mathf.Abs(transform.position.y - targetY) < 0.1f)
                {
                    break;
                }
                waitTimer += Time.deltaTime;
                yield return null;
            }
        }

        // 手牌をドロップする処理
        if (MahjongManager.instance != null && MahjongManager.instance.playerHand != null && MahjongManager.instance.playerHand.Count > 0)
        {
            var handList = MahjongManager.instance.playerHand;
            int dropCount = Mathf.Min(5, handList.Count);
            // ランダムに選ぶ
            List<Tile> tilesToDrop = handList.OrderBy(x => Random.value).Take(dropCount).ToList();

            foreach (var tile in tilesToDrop)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-3f,-1f),0.2f,0f);
                Vector3 dropPosition = transform.position + randomOffset;

                if (ItemManager.instance != null)
                {
                    ItemManager.instance.DropDiscardedTile(tile, dropPosition);
                }

                handList.Remove(tile);
            }

            if(MahjongUIManager.instance != null)
            {
                MahjongUIManager.instance.UpdateHandUI(handList);
            }
        }
        else
        {
            Debug.Log("プレイヤーの手牌が空です。");
        }

        // 停止時間待機
        yield return new WaitForSeconds(stopTime);

        isStopped = false;
    }

    private void TriggerCameraShake()
    {
        CameraMove cameraMove = null;
        if (Camera.main != null)
        {
            cameraMove = Camera.main.GetComponent<CameraMove>();
        }

        if (cameraMove == null)
        {
            cameraMove = FindFirstObjectByType<CameraMove>();
        }

        if (cameraMove != null)
        {
            cameraMove.Shake(tenbouShakeDuration, tenbouShakeMagnitude);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && isOnObstacle)
            isOnObstacle = false;
    }
}
