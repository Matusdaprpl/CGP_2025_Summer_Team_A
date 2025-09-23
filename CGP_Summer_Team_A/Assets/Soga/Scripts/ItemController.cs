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
        if (!other.CompareTag("Player")) return;
        if (manager == null || tile == null) return;

        // 上限: 15枚のときは拾わない（消さない）→16枚以上にしない
        if (manager.playerHand != null && manager.playerHand.Count >= 15)
        {
            return;
        }

        // 同じTileをそのまま手牌へ
        manager.AddTileToPlayerHand(tile);

        // 任意: 取得イベント（名前は表示用）
        OnItemPickedUp?.Invoke(tile.GetDisplayName(), 1);
        Debug.Log($"拾った牌: {tile.GetDisplayName()}");

        Destroy(gameObject);
    }
}
