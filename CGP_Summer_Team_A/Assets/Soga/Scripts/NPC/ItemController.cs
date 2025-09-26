using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    // このアイテムが表す確定済みの牌（生成時にセット）
    private Tile tile;
    private MahjongManager manager;
    private SpriteRenderer spriteRenderer;

    public static event Action<string, int> OnItemPickedUp;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // MahjongManager.SpawnItemFromMountain から呼ぶ初期化
    public void SetTile(MahjongManager mgr, Tile t)
    {
        manager = mgr;
        tile = t;

        // 見た目と名前を固定
        if (spriteRenderer != null && tile != null)
        {
            spriteRenderer.sprite = tile.sprite;
        }
        // 必要ならデバッグ表示
        // Debug.Log($"生成: {tile.GetDisplayName()}");
    }

    public Tile GetTile()
    {
        return tile;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var root = other.transform.root;
        
        if (root.CompareTag("Player"))
        {
           if (manager == null || tile == null) return;

            // 成功時のみ削除
            if (manager.AddTileToPlayerHand(tile))
            {
                Debug.Log($"プレイヤーが拾った牌: {tile.GetDisplayName()}");
                OnItemPickedUp?.Invoke(tile.suit.ToString(), tile.rank);
                Destroy(gameObject);
            }
            else
            {
                // 上限などで追加不可 → 何もしない（アイテムは残す）
                Debug.Log("手牌上限のため拾えません。アイテムは残します。");
            }
        }
        else if (root.CompareTag("NPC"))
        {
            NPCmahjong npc = other.GetComponent<NPCmahjong>();
            if (npc != null && tile != null)
            {
                if (npc.hand.Count < 15)
                {
                    npc.hand.Add(tile);
                    Debug.Log($"NPCが牌を拾った: {tile.GetDisplayName()}");
                    Destroy(gameObject);
                }
            }
        }
    }
    
}
