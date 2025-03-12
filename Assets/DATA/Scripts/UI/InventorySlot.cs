using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    Transform _imageItem;
    [SerializeField]
    Color _baseColor= Color.black;
    [SerializeField]
    Color _selectedColor = Color.black;
    Image _slotBackground = null;


    public int slotIndex { get; private set; } = -1;

    public void InitializeSlot(int index)
    {
        slotIndex = index;

        if(TryGetComponent(out _slotBackground)) _slotBackground.color = _baseColor;
       
        if (_imageItem) _imageItem.gameObject.SetActive(false);
    }

    public void SetSlot(Sprite ItemImage) 
    {
        if (_imageItem)
        {
            if (ItemImage) 
            {
                _imageItem.gameObject.SetActive(true);
                _imageItem.GetComponent<Image>().sprite = ItemImage;
            }
            else 
            {
                _imageItem.gameObject.SetActive(false);
            }
           
        }      
    }
    public void SelectSlot()
    {
        if (_slotBackground)
        {
            _slotBackground.color = _selectedColor;
        }
    }
    public void DeselectSlot()
    {
        if (_slotBackground)
        {
            _slotBackground.color = _baseColor;
        }
    }
}
