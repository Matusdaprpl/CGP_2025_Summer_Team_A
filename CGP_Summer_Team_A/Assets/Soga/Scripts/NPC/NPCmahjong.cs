using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NPCmahjong : MonoBehaviour
{
    [Header("NPC設定")]
    [Tooltip("目指す役満")]
    public Yakuman targetYakuman;
    public List<Tile> hand = new List<Tile>();

    [Header("捨て牌設定")]
    [Tooltip("捨て牌のドロップ位置オフセット")]
    [SerializeField]
    private float discardOffset = 2.0f;

    void Start()
    {
        if (targetYakuman == Yakuman.None)
        {
            targetYakuman = (Random.Range(0, 2) == 0) ? Yakuman.KokushiMusou : Yakuman.Daisangen;
        }
    }

    public void DiscardTile()
    {
        if (hand.Count == 0) return;

        Tile tileToDiscard = YakumanEvaluator.ChooseDiscardTile(hand, targetYakuman);
        hand.Remove(tileToDiscard);

        Vector3 dropPosition = new Vector3(transform.position.x - discardOffset, transform.position.y, transform.position.z);
        ItemManager.instance.DropDiscardedTile(tileToDiscard, dropPosition);

        Debug.Log($"{gameObject.name}の捨て牌: {tileToDiscard.GetDisplayName()}");
    }

    public void AddTileToHand(Tile tile)
    {
        if (tile == null || hand == null) return;
        if (hand.Count >= 15) return; // 上限チェック

        hand.Add(tile);
    }

    
}
