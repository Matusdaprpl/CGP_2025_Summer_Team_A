using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 四暗刻の判定ユーティリティ
/// (順子が存在しない、4つの刻子と1つの雀頭で構成されていることをチェック)
/// </summary>
public static class SuuankouChecker
{
    public static bool IsSuuankou(List<TileType> hand)
    {
        // 四暗刻は14枚で成立
        if (hand == null || hand.Count != 14) return false;

        // 1. 牌の種類と枚数をカウント
        var counts = hand.GroupBy(t => t)
                          .ToDictionary(g => g.Key, g => g.Count());

        int tripletsAndQuads = 0; // 3枚組と4枚組の数
        int pairs = 0;            // 2枚組の数
        
        // 2. 順子が含まれていないか、全て刻子/対子で構成されているかをチェック
        foreach (var count in counts.Values)
        {
            if (count == 1) return false; // 孤立牌(1枚)があれば、順子なしでは絶対に不成立

            if (count == 2)
            {
                pairs++;
            }
            else if (count == 3)
            {
                tripletsAndQuads++;
            }
            else if (count == 4)
            {
                // 4枚組は「1つの刻子と1つの雀頭」に分解できるものと見なす
                // 4枚組であれば、順子は存在しないので、暗刻の構成要素としては成立する。
                tripletsAndQuads++; // 刻子1つ分
                pairs++;            // 雀頭1つ分
            }
        }
        
        // 3. 最終判定: 刻子/槓子の分解結果が「刻子4つ + 雀頭1つ」になっているか？
        // (四暗刻の基本形: (3枚組)x4 + (2枚組)x1)
        // ※ 雀頭は必ず1つ、刻子/槓子は4つである必要があります。
        if (tripletsAndQuads == 4 && pairs == 1)
        {
            // この条件を満たせば、「順子がなく、全て刻子/対子/槓子で構成された4面子1雀頭」
            // つまり、四暗刻が成立していると見なせます。（鳴きがない前提）
            return true;
        }

        return false;
    }
}