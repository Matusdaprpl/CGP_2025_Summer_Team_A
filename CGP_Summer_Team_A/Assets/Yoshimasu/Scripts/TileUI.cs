using UnityEngine;
using UnityEngine.UI;

public class TileUI : MonoBehaviour
{
    public TileType tileType;
    public int handIndex; 
    private Image image;
    private Button button;
    private bool isSelected = false;
    private GameManager gm;

    void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();

        if (button != null)
        {
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(OnClick);
        }
    }

    public void Initialize(GameManager manager, int index, TileType type, Sprite sprite)
    {
        gm = manager;
        handIndex = index;
        tileType = type;
        image.sprite = sprite;
        image.color = Color.white;
    }

    private void OnClick()
    {
        if (gm != null)
        {
            gm.OnTileClicked(this);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        image.color = selected ? Color.yellow : Color.white;
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}
