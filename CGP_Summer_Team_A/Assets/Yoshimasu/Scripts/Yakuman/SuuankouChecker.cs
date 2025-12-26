using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SuuankouChecker
{
    public static bool IsSuuankou(List<Tile> hand)
    {
        if (hand == null || hand.Count != 14) return false;

        // 1. 牌の種類と枚数をカウント
        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        int countTriplets = 0; // 3枚組の数 (刻子)
        int countQuads = 0;    // 4枚組の数 (槓子)
        int countPairs = 0;    // 2枚組の数 (雀頭)
        int otherCount = 0;    // 1枚組など

        // 2. 構成をチェック
        foreach (var count in counts.Values)
        {
            if (count == 3)
            {
                countTriplets++;
            }
            else if (count == 4)
            {
                countQuads++; // 槓子も刻子と見なす
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

        // 3. 判定ロジック
        // 条件: 3枚組+4枚組の合計が4つ AND 2枚組がちょうど1つ AND 他の組がない
        if ((countTriplets + countQuads) == 4 && countPairs == 1 && otherCount == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}