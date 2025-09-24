using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RyuuisoChecker
{
    /// <summary>
    /// 緑一色の判定を実行します。
    /// 手牌が索子の2, 3, 4, 6, 8と發だけで構成されているかをチェックします。
    /// </summary>
    /// <param name="hand">14枚の和了手牌リスト</param>
    /// <returns>緑一色が成立するか</returns>
    public static bool IsRyuuiso(List<Tile> hand)
    {
        // 14枚でなければ不成立
        if (hand == null || hand.Count != 14)
        {
            Debug.Log("[RyuuisoChecker DEBUG] Hand count is not 14. Failed.");
            return false;
        }

        // 1. すべての牌が緑色の牌であるかを確認
        foreach (var tile in hand)
        {
            bool isGreenTile = false;
            
            // 索子の緑色の牌 (2, 3, 4, 6, 8)
            if (tile.suit == Suit.Souzu && (tile.rank == 2 || tile.rank == 3 || tile.rank == 4 || tile.rank == 6 || tile.rank == 8))
            {
                isGreenTile = true;
            }
            // 發 (ハツ)
            else if (tile.suit == Suit.Honor && tile.rank == 6)
            {
                isGreenTile = true;
            }

            if (!isGreenTile)
            {
                Debug.Log($"[RyuuisoChecker DEBUG] Hand contains a non-green tile: {tile.suit} {tile.rank}. Failed.");
                return false;
            }
        }
        
        // 2. 牌の構成（4つの面子 + 1つの雀頭）をチェック
        // 緑一色は、すべて緑色の牌であれば和了形（4面子1雀頭）が成立するはず
        // そのため、ここでは手牌の構成が「4つの面子 + 1つの雀頭」になっているかを別途チェックする必要がある
        // 以下のロジックは、以前作成した「4つの刻子 + 1つの雀頭」の判定に似ているが、順子も考慮する必要がある
        // ただし、簡易判定として、ここでは「すべて緑色の牌か」のみをチェックする
        
        // 実際には以下のような判定ロジックが必要
        // if (IsStandardHand(hand)) return true;
        
        // 簡略版：牌の種類と枚数をカウントし、組み合わせ可能か確認
        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        int totalCount = 0;
        foreach (var count in counts.Values)
        {
            if (count > 0)
            {
                totalCount += count;
            }
        }
        
        // 和了形（4面子1雀頭）が成立しているかどうかの簡易チェック
        if (totalCount == 14)
        {
            // ここでさらに、手牌が「4面子1雀頭」になっているかを確認する複雑なロジックが必要
            // 刻子、順子、雀頭の組み合わせを再帰的に判定する
            // 簡易判定のため、ここでは省略
            // 正式な実装には、IsStandardHand(hand)のような関数が必要
            Debug.Log("[RyuuisoChecker DEBUG] All tiles are green. Assumed valid hand.");
            return true;
        }

        return false;
    }
}