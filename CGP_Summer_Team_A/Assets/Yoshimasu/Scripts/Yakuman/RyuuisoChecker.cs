using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RyuuisoChecker
{
    public static bool IsRyuuiso(List<Tile> hand)
    {
        if (hand == null || hand.Count != 14) return false;

        // 1. すべての牌が緑色の牌であるかを確認 (索子 2, 3, 4, 6, 8, および 發)
        foreach (var tile in hand)
        {
            bool isGreenTile = false;
            
            // 索子の緑色の牌 (2, 3, 4, 6, 8)
            if (tile.suit == Suit.Souzu && (tile.rank == 2 || tile.rank == 3 || tile.rank == 4 || tile.rank == 6 || tile.rank == 8))
            {
                isGreenTile = true;
            }
            // 發 (ハツ) は Honor 6 と仮定
            else if (tile.suit == Suit.Honor && tile.rank == 6) 
            {
                isGreenTile = true;
            }

            if (!isGreenTile) return false;
        }
        
        // 2. 牌の構成（4つの面子 + 1つの雀頭）をチェック
        // 簡易判定のため、「すべて緑色か」のみをチェックし、和了形が成立していると仮定します。
        return true;
    }
}