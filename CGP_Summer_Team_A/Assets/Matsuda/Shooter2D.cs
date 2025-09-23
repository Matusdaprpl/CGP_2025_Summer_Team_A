using UnityEngine;

public class Shooter2D : MonoBehaviour
{
    [Header("弾のプレハブ")]
    public GameObject bulletPrefab;
    [Header("射出ポイント")]
    public Transform firePoint;
    [Header("弾の速度")]
    public float bulletSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
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

    Destroy (bullet,3f);
    }
}
