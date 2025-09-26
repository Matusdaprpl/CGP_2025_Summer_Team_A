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
    public event Action OnPlayerHitItem;
    public static MahjongManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void PlayerHitItem()
    {
        OnPlayerHitItem?.Invoke();
    }

    [Header("プレハブ設定")]
    public GameObject worldItemPrefab;
    public GameObject handTilePrefab;
    public Transform handPanel;
    public List<Tile> mountain;
    public List<Tile> playerHand;
    private Dictionary<string, Sprite> tileSprites;

    [Header("UI")]
    public Text mountainCountText;
    public Text playerHandCountText;

    [Header("ツモスロット間隔")]
    public float tsumoSpacerWidth = 30f;

    void Start()
    {
        LoadTileSprites();
        CreateMountain();
        SuffleMountain();
        playerHand = new List<Tile>();

        // 初期配牌
        for (int i = 0; i < 14; i++)
        {
            playerHand.Add(DrawTile());
        }
        SortHand();
        UpdateHandUI();

        OnPlayerHitItem += OnItemGetDrawnAndWaitDiscard;

        Debug.Log("MahjongManager側: 山の準備完了。牌の総数: " + mountain.Count);
    }

    public void SetTestHand(List<(Suit suit, int rank)> tilesData)
    {
        if (playerHand == null) playerHand = new List<Tile>();
        playerHand.Clear();

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
        UpdateHandUI();
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
        // 既存のUIをクリア
        foreach (Transform child in handPanel)
            Destroy(child.gameObject);

        // 基本の14枚を並べる
        int baseCount = Mathf.Min(playerHand.Count, 14);
        for (int i = 0; i < baseCount; i++)
        {
            CreateTileUI(playerHand[i], i);
        }

        // Spacerを追加
        GameObject spacer = new GameObject("TsumoSpacer");
        spacer.transform.SetParent(handPanel, false);
        LayoutElement le = spacer.AddComponent<LayoutElement>();
        le.preferredWidth = tsumoSpacerWidth;
        le.preferredHeight = handTilePrefab.GetComponent<LayoutElement>().preferredHeight;

        // ツモスロット（15枚目 or 空白）
        if (playerHand.Count == 15)
        {
            CreateTileUI(playerHand[14], 14);
        }
        else
        {
            GameObject emptySlot = new GameObject("TsumoSlot");
            emptySlot.transform.SetParent(handPanel, false);
            LayoutElement le2 = emptySlot.AddComponent<LayoutElement>();
            le2.preferredWidth = handTilePrefab.GetComponent<LayoutElement>().preferredWidth;
            le2.preferredHeight = handTilePrefab.GetComponent<LayoutElement>().preferredHeight;
        }
    }

    void CreateTileUI(Tile tile, int index)
    {
        GameObject newTileObj = Instantiate(handTilePrefab, handPanel);
        string spriteName = (tile.suit == Suit.Honor)
            ? $"Honor_{tile.rank}"
            : $"{tile.suit}_{tile.rank}";
        if (tileSprites.ContainsKey(spriteName))
        {
            newTileObj.GetComponent<Image>().sprite = tileSprites[spriteName];
        }

        Button tileButton = newTileObj.GetComponent<Button>();
        if (tileButton != null)
        {
            tileButton.onClick.AddListener(() => DiscardTile(index));
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
            UpdateHandUI();
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
        UpdateHandUI();
        UpdateMountainCountUI();
    }

    void CreateMountain()
    {
        mountain = new List<Tile>();
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

    private void UpdateMountainCountUI()
    {
        if (mountainCountText != null)
            mountainCountText.text = $"残りの牌:{mountain.Count}枚";
    }

    public Tile PeekNextTileInMountain()
    {
        if (mountain != null && mountain.Count > 0)
            return mountain[0];
        return null;
    }

    public ItemController SpawnItemFromMountain(Vector3 pos)
    {
        Tile tile = DrawTile();
        if (tile == null) return null;

        Sprite sp = null;
        if (tileSprites != null)
        {
            var key = (tile.suit == Suit.Honor) ? $"Honor_{tile.rank}" : $"{tile.suit}_{tile.rank}";
            tileSprites.TryGetValue(key, out sp);
        }
        tile.sprite = sp;

        var go = Instantiate(worldItemPrefab, pos, Quaternion.identity);
        var ic = go.GetComponent<ItemController>();
        if (ic != null)
        {
            var key = (tile.suit == Suit.Honor) ? $"Honor_{tile.rank}" : $"{tile.suit}_{tile.rank}";
            if (tileSprites.ContainsKey(key))
                tile.sprite = tileSprites[key];
            ic.SetTile(this, tile);
        }
        return ic;
    }

    public bool AddTileToPlayerHand(Tile tile)
    {
        if (tile == null) return false;
        if (playerHand == null) playerHand = new List<Tile>();
        if (playerHand.Count >= 15) return false;

        playerHand.Add(tile);
        UpdateHandUI();
        return true;
    }
}
