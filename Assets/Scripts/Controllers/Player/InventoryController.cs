using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    // Optional starting items (set in inspector)
    // [SerializeField] private List<ItemData> startingItems = new List<ItemData>();

    private bool _isInventoryOpen = false;
    public GameObject inventoryPanel;
    public GameObject Permanents_Grid;
    public GameObject Consumables_Grid;
    public GameObject Key_Grid;
    public bool hasPrisonerMask = false;
    public bool hasSlimeMask = false;
    public bool hasSkeletonMask = false;
    public bool hasKoboldMask = false;
    public GameObject PrisonerMask;
    public GameObject SlimeMask;
    public GameObject SkeletonMask;
    public GameObject KoboldMask;
    public bool hasPurpleOrb = false;
    public GameObject PurpleOrb;
    
    private static InventoryController instance;

    // public Transform slotPrefab;
    // public Sprite[] listSprites;

    // Internal storage by id
    // private readonly HashSet<string> permanentItems = new HashSet<string>();
    // private readonly Dictionary<string, int> consumableCounts = new Dictionary<string, int>();
    // private readonly HashSet<string> keyItems = new HashSet<string>();
    // private string equippedMaskId = null; // ID de la máscara equipada actualmente

    // Keep a registry of known ItemData references (for lookup/display)
    private readonly Dictionary<string, ItemData> registry = new Dictionary<string, ItemData>();

    public static InventoryController Instance { get; private set;} 
    // Events
    // public event Action<ItemData> OnItemAdded;
    // public event Action<ItemData> OnItemRemoved;

    void Update()
    { 
        ProcessKeyboardInput();
        ProcessGamepadInput();
    }

    private void ProcessKeyboardInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;
        
        if (keyboard.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }

        if (keyboard.escapeKey.wasPressedThisFrame && _isInventoryOpen)
        {
            ToggleInventory();
        }

    }

    private void ProcessGamepadInput()
    {
        Gamepad gamepad = Gamepad.current;

        if (gamepad == null)
        {
            return;
        }

        if (gamepad.yButton.wasPressedThisFrame || gamepad.triangleButton.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void Awake()
    {
        Instance = this;
        
    }

    public void ClearAll()
    {
        
    }

    public void ToggleInventory()
    {
        _isInventoryOpen = !_isInventoryOpen;
        if (_isInventoryOpen)
        {
         PrisonerMask.SetActive(hasPrisonerMask);
         SlimeMask.SetActive(hasSlimeMask);
         SkeletonMask.SetActive(hasSkeletonMask);
         KoboldMask.SetActive(hasKoboldMask);   
        }
        inventoryPanel.SetActive(_isInventoryOpen);
        
    }
}


