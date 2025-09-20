using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 国士無双の判定ユーティリティ
/// </summary>
public static class KokushiChecker
{
    private static readonly HashSet<TileType> KokushiTiles = new HashSet<TileType>
    {
        TileType.Man1, TileType.Man9,
        TileType.Pin1, TileType.Pin9,
        TileType.Sou1, TileType.Sou9,
        TileType.East, TileType.South, TileType.West, TileType.North,
        TileType.White, TileType.Green, TileType.Red
    };

    public static bool IsKokushi(List<TileType> hand)
    {
        if (hand == null || hand.Count != 14) return false;

        var unique = new HashSet<TileType>(hand);

        if (!KokushiTiles.IsSubsetOf(unique)) return false;

        foreach (var t in KokushiTiles)
        {
            if (hand.Count(x => x == t) >= 2) return true; // 雀頭あり
        }

        return false;
    }
}
