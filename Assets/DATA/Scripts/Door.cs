using UnityEngine;


public class Door : MonoBehaviour, IInteract
{

    [SerializeField]
    private string _itemForActivation = string.Empty;

    private bool doorIsActivated = false;
    private bool playerIsAiming = false;

    private PlayerController _playerController = null;

    private Outline _outline;


    private void Start()
    {
        _outline = transform.root.GetComponentInChildren<Outline>();
        if (_outline) _outline.enabled = false;
    }

    public void SetDoorActive(bool newState)
    {
        if(newState == doorIsActivated) return;
        doorIsActivated = newState;

        if (!playerIsAiming) return;

        if (doorIsActivated) 
        {
            if (_playerController) 
            {
                if (_outline)
                {
                    _outline.OutlineColor = _playerController.outlineColor;
                    _outline.enabled = true;
                }
                    
            }
            else
            {
                if (_outline) 
                {
                    _outline.enabled = false;
                }
            }
        }
        else
        {
            if (_outline)
            {
                _outline.enabled = false;
            }
        }
    }

    public void OnAimEnter(PlayerController Controller)
    {
        _playerController = Controller;
        playerIsAiming = true;

        if(doorIsActivated && _playerController)
        {
            if (_outline)
            {
                _outline.OutlineColor = Controller.outlineColor;
                _outline.enabled = true;
            }
        }

    }

    public void OnAimExit(PlayerController Controller)
    {
        playerIsAiming = false;
        _playerController = null;
        if (_outline) _outline.enabled = false;
    }

    public void OnInteract(PlayerController Controller)
    {
        if(doorIsActivated && playerIsAiming && Controller && Controller._hud)
        {
            playerIsAiming = false;
            SetDoorActive(false);
            Controller.SetInputsEnabled(false);
            Controller._hud.ShowGameEndBar();

        }
    }


    private void FixedUpdate()
    {
        if (!playerIsAiming) return;

        if (_itemForActivation != string.Empty && _playerController && _playerController.GetHandleItem() == _itemForActivation)
        {
            SetDoorActive(true);
        }
        else
        {
            SetDoorActive(false);
        }


    }

}
