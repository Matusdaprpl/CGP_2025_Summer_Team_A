using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// 【重要】MahjongManagerのTile, Suitクラスが別の名前空間にある場合、
// ここに using Soga; のように追記が必要です。

/// <summary>
/// 四槓子（スーカンツ）の判定ユーティリティ
/// (4つの4枚組（槓子）と1つの2枚組（雀頭）で構成されているかをチェック)
/// </summary>
public static class SuukantsuChecker
{
    /// <summary>
    /// 四槓子の判定を実行します。
    /// （簡易的に、手牌14枚が「4枚組×4」と「2枚組×1」で構成されているかをチェック）
    /// </summary>
    /// <param name="hand">14枚の和了手牌リスト（MahjongManagerのTileクラスを想定）</param>
    /// <returns>四槓子が成立するか</returns>
    public static bool IsSuukantsu(List<Tile> hand)
    {
        // 14枚でなければ不成立
        if (hand == null || hand.Count != 14)
        {
            Debug.Log("[SuukantsuChecker DEBUG] Hand count is not 14. Failed.");
            return false;
        }

        // 1. 牌の種類と枚数をカウント
        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());
        
        // ★★★ デバッグログを追加 ★★★
        Debug.Log("[SuukantsuChecker DEBUG] Counted tiles:");
        foreach (var pair in counts)
        {
            Debug.Log($"  - {pair.Key.suit} {pair.Key.rank}: {pair.Value}枚");
        }

        int countQuads = 0;   // 4枚組の数 (槓子)
        int countPairs = 0;   // 2枚組の数 (雀頭)
        int otherCount = 0;   // 1枚組や3枚組の数 (四槓子では不可)
        
        // 2. 構成をチェック
        foreach (var count in counts.Values)
        {
            if (count == 4)
            {
                countQuads++;
            }
            else if (count == 2)
            {
                countPairs++;
            }
            else
            {
                // 1枚組や3枚組が存在する場合は、四槓子の形ではない
                otherCount++;
            }
        }

        // 3. 判定ロジック
        // ★★★ デバッグログを追加 ★★★
        Debug.Log($"[SuukantsuChecker DEBUG] Final Counts: Quads={countQuads}, Pairs={countPairs}, Others={otherCount}");

        // 条件: 4枚組がちょうど4つ AND 2枚組がちょうど1つ AND 他の組がない
        if (countQuads == 4 && countPairs == 1 && otherCount == 0)
        {
            Debug.Log("[SuukantsuChecker DEBUG] Passed all conditions.");
            return true;
        }
        else
        {
            Debug.Log("[SuukantsuChecker DEBUG] Failed to meet all conditions.");
            return false;
        }
    }
}