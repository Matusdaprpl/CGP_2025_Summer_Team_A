using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAdjuster : MonoBehaviour
{
    [Header("牌のプレイヤーとの距離設定")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float pickupDistance = 15f;      // 後方回収ライン
    [SerializeField] private float spawnDistanceAhead = 30f;   // 前方配置基準

    [Header("維持設定")]
    [SerializeField] private int targetWorldItemCount = 40;   // 画面上に維持したい牌の数
    [SerializeField] private float respawnInterval = 1.5f;
    [SerializeField] private float minDistance = 1.0f;

    private void Start()
    {
        if (playerTransform == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        if (playerTransform == null)
        {
            Debug.LogError("Playerオブジェクトが見つかりません。");
            enabled = false;
            return;
        }

        StartCoroutine(RespawnLoop());
    }

    private IEnumerator RespawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnInterval);

            if (ItemManager.instance == null || MahjongManager.instance == null) continue;

            var items = GameObject.FindGameObjectsWithTag("Item");
            foreach (var item in items)
            {
                if (!item.activeInHierarchy) continue;
                if (item.transform.position.x >= playerTransform.position.x - pickupDistance) continue;

                var ic = item.GetComponent<ItemController>();
                if (ic == null) continue;

                ItemManager.instance.NotifyItemPickedUp(ic.GetTile(), true);
                ItemManager.instance.ReturnTiletoMountain(ic);
            }

            int deficit = targetWorldItemCount - ItemManager.instance.ActiveWorldItemCount;
            if (deficit <= 0) continue;

            for (int i = 0; i < deficit; i++)
            {
                Vector2 pos = GetValidSpawnPosition(GetExistingItemPositions());
                if (pos == Vector2.zero) continue;

                var spawned = ItemManager.instance.SpawnItemFromRecycleOrMountain(new Vector3(pos.x, pos.y, 0f));
                if (spawned != null) GetExistingItemPositions().Add(pos);
            }
        }
    }

    private Vector2 GetValidSpawnPosition(List<Vector2> existingPositions)
    {
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            float baseX = playerTransform.position.x + spawnDistanceAhead;
            float randomX = Random.Range(baseX - 5f, baseX + 100f);

            float randomY = 0f;
            var spawner = ItemSpawner.Instance;
            if (spawner != null && spawner.FixedYValues != null && spawner.FixedYValues.Length > 0)
            {
                float[] ys = spawner.FixedYValues;
                randomY = ys[Random.Range(0, ys.Length)];
            }

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
        foreach (var p in existingPositions)
        {
            if (Vector2.Distance(position, p) < minDistance) return true;
        }
        return false;
    }

    private List<Vector2> GetExistingItemPositions()
    {
        var positions = new List<Vector2>();
        var items = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item in items)
        {
            if (item.activeInHierarchy) positions.Add(item.transform.position);
        }
        return positions;
    }
}