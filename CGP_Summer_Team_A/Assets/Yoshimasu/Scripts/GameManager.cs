using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム全体を管理するクラス
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public Transform handPanel;              // 手牌を並べるUIパネル
    public GameObject tilePrefab;            // 牌UIのプレハブ
    public Button agariButton;               // 「和了」ボタン
    public Button swapButton;                // 「交換」ボタン

    [Header("Game Data")]
    public List<TileType> yama = new List<TileType>();   // 山（残り牌）
    public List<List<TileType>> playerHands = new List<List<TileType>>(); // 各プレイヤーの手牌

    private int myPlayerIndex = 0; // 自分のプレイヤー番号

    void Start()
    {
        // ▼ 山を作る（ここでは簡易的に東・南・西・北を並べておく例）
        yama.Clear();
        for (int i = 0; i < 4; i++)
        {
            yama.Add(TileType.East);
            yama.Add(TileType.South);
            yama.Add(TileType.West);
            yama.Add(TileType.North);
        }

        // ▼ 自分の手牌を作成（仮で 14 枚）
        playerHands.Clear();
        var myHand = new List<TileType>();
        myHand.Add(TileType.Man1);
        myHand.Add(TileType.Man9);
        myHand.Add(TileType.Pin1);
        myHand.Add(TileType.Pin9);
        myHand.Add(TileType.Sou1);
        myHand.Add(TileType.Sou9);
        myHand.Add(TileType.East);
        myHand.Add(TileType.South);
        myHand.Add(TileType.West);
        myHand.Add(TileType.North);
        myHand.Add(TileType.White);
        myHand.Add(TileType.Green);
        myHand.Add(TileType.Red);
        myHand.Add(TileType.Red); // 雀頭（仮）
        playerHands.Add(myHand);

        // ▼ UIを更新
        DisplayMyHand();

        // ▼ ボタンに処理を登録
        agariButton.onClick.AddListener(OnAgariButton);
        swapButton.onClick.AddListener(OnSwapButton);
    }

    /// <summary>
    /// 手牌をUIに表示
    /// </summary>
    void DisplayMyHand()
    {
        // 一旦子オブジェクトを削除
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }

        // 手牌を並べる
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
    /// 牌画像の取得（仮実装）
    /// </summary>
    Sprite GetTileSprite(TileType tile)
    {
        // 実際には Resources.Load などで画像を取得する
        return null;
    }

    /// <summary>
    /// 牌がクリックされたとき
    /// </summary>
    public void OnTileClicked(TileUI tileUI)
    {
        // いったん全ての選択を解除
        foreach (Transform child in handPanel)
        {
            var ui = child.GetComponent<TileUI>();
            if (ui != null) ui.SetSelected(false);
        }

        // クリックしたものを選択
        tileUI.SetSelected(true);
    }

    /// <summary>
    /// 「和了」ボタンが押されたとき
    /// </summary>
    void OnAgariButton()
    {
        var myHand = playerHands[myPlayerIndex];

        bool isKokushi = KokushiChecker.IsKokushi(myHand);
        if (isKokushi)
        {
            Debug.Log("国士無双です！！！");
        }
        else
        {
            Debug.Log("国士無双ではありません");
        }
    }

    /// <summary>
    /// 「交換」ボタンが押されたとき
    /// 選択した場所に新しい牌を差し替える
    /// </summary>
    void OnSwapButton()
    {
        var myHand = playerHands[myPlayerIndex];

        // 選択されている TileUI を探す
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

        if (yama.Count == 0)
        {
            Debug.Log("山がありません");
            return;
        }

        // 選択した場所を差し替え
        int index = selectedTile.handIndex;
        TileType oldTile = myHand[index];
        TileType newTile = yama[0];
        yama.RemoveAt(0);

        myHand[index] = newTile; // ← 差し替え

        Debug.Log($"交換: {oldTile} を捨てて {newTile} をツモりました（位置 {index} に差し替え）");

        // UI を更新
        DisplayMyHand();
    }
}
