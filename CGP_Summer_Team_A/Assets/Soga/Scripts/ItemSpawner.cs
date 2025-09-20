using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private int itemCount = 10;

    [Header("X座標範囲")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    [Header("X座標最小間隔")]
    [SerializeField] private float minSpacing = 3f; // アイテム同士の最低距離

    // Y座標の固定ライン（0.25 ~ -4.25 を4分割）
    private float[] laneY = { 0.25f, -1.25f, -2.75f, -4.25f };

    void Start()
    {
        if (MahjongManager.instance == null)
        {
            Debug.LogError("MahjongManagerのインスタンスが見つかりません。");
            return;
        }

        SpawnItems();
    }

    private void SpawnItems()
    {
        float lastX = Mathf.Infinity;

        for (int i = 0; i < itemCount; i++)
        {
            // ランダムXを生成（最低距離を考慮）
            float randomX;
            int attempt = 0;
            do
            {
                randomX = Random.Range(minX, maxX);
                attempt++;
                if (attempt > 20) break; // 試行回数制限
            } while (Mathf.Abs(randomX - lastX) < minSpacing);

            lastX = randomX;

            // Y座標は4ラインからランダムに選択
            float chosenY = laneY[Random.Range(0, laneY.Length)];

            Vector2 spawnPosition = new Vector2(randomX, chosenY);

            // MahjongManagerに生成を依頼
            MahjongManager.instance.SpawnItemFromMountain(spawnPosition);
        }
    }
}

