using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class MahjongUIManager : MonoBehaviour
{
    public static MahjongUIManager instance;

    [Header("UI設定")]
    public GameObject handTilePrefab;
    public Transform handPanel;
    public Text mountainCountText;
    public Text playerHandCountText;
    public float tsumoSpacerWidth = 30f;

    private Dictionary<string, Sprite> tileSprites;

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
        LoadTileSprites();
    }

    private void LoadTileSprites()
    {
        tileSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Tiles");
        foreach (Sprite sprite in sprites)
        {
            tileSprites[sprite.name] = sprite;
        }
    }

    public Dictionary<string, Sprite> GetTileSprites()
    {
        return tileSprites;
    }

    public void UpdateHandUI(List<Tile> playerHand)
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
            tileButton.onClick.AddListener(() => MahjongManager.instance.DiscardTile(index));
        }
    }

    public void UpdateMountainCountUI(int count)
    {
        if (mountainCountText != null)
            mountainCountText.text = $"残りの牌:{count}枚";
    }

}