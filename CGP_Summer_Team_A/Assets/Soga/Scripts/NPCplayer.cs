using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCplayer : Player
{
    public NPCplayer(string name) : base(name) { }

    public override Tile Discard()
    {
        Tile discarded = hand[hand.Count - 1];
        hand.RemoveAt(hand.Count - 1);
        Debug.Log($"{playerName}の捨て牌:{discarded}");
        return discarded;
    }
}
