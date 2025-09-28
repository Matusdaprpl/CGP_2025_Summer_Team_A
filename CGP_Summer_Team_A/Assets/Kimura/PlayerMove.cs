using UnityEngine;
using TMPro;

public class PlayerMove : MonoBehaviour
{
    [Header("速度設定")]
    public float startSpeed = 10f;       // 初期速度
    public float maxSpeed = 10f;         // チャージなしでの最大速度
    public float maxSpeedCharged = 15f;  // チャージ完了後の最大速度
    public float accel = 10f;            // 加速の強さ
    public float decel = 10f;            // 減速の強さ
    public float smooth = 5f;            // 通常の滑らかさ
    public float baseSpeed = 10f;        // キー離しで戻る速度

    [Header("チャージ設定")]
    public float requiredChargeTime = 2f; // チャージ完了に必要な時間
    private float chargeTime = 0f;        // 現在のチャージ時間

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
    private bool isOnObstacle = false;
    private float obstacleMaxSpeed = 2f;
    private float obstacleMinSpeed = 0f;
    private float obstacleSmooth = 20f;

    void Start()
    {
        currentSpeed = startSpeed;
        targetSpeed = startSpeed;
        remainingCountdownTime = countdownTime;

        if (countdownText != null)
            countdownText.text = Mathf.Ceil(remainingCountdownTime).ToString();

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
        bool isAccelerateKey = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool isBackwardKey = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);

        // --- チャージ処理 ---
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            chargeTime = 0f; // 加速キー離したらチャージリセット
        }
        else
        {
            chargeTime += Time.deltaTime; // キー押下中・後退中・何もしていなくてもチャージ増加
        }

        // --- 10f以上加速制限 ---
        bool canAccelerate = true;
        if (currentSpeed >= 10f && chargeTime < requiredChargeTime)
        {
            canAccelerate = false; // チャージ満タンでない場合は加速無効化
        }

        // --- 速度更新 ---
        if (isAccelerateKey && canAccelerate)
        {
            targetSpeed += accel * Time.deltaTime;
        }

        if (isBackwardKey)
        {
            targetSpeed -= decel * Time.deltaTime;
        }

        // --- キー離しで baseSpeed への補正 ---
        if (!isAccelerateKey && !isBackwardKey && !isOnObstacle)
        {
            targetSpeed = Mathf.MoveTowards(targetSpeed, baseSpeed, smooth * Time.deltaTime);
        }

        // --- 最大速度の制限 ---
        float currentMaxSpeed = (chargeTime >= requiredChargeTime) ? maxSpeedCharged : maxSpeed;
        targetSpeed = Mathf.Clamp(targetSpeed, 0f, currentMaxSpeed);

        // --- Obstacle処理 ---
        if (isOnObstacle)
        {
            targetSpeed = Mathf.Clamp(targetSpeed, obstacleMinSpeed, obstacleMaxSpeed);
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, obstacleSmooth * Time.deltaTime);
        }
        else
        {
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

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && currentSpeed <= 11f)
        {
            isOnObstacle = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && isOnObstacle)
        {
            isOnObstacle = false;
        }
    }
}
