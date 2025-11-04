using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum Suit
{
    None,    //牌がない場合のデフォルト
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
    [Header("リザルト画面設定")]
    public GameObject ResultPanel2;//NPCの役満達成
    public GameObject ResultPanel3;//ゴール達成
    public UnityEngine.UI.Image yakumanImage;

    [Header("役満スプライト設定 (NPC)")] // 追加
    [SerializeField] private Sprite npcKokushiSprite;
    [SerializeField] private Sprite npcDaisangenSprite;
    [SerializeField] private Sprite npcSuuankouSprite;
    [SerializeField] private Sprite npcDaisushiSprite;
    [SerializeField] private Sprite npcChinroutouSprite;
    [SerializeField] private Sprite npcRyuuisoSprite;
    [SerializeField] private Sprite npcTsuisoSprite;
    [SerializeField] private Sprite npcShosushiSprite;
    [SerializeField] private Sprite npcChuurenSprite;

    public event Action OnPlayerHitItem;
    public static MahjongManager instance;
    public bool roundOver = false;

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
            return;
        }

        Tile drawnTile = DrawTile();
        if (drawnTile == null) return;

        int beforeCount = playerHand.Count;
        playerHand.Add(drawnTile);
        if(beforeCount==14)
            SortHandKeepLast();
        else
            SortHand();

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
            int beforeCount = playerHand.Count;
            playerHand.Add(drawnTile);
            if(beforeCount==14)
                SortHandKeepLast();
            else
                SortHand();
            
            MahjongUIManager.instance.UpdateHandUI(playerHand);
        }
    }

    [Header("捨て牌設定")]
    public float discardOffset = 2f;

    public void DiscardTile(int handIndex)
    {
        if (handIndex < 0 || handIndex >= playerHand.Count)
        {
            Debug.Log($"無効なインデックスを捨てようとしました:{handIndex}");
            return;
        }

        Tile discardedTile = playerHand[handIndex];
        Debug.Log($"捨て牌:{discardedTile.GetDisplayName()}");

        GameObject player = GameObject.FindWithTag("Player");
        Vector3 dropPosition = player != null
            ? new Vector3(player.transform.position.x - discardOffset, player.transform.position.y + 0.2f, player.transform.position.z)
            : Vector3.zero;

        ItemManager.instance.DropDiscardedTile(discardedTile, dropPosition);

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

        int beforeCount = playerHand.Count;
        playerHand.Add(tile);
        if(beforeCount==14)
            SortHandKeepLast();
        else
            SortHand();
        
        MahjongUIManager.instance.UpdateHandUI(playerHand);
        return true;
    }

    public void PlayerHitItem()
    {
        OnPlayerHitItem?.Invoke();
    }

    public void ReturnTileToMountain(Tile tile)
    {
        if (tile != null && mountain != null)
        {
            mountain.Add(tile);
        }
    }

    public void OnNpcWin(string npcName, Yakuman yakuman, List<Tile> hand)
    {
        if (roundOver) return;
        roundOver = true;

        Debug.Log($"{npcName}が{yakuman}で上がりました！");

        Sprite spriteToShow = null;
        switch (yakuman)
        {
            case Yakuman.KokushiMusou: spriteToShow = npcKokushiSprite; break;
            case Yakuman.Daisangen: spriteToShow = npcDaisangenSprite; break;
            case Yakuman.SuuAnkou: spriteToShow = npcSuuankouSprite; break;
            case Yakuman.Daisuushii: spriteToShow = npcDaisushiSprite; break;
            case Yakuman.Chinroutou: spriteToShow = npcChinroutouSprite; break;
            case Yakuman.Ryuuiisou: spriteToShow = npcRyuuisoSprite; break;
            case Yakuman.Tsuiso: spriteToShow = npcTsuisoSprite; break;
            case Yakuman.Shousuushii: spriteToShow = npcShosushiSprite; break;
            case Yakuman.Chuuren: spriteToShow = npcChuurenSprite; break;
            default: Debug.LogWarning("未定義の役満スプライト"); break;
        }

        if (yakumanImage != null && spriteToShow != null)
        {
            yakumanImage.sprite = spriteToShow;
            yakumanImage.gameObject.SetActive(true);
        }

        if (ResultPanel2 != null)
        {
            ResultPanel2.SetActive(true);
        }
        else
        {
            Debug.LogError("ResultPanel2が設定されていません。");
        }

        var gameManager2 = FindFirstObjectByType<GameManager2>();
        if (gameManager2 != null)
        {
            gameManager2.GameOver();
        }

        var playerMove = FindFirstObjectByType<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.enabled = false;

        }
    }

    public void OnCharacterGoal(string characterName)
    {
        if (roundOver) return;
        roundOver = true;

        Debug.Log($"{characterName}がゴールしました！");

        // リザルト画面（ゴール）
        if (ResultPanel3 != null)
        {
            ResultPanel3.SetActive(true);
        }
        else
        {
            Debug.LogError("ResultPanel2が設定されていません。");
        }
    }

    // NPC役満パネル(ResultPanel2)の参照チェック
    [ContextMenu("Test/Validate ResultPanel2 refs")]
    public void DebugValidateNpcResultPanel2()
    {
        Debug.Log($"[Validate] ResultPanel2: {(ResultPanel2 ? "OK" : "NULL")}, yakumanImage: {(yakumanImage ? "OK" : "NULL")}");
        if (ResultPanel2)
        {
            var canvas = ResultPanel2.GetComponentInParent<Canvas>();
            Debug.Log($"[Validate] Parent Canvas: {(canvas ? canvas.name : "NONE")}");
            Debug.Log($"[Validate] ActiveInHierarchy(before): {ResultPanel2.activeInHierarchy}");
        }
    }

    // 強制表示（コンテキストメニューから実行可）
    [ContextMenu("Test/Show NPC ResultPanel2")]
    public void DebugShowNpcResultPanel2()
    {
        // 何かしらのスプライトを仮表示（未設定でもOK）
        if (yakumanImage != null)
        {
            var testSprite = npcKokushiSprite ?? npcDaisangenSprite ?? npcSuuankouSprite ?? npcChuurenSprite;
            if (testSprite != null)
            {
                yakumanImage.sprite = testSprite;
                yakumanImage.gameObject.SetActive(true);
            }
        }

        if (ResultPanel2 != null)
        {
            ResultPanel2.SetActive(true);
            Debug.Log("[Test] ResultPanel2 を強制表示しました。");
        }
        else
        {
            Debug.LogError("[Test] ResultPanel2 が未アサインです。");
        }
    }

#if UNITY_EDITOR
    // 再生中に F6 で強制表示（エディタのみ）
    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.F6))
        {
            DebugShowNpcResultPanel2();
        }
    }
#endif

    public void SortHandKeepLast()
    {
        if (playerHand == null || playerHand.Count <= 1) return;
        var last = playerHand[playerHand.Count - 1];
        var rest = playerHand.Take(playerHand.Count - 1)
                                .OrderBy(t => t.suit)
                                .ThenBy(t => t.rank)
                                .ToList();
        rest.Add(last);
        playerHand = rest;
    }
}