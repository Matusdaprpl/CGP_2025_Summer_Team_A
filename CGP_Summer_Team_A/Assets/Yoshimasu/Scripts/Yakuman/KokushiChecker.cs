using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class KokushiChecker
{
    public static bool IsKokushi(List<Tile> hand)
    {
        if (hand == null || hand.Count != 14) return false;

        // 13種のヤオ九牌の定義 (SuitとRankのタプルで表現)
        var kokushiKeys = new List<(Suit suit, int rank)>
        {
            (Suit.Manzu, 1), (Suit.Manzu, 9),
            (Suit.Pinzu, 1), (Suit.Pinzu, 9),
            (Suit.Souzu, 1), (Suit.Souzu, 9),
            (Suit.Honor, 1), (Suit.Honor, 2), (Suit.Honor, 3), (Suit.Honor, 4),
            (Suit.Honor, 5), (Suit.Honor, 6), (Suit.Honor, 7)
        };

        // 1. 13種のヤオ九牌をすべて含んでいるかをチェック
        var distinctHandKeys = hand.Select(t => (t.suit, t.rank)).ToHashSet();
        
        // 13種類のヤオ九牌すべてが手牌に含まれているか
        bool all13Exist = kokushiKeys.All(key => distinctHandKeys.Contains(key));
        if (!all13Exist) return false;

        // 2. 13種の中からいずれか1種類が2枚以上（雀頭）あるか
        // 13種類すべて含まれていて、合計14枚なので、必ずどれか1枚が2枚になっているはず
        if (hand.Count == 14)
        {
            return true;
        }

        return false;
    }
}