using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ChinroutouChecker
{
    /// <summary>
    /// 清老頭の判定を実行します。
    /// 手牌が「1」と「9」の牌だけで構成されているかをチェックします。
    /// </summary>
    /// <param name="hand">14枚の和了手牌リスト</param>
    /// <returns>清老頭が成立するか</returns>
    public static bool IsChinroutou(List<Tile> hand)
    {
        // 14枚でなければ不成立
        if (hand == null || hand.Count != 14)
        {
            Debug.Log("[ChinroutouChecker DEBUG] Hand count is not 14. Failed.");
            return false;
        }

        // 1. すべての牌が老頭牌（1または9）であるかを確認
        foreach (var tile in hand)
        {
            if (tile.suit == Suit.Honor) // 字牌は清老頭に含まれない
            {
                Debug.Log("[ChinroutouChecker DEBUG] Hand contains an Honor tile. Failed.");
                return false;
            }
            if (tile.rank != 1 && tile.rank != 9)
            {
                Debug.Log($"[ChinroutouChecker DEBUG] Hand contains non-routo tile: {tile.suit} {tile.rank}. Failed.");
                return false;
            }
        }

        // 2. 牌の構成（刻子、雀頭）をチェック
        // 清老頭はすべて刻子/槓子と雀頭で構成されるため、14枚の牌の種類と枚数をカウント
        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        int countTriplets = 0; // 3枚組の数
        int countQuads = 0;    // 4枚組の数
        int countPairs = 0;    // 2枚組の数
        
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
                // 1枚組が含まれる場合は不成立
                Debug.Log($"[ChinroutouChecker DEBUG] Hand contains a group of {count}. Failed.");
                return false;
            }
        }
        
        // 3. 最終判定
        // 条件：3枚組と4枚組の合計が4つ AND 2枚組が1つ
        if ((countTriplets + countQuads) == 4 && countPairs == 1)
        {
            Debug.Log("[ChinroutouChecker DEBUG] Passed all conditions.");
            return true;
        }
        else
        {
            Debug.Log($"[ChinroutouChecker DEBUG] Failed. Triplets:{countTriplets}, Quads:{countQuads}, Pairs:{countPairs}");
            return false;
        }
    }
}