using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // 背景画像の幅
    private float backgroundWidth;

    void Start()
    {
        // SpriteRendererから背景画像の幅を取得
        backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // カメラのX座標を取得
        float cameraPositionX = Camera.main.transform.position.x;

        // 背景の右端が、カメラの中心よりも左側に来たか（つまり完全に見えなくなったか）を判定
        if (transform.position.x + (backgroundWidth / 2) < cameraPositionX)
        {
            // 背景2枚分の距離を右に移動させる
            float offset = backgroundWidth * 2; // ★ここが「* 2」であることが重要です
            transform.position = new Vector3(transform.position.x + offset, transform.position.y, transform.position.z);
        }
    }
}