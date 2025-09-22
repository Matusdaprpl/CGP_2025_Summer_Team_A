using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 国士無双の判定を行うユーティリティクラス
/// </summary>
public static class KokushiChecker
{
    // 国士無双に必要な13種類の牌
    private static readonly List<Tile> KokushiTiles = new List<Tile>
    {
        new Tile(Suit.Manzu, 1, null), new Tile(Suit.Manzu, 9, null),
        new Tile(Suit.Pinzu, 1, null), new Tile(Suit.Pinzu, 9, null),
        new Tile(Suit.Souzu, 1, null), new Tile(Suit.Souzu, 9, null),
        new Tile(Suit.Honor, 1, null), new Tile(Suit.Honor, 2, null), new Tile(Suit.Honor, 3, null), new Tile(Suit.Honor, 4, null),
        new Tile(Suit.Honor, 5, null), new Tile(Suit.Honor, 6, null), new Tile(Suit.Honor, 7, null)
    };

    /// <summary>
    /// 国士無双の和了判定（14枚手牌用）
    /// </summary>
    public static bool IsKokushi(List<Tile> hand)
    {
        if (hand == null || hand.Count != 14)
        {
            return false;
        }

        var unique = new HashSet<string>(hand.Select(t => $"{t.suit}_{t.rank}"));

        // 必要な13種類がすべてあるか
        var kokushiKeys = KokushiTiles.Select(t => $"{t.suit}_{t.rank}").ToHashSet();
        if (!kokushiKeys.IsSubsetOf(unique))
        {
            return false;
        }

        // どれか1種類が2枚以上（雀頭）
        return KokushiTiles.Any(t => hand.Count(x => x.suit == t.suit && x.rank == t.rank) >= 2);
    }

    /// <summary>
    /// 国士無双のテンパイ判定（13枚手牌用）
    /// </summary>
    public static bool IsKokushiTenpai(List<Tile> hand, out List<Tile> waits)
    {
        waits = new List<Tile>();

        if (hand == null || hand.Count != 13)
        {
            return false;
        }

        var unique = new HashSet<string>(hand.Select(t => $"{t.suit}_{t.rank}"));

        // 足りない牌を列挙
        var kokushiKeys = KokushiTiles.Select(t => $"{t.suit}_{t.rank}").ToHashSet();
        var missingKeys = kokushiKeys.Except(unique).ToList();

        if (missingKeys.Count > 1)
        {
            // 2種類以上足りないならテンパイではない
            return false;
        }

        if (missingKeys.Count == 1)
        {
            // 13種すべて揃っていて雀頭がない場合 → 13面待ち
            var missingTile = KokushiTiles.First(t => $"{t.suit}_{t.rank}" == missingKeys[0]);
            waits.Add(missingTile);
        }

        // 13種すべて揃っている場合、雀頭があれば「単騎待ち」
        if (kokushiKeys.IsSubsetOf(unique))
        {
            foreach (var t in KokushiTiles)
            {
                int cnt = hand.Count(x => x.suit == t.suit && x.rank == t.rank);
                if (cnt == 2)
                {
                    // すでに雀頭がある → 待ちは他の国士牌すべて
                    waits = KokushiTiles.Where(tile => hand.Count(x => x.suit == tile.suit && x.rank == tile.rank) == 0).ToList();
                    break;
                }
            }
        }

        return waits.Count > 0;
    }
}