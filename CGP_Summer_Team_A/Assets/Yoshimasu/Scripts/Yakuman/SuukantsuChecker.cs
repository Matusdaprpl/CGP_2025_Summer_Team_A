using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// クラス名がファイル名と一致していることを確認
public static class SuukantsuChecker
{
    /// <summary>
    /// 手牌が四槓子（スーカンツ）であるか判定します。
    /// </summary>
    /// <param name="hand">手牌（14枚）</param>
    /// <returns>四槓子が成立していればtrue</returns>
    public static bool IsSuukantsu(List<Tile> hand)
    {
        // 牌の数が14枚でなければ不成立（通常、ロンやツモの直後に判定）
        if (hand == null || hand.Count != 14) return false;

        // 1. 牌の種類と枚数をカウント
        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        int countQuads = 0; // 4枚組の数（槓子）
        int countPairs = 0; // 2枚組の数（雀頭）
        int otherCount = 0; // 1枚組や3枚組の数
        
        // 2. 構成要素の枚数を数える
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
                // 四槓子は、4つの槓子と1つの雀頭で構成される役満のため、
                // 1枚組や3枚組が存在する場合は無条件で不成立。
                otherCount++;
            }
        }

        // 3. 最終判定
        // 条件：4つの4枚組（槓子）があり、残りの2枚組（雀頭）が1つであること。
        if (countQuads == 4 && countPairs == 1 && otherCount == 0)
        {
            return true;
        }

        return false;
    }
}