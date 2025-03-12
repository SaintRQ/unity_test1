using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public delegate void OnSlotChanged(int newSlotIndex);
    public event OnSlotChanged SlotChanged;

    [SerializeField]
    Transform _gameEndBar = null;

    [SerializeField]
    Transform _dot = null;

    [SerializeField]
    Transform _inventoryGrid = null;

    [SerializeField]
    GameObject _slotReference = null;

    public int lastSelectedSlot { get; private set; } = -1;
    List<InventorySlot> _inventorySlots = new List<InventorySlot>();

    public void ShowGameEndBar() 
    { 
        if(_gameEndBar) _gameEndBar.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CloseGame()
    {
       #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
       #else
    Application.Quit(); // Работает в сборке
       #endif
    }


    public void InitializeInventoryGrid(int SlotAmount) 
    {
        if (!_inventoryGrid || !_slotReference) return;

        for (int i = _inventoryGrid.childCount - 1; i >= 0; i--)
        {
            Destroy(_inventoryGrid.GetChild(i).gameObject);
        }

        for (int i = 0; i < SlotAmount; i++)
        {
            GameObject newSlot = Instantiate(_slotReference, _inventoryGrid);
            if (newSlot)
            {
                newSlot.SetActive(true);
                if(newSlot.TryGetComponent(out InventorySlot slot))
                {
                    slot.InitializeSlot(i);
                    _inventorySlots.Add(slot);
                }
            }
           
        }


    }
    public void UpdateInventoryGrid(List<FItem> Stotage) 
    { 
        if(_inventorySlots.Count == 0) return;

        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            if (i < Stotage.Count)
            {
                _inventorySlots[i].SetSlot(Stotage[i].icon);
            }
            else _inventorySlots[i].SetSlot(null);
        }
    }
    public void SelectSlot(int slotIndex)
    {
        if (_inventorySlots.Count == 0) return;

        int nextSlot = slotIndex;

        if (nextSlot >= _inventorySlots.Count) nextSlot = 0;
        else if (nextSlot < 0) nextSlot = _inventorySlots.Count - 1;

        if (lastSelectedSlot >= 0) _inventorySlots[lastSelectedSlot].DeselectSlot();
        
        _inventorySlots[nextSlot].SelectSlot();
        lastSelectedSlot = nextSlot;

        SlotChanged?.Invoke(lastSelectedSlot);

    }
}
