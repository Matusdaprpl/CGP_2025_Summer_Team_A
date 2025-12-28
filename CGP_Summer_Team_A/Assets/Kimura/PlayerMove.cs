using UnityEngine;
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
            var renderer = boostParticles.GetComponent<ParticleSystemRenderer>();
            renderer.sortingLayerName = "Default";
            renderer.sortingOrder = 5;

            var main = boostParticles.main;
            main.startColor = Color.yellow;
        }

        if (playerRenderer == null)
            playerRenderer = GetComponent<SpriteRenderer>();

        if (goalDistanceText != null)
            goalDistanceText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isCountdownActive)
        {
            UpdateGoalDistanceUI();
        }

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

            Debug.Log("プレイヤーが点棒に当たりました");
            StartCoroutine(HandleBulletHit());
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && isOnObstacle)
            isOnObstacle = false;
    }
}
