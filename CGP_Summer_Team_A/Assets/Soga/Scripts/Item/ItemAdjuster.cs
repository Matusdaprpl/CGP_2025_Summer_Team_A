using System.Collections.Generic;
using UnityEngine;

public class ItemAdjuster : MonoBehaviour
{
    [Header("調整間隔")]
    [SerializeField] private float adjustInterval = 5f; // 調整間隔（秒）

    [Header("生成間隔")]
    [SerializeField] private float minSpawnInterval = 1.0f;
    [SerializeField] private float maxSpawnInterval = 3.0f;

    private float lastAdjustTime;
    private int itemsToSpawn = 0;
    private float nextSpawnTime = 0f;

    void Update()
    {
        if (Time.time - lastAdjustTime > adjustInterval)
        {
            AdjustItemsBasedOnCars();
            lastAdjustTime = Time.time;
        }

        if (itemsToSpawn > 0 && Time.time >= nextSpawnTime)
        {
            SpawnOneItem();
            itemsToSpawn--;
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void AdjustItemsBasedOnCars()
    {
        // 最も左と最も右の車のX座標を取得
        float leftmostCarX = GetLeftmostCarX();
        float rightmostCarX = GetRightmostCarX();

        // アイテムを取得
        GameObject[] allItems = GameObject.FindGameObjectsWithTag("Item");
        List<GameObject> itemsToRemove = new List<GameObject>();
        int removedCount = 0;

        foreach (GameObject item in allItems)
        {
            float itemX = item.transform.position.x;
            if (itemX < leftmostCarX)
            {
                itemsToRemove.Add(item);
                removedCount++;

                // 山に戻す（牌を山に追加）
                ItemController ic = item.GetComponent<ItemController>();
                if (ic != null && ic.GetTile() != null)
                {
                    MahjongManager.instance.ReturnTileToMountain(ic.GetTile());
                }
            }
        }

        // 削除実行
        foreach (GameObject item in itemsToRemove)
        {
            Destroy(item);
        }

        // 生成待ち数を追加（上限チェック）
        int currentItemCount = GameObject.FindGameObjectsWithTag("Item").Length - itemsToRemove.Count; // 削除後の数
        int maxToSpawn = ItemSpawner.MaxItemCount - currentItemCount;
        itemsToSpawn += Mathf.Min(removedCount, maxToSpawn);
    }

    private void SpawnOneItem()
    {
        Vector2 newPosition = GetValidSpawnPosition();
        ItemManager.instance.SpawnItemFromMountain(new Vector3(newPosition.x, newPosition.y, 0));
    }

    private Vector2 GetValidSpawnPosition()
    {
        GameObject[] allItems = GameObject.FindGameObjectsWithTag("Item");
        float minDistance = 1.0f; // ItemSpawner と同じ
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            Vector2 position = GetNewSpawnPosition(GetRightmostCarX());
            attempts++;

            if (attempts > maxAttempts)
            {
                Debug.LogWarning("適切なスポーン位置が見つかりません。");
                return Vector2.zero;
            }

            if (IsTooClose(position, allItems, minDistance))
            {
                continue;
            }

            return position;
        }
        while (true);
    }

    private bool IsTooClose(Vector2 position, GameObject[] existingItems, float minDistance)
    {
        foreach (GameObject item in existingItems)
        {
            if (Vector2.Distance(position, item.transform.position) < minDistance)
            {
                return true;
            }
        }
        return false;
    }
    private float GetLeftmostCarX()
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        float leftmostX = float.MaxValue;

        if (player != null)
        {
            leftmostX = Mathf.Min(leftmostX, player.transform.position.x);
        }

        foreach (GameObject npc in npcs)
        {
            leftmostX = Mathf.Min(leftmostX, npc.transform.position.x);
        }

        return leftmostX;
    }

    private float GetRightmostCarX()
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        float rightmostX = float.MinValue;

        if (player != null)
        {
            rightmostX = Mathf.Max(rightmostX, player.transform.position.x);
        }

        foreach (GameObject npc in npcs)
        {
            rightmostX = Mathf.Max(rightmostX, npc.transform.position.x);
        }

        return rightmostX;
    }

    private Vector2 GetNewSpawnPosition(float rightmostCarX)
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector2.zero;
        Vector3 rightEdgeWorld = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, cam.nearClipPlane));
        float cameraRightX = rightEdgeWorld.x;

        float newX = cameraRightX + Random.Range(5f, 15f);
        ItemSpawner spawner= ItemSpawner.Instance;
        if (spawner == null || spawner.FixedYValues == null || spawner.FixedYValues.Length == 0)
        {
            Debug.LogError("ItemSpawner または FixedYValues が見つかりません。");
            return new Vector2(newX, 0);
        }
        float[] fixedYValues = spawner.FixedYValues;
        float newY = fixedYValues[Random.Range(0, fixedYValues.Length)];
        return new Vector2(newX, newY);
    }
}