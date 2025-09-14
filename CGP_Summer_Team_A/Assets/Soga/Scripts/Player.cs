using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Player
{
    public string playerName;
    public List<Tile> hand = new List<Tile>();

    public Player(string name)
    {
        playerName = name;
    }

    public void Draw(Tile tile)
    {
        hand.Add(tile);
        SortHand();
    }

    public void SortHand()
    {
        hand = hand.OrderBy(tile => tile.suit)
                   .ThenBy(tile => tile.rank)
                   .ToList();
    }

    public abstract Tile Discard();
}
