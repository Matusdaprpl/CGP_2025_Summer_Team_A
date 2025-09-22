using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 手牌を麻雀のルール（種類ごと、数字順）に基づいてソートするユーティリティクラス。
/// </summary>
public static class HandSorter
{
    // 各牌のソート順序を定義する辞書。
    // 100番台: 萬子, 200番台: 筒子, 300番台: 索子, 400番台: 字牌
    private static readonly Dictionary<TileType, int> TileSortOrder = new Dictionary<TileType, int>()
    {
        // 萬子 (Man)
        { TileType.Man1, 101 }, { TileType.Man2, 102 }, { TileType.Man3, 103 }, 
        { TileType.Man4, 104 }, { TileType.Man5, 105 }, { TileType.RedMan5, 1051 }, // 赤5萬は通常の5萬の直後に配置
        { TileType.Man6, 106 }, { TileType.Man7, 107 }, { TileType.Man8, 108 }, 
        { TileType.Man9, 109 },

        // 筒子 (Pin)
        { TileType.Pin1, 201 }, { TileType.Pin2, 202 }, { TileType.Pin3, 203 }, 
        { TileType.Pin4, 204 }, { TileType.Pin5, 205 }, { TileType.RedPin5, 2051 }, // 赤5筒は通常の5筒の直後に配置
        { TileType.Pin6, 206 }, { TileType.Pin7, 207 }, { TileType.Pin8, 208 }, 
        { TileType.Pin9, 209 },

        // 索子 (Sou)
        { TileType.Sou1, 301 }, { TileType.Sou2, 302 }, { TileType.Sou3, 303 }, 
        { TileType.Sou4, 304 }, { TileType.Sou5, 305 }, { TileType.RedSou5, 3051 }, // 赤5索は通常の5索の直後に配置
        { TileType.Sou6, 306 }, { TileType.Sou7, 307 }, { TileType.Sou8, 308 }, 
        { TileType.Sou9, 309 },

        // 字牌 (Honors)
        // 風牌 (東南西北)
        { TileType.East, 401 }, { TileType.South, 402 }, 
        { TileType.West, 403 }, { TileType.North, 404 },
        // 三元牌 (白發中)
        { TileType.White, 405 }, { TileType.Green, 406 }, { TileType.Red, 407 }
    };

    /// <summary>
    /// 手牌リストを受け取り、麻雀の標準的な順序でソートした新しいリストを返す。
    /// </summary>
    /// <param name="hand">ソート対象の手牌リスト</param>
    /// <returns>ソート済みの新しい手牌リスト</returns>
    public static List<TileType> SortHand(List<TileType> hand)
    {
        if (hand == null)
        {
            return new List<TileType>();
        }

        // LINQのOrderByメソッドを使用し、カスタム辞書の値に基づいてソート
        return hand.OrderBy(tile => 
        {
            // 辞書からソート順序値を取得
            if (TileSortOrder.TryGetValue(tile, out int orderValue))
            {
                return orderValue;
            }
            // 辞書に未定義のTileTypeがあった場合の安全策（リストの最後に配置される）
            return 9999; 
        }).ToList(); 
    }
}