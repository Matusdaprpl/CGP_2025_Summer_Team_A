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
    public Transform handPanel;          // 手牌を並べるUIパネル (13枚)
    public Transform tsumoPanel;         // ツモ牌を表示するUIパネル (1枚)
    public GameObject tilePrefab;        // 手牌用プレハブ (Button付き・TileUIあり)
    public GameObject tsumoTilePrefab;   // ツモ牌用プレハブ (Imageのみ)
    public Button agariButton;           // 「和了」ボタン
    public Button swapButton;            // 「交換」ボタン

    [Header("Game Data")]
    public List<TileType> yama = new List<TileType>();   // 山（残り牌）
    public List<List<TileType>> playerHands = new List<List<TileType>>(); // 各プレイヤーの手牌

    // 【実験用フラグ】どの役満を強制配牌するかをインスペクターで設定
    [Header("Debug Hand Selection (Set only ONE to true)")]
    public bool forceDaisangenHand = true; // 👈 今回の実験対象
    public bool forceSuuankouHand = false; 
    public bool forceKokushiHand = false; 

    private int myPlayerIndex = 0; // 自分のプレイヤー番号
    private TileType currentTsumo; // 現在のツモ牌

    void Start()
    {
        // ▼ 山を作る（萬子・筒子・索子・字牌）
        yama = GenerateFullYama();

        // ▼ 山をシャッフル
        Shuffle(yama);

        playerHands.Clear();

        // 【DEBUG強化】現在のフラグの状態を出力します
        Debug.Log($"[DEBUG FLAGS] Daisangen: {forceDaisangenHand}, Suuankou: {forceSuuankouHand}, Kokushi: {forceKokushiHand}");
        
        // ▼ 【強制配牌】実験モードの切り替え
        if (forceDaisangenHand)
        {
            CreateDaisangenHand(); // 大三元の牌をセット
        }
        else if (forceSuuankouHand)
        {
            CreateSuuankouHand(); // 四暗刻の牌をセット
        }
        else if (forceKokushiHand)
        {
            CreateKokushiHand(); // 国士無双の牌をセット
        }
        else
        {
            // ▼ 通常配牌：自分の手牌を作成（ランダムに 13 枚）
            var myHand = yama.Take(13).ToList();
            yama.RemoveRange(0, 13);
            playerHands.Add(myHand);
            DrawTsumo(); 
        }

        // ▼ UIを更新
        DisplayMyHand();

        // ▼ ボタンに処理を登録
        agariButton.onClick.AddListener(OnAgariButton);
        swapButton.onClick.AddListener(OnSwapButton);
    }

    // --------------------------------------------------------------------------------
    // 【実験用配牌ロジック】
    // --------------------------------------------------------------------------------
    
    /// <summary>
    /// 【実験用】国士無双が成立する手牌を強制的に作成し、山から除去する
    /// </summary>
    void CreateKokushiHand()
    {
        var kokushiBaseTiles = new List<TileType>
        {
            TileType.Man1, TileType.Man9, TileType.Pin1, TileType.Pin9, TileType.Sou1, TileType.Sou9,
            TileType.East, TileType.South, TileType.West, TileType.North,
            TileType.White, TileType.Green, TileType.Red
        };
        TileType jatouTile = TileType.East; 

        var fullHand = new List<TileType>(kokushiBaseTiles);
        fullHand.Add(jatouTile); 

        var myHand = fullHand.Take(13).ToList(); 
        currentTsumo = fullHand[13]; 

        playerHands.Add(myHand);

        foreach(var tile in fullHand)
        {
            yama.Remove(tile); 
        }
        
        InstantiateTsumoUI(currentTsumo);
        
        Debug.Log($"【実験モード】国士無双を配牌しました。雀頭となるツモ牌: {currentTsumo}");
    }

    /// <summary>
    /// 【実験用】四暗刻が成立する手牌を強制的に作成し、山から除去する
    /// (Man1x3, Man2x3, Man3x3, Eastx3, Southx2)
    /// </summary>
    void CreateSuuankouHand()
    {
        var fullHand = new List<TileType>
        {
            TileType.Man1, TileType.Man1, TileType.Man1,
            TileType.Man2, TileType.Man2, TileType.Man2,
            TileType.Man3, TileType.Man3, TileType.Man3,
            TileType.East, TileType.East, TileType.East,
            TileType.South, 
            TileType.South 
        };

        var myHand = fullHand.Take(13).ToList(); 
        currentTsumo = fullHand[13]; 

        playerHands.Add(myHand);

        foreach(var tile in fullHand)
        {
            yama.Remove(tile); 
        }
        
        InstantiateTsumoUI(currentTsumo);
        
        Debug.Log($"【実験モード】四暗刻を配牌しました。雀頭となるツモ牌: {currentTsumo}");
    }
    
    /// <summary>
    /// 【実験用】大三元が成立する手牌（13枚）とツモ牌（1枚）を強制的に作成し、山から除去する。
    /// (白x3, 發x3, 中x3, Pin2x3, Pin4x2) の構成。
    /// </summary>
    void CreateDaisangenHand()
    {
        var fullHand = new List<TileType>
        {
            // 大三元：刻子 1, 2, 3
            TileType.White, TileType.White, TileType.White,
            TileType.Green, TileType.Green, TileType.Green,
            TileType.Red, TileType.Red, TileType.Red,
            
            // 通常面子 (刻子)
            TileType.Pin2, TileType.Pin2, TileType.Pin2,

            // 雀頭 (ツモ牌で完成)
            TileType.Pin4, 
            TileType.Pin4 // 👈 ツモ牌
        };

        var myHand = fullHand.Take(13).ToList(); 
        currentTsumo = fullHand[13]; 

        playerHands.Add(myHand);

        foreach(var tile in fullHand)
        {
            yama.Remove(tile); 
        }
        
        InstantiateTsumoUI(currentTsumo);
        
        Debug.Log($"【実験モード】大三元を配牌しました。雀頭となるツモ牌: {currentTsumo}");
    }


    // --------------------------------------------------------------------------------
    // 【ゲームロジック】
    // --------------------------------------------------------------------------------

    /// <summary>
    /// 山を作成（全種類4枚ずつ + 赤牌）
    /// </summary>
    List<TileType> GenerateFullYama()
    {
        var list = new List<TileType>();

        // 萬子, 筒子, 索子の1〜9と赤5
        for (int i = 0; i < 3; i++) 
        {
            string prefix = i == 0 ? "Man" : i == 1 ? "Pin" : "Sou";
            for (int n = 1; n <= 9; n++)
            {
                TileType t = (TileType)System.Enum.Parse(typeof(TileType), $"{prefix}{n}");
                for (int c = 0; c < 4; c++) list.Add(t);
            }
            // 赤5を追加
            TileType redFive = (TileType)System.Enum.Parse(typeof(TileType), $"Red{prefix}5");
            var tileFive = (TileType)System.Enum.Parse(typeof(TileType), $"{prefix}5");
            list.Remove(tileFive); 
            list.Add(redFive); 
        }

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
    /// 手牌をUIに表示（ソート処理付き）
    /// </summary>
    void DisplayMyHand()
    {
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        var myHand = playerHands[myPlayerIndex];
        
        var sortedHand = HandSorter.SortHand(myHand);
        playerHands[myPlayerIndex] = sortedHand;

        for (int i = 0; i < sortedHand.Count; i++)
        {
            var tileType = sortedHand[i];
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
    /// 山から1枚ツモって TsumoPanel に表示（通常配牌時のみ使用）
    /// </summary>
    void DrawTsumo()
    {
        if (yama.Count == 0)
        {
            Debug.Log("山がありません（ツモ不可）");
            return;
        }

        currentTsumo = yama[0];
        yama.RemoveAt(0);

        InstantiateTsumoUI(currentTsumo);

        Debug.Log($"ツモ牌表示: {currentTsumo}");
    }

    /// <summary>
    /// ツモ牌のUIを作成・表示する共通処理
    /// </summary>
    void InstantiateTsumoUI(TileType tile)
    {
        foreach (Transform child in tsumoPanel)
        {
            Destroy(child.gameObject);
        }
        
        GameObject obj = Instantiate(tsumoTilePrefab, tsumoPanel);

        var image = obj.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = GetTileSprite(tile);
        }
    }

    // --------------------------------------------------------------------------------
    // 【スプライト読み込みロジック】
    // --------------------------------------------------------------------------------

    /// <summary>
    /// TileType → Sprite 読み込み
    /// </summary>
    Sprite GetTileSprite(TileType tile)
    {
        string fileName = GetSpriteFileName(tile);
        // Resourcesフォルダ内の "Tiles" サブフォルダから画像を読み込み
        Sprite sprite = Resources.Load<Sprite>("Tiles/" + fileName); 

        if (sprite == null)
        {
            Debug.LogWarning($"Spriteが見つかりません: Resources/Tiles/{fileName}");
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
            case TileType.Man1: return "man1-66-90-s"; case TileType.Man2: return "man2-66-90-s"; 
            case TileType.Man3: return "man3-66-90-s"; case TileType.Man4: return "man4-66-90-s";
            case TileType.Man5: return "man5-66-90-s"; case TileType.RedMan5: return "aka3-66-90-s"; 
            case TileType.Man6: return "man6-66-90-s"; case TileType.Man7: return "man7-66-90-s";
            case TileType.Man8: return "man8-66-90-s"; case TileType.Man9: return "man9-66-90-s";
            
            case TileType.Pin1: return "pin1-66-90-s"; case TileType.Pin2: return "pin2-66-90-s"; 
            case TileType.Pin3: return "pin3-66-90-s"; case TileType.Pin4: return "pin4-66-90-s";
            case TileType.Pin5: return "pin5-66-90-s"; case TileType.RedPin5: return "aka1-66-90-s"; 
            case TileType.Pin6: return "pin6-66-90-s"; case TileType.Pin7: return "pin7-66-90-s";
            case TileType.Pin8: return "pin8-66-90-s"; case TileType.Pin9: return "pin9-66-90-s";
            
            case TileType.Sou1: return "sou1-66-90-s"; case TileType.Sou2: return "sou2-66-90-s"; 
            case TileType.Sou3: return "sou3-66-90-s"; case TileType.Sou4: return "sou4-66-90-s";
            case TileType.Sou5: return "sou5-66-90-s"; case TileType.RedSou5: return "aka2-66-90-s"; 
            case TileType.Sou6: return "sou6-66-90-s"; case TileType.Sou7: return "sou7-66-90-s";
            case TileType.Sou8: return "sou8-66-90-s"; case TileType.Sou9: return "sou9-66-90-s";
            
            case TileType.East: return "ji1-66-90-s"; case TileType.South: return "ji2-66-90-s"; 
            case TileType.West: return "ji3-66-90-s"; case TileType.North: return "ji4-66-90-s";
            case TileType.White: return "ji6-66-90-s"; case TileType.Green: return "ji5-66-90-s";
            case TileType.Red: return "ji7-66-90-s";   
            default: return null;
        }
    }


    // --------------------------------------------------------------------------------
    // 【ユーザー入力＆ゲーム進行】
    // --------------------------------------------------------------------------------

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
        myHand.Add(currentTsumo); // ツモ牌を加えて14枚にする

        Debug.Log($"和了ボタン押下: 手牌 = {string.Join(", ", myHand)}");

        // 役満の判定を順番に行う
        if (KokushiChecker.IsKokushi(myHand))
        {
            Debug.Log("🎉 国士無双です！");
        }
        else if (SuuankouChecker.IsSuuankou(myHand))
        {
            Debug.Log("🎉 四暗刻です！");
        }
        else if (DaisangenChecker.IsDaisangen(myHand))
        {
            Debug.Log("🎉 大三元です！");
        }
        
        // ここに他の役満判定を追加していく
        
        else
        {
            Debug.Log("役満ではありません。（他の役の判定は未実装です）");
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