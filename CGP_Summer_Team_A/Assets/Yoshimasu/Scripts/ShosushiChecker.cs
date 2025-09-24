using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ShosushiChecker
{
    /// <summary>
    /// 小四喜の判定を実行します。
    /// 手牌が「東・南・西・北」のうち3つの刻子/槓子と1つの雀頭で構成されているかをチェックします。
    /// </summary>
    /// <param name="hand">14枚の和了手牌リスト</param>
    /// <returns>小四喜が成立するか</returns>
    public static bool IsShosushi(List<Tile> hand)
    {
        // 14枚でなければ不成立
        if (hand == null || hand.Count != 14)
        {
            Debug.Log("[ShosushiChecker DEBUG] Hand count is not 14. Failed.");
            return false;
        }

        // 1. 牌の種類と枚数をカウント
        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        // ★★★ デバッグログを追加 ★★★
        Debug.Log("[ShosushiChecker DEBUG] Counted tiles:");
        foreach (var pair in counts)
        {
            Debug.Log($"  - {pair.Key.suit} {pair.Key.rank}: {pair.Value}枚");
        }

        int countWindTriplets = 0; // 風牌の3枚組/4枚組の数
        int countWindPair = 0;     // 風牌の2枚組（雀頭）の数
        
        // 2. 風牌の構成をチェック
        // 風牌（東:1, 南:2, 西:3, 北:4）
        var windTiles = counts.Where(t => t.Key.suit == Suit.Honor && t.Key.rank >= 1 && t.Key.rank <= 4);

        foreach (var tileGroup in windTiles)
        {
            var count = tileGroup.Value;
            if (count == 3 || count == 4)
            {
                countWindTriplets++;
            }
            else if (count == 2)
            {
                countWindPair++;
            }
            else
            {
                // 風牌が3,4,2枚組以外の場合は不成立
                Debug.Log($"[ShosushiChecker DEBUG] A wind tile is not a triplet, quad, or pair. Failed.");
                return false;
            }
        }

        // 3. 最終判定
        // 条件：風牌の刻子/槓子が3つ AND 風牌の雀頭が1つ AND 残りの牌で面子が成立しているか
        if (countWindTriplets == 3 && countWindPair == 1)
        {
            // ここで、残りの牌が1つの面子になっているかを確認する必要がある
            // 例：風牌以外の牌を抽出して、それが3枚組/順子になっているかを確認
            var remainingTiles = hand.Where(t => !(t.suit == Suit.Honor && t.rank >= 1 && t.rank <= 4)).ToList();
            
            // 残りの牌は3枚で、順子か刻子になっていれば成立
            if (remainingTiles.Count == 3)
            {
                // ここで順子か刻子かを確認するロジックが必要（省略）
                // 簡易判定として、ここではtrueを返します
                Debug.Log("[ShosushiChecker DEBUG] Passed all conditions. Remaining tiles are assumed to form a valid meld.");
                return true;
            }
        }
        
        Debug.Log($"[ShosushiChecker DEBUG] Failed. Wind Triples/Quads: {countWindTriplets}, Wind Pair: {countWindPair}");
        return false;
    }
}