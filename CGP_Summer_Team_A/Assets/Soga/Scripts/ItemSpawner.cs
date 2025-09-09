using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private int itemCount = 10;
    [SerializeField]
    private float minX = -10f;
    [SerializeField]
    private float maxX = 10f;
    [SerializeField]
    private float minY = -2f;
    [SerializeField]
    private float maxY = -2f;

    void Start()
    {
        for (int i = 0; i < itemCount; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            Vector2 spawnPosition = new Vector2(randomX, randomY);

            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
    }

}
