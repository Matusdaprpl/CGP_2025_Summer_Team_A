using System.Collections.Generic;

using UnityEngine;

using System.Linq;



/// <summary>

/// 大三元 (Daisangen) の判定ユーティリティ

/// </summary>

public static class DaisangenChecker

{

    private static readonly List<TileType> SANGEN_PAI = new List<TileType>

    {

        TileType.White,

        TileType.Green,

        TileType.Red

    };



    /// <summary>

    /// 手牌がチー/ポン/カンを考慮した大三元であるかを判定します。

    /// </summary>

    /// <param name="hand">14枚の牌（手牌13枚 + ツモ牌1枚）</param>

    public static bool IsDaisangen(List<TileType> hand)

    {

        // 14枚でなければ不成立

        if (hand == null || hand.Count != 14) return false;



        // 1. 牌の種類ごとの枚数をカウント

        var counts = hand.GroupBy(t => t)

                         .ToDictionary(g => g.Key, g => g.Count());



        // 2. 白・發・中がすべて刻子（3枚以上）になっているかをチェック

        foreach (var tile in SANGEN_PAI)

        {

            if (!counts.ContainsKey(tile) || counts[tile] < 3)

            {

                // 三元牌のいずれかが3枚未満であれば不成立

                return false;

            }

        }

       

        // 3. 牌の残りが 4面子1雀頭の形で成立しているか？

        // ここが最も難しい部分で、「和了形判定のベースロジック」が必要ですが、

        // 大三元の3つの刻子が確定しているため、残りの (14 - 9 = 5枚) が雀頭と順子/刻子の組になっていれば成立。

        //

        // 簡易判定として、ここでは「三元牌の刻子が揃っていること」のみをチェックし、

        // 残りの牌が雀頭と面子を構成できるかは、より高度な和了形判定関数に依存するとします。

        //

        // ただし、このメソッドの目的は役満判定であり、手役の判定ではないため、

        // 最終的な和了形判定は別途必要です。

        //

        // 厳密な判定:

        // 1. 三元牌の刻子が揃っていることを確認 (上記で完了)

        // 2. 残り 5枚 (合計14枚から3枚x3セット=9枚を引く) で雀頭（2枚）と面子（3枚）が成立するかを確認。

       

        // 簡易チェックとして、一旦三元牌の刻子があることを確認できたら、

        // それだけで「大三元」の構成要素は満たしているとします。

        // ※ 最終的なゲームでは、この後、基本の和了形 (4面子1雀頭) 判定を通す必要があります。

       

        // 現状のシステムでは和了形判定関数がないため、ここでは三元牌の刻子の有無のみをチェックします。

       

        return true;

    }

}　　　　　　　　　　　