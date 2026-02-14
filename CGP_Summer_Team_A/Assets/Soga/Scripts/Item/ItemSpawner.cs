using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private int itemCount = 10;

    [Header("維持するアイテム数")]
    [SerializeField] private int targetWorldItemCount = 20;

    [Header("再配置間隔(秒)")]
    [SerializeField] private float respawnInterval = 1.5f;

    [Header("X座標範囲")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    [Header("Y座標と間隔の設定")]
    [SerializeField]
    [Tooltip("アイテムが生成されるY座標の固定値（4つ設定）")]
    private float[] fixedYValues = new float[] { -4f, -2.5f, -1f, 0f };

    [SerializeField]
    [Tooltip("アイテム同士が最低でもこれだけ離れる距離")]
    private float minDistance = 1.0f;

    public static int MaxItemCount => Instance?.itemCount ?? 10;
    public static ItemSpawner Instance;

    public float[] FixedYValues => fixedYValues;

    void Awake()
    {
        Instance = this;   
    }

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

        SpawnInitialItems();
        StartCoroutine(RespawnLoop());
    }

    private void SpawnInitialItems()
    {
        List<Vector2> spawnPositions = GetExistingItemPositions();

        for (int i = 0; i < itemCount; i++)
        {
            Vector2 spawnPosition = GetValidSpawnPosition(spawnPositions);
            if (spawnPosition == Vector2.zero) continue; // 失敗時はスキップ

            ItemManager.instance.SpawnItemFromRecycleOrMountain(new Vector3(spawnPosition.x, spawnPosition.y, 0));
            spawnPositions.Add(spawnPosition);
        }
    }

    private System.Collections.IEnumerator RespawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnInterval);

            if (ItemManager.instance == null) continue;

            int deficit = targetWorldItemCount - ItemManager.instance.ActiveWorldItemCount;
            if (deficit <= 0) continue;

            List<Vector2> spawnPositions = GetExistingItemPositions();
            for (int i = 0; i < deficit; i++)
            {
                Vector2 spawnPosition = GetValidSpawnPosition(spawnPositions);
                if (spawnPosition == Vector2.zero) continue;

                var spawned = ItemManager.instance.SpawnItemFromRecycleOrMountain(new Vector3(spawnPosition.x, spawnPosition.y, 0));
                if (spawned != null)
                {
                    spawnPositions.Add(spawnPosition);
                }
            }
        }
    }

    private Vector2 GetValidSpawnPosition(List<Vector2> existingPositions)
    {
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = fixedYValues[Random.Range(0, fixedYValues.Length)];
            Vector2 position = new Vector2(randomX, randomY);
            attempts++;

            if (attempts > maxAttempts)
            {
                Debug.LogWarning("適切なスポーン位置が見つかりません。");
                return Vector2.zero;
            }

            if (!IsTooClose(position, existingPositions))
            {
                return position;
            }
        }
        while (true);
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

    private List<Vector2> GetExistingItemPositions()
    {
        List<Vector2> positions = new List<Vector2>();
        var items = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item in items)
        {
            positions.Add(item.transform.position);
        }
        return positions;
    }
}
