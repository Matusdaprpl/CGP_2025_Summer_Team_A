using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Shooter2D : MonoBehaviour
{
    [Header("効果音")]
    public AudioClip fireSound; // 発射音のオーディオクリップ
    private AudioSource audioSource; // 音を再生するためのコンポーネント
    public TextMeshProUGUI ResultscoreText;
    public TextMeshProUGUI scoreText;
    [Header("弾のプレハブ")]
    public GameObject bulletPrefab;
    [Header("射出ポイント")]
    public Transform firePoint;
    [Header("弾の速度")]
    public float bulletSpeed = 10f;
    [Header("スコア")]
    public static int score = 15000;
    public int fireCost = 1000;
    [Header("ルール表示")]
    [SerializeField] private RuleViewer ruleViewer;

    // Start is called once before the first execution of Update after the MonoBehaviour created
    void Start()
    {
        // Start処理が必要であればここに記述
        //このオブジェクトに付いているAudioSourceコンポーネントを取得
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ruleViewer != null && ruleViewer.IsAnyPanelOpen)
            {
                return;
            }

            if (score >= fireCost)
            {
                Shoot();
            }
            else
            {
                Debug.Log("スコアが足りないので発射できまへん！");
            }
        }

        scoreText.text = "SCORE: " + score;
        ResultscoreText.text = "SCORE: " + score;
    }
    
    void Shoot()
    {
        if(bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("プレハブまたは射出ポイントが設定されていません。");
            return;
        }
     // もしfireSoundが設定されていて、audioSourceが取得できていれば音を再生
        if (audioSource != null && fireSound != null)
        {
            // PlayOneShotを使うと、他の音を中断せずに再生できるため効果音に最適
            audioSource.PlayOneShot(fireSound);
        }

        GameObject bullet = Instantiate(bulletPrefab,firePoint.position,firePoint.rotation);

        Bullet2DController bc = bullet.GetComponent<Bullet2DController>();
        if (bc != null)
        {
            bc.shooter = Bullet2DController.ShooterType.Player;
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    
        // Rigidbody2Dの線形速度を設定
        // rb.linearVelocity = firePoint.right*bulletSpeed;
        // Unityのバージョンによっては velocity を使用
        rb.linearVelocity = firePoint.right*bulletSpeed;

        score-=fireCost;

        if(GameManager2.instance != null)
        {
            GameManager2.instance.AddToPot(fireCost);
        } 

        //Debug.Log("点棒を発射！残り点数："+score);


        Destroy (bullet,3f);
    }
    

    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"🎉 役満によりスコアが加算されました！ (+{points}) 現在のスコア: {score}");
    }
}