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

    public Tile(Suit s, int r)
    {
        suit = s;
        rank = r;
    }

    public string GetSpriteName()
    {
        if (suit == Suit.Honor)
        {
            return $"Honor_{rank}";
        }
        else
        {
            return $"{suit}_{rank}";
        }
    }

    public override string ToString()
    {
        return $"{suit}_{rank}";
    }
}

public class MahjongManager : MonoBehaviour
{
    public event Action OnPlayerHitItem;

    public void PlayerHitItem()
    {
        OnPlayerHitItem?.Invoke();
    }
    public static MahjongManager instance;
    private void Awake()
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

    [Header("UI関連")]
    public GameObject tilePrefab;
    public Transform handPanel;
    private List<Tile> mountain;
    public List<Tile> playerHand;
    private Dictionary<string, Sprite> tileSprites;

    void Start()
    {
        LoadTileSprites();
        CreateMountain();
        SuffleMountain();
        playerHand = new List<Tile>();

        for (int i = 0; i < 14; i++)
        {
            playerHand.Add(DrawTile());
        }

        SortHand();

        UpdateHandUI();

        OnPlayerHitItem += OnItemGetDrawnAndWaitDiscard;

          Debug.Log("MahjongManager側の確認: 山の準備完了。牌の総数: " + mountain.Count);
    }

    void OnItemGetDrawnAndWaitDiscard()
    {
        if (mountain.Count == 0)
        {
            Debug.Log("流局です。");
            return;
        }

        if(playerHand.Count >= 15)
        {
            Debug.Log("手牌がいっぱいです。捨て牌をしてください。");
            return;
        }

        Tile drawnTile = DrawTile();
        if (drawnTile == null) return;

        playerHand.Add(drawnTile);
        SortHand();
        UpdateHandUI();
    }

    void LoadTileSprites()
    {
        tileSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Tiles");

        foreach (Sprite sprite in sprites)
        {
            tileSprites[sprite.name] = sprite;
        }
    }

    void UpdateHandUI()
    {
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerHand.Count; i++)
        {
            GameObject newTileObj = Instantiate(tilePrefab, handPanel);

            Tile tile = playerHand[i];
            string spriteName = tile.suit == Suit.Honor ? $"Honor_{tile.rank}" : $"{tile.suit}_{tile.rank}";

            if (tileSprites.ContainsKey(spriteName))
            {
                newTileObj.GetComponent<Image>().sprite = tileSprites[spriteName];
            }
            else
            {
                Debug.LogError($"スプライトが見つかりません:{spriteName}");
            }

            Button tileButton = newTileObj.GetComponent<Button>();
            if (tileButton != null)
            {
                int index = i;
                tileButton.onClick.AddListener(() => DiscardTile(index));
            }
        }
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
            drawnTile.ToString();
            SortHand();
            UpdateHandUI();
        }
    }
    public void DiscardTile(int handIndex)
    {
        if(playerHand.Count ==14)
        {
            return;
        }
        if (handIndex < 0 || handIndex >= playerHand.Count)
        {
            Debug.Log($"無効なインデックスを捨てようとしました:{handIndex}");
            return;
        }

        Tile discardedTile = playerHand[handIndex];
        Debug.Log($"捨て牌:{discardedTile.ToString()}");

        playerHand.RemoveAt(handIndex);

        UpdateHandUI();
    }
    void CreateMountain()
    {
        mountain = new List<Tile>();

        foreach (Suit s in new Suit[] { Suit.Manzu, Suit.Pinzu, Suit.Souzu })
        {
            for (int rank = 1; rank <= 9; rank++)
            {
                for (int i = 0; i < 4; i++)
                {
                    mountain.Add(new Tile(s, rank));
                }
            }
        }

        for (int rank = 1; rank <= 7; rank++)
        {
            for (int i = 0; i < 4; i++)
            {
                mountain.Add(new Tile(Suit.Honor, rank));
            }
        }

        Debug.Log($"牌の山を作成しました。合計：{mountain.Count}枚");
    }

    void SuffleMountain()
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

        UpdateMountainCountUI();

        return drawTile;
    }

    public void SortHand()
    {
        if (playerHand == null) return;
        playerHand = playerHand.OrderBy(tile => tile.suit)
                               .ThenBy(tile => tile.rank)
                               .ToList();
    }

    void PrintHand(List<Tile> hand)
    {
        string handString = "";
        foreach (Tile tile in hand)
        {
            handString += tile.ToString() + "|";
        }
        Debug.Log(handString);
    }

    [Header("UI")]
    public Text mountainCountText;
    public Text playerHandCountText;

    private void UpdateMountainCountUI()
    {
        if (mountainCountText != null)
        {
            mountainCountText.text = $"残りの牌:{mountain.Count}枚";
        }
    }
}