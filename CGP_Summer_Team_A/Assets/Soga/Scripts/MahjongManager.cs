using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public enum Suit
{
    Manzu,
    Pinzu,
    Souzu,
    Honor
}

[System.Serializable]
public class Tile
{
    public Suit suit;
    public int rank;

    public Tile(Suit s, int r)
    {
        suit = s;
        rank = r;
    }

    public override string ToString()
    {
        if (suit == Suit.Honor)
        {
            string[] honorNames = { "東", "南", "西", "北", "白", "發", "中" };
            return honorNames[rank - 1];
        }
        return $"{suit}_{rank}";
    }
}

public class MahjongManager : MonoBehaviour
{
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

        for (int i = 0; i < 13; i++)
        {
            playerHand.Add(DrawTile());
        }

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

        foreach (Tile tile in playerHand)
        {
            GameObject newTileObj = Instantiate(tilePrefab, handPanel);

            string spriteName = tile.suit == Suit.Honor ? $"Honor_{tile.rank}" : $"{tile.suit}_{tile.rank}";

            if (tileSprites.ContainsKey(spriteName))
            {
                newTileObj.GetComponent<Image>().sprite = tileSprites[spriteName];
            }
            else
            {
                Debug.LogError($"スプライトが見つかりません:{spriteName}");
            }
        }
    }

    void CreateMountain()
    {
        mountain = new List<Tile>();

        foreach (Suit s in new Suit[] { Suit.Manzu,Suit.Pinzu, Suit.Souzu })
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
            int randomIndex = Random.Range(i, mountain.Count);
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
}