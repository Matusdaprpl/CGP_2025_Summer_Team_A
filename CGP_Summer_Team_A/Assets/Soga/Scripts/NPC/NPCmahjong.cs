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
        if (npcPlayer == null && targetYakuman == Yakuman.None)
        {
            targetYakuman = (Random.Range(0, 2) == 0) ? Yakuman.KokushiMusou : Yakuman.Daisangen;
        }
    }

    public void DiscardTile()
    {
        if (hand.Count == 0) return;

        Yakuman target = (npcPlayer != null) ? npcPlayer.TargetYakuman : targetYakuman;

        string handDescription = string.Join(",", hand.Select(t => t.GetDisplayName()));
        Debug.Log($"{gameObject.name}の手牌: {handDescription}, 目標役満: {target}");

        Tile tileToDiscard = YakumanEvaluator.ChooseDiscardTile(hand, target);

        bool isNeeded = YakumanEvaluator.IsTileNeededFor(tileToDiscard, target, hand);
        string reason=isNeeded?"必要牌":"不要牌";
        Debug.Log($"{gameObject.name}の捨て牌:{tileToDiscard.GetDisplayName()}({reason})");

        hand.Remove(tileToDiscard);

        Vector3 dropPosition = new Vector3(transform.position.x - discardOffset, transform.position.y, transform.position.z);
        ItemManager.instance.DropDiscardedTile(tileToDiscard, dropPosition);
    }

    public void AddTileToHand(Tile tile)
    {
        if (tile == null || hand == null) return;
        if (hand.Count >= 15) return; // 上限チェック

        hand.Add(tile);
    }
    
    public void PrintHandToConsole(string contextMessage)
    {
        if (hand == null) return;

        string handDescription = string.Join(", ", hand.Select(t => t.GetDisplayName()));
        Debug.Log($"{gameObject.name} [{contextMessage}] 手牌 ({hand.Count}枚): {handDescription}");
    }
}
