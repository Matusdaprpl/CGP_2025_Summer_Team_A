using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// ゲーム全体を管理するクラス
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public Transform handPanel;          // 手牌を並べるUIパネル (13枚)
    public Transform tsumoPanel;         // ツモ牌を表示するUIパネル (1枚)
    public GameObject tilePrefab;        // 手牌用プレハブ (Button付き・TileUIあり)
    public GameObject tsumoTilePrefab;   // ツモ牌用プレハブ (Imageのみ)
    public Button agariButton;           // 「和了」ボタン
    public Button swapButton;            // 「交換」ボタン

    [Header("Game Data")]
    public List<TileType> yama = new List<TileType>();   // 山（残り牌）
    public List<List<TileType>> playerHands = new List<List<TileType>>(); // 各プレイヤーの手牌

    private int myPlayerIndex = 0; // 自分のプレイヤー番号
    private TileType currentTsumo; // 現在のツモ牌

    void Start()
    {
        // ▼ 山を作る（萬子・筒子・索子・字牌）
        yama = GenerateFullYama();

        // ▼ 山をシャッフル
        Shuffle(yama);

        // ▼ 自分の手牌を作成（ランダムに 13 枚）
        playerHands.Clear();
        var myHand = yama.Take(13).ToList();
        yama.RemoveRange(0, 13);
        playerHands.Add(myHand);

        // ▼ UIを更新
        DisplayMyHand();
        DrawTsumo(); // ツモ牌を1枚表示

        // ▼ ボタンに処理を登録
        agariButton.onClick.AddListener(OnAgariButton);
        swapButton.onClick.AddListener(OnSwapButton);
    }

    /// <summary>
    /// 山を作成（全種類4枚ずつ + 赤牌）
    /// </summary>
    List<TileType> GenerateFullYama()
    {
        var list = new List<TileType>();

        // 萬子
        for (int n = 1; n <= 9; n++)
        {
            TileType t = (TileType)System.Enum.Parse(typeof(TileType), $"Man{n}");
            for (int i = 0; i < 4; i++) list.Add(t);
        }
        list.Add(TileType.RedMan5);

        // 筒子
        for (int n = 1; n <= 9; n++)
        {
            TileType t = (TileType)System.Enum.Parse(typeof(TileType), $"Pin{n}");
            for (int i = 0; i < 4; i++) list.Add(t);
        }
        list.Add(TileType.RedPin5);

        // 索子
        for (int n = 1; n <= 9; n++)
        {
            TileType t = (TileType)System.Enum.Parse(typeof(TileType), $"Sou{n}");
            for (int i = 0; i < 4; i++) list.Add(t);
        }
        list.Add(TileType.RedSou5);

        // 字牌
        TileType[] honors = { TileType.East, TileType.South, TileType.West, TileType.North,
                              TileType.White, TileType.Green, TileType.Red };
        foreach (var h in honors)
        {
            for (int i = 0; i < 4; i++) list.Add(h);
        }

        return list;
    }

    /// <summary>
    /// 山をシャッフル
    /// </summary>
    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    /// <summary>
    /// 手牌をUIに表示
    /// </summary>
    void DisplayMyHand()
    {
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        var myHand = playerHands[myPlayerIndex];
        for (int i = 0; i < myHand.Count; i++)
        {
            var tileType = myHand[i];
            GameObject obj = Instantiate(tilePrefab, handPanel);
            var tileUI = obj.GetComponent<TileUI>();
            if (tileUI != null)
            {
                Sprite sprite = GetTileSprite(tileType);
                tileUI.Initialize(this, i, tileType, sprite);
            }
        }
    }

    /// <summary>
    /// 山から1枚ツモって TsumoPanel に表示
    /// </summary>
    void DrawTsumo()
    {
        foreach (Transform child in tsumoPanel)
        {
            Destroy(child.gameObject);
        }

        if (yama.Count == 0)
        {
            Debug.Log("山がありません（ツモ不可）");
            return;
        }

        currentTsumo = yama[0];
        yama.RemoveAt(0);

        GameObject obj = Instantiate(tsumoTilePrefab, tsumoPanel);

        var image = obj.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = GetTileSprite(currentTsumo);
        }

        Debug.Log($"ツモ牌表示: {currentTsumo}");
    }

    /// <summary>
    /// TileType → Sprite 読み込み
    /// </summary>
    Sprite GetTileSprite(TileType tile)
    {
        string fileName = GetSpriteFileName(tile);
        Sprite sprite = Resources.Load<Sprite>("Tiles/" + fileName);

        if (sprite == null)
        {
            Debug.LogWarning($"Spriteが見つかりません: {fileName}");
        }
        return sprite;
    }

    /// <summary>
    /// TileType → 画像ファイル名
    /// </summary>
    string GetSpriteFileName(TileType tile)
    {
        switch (tile)
        {
            // 萬子
            case TileType.Man1: return "man1-66-90-s";
            case TileType.Man2: return "man2-66-90-s";
            case TileType.Man3: return "man3-66-90-s";
            case TileType.Man4: return "man4-66-90-s";
            case TileType.Man5: return "man5-66-90-s";
            case TileType.RedMan5: return "aka3-66-90-s";
            case TileType.Man6: return "man6-66-90-s";
            case TileType.Man7: return "man7-66-90-s";
            case TileType.Man8: return "man8-66-90-s";
            case TileType.Man9: return "man9-66-90-s";

            // 筒子
            case TileType.Pin1: return "pin1-66-90-s";
            case TileType.Pin2: return "pin2-66-90-s";
            case TileType.Pin3: return "pin3-66-90-s";
            case TileType.Pin4: return "pin4-66-90-s";
            case TileType.Pin5: return "pin5-66-90-s";
            case TileType.RedPin5: return "aka1-66-90-s";
            case TileType.Pin6: return "pin6-66-90-s";
            case TileType.Pin7: return "pin7-66-90-s";
            case TileType.Pin8: return "pin8-66-90-s";
            case TileType.Pin9: return "pin9-66-90-s";

            // 索子
            case TileType.Sou1: return "sou1-66-90-s";
            case TileType.Sou2: return "sou2-66-90-s";
            case TileType.Sou3: return "sou3-66-90-s";
            case TileType.Sou4: return "sou4-66-90-s";
            case TileType.Sou5: return "sou5-66-90-s";
            case TileType.RedSou5: return "aka2-66-90-s";
            case TileType.Sou6: return "sou6-66-90-s";
            case TileType.Sou7: return "sou7-66-90-s";
            case TileType.Sou8: return "sou8-66-90-s";
            case TileType.Sou9: return "sou9-66-90-s";

            // 字牌
            case TileType.East: return "ji1-66-90-s";
            case TileType.South: return "ji2-66-90-s";
            case TileType.West: return "ji3-66-90-s";
            case TileType.North: return "ji4-66-90-s";
            case TileType.White: return "ji6-66-90-s";
            case TileType.Green: return "ji5-66-90-s";
            case TileType.Red: return "ji7-66-90-s";
        }
        return null;
    }

    /// <summary>
    /// 牌クリック時（TileUIから呼ばれる）
    /// </summary>
    public void OnTileClicked(TileUI tileUI)
    {
        foreach (Transform child in handPanel)
        {
            var ui = child.GetComponent<TileUI>();
            if (ui != null) ui.SetSelected(false);
        }
        tileUI.SetSelected(true);
    }

    /// <summary>
    /// 「和了」ボタン
    /// </summary>
    void OnAgariButton()
    {
        var myHand = new List<TileType>(playerHands[myPlayerIndex]);
        myHand.Add(currentTsumo);

        Debug.Log($"和了ボタン押下: 手牌 = {string.Join(", ", myHand)}");

        if (KokushiChecker.IsKokushi(myHand))
        {
            Debug.Log("🎉 国士無双です！");
        }
    }

    /// <summary>
    /// 「交換」ボタン → 選択した牌を捨ててツモと入れ替える
    /// </summary>
    void OnSwapButton()
    {
        var myHand = playerHands[myPlayerIndex];
        TileUI selectedTile = null;

        foreach (Transform child in handPanel)
        {
            var tileUI = child.GetComponent<TileUI>();
            if (tileUI != null && tileUI.IsSelected())
            {
                selectedTile = tileUI;
                break;
            }
        }

        if (selectedTile == null)
        {
            Debug.Log("交換する牌を選択してください");
            return;
        }

        if (currentTsumo == default)
        {
            Debug.Log("ツモ牌がありません");
            return;
        }

        int index = selectedTile.handIndex;
        TileType oldTile = myHand[index];
        myHand[index] = currentTsumo;

        Debug.Log($"交換: {oldTile} を捨てて {currentTsumo} を手牌に追加（位置 {index} に差し替え）");

        DisplayMyHand();
        DrawTsumo(); // 新しいツモ牌を引く
    }
}
