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
    /// 国士無双判定
    /// 手牌は14枚（和了時）であることを期待します。
    /// </summary>
    public static bool IsKokushi(List<TileType> hand)
    {
        if (hand == null)
        {
            Debug.LogWarning("[KokushiChecker] hand is null");
            return false;
        }

        if (hand.Count != 14)
        {
            // 国士は基本的に14枚（和了状態）で判定します
            return false;
        }

        // ユニークな牌種類を取得
        var unique = new HashSet<TileType>(hand);

        // 必要な13種類が全て含まれているか
        if (!KokushiTiles.IsSubsetOf(unique))
        {
            return false;
        }

        // 13種類のうちどれかが2枚以上（雀頭）あれば国士成立
        foreach (var t in KokushiTiles)
        {
            int cnt = hand.Count(x => x == t);
            if (cnt >= 2) return true;
        }

        return false;
    }
}
