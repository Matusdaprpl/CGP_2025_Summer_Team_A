using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCplayer : Player
{
    public NPCplayer(string name) : base(name) { }

    public override Tile Discard()
    {
        int index = UnityEngine.Random.Range(0, hand.Count);
        Tile discarded = hand[index];
        hand.RemoveAt(index);
        Debug.Log($"{playerName}の捨て牌:{discarded}");
        return discarded;
    }
}
