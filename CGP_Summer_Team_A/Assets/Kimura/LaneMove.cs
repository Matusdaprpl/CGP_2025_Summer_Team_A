using UnityEngine;

public class LaneMove : MonoBehaviour
{
    public float topY = 0.25f;       // 上端のY座標
    public float bottomY = -4.25f;   // 下端のY座標
    public int laneCount = 4;     // レーン数（4等分）
    public float moveSpeed = 5f;  // スムーズ移動の速度

    private int currentLane = 0;     // 現在のレーン番号（0〜laneCount-1）
    private float[] lanePositions;   // レーンごとのY座標

    void Start()
    {
        // レーンごとのY座標を計算
        lanePositions = new float[laneCount];
        float interval = (topY - bottomY) / (laneCount - 1);

        for (int i = 0; i < laneCount; i++)
        {
            lanePositions[i] = bottomY + interval * i;
        }

        // 初期位置を中央レーンにセット
        currentLane = laneCount / 2;
        transform.position = new Vector3(
            transform.position.x,
            lanePositions[currentLane],
            transform.position.z
        );
    }

    void Update()
    {
        // 上キー or W → 上のレーンへ
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && currentLane < laneCount - 1)
        {
            currentLane++;
        }

        // 下キー or S → 下のレーンへ
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && currentLane > 0)
        {
            currentLane--;
        }

        // ターゲット座標を計算
        Vector3 targetPos = new Vector3(transform.position.x, lanePositions[currentLane], transform.position.z);

        // スムーズに補間して移動
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }
}
