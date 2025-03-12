using UnityEngine;

public class Item : MonoBehaviour, IInteract
{
    [SerializeField]
    private string _itemID;
    private Outline _outline;

    private void Start()
    {
        _outline = transform.root.GetComponentInChildren<Outline>();
        if (_outline) _outline.enabled = false;
    }

    public void DisableCollision()
    {
        var collision = GetComponent<Collider>();
        if(collision) collision.enabled = false;
    }

    public void OnAimEnter(PlayerController Controller)
    {
        if (_outline && Controller) 
        {
            _outline.OutlineColor = Controller.outlineColor;
            _outline.enabled = true;
        }
        
    }

    public void OnAimExit(PlayerController Controller)
    {

        if (_outline) _outline.enabled = false;
    }

    public void OnInteract(PlayerController Controller)
    {
        if (Controller && Controller.TakeItem(_itemID)) 
        {       
            Destroy(gameObject);
        }
    }


}
