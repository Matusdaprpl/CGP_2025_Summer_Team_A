using System;
using Unity.Mathematics;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public string itemName;
    public int quantity;

    public static event Action<string, int> OnItemPickedUp;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            MahjongManager manager = MahjongManager.instance;

            if (manager != null && manager.playerHand.Count >= 15)
            {
                return;
            }
            
            if (playerInventory != null)
            {
                playerInventory.AddItem(itemName);
                OnItemPickedUp?.Invoke(itemName, 1);

                if (MahjongManager.instance != null)
                {
                    MahjongManager.instance.PlayerHitItem();
                }

                Destroy(gameObject);
            }
        }
    }
}
