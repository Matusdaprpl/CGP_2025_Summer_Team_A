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
    public float tsumoSpacerWidth = 200f;

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

// MahjongUIManager.cs 内

    public void UpdateHandUI(List<Tile> playerHand)
    {
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        if (playerHand == null) return;

        // 手牌リストをループしてUIを生成
        for (int i = 0; i < playerHand.Count; i++)
        {
            // 手牌が15枚あり、これから15枚目 (インデックス14) を表示する直前の場合
            if (playerHand.Count == 15 && i == 14)
            {
                // スペーサーオブジェクトを生成して間に挿入
                GameObject spacer = new GameObject("TsumoSpacer",typeof(RectTransform));
                spacer.transform.SetParent(handPanel, false);

                RectTransform rt = spacer.GetComponent<RectTransform>();
                rt.localScale = Vector3.one;

                LayoutElement le = spacer.AddComponent<LayoutElement>();
                le.minWidth = tsumoSpacerWidth;
                le.preferredWidth = tsumoSpacerWidth;
                le.flexibleWidth = 0f;
                le.layoutPriority = 1; // 他の要素より優先度を高く設定
                
                // RectTransformのサイズを明示的に設定
                rt.sizeDelta = new Vector2(tsumoSpacerWidth, rt.sizeDelta.y);

                // プレハブの高さを取得して設定（レイアウトの崩れを防ぐため）
                var prefabLayout = handTilePrefab.GetComponent<LayoutElement>();
                if (prefabLayout != null)
                {
                    le.preferredHeight = prefabLayout.preferredHeight;
                }
            }

            // 牌のUIを生成
            CreateTileUI(playerHand[i], i);
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