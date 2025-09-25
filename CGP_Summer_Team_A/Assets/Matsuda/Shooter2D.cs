using UnityEngine;
using TMPro;

public class Shooter2D : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    [Header("弾のプレハブ")]
    public GameObject bulletPrefab;
    [Header("射出ポイント")]
    public Transform firePoint;
    [Header("弾の速度")]
    public float bulletSpeed = 10f;
    [Header("スコア")]
    public int score = 10000;
    public int fireCost = 1000;
    
    // Start is called once before the first execution of Update after the MonoBehaviour created
    void Start()
    {
        // Start処理が必要であればここに記述
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (score >=fireCost) 
            {
                Shoot();
            }
            else
            {
                Debug.Log("スコアが足りないので発射できまへん！");
            }
        }
        scoreText.text = "SCORE: " + score;
    }
    
    void Shoot()
    {
        if(bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("プレハブまたは射出ポイントが設定されていません。");
            return;
        }
    

    GameObject bullet = Instantiate(bulletPrefab,firePoint.position,firePoint.rotation);

    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    
    // Rigidbody2Dの線形速度を設定
    // rb.linearVelocity = firePoint.right*bulletSpeed;
    // Unityのバージョンによっては velocity を使用
    rb.linearVelocity = firePoint.right*bulletSpeed;

    score-=fireCost;

    Debug.Log("点棒を発射！残り点数："+score);


    Destroy (bullet,3f);
    }
    
    // ★★★ 役満判定からのスコア加算機能（追加・修正箇所） ★★★
    /// <summary>
    /// スコアを加算し、役満の点数処理を行います。
    /// </summary>
    /// <param name="points">加算する点数</param>
    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"🎉 役満によりスコアが加算されました！ (+{points}) 現在のスコア: {score}");
    }
    // ★★★ 役満判定からのスコア加算機能（ここまで） ★★★
}