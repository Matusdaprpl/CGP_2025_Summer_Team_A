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

    public ItemController SpawnItemFromMountain(Vector3 pos)
    {
        Tile tile = MahjongManager.instance.DrawTile();
        if (tile == null) return null;

        ApplyTileSprite(tile);
        return CreateWorldItem(tile, pos, false);
    }

    public ItemController SpawnItemFromRecycleOrMountain(Vector3 pos)
    {
        const int maxAttempts = 50;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            bool fromRecycle = recyclePool.Count > 0;
            Tile tile = null;

            if (fromRecycle)
            {
                tile = recyclePool[0];
                recyclePool.RemoveAt(0);
            }
            else
            {
                tile = MahjongManager.instance.DrawTile();
            }

            if (tile == null) return null;

            if (!CanSpawnTile(tile))
            {
                if (fromRecycle)
                {
                    recyclePool.Add(tile);
                }
                else
                {
                    MahjongManager.instance.ReturnTileToMountain(tile);
                }
                continue;
            }

            ApplyTileSprite(tile);
            return CreateWorldItem(tile, pos, true);
        }

        return null;
    }

    public void DropDiscardedTile(Tile discardedTile, Vector3 dropPosition)
    {
        if (discardedTile == null || worldItemPrefab == null)
        {
            Debug.LogError("DropDiscardedTile: discardedTile または worldItemPrefab が null です。");
            return;
        }

        ApplyTileSprite(discardedTile);
        CreateWorldItem(discardedTile, dropPosition, true);
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

        var go = Instantiate(worldItemPrefab, pos, Quaternion.identity);
        var ic = go.GetComponent<ItemController>();
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
    public void DropItem(Tile tile, Vector3 position)
    {
    // 牌をドロップする処理をここに記述
    // 例: Instantiate(tilePrefab, position, Quaternion.identity);
    }
}