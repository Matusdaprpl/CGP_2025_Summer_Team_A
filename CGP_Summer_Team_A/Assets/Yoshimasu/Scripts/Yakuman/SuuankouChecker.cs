using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// 【重要】MahjongManagerのTile, Suitクラスが別の名前空間にある場合、
// ここに using Soga; のように追記が必要です。

/// <summary>
/// 四暗刻（スーアンコウ）の判定ユーティリティ
/// (4つの3枚組（刻子）と1つの2枚組（雀頭）で構成されているかをチェック)
/// </summary>
public static class SuuankouChecker
{
    /// <summary>
    /// 四暗刻の判定を実行します。
    /// （簡易的に、手牌14枚が「3枚組×4」と「2枚組×1」で構成されているかをチェック）
    /// </summary>
    /// <param name="hand">14枚の和了手牌リスト</param>
    /// <returns>四暗刻が成立するか</returns>
    public static bool IsSuuankou(List<Tile> hand)
    {
        // 14枚でなければ不成立
        if (hand == null || hand.Count != 14)
        {
            return false;
        }

        // 1. 牌の種類と枚数をカウント
        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        int countTriplets = 0; // 3枚組の数 (刻子)
        int countPairs = 0;    // 2枚組の数 (雀頭)
        int otherCount = 0;    // 1枚組や4枚組の数 (四暗刻では不可)

        // 2. 構成をチェック
        foreach (var count in counts.Values)
        {
            if (count == 3)
            {
                countTriplets++;
            }
            else if (count == 2)
            {
                countPairs++;
            }
            // 4枚組（槓子）が含まれる場合は四暗刻にならない
            else if (count == 4)
            {
                // 四暗刻単騎待ちを考慮する場合、ここのロジックはより複雑になります。
                // 簡易版では4枚組がある場合は不成立とします。
                otherCount++;
            }
            else
            {
                // 1枚組が含まれる場合は不成立
                otherCount++;
            }
        }

        // 3. 判定ロジック
        // 条件: 3枚組がちょうど4つ AND 2枚組がちょうど1つ AND 他の組がない
        if (countTriplets == 4 && countPairs == 1 && otherCount == 0)
        {
            //Debug.Log("[SuuankouChecker DEBUG] Passed all conditions.");
            return true;
        }
        else
        {
            //Debug.Log($"[SuuankouChecker DEBUG] Failed. Triplets:{countTriplets}, Pairs:{countPairs}, Others:{otherCount}");
            return false;
        }
    }
}