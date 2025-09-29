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
    public float boostDuration = 3f;  // ブースト時間
    public float boostSpeed = 20f;    // ブースト最大速度
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

    void Start()
    {
        currentSpeed = startSpeed;
        targetSpeed = startSpeed;
        remainingCountdownTime = countdownTime;

        if (countdownText != null)
            countdownText.text = Mathf.Ceil(remainingCountdownTime).ToString();

        audioSource = GetComponent<AudioSource>();

        if(boostParticles == null)
            boostParticles = GetComponentInChildren<ParticleSystem>();

        if(boostParticles != null)
        {
            boostParticles.Stop(); // 初期状態は停止

            // パーティクル色を黄色に変更
            var main = boostParticles.main;
            main.startColor = Color.yellow;
        }
    }

    void Update()
    {
        // --- カウントダウン処理 ---
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

                if (raceBGM != null)
                    raceBGM.Play();
            }
            return;
        }

        // --- 入力処理 ---
        bool isAccelerateKey = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool isBackwardKey = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);

        // --- チャージ処理 ---
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            chargeTime = 0f;
        }
        else
        {
            chargeTime += Time.deltaTime;
        }

        // --- ブースト開始（チャージ完了かつキー押下時） ---
        if (!isBoostActive && chargeTime >= requiredChargeTime && Input.GetKeyDown(KeyCode.D))
        {
            isBoostActive = true;
            boostTimer = 0f;

            if(boostParticles != null)
                boostParticles.Play();
        }

        // --- ブースト処理 ---
        if (isBoostActive)
        {
            boostTimer += Time.deltaTime;

            // boostSpeed まで徐々に加速
            targetSpeed = Mathf.MoveTowards(targetSpeed, boostSpeed, (boostSpeed - baseSpeed) / boostDuration * Time.deltaTime);

            // boostDuration 秒経過で終了
            if (boostTimer >= boostDuration)
            {
                isBoostActive = false;
                boostTimer = 0f;
                chargeTime = 0f;

                targetSpeed = baseSpeed;

                if(boostParticles != null)
                    boostParticles.Stop();
            }
        }
        else
        {
            // --- 通常加速処理 ---
            bool canAccelerate = true;
            if(currentSpeed >= 10f && chargeTime < requiredChargeTime)
                canAccelerate = false;

            if(isAccelerateKey && canAccelerate)
                targetSpeed += accel * Time.deltaTime;

            if(isBackwardKey)
                targetSpeed -= decel * Time.deltaTime;

            if(!isAccelerateKey && !isBackwardKey && !isOnObstacle)
                targetSpeed = Mathf.MoveTowards(targetSpeed, baseSpeed, smooth * Time.deltaTime);
        }

        // --- 最大速度制限 ---
        float currentMaxSpeed = isBoostActive ? boostSpeed : ((chargeTime >= requiredChargeTime) ? maxSpeedCharged : maxSpeed);
        targetSpeed = Mathf.Clamp(targetSpeed, 0f, currentMaxSpeed);

        // --- 障害物制限 ---
        if(isOnObstacle)
        {
            targetSpeed = Mathf.Clamp(targetSpeed, 0f, obstacleMaxSpeed);
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, obstacleSmooth * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, smooth * Time.deltaTime);
        }

        // --- 移動 ---
        transform.Translate(Vector2.right * currentSpeed * Time.deltaTime);

        // --- 光る処理（ParticleSystem） ---
        if(boostParticles != null)
        {
            // チャージ完了 or ブースト中は光らせる
            if(isBoostActive || chargeTime >= requiredChargeTime)
            {
                if(!boostParticles.isPlaying)
                    boostParticles.Play();
            }
            else
            {
                if(boostParticles.isPlaying)
                    boostParticles.Stop();
            }
        }
    }

    void HideCountdownText()
    {
        if(countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Obstacle") && currentSpeed <= 11f)
            isOnObstacle = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Obstacle") && isOnObstacle)
            isOnObstacle = false;
    }
}
