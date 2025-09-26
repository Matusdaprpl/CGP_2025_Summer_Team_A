using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public enum Suit
{
    Manzu,
    Pinzu,
    Souzu,
    Honor
}

public class Tile
{
    public Suit suit;
    public int rank;
    public Sprite sprite;

    public Tile(Suit suit, int rank, Sprite sprite)
    {
        this.suit = suit;
        this.rank = rank;
        this.sprite = sprite;
    }

    public string GetDisplayName() =>
        (suit == Suit.Honor) ? $"Honor_{rank}" : $"{suit}_{rank}";
}

public class MahjongManager : MonoBehaviour
{
    public event Action OnPlayerHitItem;
    public static MahjongManager instance;

    public List<Tile> mountain;
    public List<Tile> playerHand;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CreateMountain();
        ShuffleMountain();
        playerHand = new List<Tile>();

        // 初期配牌
        for (int i = 0; i < 14; i++)
        {
            playerHand.Add(DrawTile());
        }
        SortHand();
        MahjongUIManager.instance.UpdateHandUI(playerHand);

        OnPlayerHitItem += OnItemGetDrawnAndWaitDiscard;

        Debug.Log("MahjongManager側: 山の準備完了。牌の総数: " + mountain.Count);
    }

    public void SetTestHand(List<(Suit suit, int rank)> tilesData)
    {
        if (playerHand == null) playerHand = new List<Tile>();
        playerHand.Clear();

        var tileSprites = MahjongUIManager.instance.GetTileSprites();
        foreach (var data in tilesData)
        {
            string spriteName = (data.suit == Suit.Honor)
                ? $"Honor_{data.rank}"
                : $"{data.suit}_{data.rank}";
            Sprite sprite = tileSprites.ContainsKey(spriteName)
                ? tileSprites[spriteName]
                : null;

            if (sprite == null)
                Debug.LogError($"テスト配牌用スプライトが見つかりません: {spriteName}");

            playerHand.Add(new Tile(data.suit, data.rank, sprite));
        }

        SortHand();
        MahjongUIManager.instance.UpdateHandUI(playerHand);
        Debug.Log($"【テスト配牌完了】手牌が {playerHand.Count} 枚に設定されました。");
    }

    void OnItemGetDrawnAndWaitDiscard()
    {
        if (mountain.Count == 0)
        {
            Debug.Log("流局です。");
            return;
        }
        if (playerHand.Count >= 15)
        {
            Debug.Log("手牌がいっぱいです。捨て牌をしてください。");
            return;
        }

        Tile drawnTile = DrawTile();
        if (drawnTile == null) return;

        playerHand.Add(drawnTile);
        MahjongUIManager.instance.UpdateHandUI(playerHand);
    }

    public void PlayerDraw()
    {
        if (mountain.Count == 0)
        {
            Debug.Log("流局です。");
            return;
        }
        Tile drawnTile = DrawTile();
        if (drawnTile != null)
        {
            playerHand.Add(drawnTile);
            MahjongUIManager.instance.UpdateHandUI(playerHand);
        }
    }

    public void DiscardTile(int handIndex)
    {
        if (playerHand.Count <= 14)
        {
            Debug.Log("ツモってください。");
            return;
        }
        if (handIndex < 0 || handIndex >= playerHand.Count)
        {
            Debug.Log($"無効なインデックスを捨てようとしました:{handIndex}");
            return;
        }

        Tile discardedTile = playerHand[handIndex];
        Debug.Log($"捨て牌:{discardedTile.GetDisplayName()}");

        mountain.Add(discardedTile);
        playerHand.RemoveAt(handIndex);
        SortHand();
        MahjongUIManager.instance.UpdateHandUI(playerHand);
        MahjongUIManager.instance.UpdateMountainCountUI(mountain.Count);
    }

    void CreateMountain()
    {
        mountain = new List<Tile>();
        var tileSprites = MahjongUIManager.instance.GetTileSprites();
        foreach (Suit s in new Suit[] { Suit.Manzu, Suit.Pinzu, Suit.Souzu })
        {
            for (int rank = 1; rank <= 9; rank++)
            {
                string spriteName = $"{s}_{rank}";
                if (tileSprites.ContainsKey(spriteName))
                {
                    Sprite sprite = tileSprites[spriteName];
                    for (int i = 0; i < 4; i++)
                        mountain.Add(new Tile(s, rank, sprite));
                }
            }
        }

        for (int rank = 1; rank <= 7; rank++)
        {
            string spriteName = $"Honor_{rank}";
            if (tileSprites.ContainsKey(spriteName))
            {
                Sprite sprite = tileSprites[spriteName];
                for (int i = 0; i < 4; i++)
                    mountain.Add(new Tile(Suit.Honor, rank, sprite));
            }
        }

        Debug.Log($"牌の山を作成しました。合計：{mountain.Count}枚");
    }

    void ShuffleMountain()
    {
        if (mountain == null) return;
        for (int i = 0; i < mountain.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, mountain.Count);
            Tile temp = mountain[i];
            mountain[i] = mountain[randomIndex];
            mountain[randomIndex] = temp;
        }
        Debug.Log("山をシャッフルしました。");
    }

    public Tile DrawTile()
    {
        if (mountain == null || mountain.Count == 0)
        {
            Debug.Log("山に牌がありません！");
            return null;
        }
        Tile drawTile = mountain[0];
        mountain.RemoveAt(0);
        MahjongUIManager.instance.UpdateMountainCountUI(mountain.Count);
        return drawTile;
    }

    public void SortHand()
    {
        if (playerHand == null) return;
        playerHand = playerHand.OrderBy(tile => tile.suit)
                               .ThenBy(tile => tile.rank)
                               .ToList();
    }

    public Tile PeekNextTileInMountain()
    {
        if (mountain != null && mountain.Count > 0)
            return mountain[0];
        return null;
    }

    public bool AddTileToPlayerHand(Tile tile)
    {
        if (tile == null) return false;
        if (playerHand == null) playerHand = new List<Tile>();
        if (playerHand.Count >= 15) return false;

        playerHand.Add(tile);
        MahjongUIManager.instance.UpdateHandUI(playerHand);
        return true;
    }

    public void PlayerHitItem()
    {
        OnPlayerHitItem?.Invoke();
    }

    
}