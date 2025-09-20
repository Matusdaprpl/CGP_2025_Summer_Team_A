using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("速度設定")]
    public float startSpeed = 5f;    
    public float maxSpeed = 20f;     
    public float accel = 10f;        
    public float decel = 10f;        
    public float smooth = 3f;        
    public float idleSpeed = 8f;     
    public float minSpeed = 3f;      

    private float targetSpeed;       
    private float currentSpeed;      

    void Start()
    {
        currentSpeed = startSpeed;
        targetSpeed = startSpeed;
    }

    void Update()
    {
        float input = 0f;

        // 右キーは加速
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) input = 1f;

        // 左キーは減速（右方向速度を下げるだけ）
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) input = -1f;

        if (input > 0f)
        {
            // 右キー押下 → 加速
            targetSpeed += accel * Time.deltaTime;
            targetSpeed = Mathf.Max(targetSpeed, minSpeed);
        }
        else if (input < 0f)
        {
            // 左キー押下 → 減速（最低速度 minSpeed 以下にはならない）
            targetSpeed -= decel * Time.deltaTime;
            targetSpeed = Mathf.Max(targetSpeed, minSpeed);
        }
        else
        {
            // キーを離したときは idleSpeed まで減速
            targetSpeed = Mathf.MoveTowards(targetSpeed, idleSpeed, decel * Time.deltaTime);
        }

        // 最大速度制限
        targetSpeed = Mathf.Clamp(targetSpeed, minSpeed, maxSpeed);

        // 滑らかに追従
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * smooth);

        // 移動
        transform.Translate(Vector2.right * currentSpeed * Time.deltaTime);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
