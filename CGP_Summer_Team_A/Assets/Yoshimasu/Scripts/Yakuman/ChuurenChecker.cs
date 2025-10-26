using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ChuurenChecker
{
    public static bool IsChuuren(List<Tile> hand)
    {
        if (hand == null || hand.Count != 14) return false;

        // 1. 単一の数牌（マンズ、ピンズ、ソウズ）かを確認
        var validSuits = new[] { Suit.Manzu, Suit.Pinzu, Suit.Souzu };
        var distinctSuits = hand.Select(t => t.suit).Distinct().ToList();

        if (distinctSuits.Count != 1 || !validSuits.Contains(distinctSuits.First())) return false;
        
        Suit targetSuit = distinctSuits.First();

        // 2. 牌の数をカウント
        var counts = hand.Where(t => t.suit == targetSuit && t.rank >= 1 && t.rank <= 9)
                       .GroupBy(t => t.rank)
                       .ToDictionary(g => g.Key, g => g.Count());
        
        // 3. 待ち牌を除いた基本形 (1112345678999) をチェック
        var required = new Dictionary<int, int> { { 1, 3 }, { 9, 3 } };
        for (int r = 2; r <= 8; r++) required.Add(r, 1);
        
        for (int r = 1; r <= 9; r++)
        {
            // 該当の牌が2枚以上あれば、それを雀頭候補として基本形と比較
            if (counts.ContainsKey(r) && counts[r] >= required[r] + 1)
            {
                bool isBaseForm = true;
                
                for (int i = 1; i <= 9; i++)
                {
                    int currentCount = counts.ContainsKey(i) ? counts[i] : 0;
                    int requiredCount = required.ContainsKey(i) ? required[i] : 0; // 安全のためContainsKeyを追加

                    if (i == r) // 雀頭候補（r）の場合は1枚引いて確認
                    {
                        if (currentCount - 1 != requiredCount) isBaseForm = false;
                    }
                    else // それ以外の牌は requiredCountと完全に一致する必要がある
                    {
                        if (currentCount != requiredCount) isBaseForm = false;
                    }
                }
                
                if (isBaseForm) return true;
            }
        }

        return false;
    }
}