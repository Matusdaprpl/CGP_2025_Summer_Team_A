using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TileUI : MonoBehaviour
{
    public int handIndex;
    public TileType tileType;

    private Button button;
    private Image image;
    private GameManager gameManager;
    private bool isSelected = false;

    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    public void Initialize(GameManager gm, int index, TileType tile, Sprite sprite)
    {
        gameManager = gm;
        handIndex = index;
        tileType = tile;

        if (image != null)
        {
            image.sprite = sprite;
            image.preserveAspect = true;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        gameManager.OnTileClicked(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        transform.localScale = selected ? Vector3.one * 1.1f : Vector3.one;
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}
