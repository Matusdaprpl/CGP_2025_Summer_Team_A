using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public List<Player> players = new List<Player>();
    private int currentTurn = 0;
    private MahjongManager mahjongManager;

    private void Start()
    {
        mahjongManager = FindFirstObjectByType<MahjongManager>();

        players = new List<Player>
        {
            new Humanplayer("東"),
            new NPCplayer("南"),
            new NPCplayer("西"),
            new NPCplayer("北")
        };

        for (int i = 0; i < 13; i++)
        {
            foreach (var p in players)
            {
                p.Draw(mahjongManager.DrawTile());
            }
        }

        StartTurn();
    }

    private void StartTurn()
    {
        Player currentPlayer = players[currentTurn];
        Debug.Log($"{currentPlayer.playerName}のターン");

        Tile drawnTile = mahjongManager.DrawTile();
        currentPlayer.Draw(drawnTile);

        Tile discardedTile = currentPlayer.Discard();
        NextTurn();
    }

    private void NextTurn()
    {
        currentTurn = (currentTurn + 1) % players.Count;
        StartTurn();
    }

    private void OnPlayerPickedItem()
    {
        NextTurn();
    }
}
