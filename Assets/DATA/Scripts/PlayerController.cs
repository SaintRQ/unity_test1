using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 5f;
    [SerializeField]
    float mouseSensitivity = 2f;

    [SerializeField]
    LayerMask interactLayerMask;
    [SerializeField]
    float interactDistance = 3f;
    [SerializeField]
    float dropForce = 5f;

    
    public Color outlineColor = Color.white;

    //----------------------------------------------  
    private Camera _playerCamera = null;
    private CharacterController _characterController = null;
    private Inventory _inventory = null;
    public HUDManager _hud { get; private set; } = null;

    private IInteract _lastAimTarget = null;

    public bool controllerIsInitialized { get; private set; } = false;
    private void InitializeController()
    {

        _playerCamera = GetComponentInChildren<Camera>();
        _characterController = GetComponentInChildren<CharacterController>();
        _inventory = GetComponentInChildren<Inventory>();

        _hud = FindObjectOfType<HUDManager>();


        if (_playerCamera && _characterController && _inventory && _hud) 
        {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            controllerIsInitialized = true;

            SetInputsEnabled(true);
            Debug.Log("Controller is initialized");
        } 

        else Debug.LogError("Controller is not initialized");     
    }

  //----------------------------------------------
    public void SetInputsEnabled(bool newState)
    {
        inputsIsEnabled = newState;
    }
    private void Gravity()
    {
        if (_characterController.isGrounded)
        {
            verticalVelocity = -2f; 
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime; 
        }

        _characterController.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }

    public bool inputsIsEnabled { get; private set; } = false;
    private float gravity = 9.81f;
    private float verticalVelocity = 0f;
    private float _rotationX = 0f;

  //----------------------------------------------

    private void HandleMovement()
    {
        float moveForward = 0f;
        float moveRight = 0f;

        if (Input.GetKey(KeyCode.W)) moveForward = 1f;
        if (Input.GetKey(KeyCode.S)) moveForward = -1f;
        if (Input.GetKey(KeyCode.A)) moveRight = -1f;
        if (Input.GetKey(KeyCode.D)) moveRight = 1f;

        Vector3 move = transform.right * moveRight + transform.forward * moveForward;
        _characterController.Move(move * moveSpeed * Time.deltaTime);
    }
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

        _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    private void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E) && _lastAimTarget != null) _lastAimTarget.OnInteract(this);
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            int currentItemIndex = _hud.lastSelectedSlot;        
            DropItem(_inventory.GetItemData(currentItemIndex).id);

        }
    }
    private void HandleInteractTrace() 
    {
        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
        RaycastHit hit;
        IInteract interactable = null;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayerMask))
        {
            interactable = hit.collider.transform.GetComponent<IInteract>();

            if (interactable != null)
            {
                if (interactable == _lastAimTarget) return;
                else 
                {
                    if(_lastAimTarget != null) _lastAimTarget.OnAimExit(this);
                    _lastAimTarget = interactable;
                    _lastAimTarget.OnAimEnter(this);
                }
    
            }
            else 
            {
                if(_lastAimTarget != null)
                {
                    _lastAimTarget.OnAimExit(this);
                    _lastAimTarget = null;
                }
            }
        }
        else 
        {
            if (_lastAimTarget != null)
            {
                _lastAimTarget.OnAimExit(this);
                _lastAimTarget = null;
            }
        }
    }
    private void HandleSlotSelection() 
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            _hud.SelectSlot(_hud.lastSelectedSlot + 1);
        }
        else if (scroll < 0f)
        {
            _hud.SelectSlot(_hud.lastSelectedSlot - 1);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)) _hud.SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) _hud.SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) _hud.SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) _hud.SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) _hud.SelectSlot(4);
    }


    public bool TakeItem(string ItemID)
    { 
        if(controllerIsInitialized && inputsIsEnabled) 
        { 
            return _inventory.AddItem(ItemID);
        }
        return false;
    }
    public bool DropItem(string ItemID)
    {
        if(controllerIsInitialized && inputsIsEnabled) 
        {
            if (_inventory.RemoveItem(ItemID))
            {
                var itemData = _inventory.GetItemData(ItemID);
                if (itemData.IsValid)
                {
                    GameObject item = Instantiate(itemData.asset, FindDropLocation(), Quaternion.identity);
                    Rigidbody rb = null;

                    if (item && item.TryGetComponent(out rb)) 
                    {
                        rb.AddForce(_playerCamera.transform.forward * rb.mass * dropForce, ForceMode.Impulse);
                        return true;
                    }                   
                }
            }

        }

        return false;
    }

    public string GetHandleItem()
    {
        if (controllerIsInitialized && inputsIsEnabled)
        {
            return _inventory.GetItemData(_hud.lastSelectedSlot).id;
        }
        return string.Empty;
    }

    //----------------------------------------------


    private Vector3 FindDropLocation() 
    {
        return _playerCamera.transform.position + _playerCamera.transform.forward * (_characterController.radius * 1.1f);      
    } 


    void Start()
    {
        InitializeController();
    }

    void Update()
    {

        if (!controllerIsInitialized) return;

        Gravity();

        if (!inputsIsEnabled) return;
        HandleMovement();
        HandleMouseLook();
        HandleInteractTrace();
        HandleInteract();
        HandleSlotSelection();

    }

}
