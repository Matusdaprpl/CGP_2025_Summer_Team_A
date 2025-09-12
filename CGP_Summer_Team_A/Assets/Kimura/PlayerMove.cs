using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("速度設定")]
    public float startSpeed = 5f;    // 初期速度
    public float maxSpeed = 20f;     // 最大速度
    public float accel = 10f;        // 加速の強さ
    public float decel = 10f;        // 減速の強さ
    public float smooth = 3f;        // 滑らかさ

    private float targetSpeed;       // 入力による目標速度
    private float currentSpeed;      // 実際の速度

    void Start()
    {
        currentSpeed = startSpeed;
        targetSpeed = startSpeed;
    }

    void Update()
    {
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


    // 現在の速度を取得するメソッド
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
