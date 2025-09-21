using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 大三元 (Daisangen) の判定ユーティリティ
/// </summary>
public static class DaisangenChecker
{
    private static readonly List<Tile> SANGEN_PAI = new List<Tile>
    {
        new Tile(Suit.Honor, 5, null), // 白
        new Tile(Suit.Honor, 6, null), // 發
        new Tile(Suit.Honor, 7, null)  // 中
    };

    /// <summary>
    /// 手牌がチー/ポン/カンを考慮した大三元であるかを判定します。
    /// </summary>
    /// <param name="hand">14枚の牌（手牌13枚 + ツモ牌1枚）</param>
    public static bool IsDaisangen(List<Tile> hand)
    {
        // 14枚でなければ不成立
        if (hand == null || hand.Count != 14) return false;

        // 1. 牌の種類ごとの枚数をカウント
        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        // 2. 白・發・中がすべて刻子（3枚以上）になっているかをチェック
        foreach (var tile in SANGEN_PAI)
        {
            var key = new { tile.suit, tile.rank };
            if (!counts.ContainsKey(key) || counts[key] < 3)
            {
                // 三元牌のいずれかが3枚未満であれば不成立
                return false;
            }
        }

        // 3. 牌の残りが 4面子1雀頭の形で成立しているか？
        // 簡易チェックとして、三元牌の刻子があることを確認できたら、
        // それだけで「大三元」の構成要素は満たしているとします。
        // ※ 最終的なゲームでは、この後、基本の和了形 (4面子1雀頭) 判定を通す必要があります。

        return true;
    }
}