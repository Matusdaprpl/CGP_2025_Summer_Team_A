using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    [Header("プレハブ設定")]
    public GameObject worldItemPrefab;

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

    public ItemController SpawnItemFromMountain(Vector3 pos)
    {
        Tile tile = MahjongManager.instance.DrawTile();
        if (tile == null) return null;

        var tileSprites = MahjongUIManager.instance.GetTileSprites(); // UIマネージャーからスプライトを取得
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
            ic.SetTile(MahjongManager.instance, tile);
        }
        return ic;
    }

    

    // 必要に応じて追加メソッド
}