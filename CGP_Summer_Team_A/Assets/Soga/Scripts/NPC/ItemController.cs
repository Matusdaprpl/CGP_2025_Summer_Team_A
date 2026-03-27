using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    private Tile tile;
    private MahjongManager manager;
    private SpriteRenderer spriteRenderer;
    private bool isRecyclable;
    private bool hasNotifiedManager;
    //private bool consumed; ←NPCとPlayerで拾った牌を区別するときに使う
    public static event Action<string, int> OnItemPickedUp;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTile(MahjongManager mgr, Tile t, bool recyclable)
    {
        manager = mgr;
        tile = t;
        isRecyclable = recyclable;

        hasNotifiedManager = false;
        //consumed = false;

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

            bool added = manager.AddTileToPlayerHand(tile);
            if (!added)
            {
                return;
            }

            NotifyManagerPickedUp(false);

            OnItemPickedUp?.Invoke(tile.suit.ToString(), tile.rank);
            UnityEngine.Debug.Log($"拾った牌:{tile.GetDisplayName()}");

            ItemManager.instance.ReturnTiletoMountain(this);
        }
        else if (other.CompareTag("NPC"))
        {
            if (tile == null) return;

            NPCmahjong npcMahjong = other.GetComponent<NPCmahjong>();
            if (npcMahjong == null || npcMahjong.hand == null)
            {
                return;
            }

            if (npcMahjong.hand.Count >= 15)
            {
                return;
            }

            // NPCが牌を拾ったら手牌に加える。
            npcMahjong.AddTileToHand(tile);

            NotifyManagerPickedUp(false);
            OnItemPickedUp?.Invoke(tile.suit.ToString(), tile.rank);
            ItemManager.instance?.ReturnTiletoMountain(this);
        }
    }

    private void NotifyManagerPickedUp(bool recycled)
    {
        if (hasNotifiedManager || ItemManager.instance == null) return;
        ItemManager.instance.NotifyItemPickedUp(tile, recycled);
        hasNotifiedManager = true;
    }
}
