using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using NUnit.Framework;

public static class ChuurenChecker
{
    public static bool IsChuuren(List<Tile> hand)
    {
        if (hand == null || hand.Count != 14) return false;

        var validSuits = new[] { Suit.Manzu, Suit.Pinzu, Suit.Souzu };
        if (hand.Any(t => !validSuits.Contains(t.suit) || t.rank < 1 || t.rank > 9)) return false;

        var counts = hand.GroupBy(t => new { t.suit, t.rank })
                       .ToDictionary(g => g.Key, g => g.Count());

        foreach (var suit in validSuits)
        {
            var suitCounts = counts.Where(c => c.Key.suit == suit)
                                   .ToDictionary(c => c.Key.rank, c => c.Value);

            if (!Enumerable.Range(1, 9).All(r => suitCounts.ContainsKey(r) && suitCounts[r] >= 1)) continue;

            var headCandidates = new[] { 1, 9, 2, 8, 3, 7, 4, 6, 5 };
            foreach (var head in headCandidates)
            {
                if (suitCounts[head] >= 2)
                {
                    var remaining= suitCounts.ToDictionary(c => c.Key, c => c.Value);
                    remaining[head] -= 2;

                    if (remaining.All(c => c.Value == 1))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
