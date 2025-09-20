using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NPCmahjong : MonoBehaviour
{
    [Header("NPC設定")]
    [Tooltip("目指す役満")]
    public Yakuman targetYakuman;
    public List<Tile> hand = new List<Tile>();
    private DrawTrigger drawTrigger;

    void Awake()
    {
        drawTrigger = GetComponent<DrawTrigger>();
        if (drawTrigger == null)
        {
            Debug.LogError("DrawTriggerコンポーネントが見つかりません。");
            return;
        }
        drawTrigger.OnDrawRequested += HandleDrawRequest;
    }

    void Start()
    {
        if (targetYakuman == Yakuman.None)
        {
            targetYakuman = (Random.Range(0, 2) == 0) ? Yakuman.KokushiMusou : Yakuman.Daisangen;
        }
    }

    private void HandleDrawRequest()
    {
        Tile drawnTile = MahjongManager.instance.DrawTile();
        if (drawnTile == null) return;

        hand.Add(drawnTile);
        DiscardTile();
    }

    private void DiscardTile()
    {
        if (hand.Count == 0) return;

        Tile tileToDiscard = YakumanEvaluator.ChooseDiscardTile(hand, targetYakuman);
        hand.Remove(tileToDiscard);
        Debug.Log($"NPCの捨て牌: {tileToDiscard.GetDisplayName()}");
    }

    void OnDestroy()
    {
        if (drawTrigger != null)
        {
            drawTrigger.OnDrawRequested -= HandleDrawRequest;
        }
    }
}
