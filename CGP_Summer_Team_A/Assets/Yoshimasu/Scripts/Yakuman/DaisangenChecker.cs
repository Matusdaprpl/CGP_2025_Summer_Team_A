using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DaisangenChecker
{
    // 三元牌のRank (あなたのプロジェクト設定に基づき5, 6, 7と仮定)
    private const int HAKU_RANK = 5; // 白
    private const int HATSU_RANK = 6; // 發
    private const int CHUN_RANK = 7; // 中

    public static bool IsDaisangen(List<Tile> hand)
    {
        if (hand == null || hand.Count != 14) return false;

        // 1. 各三元牌が3枚以上あるか確認
        bool hasHaku = hand.Count(t => t.suit == Suit.Honor && t.rank == HAKU_RANK) >= 3;
        bool hasHatsu = hand.Count(t => t.suit == Suit.Honor && t.rank == HATSU_RANK) >= 3;
        bool hasChun = hand.Count(t => t.suit == Suit.Honor && t.rank == CHUN_RANK) >= 3;

        // 2. 3種類すべてが3枚以上であれば成立
        if (hasHaku && hasHatsu && hasChun)
        {
            // 役満判定では、手牌が「4面子1雀頭」の形になっている必要がありますが、
            // 牌の種類と枚数の条件が満たされていれば、ここでは成立とします。（簡易判定）
            return true;
        }

        return false;
    }
}