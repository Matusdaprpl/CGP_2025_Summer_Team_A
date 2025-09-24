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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    rb.linearVelocity = firePoint.right*bulletSpeed;

    score-=fireCost;

    Debug.Log("点棒を発射！残り点数："+score);


    Destroy (bullet,3f);
    }
}
