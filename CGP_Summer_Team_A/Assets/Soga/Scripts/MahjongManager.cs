using UnityEngine;
using UnityEngine.UI;
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
    public static MahjongManager instance;

    public List<Tile> mountain;
    public List<Tile> playerHand;
    public Tile lastDrawnTile;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CreateMountain();
        ShuffleMountain();
        playerHand = new List<Tile>();

        for (int i = 0; i < 14; i++)
        {
            playerHand.Add(DrawTile());
        }
        SortHand();
        MahjongUIManager.instance.UpdateHandUI(playerHand);

        Debug.Log("MahjongManager側の確認: 山の準備完了。牌の総数: " + mountain.Count);
    }

    public void DiscardTile(int index)
    {
        if (index < 0 || index >= playerHand.Count) return;

        Tile discardedTile = playerHand[index];
        playerHand.RemoveAt(index);

        // 捨て牌を地面にドロップ
        Vector3 dropPosition = transform.position; // プレイヤーの位置（必要に応じて調整）
        ItemManager.instance.DropDiscardedTile(discardedTile, dropPosition);

        SortHand();
        MahjongUIManager.instance.UpdateHandUI(playerHand);
    }

    public bool AddTileToPlayerHand(Tile tile)
    {
        if (tile == null || playerHand.Count >= 15) return false;

        playerHand.Add(tile);
        SortHand();
        MahjongUIManager.instance.UpdateHandUI(playerHand);
        return true;
    }

    public Tile DrawTile()
    {
        if (mountain == null || mountain.Count == 0) return null;

        Tile drawTile = mountain[0];
        mountain.RemoveAt(0);
        return drawTile;
    }

    public void SortHand()
    {
        if (playerHand == null) return;

        if (lastDrawnTile != null && playerHand.Contains(lastDrawnTile))
        {
            playerHand.Remove(lastDrawnTile);
            playerHand = playerHand.OrderBy(tile => tile.suit).ThenBy(tile => tile.rank).ToList();
            playerHand.Add(lastDrawnTile);
        }
        else
        {
            playerHand = playerHand.OrderBy(tile => tile.suit).ThenBy(tile => tile.rank).ToList();
        }
    }

    void CreateMountain()
    {
        mountain = new List<Tile>();
        var suits = new[] { Suit.Manzu, Suit.Pinzu, Suit.Souzu };

        // 萬子、筒子、索子を生成
        foreach (var suit in suits)
        {
            for (int rank = 1; rank <= 9; rank++)
            {
                // 各牌を4枚ずつ追加
                for (int i = 0; i < 4; i++)
                {
                    // スプライトの取得はSetTestHandと同様の方法で行う必要があります
                    // ここでは一旦nullで仮置きしますが、実際にはスプライトを読み込む処理が必要です
                    mountain.Add(new Tile(suit, rank, null)); 
                }
            }
        }

        // 字牌を生成
        for (int rank = 1; rank <= 7; rank++)
        {
            // 各牌を4枚ずつ追加
            for (int i = 0; i < 4; i++)
            {
                mountain.Add(new Tile(Suit.Honor, rank, null));
            }
        }
        
        Debug.Log($"牌の山を作成しました。合計：{mountain.Count}枚");
    }

    void ShuffleMountain()
    {
        if (mountain == null) return;

        // Fisher-Yatesアルゴリズムでシャッフル
        System.Random rng = new System.Random();
        int n = mountain.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Tile value = mountain[k];
            mountain[k] = mountain[n];
            mountain[n] = value;
        }
        Debug.Log("山をシャッフルしました。");
    }

    public void SetTestHand(List<(Suit suit, int rank)> handData)
    {
        playerHand = new List<Tile>();
        foreach (var (suit, rank) in handData)
        {
            // スプライトを取得（Resources/Tiles から）
            Sprite sprite = Resources.Load<Sprite>($"{suit}_{rank}");
            if (sprite == null && suit == Suit.Honor)
            {
                sprite = Resources.Load<Sprite>($"Honor_{rank}");
            }
            Tile tile = new Tile(suit, rank, sprite);
            playerHand.Add(tile);
        }
        SortHand();
        MahjongUIManager.instance.UpdateHandUI(playerHand);
    }

    [Header("プレハブ設定")]
    public GameObject itemPrefab; // ← 追加: インスペクターから設定

    // ...existing code...
    public void SpawnItemFromMountain(Vector2 position)
    {
        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogError("itemPrefab が設定されていません。インスペクターで割り当ててください。");
        }
    }
     
    // Existing fields and methods

    // Add this method to fix the error
    public void PlayerHitItem()
    {
        Debug.Log("PlayerHitItem called from MahjongManager.");
        // Implement your logic here
    }
}