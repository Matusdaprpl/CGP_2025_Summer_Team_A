using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("初期スポーン設定")]
    [SerializeField] private int itemCount = 10;

    [Header("Y座標と間隔の設定")]
    [SerializeField] private float[] fixedYValues = new float[] { -4f, -2.5f, -1f, 0f };
    [SerializeField] private float minDistance = 1.0f;

    public static int MaxItemCount => Instance?.itemCount ?? 10;
    public static ItemSpawner Instance;
    public float[] FixedYValues => fixedYValues;

    [Header("初期配置")]
    [SerializeField] private float intialSpawnMinX =0f;
    [SerializeField] private float initialSpawnMaxX = 20f;

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
    }

    private void SpawnInitialItems()
    {
        List<Vector2> spawnPositions = GetExistingItemPositions();

        for (int i = 0; i < itemCount; i++)
        {
            Vector2 spawnPosition = GetValidSpawnPosition(spawnPositions);
            if (spawnPosition == Vector2.zero) continue;

            ItemManager.instance.SpawnItemFromRecycleOrMountain(new Vector3(spawnPosition.x, spawnPosition.y, 0));
            spawnPositions.Add(spawnPosition);
        }
    }

    private Vector2 GetValidSpawnPosition(List<Vector2> existingPositions)
    {
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            float randomX = Random.Range(intialSpawnMinX, initialSpawnMaxX);
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
            if (Vector2.Distance(position, existingPos) < minDistance) return true;
        }
        return false;
    }

    private List<Vector2> GetExistingItemPositions()
    {
        List<Vector2> positions = new List<Vector2>();
        var items = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item in items) positions.Add(item.transform.position);
        return positions;
    }
}
