using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MahjongUIManager : MonoBehaviour
{
    public static MahjongUIManager instance;

    [Header("UI設定")]
    public Text mountainCountText;
    public Text playerHandCountText;
    public GameObject handTilePrefab;
    public Transform handPanel;
    public float tileSpacing = 30f;

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

    public void UpdateHandUI(List<Tile> playerHand)
    {
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerHand.Count; i++)
        {
            if (playerHand.Count == 15 && i == 14)
            {
                GameObject spacer = new GameObject("TileSpacer");
                spacer.transform.SetParent(handPanel, false);
                LayoutElement le = spacer.AddComponent<LayoutElement>();
                le.preferredWidth = tileSpacing;
            }

            GameObject newTileObj = Instantiate(handTilePrefab, handPanel);
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
                tileButton.onClick.AddListener(() => MahjongManager.instance.DiscardTile(index));
            }
        }

        UpdateMountainCountUI(MahjongManager.instance.mountain.Count);
        UpdatePlayerHandCountUI(playerHand.Count);
    }

    public void UpdateMountainCountUI(int count)
    {
        if (mountainCountText != null)
        {
            mountainCountText.text = $"山: {count}";
        }
    }

    public void UpdatePlayerHandCountUI(int count)
    {
        if (playerHandCountText != null)
        {
            playerHandCountText.text = $"手牌: {count}";
        }
    }
}