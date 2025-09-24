using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DaisushiChecker
{
    /// <summary>
    /// 大四喜の判定を実行します。
    /// 手牌が「東・南・西・北」の4つの刻子/槓子と、1つの雀頭で構成されているかをチェックします。
    /// </summary>
    /// <param name="hand">14枚の和了手牌リスト</param>
    /// <returns>大四喜が成立するか</returns>
    public static bool IsDaisushi(List<Tile> hand)
    {
        // 14枚でなければ不成立
        if (hand == null || hand.Count != 14)
        {
            Debug.Log("[DaisushiChecker DEBUG] Hand count is not 14. Failed.");
            return false;
        }

        // 1. 牌の種類と枚数をカウント
        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        // ★★★ デバッグログを追加 ★★★
        Debug.Log("[DaisushiChecker DEBUG] Counted tiles:");
        foreach (var pair in counts)
        {
            Debug.Log($"  - {pair.Key.suit} {pair.Key.rank}: {pair.Value}枚");
        }

        int countWindTriplets = 0; // 風牌の3枚組/4枚組の数
        int countPairs = 0;        // 雀頭の数
        int otherCount = 0;        // その他の牌の数

        // 2. 牌の構成をチェック
        foreach (var tileGroup in counts)
        {
            var tile = tileGroup.Key;
            var count = tileGroup.Value;

            if (tile.suit == Suit.Honor && (tile.rank >= 1 && tile.rank <= 4)) // 風牌の判定
            {
                if (count == 3 || count == 4)
                {
                    countWindTriplets++;
                }
                else
                {
                    // 風牌が3枚組や4枚組でない場合は不成立
                    Debug.Log("[DaisushiChecker DEBUG] A wind tile is not a triplet or quad. Failed.");
                    return false;
                }
            }
            else if (count == 2)
            {
                countPairs++;
            }
            else
            {
                otherCount++;
            }
        }

        // 3. 最終判定
        // ★★★ デバッグログを追加 ★★★
        Debug.Log($"[DaisushiChecker DEBUG] Wind Triples/Quads: {countWindTriplets}, Pairs: {countPairs}, Others: {otherCount}");

        // 条件：風牌の刻子/槓子が4つ AND 2枚組が1つ AND 他の牌がない
        if (countWindTriplets == 4 && countPairs == 1 && otherCount == 0)
        {
            Debug.Log("[DaisushiChecker DEBUG] Passed all conditions.");
            return true;
        }
        else
        {
            Debug.Log("[DaisushiChecker DEBUG] Failed to meet all conditions.");
            return false;
        }
    }
}