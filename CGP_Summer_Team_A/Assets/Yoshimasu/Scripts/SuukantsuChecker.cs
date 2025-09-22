using System.Collections.Generic;
using System.Linq;
using UnityEngine; // Debug.Logを使用するために必要に応じて

/// <summary>
/// 四槓子（スーカンツ）の判定ユーティリティ
/// (4つの4枚組（槓子）と1つの2枚組（雀頭）で構成されているかをチェック)
/// </summary>
public static class SuukantsuChecker
{
    /// <summary>
    /// 四槓子の判定を実行します。
    /// （簡易的に、手牌14枚が「4枚組×4」と「2枚組×1」で構成されているかをチェック）
    /// </summary>
    /// <param name="hand">14枚の和了手牌リスト（MahjongManagerのTileクラスを想定）</param>
    /// <returns>四槓子が成立するか</returns>
    public static bool IsSuukantsu(List<Tile> hand)
    {
        // 14枚でなければ不成立
        if (hand == null || hand.Count != 14) return false;

        // 1. 牌の種類と枚数をカウント
        // Tileオブジェクトは参照型であるため、suitとrankの複合キーでグループ化します。
        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                         .ToDictionary(g => g.Key, g => g.Count());

        int countQuads = 0;   // 4枚組の数 (槓子)
        int countPairs = 0;   // 2枚組の数 (雀頭)
        int otherCount = 0;   // 1枚組や3枚組の数 (四槓子では不可)

        // 【★デバッグログ★】牌の構成を出力
        // Debug.Log("[SuukantsuChecker DEBUG] 牌の構成:");
        // foreach (var item in counts)
        // {
        //     Debug.Log($"  {item.Key.suit}_{item.Key.rank}: {item.Value}枚");
        // }
        
        // 2. 構成をチェック
        foreach (var count in counts.Values)
        {
            if (count == 4)
            {
                countQuads++;
            }
            else if (count == 2)
            {
                countPairs++;
            }
            else
            {
                // 1枚組や3枚組が存在する場合は、四槓子の形ではない
                otherCount++;
            }
        }

        // 3. 判定ロジック
        // 条件: 4枚組がちょうど4つ AND 2枚組がちょうど1つ AND 他の組がない
        // 4 x 4枚 + 1 x 2枚 = 18枚ではありません。14枚手牌内で4x4と1x2が完結している必要があります。
        // 例：{A,A,A,A}, {B,B,B,B}, {C,C,C,C}, {D,D,D,D}, {E,E} -> 計18枚
        // 正しくは：{A,A,A,A}, {B,B,B,B}, {C,C,C,C}, {D,D} + ツモ牌X (計14枚)
        // ★このチェッカーは、和了時点の14枚の手牌のみを判定します。

        // 牌の構成が 4枚組x4 + 2枚組x1 の形に完全に合致すること
        if (countQuads == 4 && countPairs == 1 && otherCount == 0)
        {
            Debug.Log("[SuukantsuChecker DEBUG] Matched Suukantsu pattern: 4x4 + 1x2.");
            // ※ 厳密には暗槓/明槓の区別や、和了牌が雀頭になった場合のチェックが必要ですが、
            //    牌の構成としてはこの形を四槓子と見なします。
            return true;
        }

        return false;
    }
}