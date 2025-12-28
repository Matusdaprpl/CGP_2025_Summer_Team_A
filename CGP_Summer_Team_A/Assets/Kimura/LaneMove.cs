using UnityEngine;

public class LaneMove : MonoBehaviour
{
    [Header("レーン設定")]
    public float topY = -1.25f;       // 上端のY座標
    public float bottomY = -5.75f;   // 下端のY座標
    public int laneCount = 4;        // レーン数（4等分）
    public float moveSpeed = 5f;     // スムーズ移動の速度

    private int currentLane = 1;     // 現在のレーン番号（0〜laneCount-1）
    private float[] lanePositions;   // レーンごとのY座標
    private bool isOnObstacle = false; // Obstacleに当たっているかどうか
    private PlayerMove playerMove;

    void Start()
    {
    // レーンごとのY座標を計算
        lanePositions = new float[laneCount];

    // laneCountが1より大きい場合のみ間隔を計算
        if (laneCount > 1)
        {
            float interval = (topY - bottomY) / (laneCount - 1);
            for (int i = 0; i < laneCount; i++)
            {
            lanePositions[i] = bottomY + interval * i;
            }
        }
        else if (laneCount == 1)
        {
        // レーンが1つなら、中央（bottomYとtopYの平均）に配置
        lanePositions[0] = (topY + bottomY) / 2f;
        }

        currentLane = (laneCount - 1) / 2;

    // currentLaneが配列の範囲内にあることを念のため確認
        if (currentLane >= 0 && currentLane < laneCount)
        {
            transform.position = new Vector3(
            transform.position.x,
            lanePositions[currentLane],
            transform.position.z
            );
        }

        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        // カウントダウン中・Obstacle中・停止中はレーン変更を禁止
        bool canChangeLane = !isOnObstacle;
        if (playerMove != null)
        {
            if (playerMove.IsCountdownActive || playerMove.IsStopped)
            {
                canChangeLane = false;
            }
        }

        if (canChangeLane)
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
        }

        // ターゲット座標を計算（常にレーン中央）
        Vector3 targetPos = new Vector3(transform.position.x, lanePositions[currentLane], transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }

    // Obstacleに入ったとき
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            isOnObstacle = true;
        }
    }

    // Obstacleから出たとき
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            isOnObstacle = false;
        }
    }

    public float GetTargetY()
    {
        if (lanePositions != null && currentLane >= 0 && currentLane < lanePositions.Length)
        {
            return lanePositions[currentLane];
        }
        return transform.position.y;
    }
}
