using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEngine;

public static class YakumanEvaluator
{
    // 手牌と目標役満から、最も不要な牌を1枚選んで返す
    public static Tile ChooseDiscardTile(List<Tile> hand, Yakuman target)
    {
        var unnecessaryTiles=hand.Where(tile=>!IsTileNeededFor(tile,target,hand)).ToList();

        if (unnecessaryTiles.Any())
        {
            return unnecessaryTiles[UnityEngine.Random.Range(0, unnecessaryTiles.Count)];
        }
        
        var necessaryTiles=hand.Where(tile=>IsTileNeededFor(tile,target,hand)).ToList();
        if (necessaryTiles.Any())
        {
            var tileCounts = necessaryTiles.GroupBy(tile => tile.GetDisplayName())
                                         .ToDictionary(g => g.Key, g => g.Count());

            var leastDuplicatedTile = necessaryTiles.OrderBy(tile => tileCounts[tile.GetDisplayName()]).First();
            return leastDuplicatedTile;
        }
        switch (target)
            {
                case Yakuman.KokushiMusou:
                    return ChooseDiscardForKokushi(hand);
                case Yakuman.Daisangen:
                    return ChooseDiscardForDaisangen(hand);
                case Yakuman.SuuAnkou:
                    return ChooseDiscardForSuuankou(hand);
                case Yakuman.Tsuiso:
                    return ChooseDiscardForTsuuiisou(hand);
                case Yakuman.Chinroutou:
                    return ChooseDiscardForChinroutou(hand);
                case Yakuman.Ryuuiisou:
                    return ChooseDiscardForRyuuiisou(hand);
                case Yakuman.Shousuushii:
                case Yakuman.Daisuushii:
                    return ChooseDiscardForSuushii(hand);
                case Yakuman.Chuuren:
                    return ChooseDiscardForChuuren(hand);
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
                {
                    var requiredHonors = new HashSet<string> { "Honor_5", "Honor_6", "Honor_7" };
                    // 修正: ドラゴン牌のみ必要。他の牌は不要
                    return requiredHonors.Contains(tileName);
                }

            case Yakuman.SuuAnkou:
                // 修正: 四暗刻では全ての牌が重複して必要になるため、4枚未満の牌は必要
                return hand.Count(t => t.GetDisplayName() == tileName) < 4;

            case Yakuman.Tsuiso:
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

            case Yakuman.Chuuren:
                if (tile.suit == Suit.Honor) return false;
                var dominantSuit = hand.Where(t => t.suit != Suit.Honor)
                    .GroupBy(t => t.suit)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();
                if (dominantSuit == Suit.None) return true;
                return tile.suit == dominantSuit;

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

    private static Tile ChooseDiscardForChuuren(List<Tile> hand)
    {
        var dominantSuit = hand.Where(t => t.suit != Suit.Honor)
                                .GroupBy(t => t.suit)
                                .OrderByDescending(g => g.Count())
                                .Select(g => g.Key)
                                .FirstOrDefault();

        if (dominantSuit == Suit.None)
        {
            return hand.Last();
        }

        // 修正: dominantSuit の牌を捨てるのは間違っている。まず dominantSuit 以外の牌を捨てる
        var uselessTile = hand.FirstOrDefault(t => t.suit != dominantSuit);
        if (uselessTile != null)
        {
            return uselessTile;
        }

        var counts = hand.GroupBy(t => t.rank).ToDictionary(g => g.Key, g => g.Count()); // 修正: ToDictionaty -> ToDictionary

        // 修正: 2-8の牌で2枚以上あるものを捨てる
        var extraMiddleTile = hand.FirstOrDefault(t => t.rank >= 2 && t.rank <= 8 && counts[t.rank] > 1);
        if (extraMiddleTile != null)
        {
            return extraMiddleTile;
        }

        var extraTerminalTile = hand.FirstOrDefault(t => (t.rank == 1 || t.rank == 9) && counts[t.rank] > 3);
        if (extraTerminalTile != null)
        {
            return extraTerminalTile;
        }

        return hand.Last();
    }

    public static bool IsYakumanComplete(List<Tile> hand, Yakuman target)
    {
        if (hand == null) return false;

        //14枚ちょうどの時
        if (hand.Count == 14)
        {
            return Evaluate14(hand, target);
        }

        //15枚の時
        if (hand.Count == 15)
        {
            for (int i = 0; i < hand.Count; i++)
            {
                var temp = new List<Tile>(hand.Count - 1);
                for (int j = 0; j < hand.Count; j++)
                {
                    if (j != i) temp.Add(hand[j]);
                }
                if (Evaluate14(temp, target)) return true;
            }
            return false;
        }

        return false;   
    }

    private static bool Evaluate14(List<Tile> hand14, Yakuman target)
    {
        switch (target)
        {
            case Yakuman.Daisuushii:
                return DaisushiChecker.IsDaisushi(hand14);            // 大四喜
            case Yakuman.KokushiMusou:
                return KokushiChecker.IsKokushi(hand14);              // 国士無双
            case Yakuman.Daisangen:
                return DaisangenChecker.IsDaisangen(hand14);          // 大三元
            case Yakuman.SuuAnkou:
                return SuuankouChecker.IsSuuankou(hand14);            // 四暗刻
            case Yakuman.Tsuiso:
                return TsuisoChecker.IsTsuiso(hand14);                // 字一色
            case Yakuman.Chinroutou:
                return ChinroutouChecker.IsChinroutou(hand14);        // 清老頭
            case Yakuman.Ryuuiisou:
                return RyuuisoChecker.IsRyuuiso(hand14);               // 緑一色
            case Yakuman.Shousuushii:
                return ShosushiChecker.IsShosushi(hand14);              // 小四喜
            case Yakuman.Chuuren:
                return ChuurenChecker.IsChuuren(hand14);              // 九蓮宝燈
            default:
                return false;
        }
    }
}