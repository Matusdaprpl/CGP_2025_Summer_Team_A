using UnityEngine;

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

    public void SpawnItemFromMountain(Vector2 position)
    {
        if (worldItemPrefab == null)
        {
            Debug.LogError("worldItemPrefab が設定されていません。");
            return;
        }

        GameObject itemObj = Instantiate(worldItemPrefab, position, Quaternion.identity);
        ItemController itemController = itemObj.GetComponent<ItemController>();
        if (itemController != null)
        {
            Tile tile = MahjongManager.instance.DrawTile();
            if (tile != null)
            {
                itemController.SetTile(MahjongManager.instance, tile);
            }
        }
    }

    public void DropDiscardedTile(Tile discardedTile, Vector3 dropPosition)
    {
        if (worldItemPrefab == null || discardedTile == null)
        {
            Debug.LogError("worldItemPrefab または discardedTile が無効です。");
            return;
        }

        GameObject itemObj = Instantiate(worldItemPrefab, dropPosition, Quaternion.identity);
        ItemController itemController = itemObj.GetComponent<ItemController>();
        if (itemController != null)
        {
            itemController.SetTile(MahjongManager.instance, discardedTile);
        }
    }
}