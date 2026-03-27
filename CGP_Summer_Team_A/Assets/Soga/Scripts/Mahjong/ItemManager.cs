using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    [Header("プレハブ設定")]
    public GameObject worldItemPrefab;

    private readonly List<Tile> recyclePool = new List<Tile>();
    private int activeWorldItemCount = 0;
    public int ActiveWorldItemCount => activeWorldItemCount;

    private Queue<ItemController> objectPool = new Queue<ItemController>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void AddToRecyclePool(Tile tile)
    {
        if (tile == null)
        {
            return;
        }

        if(!recyclePool.Contains(tile))
        {
            recyclePool.Add(tile);
        }
    }

    public ItemController SpawnItemFromMountain(Vector3 pos)
    {
        var mm = MahjongManager.instance;
        if (mm == null || mm.mountain == null || mm.mountain.Count <= 0) return null;

        int attempts = mm.mountain.Count;
        for (int i = 0; i < attempts; i++)
        {
            Tile tile = mm.DrawTile();
            if (tile == null) return null;

            if (!CanSpawnTile(tile))
            {
                recyclePool.Add(tile);
                continue;
            }

            ApplyTileSprite(tile);
            return CreateWorldItem(tile, pos, false);
        }

        return null;
    }

    public ItemController SpawnItemFromRecycleOrMountain(Vector3 pos)
    {
        if (!TryTakeRandomSpawnableTile(out Tile tile))
        {
            return null;
        }

        ApplyTileSprite(tile);
        return CreateWorldItem(tile, pos, true);
    }

    private bool TryTakeRandomSpawnableTile(out Tile tile)
    {
        tile = null;
        var mm = MahjongManager.instance;
        if (mm == null || mm.mountain == null) return false;

        int mountainCount = mm.mountain.Count;
        int recycleCount = recyclePool.Count;
        if (mountainCount + recycleCount <= 0) return false;

        bool tryMountainFirst = mountainCount > 0;
        if (TryTakeFromSource(!tryMountainFirst, mm, out tile)) return true;
        if (TryTakeFromSource(tryMountainFirst, mm, out tile)) return true;

        return false;
    }

    private bool TryTakeFromSource(bool fromRecycle, MahjongManager mm, out Tile tile)
    {
        tile = null;

        if (fromRecycle)
        {
            return TryTakeFromList(recyclePool, out tile);
        }
        else
        {
            return TryTakeFromList(mm.mountain, out tile);
        }
    }

    private bool TryTakeFromList(List<Tile> source, out Tile tile)
    {
        tile = null;
        if (source == null || source.Count <= 0) return false;

        int count = source.Count;
        int startIndex = Random.Range(0, count);

        // ランダム開始位置から1周して、スポーン可能な牌を確実に探す。
        for (int offset = 0; offset < count; offset++)
        {
            int index = (startIndex + offset) % count;
            Tile candidate = source[index];

            if (candidate == null)
            {
                source.RemoveAt(index);
                return TryTakeFromList(source, out tile);
            }

            if (!CanSpawnTile(candidate)) continue;

            source.RemoveAt(index);
            tile = candidate;
            return true;
        }

        return false;
    }

    public void DropDiscardedTile(Tile discardedTile, Vector3 dropPosition)
    {
        if (discardedTile == null || worldItemPrefab == null)
        {
            Debug.LogError("DropDiscardedTile: discardedTile または worldItemPrefab が null です。");
            return;
        }

        if(!CanSpawnTile(discardedTile))
        {
            AddToRecyclePool(discardedTile);
            return;
        }

        ApplyTileSprite(discardedTile);
        CreateWorldItem(discardedTile, dropPosition, true);
    }

    public void DropItem(Tile tile, Vector3 position)
    {
        // 既存の捨て牌ドロップ処理に統一
        DropDiscardedTile(tile, position);
    }

    public void NotifyItemPickedUp(Tile tile, bool isRecyclable)
    {
        if (activeWorldItemCount > 0)
        {
            activeWorldItemCount--;
        }

        if (isRecyclable && tile != null)
        {
            recyclePool.Add(tile);
        }
    }

    private ItemController CreateWorldItem(Tile tile, Vector3 pos, bool isRecyclable)
    {
        if (worldItemPrefab == null) return null;

        ItemController ic;
        if(objectPool.Count > 0)
        {
            ic = objectPool.Dequeue();
            // 破棄されていないか確認
            if (ic == null || ic.gameObject == null)
            {
                ic = null;
            }
            else
            {
                ic.gameObject.SetActive(true);
                ic.transform.position = pos;
            }
        }
        else
        {
            ic = null;
        }

        // プールから有効なオブジェクトが取得できなかった場合は新規作成
        if (ic == null)
        {
            var go = Instantiate(worldItemPrefab, pos, Quaternion.identity);
            ic = go.GetComponent<ItemController>();
        }

        if (ic != null)
        {
            ic.SetTile(MahjongManager.instance, tile, isRecyclable);
            activeWorldItemCount++;
        }
        else
        {
            Debug.LogError("CreateWorldItem: ItemController コンポーネントが見つかりません。");
        }
        return ic;
    }

    private void ApplyTileSprite(Tile tile)
    {
        if (tile == null) return;

        var tileSprites = MahjongUIManager.instance.GetTileSprites();
        if (tileSprites != null)
        {
            var key = (tile.suit == Suit.Honor) ? $"Honor_{tile.rank}" : $"{tile.suit}_{tile.rank}";
            if (tileSprites.TryGetValue(key, out var sp))
            {
                tile.sprite = sp;
            }
        }
    }

    private bool CanSpawnTile(Tile tile)
    {
        if (tile == null) return false;

        int sameCount = 0;
        var items = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item in items)
        {
            var ic = item.GetComponent<ItemController>();
            if (ic == null) continue;
            var t = ic.GetTile();
            if (t == null) continue;

            if (t.suit == tile.suit && t.rank == tile.rank)
            {
                sameCount++;
                if (sameCount >= 4)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void ReturnTiletoMountain(ItemController tile)
    {
        if(tile == null) return;
        tile.gameObject.SetActive(false);
        objectPool.Enqueue(tile);
    }
}