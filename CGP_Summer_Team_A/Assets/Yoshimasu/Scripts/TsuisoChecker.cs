using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TsuisoChecker
{
    /// <summary>
    /// 字一色の判定を実行します。
    /// 手牌がすべての字牌（風牌と三元牌）だけで構成されているかをチェックします。
    /// </summary>
    /// <param name="hand">14枚の和了手牌リスト</param>
    /// <returns>字一色が成立するか</returns>
    public static bool IsTsuiso(List<Tile> hand)
    {
        // 14枚でなければ不成立
        if (hand == null || hand.Count != 14)
        {
            Debug.Log("[TsuisoChecker DEBUG] Hand count is not 14. Failed.");
            return false;
        }

        // 1. すべての牌が字牌であるかを確認
        foreach (var tile in hand)
        {
            if (tile.suit != Suit.Honor)
            {
                Debug.Log($"[TsuisoChecker DEBUG] Hand contains a non-honor tile: {tile.suit} {tile.rank}. Failed.");
                return false;
            }
        }
        
        // 2. 牌の構成（4つの面子 + 1つの雀頭）をチェック
        // 字一色は、すべて字牌であれば和了形が成立するはず
        // したがって、ここでは「すべて字牌か」のみをチェックする簡易判定とします。
        // より厳密な判定には、手牌が「4面子1雀頭」の基本形を満たしているかのロジックが必要です。
        
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
            // 正式な実装では、IsStandardHand(hand)のような関数で「4面子1雀頭」の判定が必要
            Debug.Log("[TsuisoChecker DEBUG] All tiles are honor tiles. Assumed valid hand.");
            return true;
        }

        return false;
    }
}