using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCplayer : MonoBehaviour
{
    public List<Tile> hand = new List<Tile>();

    public void Draw(Tile tile)
    {
        hand.Add(tile);
        SortHand();
    }

    public Tile Discard()
    {
        Tile discarded = hand[hand.Count - 1];
        hand.RemoveAt(hand.Count - 1);
        return discarded;
    }

    void SortHand()
    {
        hand = hand.OrderBy(tile => tile.suit)
                   .ThenBy(tile => tile.rank)
                   .ToList();
    }
}
