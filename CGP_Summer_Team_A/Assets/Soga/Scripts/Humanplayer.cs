using UnityEngine;

public class Humanplayer : Player
{
    public Humanplayer(string name) : base(name){}

    public override Tile Discard()
    {
        Tile discarded = hand[0];
        hand.RemoveAt(0);
        Debug.Log($"{playerName}の捨て牌:{discarded}");
        return discarded;
    }
}
