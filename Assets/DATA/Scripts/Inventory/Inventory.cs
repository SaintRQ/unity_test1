using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private ItemsData _itemsData;
    [SerializeField]
    private Transform _itemHandle = null;

    [SerializeField]
    private float _selectItemAnimationTime = 0.25f;

    private int _maxInventorySlots = 5;
    private HUDManager _hud = null;
    private GameObject _lastSelectedItem = null;

    private Vector3 _itemHandleStartPosition = new Vector3(0, -0.2f, 0.1f);
    private Vector3 _itemHandleEndPosition= Vector3.zero;

    private List<FItem> _storage = new List<FItem>();
    public bool inventoryIsInitialized { get; private set; } = false;

    void Start()
    {
        InitializeInventory();
    }
    private void InitializeInventory()
    {
        _hud = FindObjectOfType<HUDManager>();

        if (_itemsData && _hud && _itemHandle)
        {
            _hud.InitializeInventoryGrid(_maxInventorySlots);
            _hud.SelectSlot(0);
            _hud.SlotChanged += OnSlotChanged;

            inventoryIsInitialized = true;

            Debug.Log("Inventory is initialized");
        }
        else Debug.LogError("Inventory is not initialized");
    }


//---------------------------------------------------

    public bool AddItem(string ItemID) 
    {
        if(!inventoryIsInitialized || ItemID == string.Empty || _storage.Count >= _maxInventorySlots) return false;

        var itemData = GetItemData(ItemID);
        if (itemData.IsValid)
        {
            _storage.Add(itemData); 
            _hud.UpdateInventoryGrid(_storage);
            _hud.SelectSlot(_storage.Count - 1);
            return true;
        }
        
        return false;
    }
    public bool RemoveItem(string ItemID)
    {
        if (!inventoryIsInitialized || ItemID == string.Empty) return false;
     
        for(int i = 0; i < _storage.Count; i++)
        {
            if (_storage[i].id == ItemID)
            {
                _storage.RemoveAt(i);
                _hud.UpdateInventoryGrid(_storage);
                if (_lastSelectedItem) Destroy(_lastSelectedItem);

                _hud.SelectSlot(Mathf.Clamp(_storage.Count - 1, 0,  _storage.Count - 1)); 
                return true;
            }
        }


        return false;
    }
    public FItem GetItemData(int slotIndex)
    {
        if (!inventoryIsInitialized || slotIndex < 0 || slotIndex >= _storage.Count) return new FItem();
        return _storage[slotIndex];
    }
    public FItem GetItemData(string itemID)
    {
        if (!inventoryIsInitialized) return new FItem();

        var items = _itemsData.GetItems();
        if (items == null || items.Count == 0) return new FItem();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == itemID)
            {
                return items[i];
            }
        }

        return new FItem();
    }

    public int GetInventoryMasSlots() { return _maxInventorySlots; }

 //--------------------------------------------------- Events
    private void OnSlotChanged(int newSlotIndex)
    {

        if (_lastSelectedItem) Destroy(_lastSelectedItem);

        if  (newSlotIndex >= 0 && newSlotIndex < _storage.Count)
        {
            FItem item = GetItemData(_storage[newSlotIndex].id);
            if (item.IsValid)
            {
                if (_itemHandle) 
                {
                    StopAllCoroutines();

                    _itemHandleEndPosition = item.handlePosition;
                 
                    StartCoroutine(AnimateItemHandle());

                } 
                _lastSelectedItem = Instantiate(item.asset, _itemHandle.position, _itemHandle.rotation);


                if (_lastSelectedItem) 
                {
                    var itemRef = _lastSelectedItem.GetComponent<Item>();
                    if (itemRef) itemRef.DisableCollision();

                    var rb = _lastSelectedItem.transform.root.GetComponentInChildren<Rigidbody>();
                    if (rb) rb.isKinematic = true;

                   
                    _lastSelectedItem.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    _lastSelectedItem.transform.SetParent(_itemHandle);
                 

                }
                
            }
 
        }
    }
 //--------------------------------------------------- 

    private IEnumerator AnimateItemHandle()
    {
        if (_itemHandle == null) yield break;

        float elapsedTime = 0f;
        Vector3 startPos = _itemHandleStartPosition;
        Vector3 endPos = _itemHandleEndPosition;

        while (elapsedTime < _selectItemAnimationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _selectItemAnimationTime;
            t = Mathf.SmoothStep(0f, 1f, t); 

            _itemHandle.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        _itemHandle.localPosition = endPos;
    }

}
