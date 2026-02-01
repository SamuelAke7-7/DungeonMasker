using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    // Optional starting items (set in inspector)
    [SerializeField] private List<ItemData> startingItems = new List<ItemData>();

    private bool _isInventoryOpen = false;
    public GameObject inventoryPanel;
    public GameObject Permanents_Grid;
    public GameObject Consumables_Grid;
    public GameObject Key_Grid;

    public Transform slotPrefab;
    public Sprite[] listSprites;

    // Internal storage by id
    private readonly HashSet<string> permanentItems = new HashSet<string>();
    private readonly Dictionary<string, int> consumableCounts = new Dictionary<string, int>();
    private readonly HashSet<string> keyItems = new HashSet<string>();
    private string equippedMaskId = null; // ID de la máscara equipada actualmente

    // Keep a registry of known ItemData references (for lookup/display)
    private readonly Dictionary<string, ItemData> registry = new Dictionary<string, ItemData>();

    public static InventoryController Instance { get; private set;} 
    // Events
    public event Action<ItemData> OnItemAdded;
    public event Action<ItemData> OnItemRemoved;

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

        if (keyboard.tKey.wasPressedThisFrame)//This is my test key T :)
        {
            // RemoveItem(new ItemData()
            // {
            //     id= "1"
            // });
            Debug.Log("Someone pressed the T key");
        }

    }

    private void ProcessGamepadInput()
    {
        Gamepad gamepad = Gamepad.current;

        if (gamepad.yButton.wasPressedThisFrame || gamepad.triangleButton.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void Awake()
    {
        Instance = this;
        // register starting items

        startingItems.Add(new ItemData()
        {
            id= "1",
            displayName= "PrisonerMask",
            type= ItemType.Permanent,
            icon = listSprites[0]
        });

        foreach (var it in startingItems)
        {
            if (it == null) continue;
            RegisterItem(it);
            if (it.type == ItemType.Consumable)
                AddItem(it, 1);
            else
                AddItem(it, 0);
        }

    }


    private void RegisterItem(ItemData item)
    {
        if (item == null || string.IsNullOrEmpty(item.id)) return;
        if (!registry.ContainsKey(item.id)) registry[item.id] = item;
    }

    // Adds an item. For consumables, provide amount > 0. For permanent/key amount is ignored.
    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || string.IsNullOrEmpty(item.id)) return false;
        RegisterItem(item);

        switch (item.type)
        {
            case ItemType.Permanent:
                if (permanentItems.Contains(item.id)) return false;
                permanentItems.Add(item.id);
                OnItemAdded?.Invoke(item);
                Transform slot = GameObject.Instantiate(slotPrefab,Permanents_Grid.transform);
                slot.gameObject.GetComponent<ContainerItemUseCase>().Item = item;
                return true;

            case ItemType.Consumable:
                if (amount <= 0) amount = 1;
                if (!consumableCounts.ContainsKey(item.id)) consumableCounts[item.id] = 0;
                consumableCounts[item.id] += amount;
                OnItemAdded?.Invoke(item);
                return true;

            case ItemType.Key:
                if (keyItems.Contains(item.id)) return false;
                keyItems.Add(item.id);
                OnItemAdded?.Invoke(item);
                return true;

            default:
                return false;
        }
    }

    // Removes an item. For consumables, reduces count by amount. Returns true if an item was removed.
    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null || string.IsNullOrEmpty(item.id)) return false;

        switch (item.type)
        {
            case ItemType.Permanent:
                if (!permanentItems.Remove(item.id)) return false;
                OnItemRemoved?.Invoke(item);
                foreach (Transform child in Permanents_Grid.transform)
                {
                    ContainerItemUseCase container = child.gameObject.GetComponent<ContainerItemUseCase>();
                    if (container != null && container.Item != null && container.Item.id == item.id)
                    {
                        GameObject.Destroy(child.gameObject);
                        break;
                    }
                }

                return true;

            case ItemType.Consumable:
                if (!consumableCounts.TryGetValue(item.id, out var current) || current <= 0) return false;
                current -= Math.Max(1, amount);
                if (current <= 0)
                    consumableCounts.Remove(item.id);
                else
                    consumableCounts[item.id] = current;
                OnItemRemoved?.Invoke(item);
                return true;

            case ItemType.Key:
                if (!keyItems.Remove(item.id)) return false;
                OnItemRemoved?.Invoke(item);
                return true;

            default:
                return false;
        }
    }

    public bool HasItem(ItemData item)
    {
        if (item == null || string.IsNullOrEmpty(item.id)) return false;
        switch (item.type)
        {
            case ItemType.Permanent: return permanentItems.Contains(item.id);
            case ItemType.Consumable: return consumableCounts.TryGetValue(item.id, out var c) && c > 0;
            case ItemType.Key: return keyItems.Contains(item.id);
            default: return false;
        }
    }

    public int GetConsumableCount(ItemData item)
    {
        if (item == null || string.IsNullOrEmpty(item.id)) return 0;
        consumableCounts.TryGetValue(item.id, out var c);
        return c;
    }

    // Helpers to query by id
    public bool HasItemById(string id, ItemType type)
    {
        if (string.IsNullOrEmpty(id)) return false;
        switch (type)
        {
            case ItemType.Permanent: return permanentItems.Contains(id);
            case ItemType.Consumable: return consumableCounts.TryGetValue(id, out var c) && c > 0;
            case ItemType.Key: return keyItems.Contains(id);
            default: return false;
        }
    }

    public int GetConsumableCountById(string id)
    {
        if (string.IsNullOrEmpty(id)) return 0;
        consumableCounts.TryGetValue(id, out var c);
        return c;
    }

    // Optional: get registered ItemData for display
    public ItemData GetItemData(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        registry.TryGetValue(id, out var it);
        return it;
    }

    // Clear all (useful for debugging)
    public void ClearAll()
    {
        
    }

    public void ToggleInventory()
    {
        _isInventoryOpen = !_isInventoryOpen;

        inventoryPanel.SetActive(_isInventoryOpen);

    }

    public void EquipMask(string maskId)
    {
        if (string.IsNullOrEmpty(maskId)) return;
        if (!permanentItems.Contains(maskId)) return;
        equippedMaskId = maskId;
    }
    
    public void UnequipMask()
    {
        equippedMaskId = null;
    }
    public ItemData GetEquippedMask()
    {
        if (string.IsNullOrEmpty(equippedMaskId)) return null;
        return GetItemData(equippedMaskId);
    }
}


