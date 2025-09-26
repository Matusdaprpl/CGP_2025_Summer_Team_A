using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;   // Obstacle のプレハブ
    [SerializeField] private int obstacleCount = 10;      // 出現数

    [Header("X座標範囲")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    [Header("Y座標と間隔の設定")]
    [SerializeField]
    [Tooltip("Obstacle が生成されるY座標の固定値（複数設定可能）")]
    private float[] fixedYValues = new float[] { 0.165f, -1.33f, -2.76f, -4.26f };

    [SerializeField]
    [Tooltip("Obstacle 同士が最低でもこれだけ離れる距離")]
    private float minDistance = 1.0f;

    void Start()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("Obstacle Prefab が設定されていません。");
            return;
        }

        if (fixedYValues == null || fixedYValues.Length == 0)
        {
            Debug.LogError("固定Y座標が設定されていません。");
            return;
        }

        List<Vector2> spawnPositions = new List<Vector2>();

        for (int i = 0; i < obstacleCount; i++)
        {
            Vector2 spawnPosition;
            int attempts = 0;
            const int maxAttempts = 100;

            do
            {
                float randomX = Random.Range(minX, maxX);
                float randomY = fixedYValues[Random.Range(0, fixedYValues.Length)];
                spawnPosition = new Vector2(randomX, randomY);
                attempts++;

                if (attempts > maxAttempts)
                {
                    Debug.LogWarning("適切なスポーン位置が見つかりません。スポーンを中止します。");
                    return;
                }
            }
            while (IsTooClose(spawnPosition, spawnPositions));

            // Obstacle を生成
            Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
            spawnPositions.Add(spawnPosition);
        }
    }

    private bool IsTooClose(Vector2 position, List<Vector2> existingPositions)
    {
        foreach (Vector2 existingPos in existingPositions)
        {
            if (Vector2.Distance(position, existingPos) < minDistance)
            {
                return true;
            }
        }
        return false;
    }
}
