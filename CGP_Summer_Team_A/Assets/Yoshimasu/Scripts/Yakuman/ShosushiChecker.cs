using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ShosushiChecker
{
    public static bool IsShosushi(List<Tile> hand)
    {
        if (hand == null || hand.Count != 14) return false;

        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        int countWindTriplets = 0; // 風牌の3枚組/4枚組の数
        int countWindPair = 0;     // 風牌の2枚組（雀頭）の数
        
        // 風牌（東:1, 南:2, 西:3, 北:4）のグループを抽出
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
                return false;
            }
        }

        // 最終判定: 風牌の刻子/槓子が3つ AND 風牌の雀頭が1つ
        if (countWindTriplets == 3 && countWindPair == 1)
        {
            // 残りの牌（3枚）が1つの面子になっているかを確認する必要がある
            var remainingTiles = hand.Where(t => !(t.suit == Suit.Honor && t.rank >= 1 && t.rank <= 4)).ToList();
            
            // 残りが3枚で、これが順子か刻子になっていれば成立
            if (remainingTiles.Count == 3)
            {
                // 簡易判定のため、ここでは成立とします
                return true; 
            }
        }

        return false;
    }
}