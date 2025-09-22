using System.Collections.Generic;
using System.Linq;

public static class YakumanEvaluator
{
    // 手牌と目標役満から、最も不要な牌を1枚選んで返す
    public static Tile ChooseDiscardTile(List<Tile> hand, Yakuman target)
    {
        switch (target)
        {
            case Yakuman.KokushiMusou:
                return ChooseDiscardForKokushi(hand);
            case Yakuman.Daisangen:
                return ChooseDiscardForDaisangen(hand);
            // 他の役満のロジックも追加
            default:
                // デフォルトは適当に一番右の牌を捨てる
                return hand.Last();
        }
    }

    // 国士無双用の捨て牌ロジック
    private static Tile ChooseDiscardForKokushi(List<Tile> hand)
    {
        // 1,9,字牌のリスト（国士無双に必要な牌）
        var requiredTiles = new HashSet<string> {
            "Manzu_1", "Manzu_9", "Pinzu_1", "Pinzu_9", "Souzu_1", "Souzu_9",
            "Honor_1", "Honor_2", "Honor_3", "Honor_4", "Honor_5", "Honor_6", "Honor_7"
        };

        // 不要牌（中張牌）があればそれを最優先で捨てる
        var uselessTile = hand.FirstOrDefault(t => !requiredTiles.Contains(t.GetDisplayName()));
        if (uselessTile != null) return uselessTile;

        // 全てが国士無双の構成牌の場合、ダブっている牌を捨てる
        var duplicateTile = hand.GroupBy(t => t.GetDisplayName())
                                .Where(g => g.Count() > 1)
                                .Select(g => g.First())
                                .FirstOrDefault();
        if (duplicateTile != null) return duplicateTile;

        // テンパイしている場合、ツモった牌をそのまま捨てる
        return hand.Last();
    }

    // 大三元用の捨て牌ロジック
    private static Tile ChooseDiscardForDaisangen(List<Tile> hand)
    {
        // 優先して保持する牌（白・發・中）
        var requiredHonors = new HashSet<string> { "Honor_5", "Honor_6", "Honor_7" };

        // 最も不要な牌を探す（数牌の孤立牌など）
        // 1. 三元牌以外で、かつ他の牌と全く関連のない孤立した数牌
        // 2. 辺張や嵌張を構成している牌より、ただの1枚の客風牌
        // （ロジックは複雑になるため、ここでは単純な例を示す）

        // 三元牌以外をすべて不要候補とする
        var discardCandidates = hand.Where(t => !requiredHonors.Contains(t.GetDisplayName())).ToList();

        if (discardCandidates.Any())
        {
            // 不要候補の中から、さらに優先度の低いもの（例：端に近い数牌など）を選ぶ
            return discardCandidates.OrderBy(t => t.rank == 1 || t.rank == 9 ? 0 : 1).First();
        }

        // 捨てる候補がない場合（全て三元牌）、ツモった牌を捨てる
        return hand.Last();
    }
}