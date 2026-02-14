using System;
using System.Diagnostics;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    // このアイテムが表す確定済みの牌（生成時にセット）
    private Tile tile;
    private MahjongManager manager;
    private SpriteRenderer spriteRenderer;
    private bool isRecyclable;
    private bool hasNotifiedManager;

    public static event Action<string, int> OnItemPickedUp;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // MahjongManager.SpawnItemFromMountain から呼ぶ初期化
    public void SetTile(MahjongManager mgr, Tile t, bool recyclable)
    {
        manager = mgr;
        tile = t;
        isRecyclable = recyclable;

        // 見た目と名前を固定
        if (spriteRenderer != null && tile != null)
        {
            spriteRenderer.sprite = tile.sprite;
        }
    }

    public Tile GetTile()
    {
        return tile;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (manager == null || tile == null) return;

            if (manager.playerHand != null && manager.playerHand.Count >= 15)
            {
                return;
            }

            manager.AddTileToPlayerHand(tile);

            NotifyManagerPickedUp();

            OnItemPickedUp?.Invoke(tile.suit.ToString(), tile.rank);
            UnityEngine.Debug.Log($"拾った牌:{tile.GetDisplayName()}");

            Destroy(gameObject);

        }
        else if (other.CompareTag("NPC"))
        {
            OnItemPickedUp?.Invoke(tile.suit.ToString(), tile.rank);
        }
    }

    private void OnDestroy()
    {
        if (!hasNotifiedManager && ItemManager.instance != null)
        {
            ItemManager.instance.NotifyItemPickedUp(tile, isRecyclable);
            hasNotifiedManager = true;
        }
    }

    private void NotifyManagerPickedUp()
    {
        if (hasNotifiedManager || ItemManager.instance == null) return;
        ItemManager.instance.NotifyItemPickedUp(tile, isRecyclable);
        hasNotifiedManager = true;
    }
}
