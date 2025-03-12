using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FItem
{
    public string id;
    public GameObject asset;
    public Sprite icon;
    public Vector3 handlePosition;

    public bool IsValid
    {
        get
        {
            return id != string.Empty && asset != null;
        }
    }
}

[CreateAssetMenu(fileName = "ItemsData", menuName = "Inventory/ItemsData")]
public class ItemsData : ScriptableObject
{
    [SerializeField]
    private List<FItem> itemsData = new List<FItem>();

    public List<FItem> GetItems() => itemsData;
}
