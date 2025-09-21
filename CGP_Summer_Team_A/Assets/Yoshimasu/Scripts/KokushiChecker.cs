using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 国士無双の判定を行うユーティリティクラス
/// </summary>
public static class KokushiChecker
{
    // 国士無双に必要な13種類の牌
    private static readonly HashSet<TileType> KokushiTiles = new HashSet<TileType>
    {
        TileType.Man1, TileType.Man9,
        TileType.Pin1, TileType.Pin9,
        TileType.Sou1, TileType.Sou9,
        TileType.East, TileType.South, TileType.West, TileType.North,
        TileType.White, TileType.Green, TileType.Red
    };

    /// <summary>
    /// 国士無双の和了判定（14枚手牌用）
    /// </summary>
    public static bool IsKokushi(List<TileType> hand)
    {
        if (hand == null || hand.Count != 14)
        {
            return false;
        }

        var unique = new HashSet<TileType>(hand);

        // 必要な13種類がすべてあるか
        if (!KokushiTiles.IsSubsetOf(unique))
        {
            return false;
        }

        // どれか1種類が2枚以上（雀頭）
        return KokushiTiles.Any(t => hand.Count(x => x == t) >= 2);
    }

    /// <summary>
    /// 国士無双のテンパイ判定（13枚手牌用）
    /// </summary>
    public static bool IsKokushiTenpai(List<TileType> hand, out List<TileType> waits)
    {
        waits = new List<TileType>();

        if (hand == null || hand.Count != 13)
        {
            return false;
        }

        var unique = new HashSet<TileType>(hand);

        // 足りない牌を列挙
        var missing = KokushiTiles.Except(unique).ToList();

        if (missing.Count > 1)
        {
            // 2種類以上足りないならテンパイではない
            return false;
        }

        if (missing.Count == 1)
        {
            // 13種すべて揃っていて雀頭がない場合 → 13面待ち
            waits.Add(missing[0]);
        }

        // 13種すべて揃っている場合、雀頭があれば「単騎待ち」
        if (KokushiTiles.IsSubsetOf(unique))
        {
            foreach (var t in KokushiTiles)
            {
                int cnt = hand.Count(x => x == t);
                if (cnt == 2)
                {
                    // すでに雀頭がある → 待ちは他の国士牌すべて
                    waits = KokushiTiles.Where(tile => !hand.Contains(tile) || hand.Count(x => x == tile) == 1).ToList();
                    break;
                }
            }
        }

        return waits.Count > 0;
    }
}
