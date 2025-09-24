using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private int itemCount = 10;

    [Header("X座標範囲")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField]
    private float minX = -10f;

    [SerializeField]
    private float maxX = 10f;

    [Header("Y座標と間隔の設定")]
    [SerializeField]
    [Tooltip("アイテムが生成されるY座標の固定値（4つ設定）")]
    private float[] fixedYValues = new float[] { -4.5f, -4f, -2f, 0f };
 
    [SerializeField]
    [Tooltip("アイテム同士が最低でもこれだけ離れる距離")]
    private float minDistance = 1.0f;

    void Start()
    {
        if (MahjongManager.instance == null)
        {
            Debug.LogError("MahjongManagerのインスタンスが見つかりません。");
            return;
        }

        if (fixedYValues == null || fixedYValues.Length == 0)
        {
            Debug.LogError("固定位置が設定されていません。");
            return;
        }

        List<Vector2> spawnPositions = new List<Vector2>();

        for (int i = 0; i < itemCount; i++)
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

            Vector2 spawnPosition = new Vector2(randomX, chosenY);

            // MahjongManagerに生成を依頼
            MahjongManager.instance.SpawnItemFromMountain(spawnPosition);
            spawnPositions.Add(spawnPosition);
        }
    }
    
    private bool IsTooClose(Vector2 position, List<Vector2> existingPositions)
    {
        foreach (Vector2 existingPos in existingPositions)
        {
            // 2点間の距離が、設定した最低距離より近い場合は true を返す
            if (Vector2.Distance(position, existingPos) < minDistance)
            {
                return true;
            }
        }
        // どのアイテムにも近くない場合は false を返す
        return false;
    }
}

