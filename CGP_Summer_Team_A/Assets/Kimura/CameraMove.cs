using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;       // プレイヤー Transform
    public PlayerMove playerMove;  // PlayerMove スクリプト参照
    public float smoothSpeed = 5f; // 追従の滑らかさ

    public float offsetAtZero = 3f;
    public float offsetAtMax = 9f;

    private float fixedY;

    void Start()
    {
        fixedY = transform.position.y; // カメラのY位置を固定
    }

    void LateUpdate()
    {
        if (player == null || playerMove == null) return;

        float speed = playerMove.GetCurrentSpeed(); // 0～maxSpeed

        // オフセットを速度に応じて補間
        float targetOffset = Mathf.Lerp(offsetAtZero, offsetAtMax, speed / playerMove.maxSpeed);

        // 目標位置（横方向のみ）
        Vector3 desiredPosition = new Vector3(
            player.position.x + targetOffset,
            fixedY,
            transform.position.z
        );

        // スムーズに追従
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
