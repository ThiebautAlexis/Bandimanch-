using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LDJ_UIManager : MonoBehaviour
{
    /* LDJ_UIManager :
	*
	* Manages the in game UI
    * 
    * Objectives :
    *   • Core :

    *       - Select an item in the backpack
    *   
    *   • Polish :
    *       - 
    *       
    * Dones :
    *   • Core :
    *       - Open / Close the backpack
    *       - Update the backpack content
	*/

    #region Events
    // When refreshing the inventory selection
    public event Action OnRefreshInventory = null;
    // When adding an item to the inventory
    public event Action<LDJ_ObjectCharacteristics> OnAddInventoryObject = null;
    // When removing an item from the inventory
    public event Action<LDJ_ObjectCharacteristics> OnRemoveInventoryItem = null;
    // When the inventory selected in set
    public event Action<LDJ_UIInventoryItem> OnSetSelectedItem = null;

    // Event called when a menu is opened or closed
    public event Action<bool> OnMenuOpened = null;
    #endregion

    #region Fields / Accessors
    // Can the pause menu be opened or closed ?
    [SerializeField] public bool CanOpenPauseMenu { get; set; }

    [Header("Anchors :")]
    // The full inventory anchor
    [SerializeField] private GameObject fullInventoryAnchor = null;
    // The health anchor
    [SerializeField] private GameObject healthAnchor = null;

    // Death menu anchor
    [SerializeField] private GameObject deathMenuAnchor = null;
    // End game meu anchor
    [SerializeField] private GameObject endGameMenuAnchor = null;
    // End map menu anchor
    [SerializeField] private GameObject endMapMenuAnchor = null;
    // Main menu anchor
    [SerializeField] private GameObject mainMenuAnchor = null;
    // Pause menu anchor
    [SerializeField] private GameObject pauseMenuAnchor = null;

    [Header("Default Menus Buttons :")]
    // The default activated button for death menu
    [SerializeField] private Button deathMenuDefaultButton = null;
    // The default activated button for end game menu
    [SerializeField] private Button endGameMenuDefaultButton = null;
    // The default activated button for end map menu
    [SerializeField] private Button endMapMenuDefaultButton = null;
    // The default activated button for main menu
    [SerializeField] private Button mainMenuDefaultButton = null;
    // The first selected button of the pause menu
    [SerializeField] private Button pauseMenuDefaultButton = null;

    [Header("Health :")]
    // The current health of the player
    [SerializeField] private List<GameObject> playerHealthHearts = new List<GameObject>();

    [Header("Inventory Items :")]
    // All UI inventory items
    [SerializeField] private List<LDJ_UIInventoryItem> inventoryItems = new List<LDJ_UIInventoryItem>();
    // The index of the current selected item
    [SerializeField] private int selectedItemIndex = 0;
    public int SelectedItemIndex
    {
        get { return selectedItemIndex; }
        set
        {
            if (value < 0) value = inventoryItems.Count - 1;
            if (value > inventoryItems.Count - 1) value = 0;

            selectedItemIndex = value;
        }
    }
    public LDJ_UIInventoryItem GetSelectedItem
    {
        get { return inventoryItems[selectedItemIndex]; }
    }
    // The distance of the UI items elements from the anchor
    [SerializeField] private int itemDistanceFromAnchor = 200;

    [Header("Event System :")]
    // The event system of the canvas
    [SerializeField] private EventSystem eventSystem = null;
    #endregion

    #region Singleton
    // The singleton instance of this script
    [SerializeField] public static  LDJ_UIManager Instance = null;
    #endregion

    #region Methods
    #region Original Methods
    #region Health
    /// <summary>
    /// Set the health of the player in UI
    /// </summary>
    /// <param name="_health"></param>
    public void SetHealth(int _health)
    {
        // Get the amount of health to change
        int _difference = _health - playerHealthHearts.Count;

        // If hearts should be added
        if (_difference > 0)
        {
            for (int _i = 0; _i < _difference; _i++)
            {
                playerHealthHearts.Add(Instantiate(Resources.Load<GameObject>("Health"), healthAnchor.transform, true));
            }
        }
        // If heart should be removed
        else
        {
            for (int _i = 0; _i < -_difference; _i++)
            {
                GameObject _destroy = playerHealthHearts.Last();
                playerHealthHearts.Remove(_destroy);

                Destroy(_destroy);
            }
        }
    }
    #endregion

    #region Inventory
    /// <summary>
    /// Get the selected inventory item
    /// </summary>
    /// <param name="_doIncrease">Do increase or decrease the inventory selected item ?</param>
    /// <returns>Returns the selected item</returns>
    public LDJ_ObjectCharacteristics ScrollSelectedItem(bool _doIncrease)
    {
        // If not having item, return
        if (inventoryItems.Count == 0) return null;

        if (_doIncrease) SelectedItemIndex++;
        else SelectedItemIndex--;

        // Get the selected item and refresh
        LDJ_UIInventoryItem _selected = inventoryItems[selectedItemIndex];
        RefreshInventory();

        // Activate the event
        OnSetSelectedItem?.Invoke(_selected);

        // Returns its first object
        return _selected.ObjectsReferences[0];
    }

    /// <summary>
    /// Refreshes the inventory content
    /// </summary>
    /// <param name="_object">New object in the inventory</param>
    public void AddInventoryItem(LDJ_ObjectCharacteristics _object)
    {
        // If there is already an inventory item of the same type not full, add it to it
        LDJ_UIInventoryItem _inventoryItem = null;

        if ((_inventoryItem = inventoryItems.Where(i => i.ObjectsReferences[0].ResourcePrefabName == _object.ResourcePrefabName && i.ObjectsReferences.Count <= i.ObjectsReferences[0].MaxStackAmount).FirstOrDefault()) != null)
        {
            _inventoryItem.AddItem(_object);
            return;
        }

        // If not inventory item existing contain the same type, instantiate the new object UI element and initializes it
        LDJ_UIInventoryItem _newItem = Instantiate(Resources.Load<LDJ_UIInventoryItem>("InventoryItem"), fullInventoryAnchor.transform, false);
        _newItem.Init(_object);

        // Add the instantiated object to the list
        inventoryItems.Add(_newItem);

        // Activate the event
        OnAddInventoryObject?.Invoke(_object);

        // Refresh the inventory
        RefreshInventory();
    }

    /// <summary>
    /// Refresh the inventory selection
    /// </summary>
    public void RefreshInventory()
    {
        // Set the disposition of each element in the UI
        int _counter = selectedItemIndex;

        for (int _i = 0; _i < inventoryItems.Count; _i++)
        {
            float _theta = (2 * Mathf.PI / inventoryItems.Count) * _i;
            inventoryItems[_counter].transform.localPosition = new Vector3(Mathf.Sin(_theta), Mathf.Cos(_theta), 0f) * itemDistanceFromAnchor;

            _counter++;
            if (_counter > inventoryItems.Count - 1) _counter = 0;
        }

        // Activate the event
        OnRefreshInventory?.Invoke();
    }

    /// <summary>
    /// Removes an object from the inventory
    /// </summary>
    /// <param name="_object">Object reference to remove</param>
    /// <returns>Returns true if the stack is empty, false otherwise</returns>
    public LDJ_ObjectCharacteristics RemoveInventoryItem(LDJ_ObjectCharacteristics _object)
    {
        // If there is already an inventory item of the same type not full, add it to it
        LDJ_UIInventoryItem _inventoryItem = null;
        if (_inventoryItem = inventoryItems.Where(i => i.ObjectsReferences.Contains(_object)).First())
        {
            // Is this the selected item ?
            bool _isSelectedItem = inventoryItems[selectedItemIndex].ObjectsReferences.SequenceEqual(_inventoryItem.ObjectsReferences);

            // Removes an element, and destroy it if needed
            if (_inventoryItem.RemoveItem(_object))
            {
                // Removes the element from the list & destroy the object
                inventoryItems.Remove(_inventoryItem);
                Destroy(_inventoryItem.gameObject);

                // Activate the event
                OnRemoveInventoryItem?.Invoke(_object);

                if (_isSelectedItem) SelectedItemIndex = 0;

                // Refresh the inventory
                RefreshInventory();

                // If this stack is empty, returns the first element of the last one
                if (inventoryItems.Count > 0)
                {
                    return inventoryItems[0].ObjectsReferences[0];
                }
                else
                {
                    return null;
                }
            }
        }
        return _inventoryItem.ObjectsReferences.Last();
    }
    #endregion

    #region Menu
    /// <summary>
    /// Opens or closes the death menu
    /// </summary>
    public void DeathMenu()
    {
        // Get if the menu should be opened or closed
        bool _doOpen = !deathMenuAnchor.activeInHierarchy;

        // Active the menu
        deathMenuAnchor.SetActive(_doOpen);
        healthAnchor.SetActive(!_doOpen);

        // Set the cursor
        Cursor.visible = _doOpen;

        // Set the default button as activated
        if (_doOpen)
        {
            eventSystem.SetSelectedGameObject(deathMenuDefaultButton.gameObject);
        }
        else
        {
            eventSystem.SetSelectedGameObject(null);
        }

        // Set if the pause menu can be opened or closed
        CanOpenPauseMenu = !_doOpen;

        // Calls the menu event
        OnMenuOpened?.Invoke(_doOpen);
    }

    /// <summary>
    /// Opens or closes the end of the game menu
    /// </summary>
    public void EndGameMenu()
    {
        // Get if the menu should be opened or closed
        bool _doOpen = !endGameMenuAnchor.activeInHierarchy;

        // Active the menu
        endGameMenuAnchor.SetActive(_doOpen);
        healthAnchor.SetActive(!_doOpen);

        // Set the cursor
        Cursor.visible = _doOpen;

        // Set the default button as activated
        if (_doOpen)
        {
            eventSystem.SetSelectedGameObject(endGameMenuDefaultButton.gameObject);
        }
        else
        {
            eventSystem.SetSelectedGameObject(null);
        }

        // Set if the pause menu can be opened or closed
        CanOpenPauseMenu = !_doOpen;

        // Calls the menu event
        OnMenuOpened?.Invoke(_doOpen);
    }

    /// <summary>
    /// Opens or closes the end map menu
    /// </summary>
    public void EndMapMenu()
    {
        // Get if the menu should be opened or closed
        bool _doOpen = !endMapMenuAnchor.activeInHierarchy;

        // Active the menu
        endMapMenuAnchor.SetActive(_doOpen);
        healthAnchor.SetActive(!_doOpen);

        // Set the cursor
        Cursor.visible = _doOpen;

        // Set the default button as activated
        if (_doOpen)
        {
            eventSystem.SetSelectedGameObject(endMapMenuDefaultButton.gameObject);
        }
        else
        {
            eventSystem.SetSelectedGameObject(null);
        }

        // Set if the pause menu can be opened or closed
        CanOpenPauseMenu = !_doOpen;

        // Calls the menu event
        OnMenuOpened?.Invoke(_doOpen);
    }

    /// <summary>
    /// Opens or closes the main menu
    /// </summary>
    public void MainMenu()
    {
        // Get if the menu should be opened or closed
        bool _doOpen = !mainMenuAnchor.activeInHierarchy;

        // Active the menu
        mainMenuAnchor.SetActive(_doOpen);
        healthAnchor.SetActive(!_doOpen);

        // Set the cursor
        Cursor.visible = _doOpen;

        // Set the default button as activated
        if (_doOpen)
        {
            eventSystem.SetSelectedGameObject(mainMenuDefaultButton.gameObject);
        }
        else
        {
            eventSystem.SetSelectedGameObject(null);
        }

        // Set if the pause menu can be opened or closed
        CanOpenPauseMenu = !_doOpen;

        // Calls the menu event
        OnMenuOpened?.Invoke(_doOpen);
    }

    /// <summary>
    /// Opens or closes the pause menu
    /// </summary>
    public void PauseMenu()
    {
        // If the pause menu cannot be opened or closed, return
        if (!CanOpenPauseMenu) return;

        // Get if the menu should be opened or closed
        bool _doOpen = !pauseMenuAnchor.activeInHierarchy;

        // Active the menu
        pauseMenuAnchor.SetActive(_doOpen);

        // Set the cursor
        Cursor.visible = _doOpen;

        // Set the default button as activated
        if (_doOpen)
        {
            eventSystem.SetSelectedGameObject(pauseMenuDefaultButton.gameObject);
        }
        else
        {
            eventSystem.SetSelectedGameObject(null);
        }

        // Calls the menu event
        OnMenuOpened?.Invoke(_doOpen);
    }
    #endregion
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Set the singleton instance as this if there is none, or destroy this
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        // If there is no event system reference, get it in the scene
        if (!eventSystem)
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }
    }

    private void OnDestroy()
    {
        // Remove the static instance if this is this
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
	}
    #endregion
    #endregion
}
