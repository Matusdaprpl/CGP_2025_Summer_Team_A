using UnityEngine;
using TMPro;

public class PlayerMove : MonoBehaviour
{
    [Header("速度設定")]
    public float startSpeed = 5f;    // 初期速度
    public float maxSpeed = 20f;     // 最大速度
    public float accel = 10f;        // 加速の強さ
    public float decel = 10f;        // 減速の強さ
    public float smooth = 3f;        // 滑らかさ

    [Header("カウントダウン設定")]
    [Tooltip("カウントダウンの時間（秒）")]
    public float countdownTime = 3f;

    [Tooltip("カウントダウンのUIテキスト")]
    public TMP_Text countdownText;

    [Header("サウンド設定")]
    public AudioSource raceBGM; // BGM
    public AudioClip itemGetSE;
    private AudioSource audioSource;

    private float targetSpeed;       // 入力による目標速度
    private float currentSpeed;      // 実際の速度
    private float remainingCountdownTime;
    private bool isCountdownActive = true;
    public bool IsCountdownActive => isCountdownActive;

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
                Debug.Log("レース開始！！");

                if (raceBGM != null)
                {
                    raceBGM.Play();
                }
            }
            return;
        }

        // --- 入力処理 ---
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            targetSpeed += accel * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            targetSpeed -= decel * Time.deltaTime;
        }

        // 速度制限（0 ～ maxSpeed）
        targetSpeed = Mathf.Clamp(targetSpeed, 0f, maxSpeed);

        // --- 補間で滑らかに変化 ---
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * smooth);

        // --- 実際の移動（Transformで直接動かす） ---
        transform.Translate(Vector2.right * currentSpeed * Time.deltaTime);
    }

    void HideCountdownText()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    // 現在の速度を取得するメソッド
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
     private void OnTriggerEnter2D(Collider2D other)
    {
        // 接触した相手のタグが "Item" だったら
        if (other.gameObject.CompareTag("Item"))
        {
            // アイテム取得音を再生する
            if (audioSource != null && itemGetSE != null)
            {
                // PlayOneShotを使うと、連続でアイテムを取っても音が途切れず鳴らせる
                audioSource.PlayOneShot(itemGetSE);
            }

            // 接触したアイテムのオブジェクトを消す
            Destroy(other.gameObject);
        }
    }
}
