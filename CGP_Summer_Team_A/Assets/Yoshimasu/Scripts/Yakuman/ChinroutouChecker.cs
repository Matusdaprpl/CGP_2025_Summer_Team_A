using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ChinroutouChecker
{
    public static bool IsChinroutou(List<Tile> hand)
    {
        if (hand == null || hand.Count != 14) return false;

        // 1. すべての牌が老頭牌（1または9）であるかを確認
        foreach (var tile in hand)
        {
            if (tile.suit == Suit.Honor) return false; // 字牌は含まれない
            if (tile.rank != 1 && tile.rank != 9) return false;
        }

        // 2. 牌の構成（刻子、雀頭）をチェック
        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        int countTriplets = 0; 
        int countQuads = 0;    
        int countPairs = 0;    
        
        foreach (var count in counts.Values)
        {
            if (count == 3)
            {
                countTriplets++;
            }
            else if (count == 4)
            {
                countQuads++;
            }
            else if (count == 2)
            {
                countPairs++;
            }
            else
            {
                return false;
            }
        }
        
        // 3. 最終判定: 刻子/槓子4つと雀頭1つ
        if ((countTriplets + countQuads) == 4 && countPairs == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}