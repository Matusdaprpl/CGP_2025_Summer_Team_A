using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public Dictionary<string, int> items = new Dictionary<string, int>();

    public void AddItem(string itemName)
    {
        if (!items.ContainsKey(itemName))
        {
            items[itemName] = 1;
        }
    }

}
