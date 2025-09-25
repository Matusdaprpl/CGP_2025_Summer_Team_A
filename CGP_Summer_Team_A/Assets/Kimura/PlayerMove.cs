using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Rendering;

public class PlayerMove : MonoBehaviour
{
    [Header("速度設定")]
    public float startSpeed = 10f;  // 初期速度
    public float maxSpeed = 20f;     // 最大速度
    public float accel = 10f;        // 加速の強さ
    public float decel = 10f;        // 減速の強さ
    public float smooth = 5f;        // currentSpeed の滑らかさ
    public float baseSpeed = 10f;    // キーを離したとき戻る速度

    [Header("カウントダウン設定")]
    public float countdownTime = 3f;
    public TMP_Text countdownText;

    [Header("サウンド設定")]
    public AudioSource raceBGM; // BGM
    public AudioClip itemGetSE;
    public AudioClip countdownSE;
    private AudioSource audioSource;

    private float targetSpeed;       // 入力による目標速度
    private float currentSpeed;      // 実際の速度

    [Range(0f, 1f)]
    public float bgmVolume = 0.5f; // BGMの音量
    public float bgmFadeInSeconds = 1f;


    private float remainingCountdownTime;
    private bool isCountdownActive = true;
    public bool IsCountdownActive => isCountdownActive;

    void Start()
    {
        // 初期化
        isCountdownActive = true; // カウントダウン中フラグを立てる
        currentSpeed = 0f;
        targetSpeed = baseSpeed;
        audioSource = GetComponent<AudioSource>();
        ItemController.OnItemPickedUp += PlayItemGetSound;

        // テキストを初期化
        if (countdownText != null) countdownText.text = "";

        // カウントダウンシーケンスを開始
        StartCoroutine(CountdownCoroutine());
    }

    void OnDestroy()
    {
        ItemController.OnItemPickedUp -= PlayItemGetSound;
    }

    private IEnumerator CountdownCoroutine()
    {
        yield return new WaitForEndOfFrame();

        if (audioSource != null && countdownSE != null)
        {
            audioSource.PlayOneShot(countdownSE);
        }

        float timer = countdownTime;
        while (timer > 0)
        {
            if (countdownText != null)
            {
                // テキストを更新 (3, 2, 1)
                countdownText.text = Mathf.Ceil(timer).ToString();
            }
            timer -= Time.deltaTime;
            yield return null; // 次のフレームまで待つ
        }

        // カウントダウン終了処理
        isCountdownActive = false; // 移動を許可
        Debug.Log("レース開始！！");

        if (countdownText != null)
        {
            countdownText.text = "Go!";
            Invoke("HideCountdownText", 1f); // 1秒後に "Go!" を隠す
        }

        // BGMをフェードインで再生
        if (raceBGM != null)
        {
            StartCoroutine(PlayBGMWithFadeIn());
        }
        else
        {
            Debug.LogWarning("raceBGM が未割り当てです。AudioSource を割り当ててください。");
        }
    }

    private void PlayItemGetSound(string suit, int rank)
    {
        if (audioSource != null && itemGetSE != null)
        {
            audioSource.PlayOneShot(itemGetSE);
        }
    }

    void Update()
    { // 修正: カウントダウン中は以降の処理をすべてスキップ
        if (isCountdownActive)
        {
            return;
        }

        // --- ここから下は、既存の入力処理と移動処理 (変更なし) ---
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

        if (!isKeyPressed)
        {
            targetSpeed = Mathf.MoveTowards(targetSpeed, baseSpeed, smooth * Time.deltaTime);
        }

        targetSpeed = Mathf.Clamp(targetSpeed, 0f, maxSpeed);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, smooth * Time.deltaTime);
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

    private IEnumerator PlayBGMWithFadeIn()
    {
        if (raceBGM.clip == null)
        {
            Debug.LogWarning("raceBGM に AudioClip が未設定です。");
            yield break;
        }

        raceBGM.loop = true;
        float target = Mathf.Clamp01(bgmVolume);
        raceBGM.volume = 0f;
        raceBGM.Play();

        float timer = 0f;
        while (timer < bgmFadeInSeconds)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / bgmFadeInSeconds);
            raceBGM.volume = Mathf.Lerp(0f, target, progress);
            yield return null;
        }
        raceBGM.volume = target;
     
    } 
}
