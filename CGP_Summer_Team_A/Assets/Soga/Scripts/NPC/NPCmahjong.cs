using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NPCmahjong : MonoBehaviour
{
    [Header("NPC設定")]
    [Tooltip("目指す役満")]
    public Yakuman targetYakuman;
    public List<Tile> hand = new List<Tile>();

    [Header("参照")]
    [SerializeField]
    private NPCplayer npcPlayer;

    [Header("捨て牌設定")]
    [Tooltip("捨て牌のドロップ位置オフセット")]
    [SerializeField]
    private float discardOffset = 2.0f;

    private void Awake()
    {
        if (npcPlayer == null) npcPlayer = GetComponent<NPCplayer>();
    }

    void Start()
    {
        for (int i = 0; i < 14; i++)
        {
            Tile tile = MahjongManager.instance.DrawTile();
            if (tile != null)
            {
                hand.Add(tile);
            }
        }
        Debug.Log($"{gameObject.name}の初期手牌: {string.Join(", ", hand.Select(t => t.GetDisplayName()))}");

        if (npcPlayer == null && targetYakuman == Yakuman.None)
        {
            targetYakuman = (Random.Range(0, 2) == 0) ? Yakuman.KokushiMusou : Yakuman.Daisangen;
        }
    }

    public void DiscardTile()
    {
        if (hand.Count == 0) return;

        Yakuman target = (npcPlayer != null) ? npcPlayer.TargetYakuman : targetYakuman;

        Tile tileToDiscard = YakumanEvaluator.ChooseDiscardTile(hand, target);
        hand.Remove(tileToDiscard);

        Vector3 dropPosition = new Vector3(transform.position.x - discardOffset, transform.position.y + 0.2f, transform.position.z);
        ItemManager.instance.DropDiscardedTile(tileToDiscard, dropPosition);

        Debug.Log($"{gameObject.name}の捨て牌: {tileToDiscard.GetDisplayName()}");
    }

    public void AddTileToHand(Tile tile)
    {
        if (tile == null || hand == null) return;
        if (hand.Count >= 15) return; // 上限チェック

        hand.Add(tile);
    }
    // オブジェクトが他のコライダーと衝突した瞬間に呼び出されるメソッド
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 衝突した相手のタグが "Bullet" だった場合
        if (other.gameObject.CompareTag("Bullet"))
        {
            DropItem(); // アイテムをドロップする
            Destroy(other.gameObject); // 衝突した弾を消す
            Debug.Log($"{gameObject.name}が弾に当たりました。");
        }
    }

    private void DropItem()
    {
        Debug.Log($"がドロップした牌: ");
        //if (hand.Count == 0) return;

        // ランダムに手牌から1枚選ぶ
        int randomIndex = Random.Range(0, hand.Count);
        Tile tileToDrop = hand[randomIndex];
        hand.RemoveAt(randomIndex);

        // アイテムをドロップする位置を計算
        Vector3 dropPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // アイテムをドロップ
        ItemManager.instance.DropItem(tileToDrop, dropPosition);

        Debug.Log($"{gameObject.name}がドロップした牌: {tileToDrop.GetDisplayName()}");
    }

    public void PrintHandToConsole(string context)
    {
        string handDescription = string.Join(",", hand.Select(t => t.GetDisplayName()));
        Debug.Log($"{context}: {handDescription}");
    }
}