using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public Rigidbody2D playerRb;    // プレイヤーの Rigidbody2D をアタッチ
    public TMP_Text speedText;      // TextMeshPro の UI

    void Update()
    {
        if (playerRb == null) return;

        // Rigidbody の速度ベクトルの大きさを取得
        float speed = playerRb.linearVelocity.magnitude;

        // km/h 風に見せるため倍率をかける（例: 10倍）
        float speedKmh = speed * 10f;

        // 小数点なしで表示
        speedText.text = speedKmh.ToString("F0") + " km/h";
    }
}

