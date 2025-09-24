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
            case Yakuman.SuuAnkou:
                return ChooseDiscardForSuuankou(hand);
            case Yakuman.Tsuuiisou:
                return ChooseDiscardForTsuuiisou(hand);
            case Yakuman.Chinroutou:
                return ChooseDiscardForChinroutou(hand);
            case Yakuman.Ryuuiisou:
                return ChooseDiscardForRyuuiisou(hand);
            case Yakuman.Shousuushii:
            case Yakuman.Daisuushii:
                return ChooseDiscardForSuushii(hand);

            // --- 未実装の役満 ---
            case Yakuman.ChuurenPoutou:
            case Yakuman.SuuKantsu:
                // TODO: 九蓮宝燈や四槓子のロジックは非常に複雑なため、後で実装
                return hand.Last();

            default:
                // デフォルトは適当に一番右の牌を捨てる
                return hand.Last();
        }
    }

    public static bool IsTileNeededFor(Tile tile, Yakuman target, List<Tile> hand)
    {
        string tileName = tile.GetDisplayName();
        switch (target)
        {
            case Yakuman.KokushiMusou:
                var requiredTiles = new HashSet<string> { "Manzu_1", "Manzu_9", "Pinzu_1", "Pinzu_9", "Souzu_1", "Souzu_9", "Honor_1", "Honor_2", "Honor_3", "Honor_4", "Honor_5", "Honor_6", "Honor_7" };
                return requiredTiles.Contains(tileName);

            case Yakuman.Daisangen:
                var requiredHonors = new HashSet<string> { "Honor_5", "Honor_6", "Honor_7" };
                if (requiredHonors.Contains(tileName)) return true;
                // すでに持っている牌と同じなら、刻子を作るために必要
                return hand.Any(t => t.GetDisplayName() == tileName);

            case Yakuman.SuuAnkou:
                // すでに持っている牌と同じなら、刻子を作るために必要
                return hand.Any(t => t.GetDisplayName() == tileName);

            case Yakuman.Tsuuiisou:
                // 字牌なら必要
                return tile.suit == Suit.Honor;

            case Yakuman.Chinroutou:
                // 1,9牌なら必要
                return tile.rank == 1 || tile.rank == 9;

            case Yakuman.Ryuuiisou:
                var greenTiles = new HashSet<string> { "Souzu_2", "Souzu_3", "Souzu_4", "Souzu_6", "Souzu_8", "Honor_6" };
                return greenTiles.Contains(tileName);

            case Yakuman.Shousuushii:
            case Yakuman.Daisuushii:
                var requiredWinds = new HashSet<string> { "Honor_1", "Honor_2", "Honor_3", "Honor_4" };
                return requiredWinds.Contains(tileName);

            default:
                return false; // 未実装の役は判定不可
        }
    }

    // 国士無双用の捨て牌ロジック
    private static Tile ChooseDiscardForKokushi(List<Tile> hand)
    {
        var requiredTiles = new HashSet<string> { "Manzu_1", "Manzu_9", "Pinzu_1", "Pinzu_9", "Souzu_1", "Souzu_9", "Honor_1", "Honor_2", "Honor_3", "Honor_4", "Honor_5", "Honor_6", "Honor_7" };
        var uselessTile = hand.FirstOrDefault(t => !requiredTiles.Contains(t.GetDisplayName()));
        if (uselessTile != null) return uselessTile;
        var duplicateTile = hand.GroupBy(t => t.GetDisplayName()).Where(g => g.Count() > 1).Select(g => g.First()).FirstOrDefault();
        if (duplicateTile != null) return duplicateTile;
        return hand.Last();
    }

    // 大三元用の捨て牌ロジック
    private static Tile ChooseDiscardForDaisangen(List<Tile> hand)
    {
        var requiredHonors = new HashSet<string> { "Honor_5", "Honor_6", "Honor_7" };
        var discardCandidates = hand.Where(t => !requiredHonors.Contains(t.GetDisplayName())).ToList();
        if (discardCandidates.Any())
        {
            return discardCandidates.OrderBy(t => (t.rank >= 2 && t.rank <= 8) ? 0 : 1).First();
        }
        return hand.Last();
    }
    
    // 四暗刻用の捨て牌ロジック
    private static Tile ChooseDiscardForSuuankou(List<Tile> hand)
    {
        // 対子や刻子になっていない孤立牌を最優先で捨てる
        var singleTile = hand.GroupBy(t => t.GetDisplayName())
                             .Where(g => g.Count() == 1)
                             .Select(g => g.First())
                             .FirstOrDefault();

        if (singleTile != null) return singleTile;

        // 孤立牌がない場合（全て対子か刻子）、ツモった牌を捨てる
        return hand.Last();
    }

    // 字一色用の捨て牌ロジック
    private static Tile ChooseDiscardForTsuuiisou(List<Tile> hand)
    {
        // 字牌でない牌（数牌）を最優先で捨てる
        var numberTile = hand.FirstOrDefault(t => t.suit != Suit.Honor);
        if (numberTile != null) return numberTile;

        // 全て字牌の場合、孤立している字牌を捨てる
        return ChooseDiscardForSuuankou(hand); // 四暗刻のロジックを流用
    }

    // 清老頭用の捨て牌ロジック
    private static Tile ChooseDiscardForChinroutou(List<Tile> hand)
    {
        // 1,9牌でない牌（中張牌と字牌）を最優先で捨てる
        var uselessTile = hand.FirstOrDefault(t => !(t.rank == 1 || t.rank == 9));
        if (uselessTile != null) return uselessTile;

        // 全て1,9牌の場合、孤立している牌を捨てる
        return ChooseDiscardForSuuankou(hand); // 四暗刻のロジックを流用
    }

    // 緑一色用の捨て牌ロジック
    private static Tile ChooseDiscardForRyuuiisou(List<Tile> hand)
    {
        // 緑一色の構成牌
        var greenTiles = new HashSet<string> {
            "Souzu_2", "Souzu_3", "Souzu_4", "Souzu_6", "Souzu_8",
            "Honor_6" // 發
        };

        // 緑一色の構成牌でなければ、それを捨てる
        var notGreenTile = hand.FirstOrDefault(t => !greenTiles.Contains(t.GetDisplayName()));
        if (notGreenTile != null) return notGreenTile;

        // 全て緑一色の構成牌の場合、孤立している牌を捨てる
        return ChooseDiscardForSuuankou(hand); // 四暗刻のロジックを流用
    }

    // 大四喜・小四喜用の捨て牌ロジック
    private static Tile ChooseDiscardForSuushii(List<Tile> hand)
    {
        // 風牌（東南西北）を保持し、それ以外を捨てる
        var requiredWinds = new HashSet<string> { "Honor_1", "Honor_2", "Honor_3", "Honor_4" };
        
        var discardCandidate = hand.FirstOrDefault(t => !requiredWinds.Contains(t.GetDisplayName()));
        if (discardCandidate != null) return discardCandidate;

        // 全て風牌の場合、孤立牌を捨てる
        return ChooseDiscardForSuuankou(hand); // 四暗刻のロジックを流用
    }
}