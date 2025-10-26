using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DaisushiChecker
{
    public static bool IsDaisushi(List<Tile> hand)
    {
        if (hand == null || hand.Count != 14) return false;

        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        int countWindTriplets = 0; // 風牌の3枚組/4枚組の数
        int countPairs = 0;        // 雀頭の数
        int otherCount = 0;        // その他の牌の数

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

        // 3. 最終判定: 風牌の刻子/槓子が4つ AND 雀頭が1つ AND 他の牌がない
        if (countWindTriplets == 4 && countPairs == 1 && otherCount == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}