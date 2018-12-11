using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum UpgradeType
{
    Health,
    Inventory,
    Speed,
    Weight
}

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class LDJ_Player : MonoBehaviour
{
    /* LDJ_CharacterController :
	*
	* [Description]
    * 
    * Objectives :
    *   • Core :
    *       - 
    *   
    *   • Polish :
    *       - Make the speed of the character increase before being at its maximum value
    *       
    * Dones :
    *   • Core :
    *       - Move the player in the area
    *       - Grab objects on ground
    *       - Manages the inventory
    *       - Throw objects behind or in front of the player
    *       - Fire camp system
    *       - Upgrade statistics
	*/

    #region Events
    // When dying
    public UnityEvent OnDied = null;
    // When grab object
    public UnityEvent OnGrabObject = null;
    // When taking damages
    public UnityEvent OnTakeDamage = null;
    // When throwing object
    public UnityEvent OnThrowObject = null;
    #endregion

    #region Fields / Accessors
    #region Debug, Detection & Others
    [Header("# Debug, Detection & Others #")]
    #endregion

    #region State
    [Header("State :")]
    // Is the character currently controllable by the player ?
    [SerializeField] private bool isControllable = true;
    public bool IsControllable
    {
        get { return isControllable; }
    }
    // Is the character curretnyl invulnerable ?
    [SerializeField] private bool isInvulnerable = false;
    public bool IsInvulnerable
    {
        get { return isInvulnerable; }
    }
    #endregion

    #region Statistics
    [Header("Statistics :")]
    // The speed movement of the character
    [SerializeField] private float speed = 1;
    public float Speed
    {
        get { return speed; }
    }
    // The force used to throw objects
    [SerializeField] private float throwForceVelocity = 1;
    public float ThrowForceVelocity
    {
        get { return throwForceVelocity; }
    }

    // The currency amount the character has
    [SerializeField] private int currency = 0;
    public int Currency
    {
        get { return currency; }
    }

    // The current health of the character
    [SerializeField] private int health = 3;
    public int Health
    {
        get { return health; }
        private set
        {
            value = Mathf.Clamp(value, 0, maxHealth);

            LDJ_UIManager.Instance.SetHealth(value);
            LDJ_CameraBehaviour.Instance.ScreenShake();

            if (value == 0)
            {
                StartCoroutine(Death());
            }

            health = value;
        }
    }
    // The time the character is invulnerable after hit
    [SerializeField] private float invulnerabilityTime = 1f;
    public float InvulnerabilityTime
    {
        get { return invulnerabilityTime; }
    }
    // The current amount of obejct in the character's backpack
    public int ObjectAmount
    {
        get { return objects.Count; }
    }
    // The strength of the character
    [SerializeField] private int strength = 1;
    public int Strength
    {
        get { return strength; }
    }
    // The current weight of the character's backpack
    [SerializeField] private int weight = 0;
    public int Weight
    {
        get { return weight; }
    }
    #endregion

    #region Statistics Limits
    [Header("Statistics Limits :")]
    // The maximum health of the player
    [SerializeField] private int maxHealth = 100;
    public int MaxHealth
    {
        get { return maxHealth; }
    }
    // The maximum amount of object the character can have in his backpack
    [SerializeField] private int maxObjectAmount = 12;
    public int MaxObjectAmount
    {
        get { return maxObjectAmount; }
    }
    // The maximum weight of the character's backpack
    [SerializeField] private int maxWeight = 50;
    public int MaxWeight
    {
        get { return maxWeight; }
    }
    #endregion

    #region Inventory
    [Header("Inventory :")]
    // List of all objects the player possesses
    [SerializeField] private List<LDJ_ObjectCharacteristics> objects = new List<LDJ_ObjectCharacteristics>();
    // The actually selected object in the list
    [SerializeField] private LDJ_ObjectCharacteristics selectedObject = null;
    public LDJ_ObjectCharacteristics SelectedObject
    {
        get { return selectedObject; }
    }
    #endregion

    #region Components & References
    [Header("Components & References :")]
    // The animator of the character
    [SerializeField] private Animator animator = null;
    // The box collider of the character
    [SerializeField] private new BoxCollider collider = null;
    // The half extents of the collider to use in overlap
    [SerializeField] private Vector3 halfExtents = Vector3.one;
    // What layer the object collides
    [SerializeField] private LayerMask whatCollide = new LayerMask();
    // All the character materials
    [SerializeField] private List<Material> materials = new List<Material>();
    // The rigidbody of the character
    [SerializeField] private new Rigidbody rigidbody = null;
    // The sprite of the aims cursos
    [SerializeField] private SpriteRenderer aimsCursor = null;
    #endregion

    #region Detection Box
    [Header("Detection Box :")]
    // Extents of the detection box
    [SerializeField] Vector3 detectionBoxExtents = Vector3.one;
    // What the character detect to grab
    [SerializeField] private LayerMask whatDetect = new LayerMask();
    #endregion

    #region Aims Look
    [Header("Aims Look :")]
    // The X & Y look movement of the player
    [SerializeField] private float lookX = 0;
    [SerializeField] private float lookY = 0;
    #endregion

    #region Input Names
    [Header("Input Names :")]
    // Inputs for the horizontal & vertical movements
    [SerializeField] private string horizontalInput = "Horizontal";
    [SerializeField] private string verticalInput = "Vertical";

    // Inputs to grab and throw objects
    [SerializeField] private string interactInput = "Interact";
    [SerializeField] private string throwInput = "Throw";
    [SerializeField] private AxisToInput throwAxisToInput = new AxisToInput("Throw Axis");

    // Submit & cancel inputs
    [SerializeField] private string submitInput = "Submit";
    [SerializeField] private string cancelInput = "Cancel";

    // Menus & Inventory inputs
    [SerializeField] private string inventoryPlusInput = "Inventory +";
    [SerializeField] private string inventoryMinusInput = "Inventory -";
    [SerializeField] private string inventoryScrollAxis = "Inventory Scroll";
    [SerializeField] private string menuInput = "Menu";

    // The X & Y player's look movement inputs
    [SerializeField] private string lookXInput = "Look X";
    [SerializeField] private string lookYInput = "Look Y";
    #endregion
    #endregion

    #region Singleton
    // The singleton instance of this script
    public static LDJ_Player Instance = null;
    #endregion

    #region Methods
    #region Original Methods
    // Death of the player
    private IEnumerator Death()
    {
        isControllable = false;
        OnDied?.Invoke();
        animator.SetTrigger("Death");

        yield return new WaitForSeconds(2.5f);

        LDJ_UIManager.Instance.DeathMenu();

        // Sound
        // AkSoundEngine.PostEvent("Stop_Footsetps", gameObject);
        //AkSoundEngine.PostEvent("Play_Die", gameObject);
    }

    // Make the player eat all the food
    public void Eat()
    {
        // Get all cookers and food items
        LDJ_ObjectCharacteristics[] _cookers = objects.Where(o => o.ObjectType == ObjectType.Cooker).ToArray();
        LDJ_ObjectCharacteristics[] _foods = objects.Where(o => o.ObjectType == ObjectType.Food).ToArray();

        // Get the total cooker coefficient & food total restore force
        float _cookerCoef = 0;
        int _foodPower = 0;

        foreach (LDJ_ObjectCharacteristics _cooker in _cookers)
        {
            // Increase the cooker coef, then remove the element from inventory
            _cookerCoef += _cooker.CookerPower;

            // Remove the thrown object from the list
            objects.Remove(_cooker);

            // Set the weight
            weight -= _cooker.Weight;

            // Get the new selected object
            LDJ_UIManager.Instance.RemoveInventoryItem(_cooker);

            // Destroy the removed object
            if (_cooker.Instance)
            {
                Destroy(_cooker.Instance.gameObject);
            }
        }
        foreach (LDJ_ObjectCharacteristics _food in _foods)
        {
            // Increase thefood powa, then remove the element from inventory
            _foodPower += _food.FoodRestoration;

            // Remove the thrown object from the list
            objects.Remove(_food);

            // Set the weight
            weight -= _food.Weight;

            // Get the new selected object
            LDJ_UIManager.Instance.RemoveInventoryItem(_food);

            // Destroy the removed object
            if (_food.Instance)
            {
                Destroy(_food.Instance.gameObject);
            }
        }

        selectedObject = LDJ_UIManager.Instance.GetSelectedItem.ObjectsReferences[0];

        // Sound
        //AkSoundEngine.PostEvent("Play_Eat", gameObject);

        // Get the total restoration amount
        Health += Mathf.RoundToInt(_cookerCoef) * _foodPower;
    }

    // Makes the character interact with the nearest object in range
    private void InteractObject()
    {
        // Grab the nearest object in range if there is one, and add it to the inventory
        Collider[] _inRange = new Collider[] { };

        // Get game objects in range
        if ((_inRange = Physics.OverlapBox(transform.position, detectionBoxExtents, Quaternion.identity, whatDetect)).Length > 0)
        {
            // Order all objects in range
            _inRange.OrderBy(o => Vector3.Distance(transform.position, o.transform.position));

            // Get the nearest meat tree if it is
            LDJ_MeatTree _nearestTree = _inRange.Select(n => n.GetComponent<LDJ_MeatTree>()).Where(n => n != null && n.HasMeat).FirstOrDefault();

            // Get the nearest chest if it is
            LDJ_ChestMecanic _nearestChest = _inRange.Select(n => n.GetComponent<LDJ_ChestMecanic>()).Where(n => n != null && !n.IsOpen).FirstOrDefault();

            // Get the nearest object if it is
            LDJ_Object _nearestObject = _inRange.Select(o => o.GetComponent<LDJ_Object>()).Where(o => o != null && o.CanBeGrabbed).FirstOrDefault();

            // If there is a tree around
            if (_nearestTree)
            {
                objects.Add(_nearestTree.Ham);

                weight += _nearestTree.Ham.Weight;

                LDJ_UIManager.Instance.AddInventoryItem(_nearestTree.TakeMeat());

                // Sound
                //AkSoundEngine.PostEvent("Play_ObjectPickup", gameObject);

                // If not having a selected object yet, take this one
                if (objects.Count == 1)
                {
                    selectedObject = _nearestTree.Ham;
                }
                return;
            }
            // If here is a chest around
            if (_nearestChest)
            {
                _nearestChest.MakeInteraction();
                return;
            }

            // If there is an object around
            else if (_nearestObject)
            {
                // If the object cannot be grabbed, return
                if ((weight + _nearestObject.ObjectCharacteristics.Weight > maxWeight) || ObjectAmount == maxObjectAmount)
                {
                    return;
                }

                // Grab it, and disable it
                _nearestObject.gameObject.SetActive(false);
                objects.Add(_nearestObject.ObjectCharacteristics);

                // Set the weight
                weight += _nearestObject.ObjectCharacteristics.Weight;

                // Activate the UI linked method
                LDJ_UIManager.Instance.AddInventoryItem(_nearestObject.ObjectCharacteristics);

                // Sound
                //AkSoundEngine.PostEvent("Play_ObjectPickup", gameObject);

                // If not having a selected object yet, take this one
                if (objects.Count == 1)
                {
                    selectedObject = _nearestObject.ObjectCharacteristics;
                }
            }
        }
    }

    // Checks the player's inputs and executes actions in consequences
    private void InputActions()
    {
        // If the player is dead, return
        if (health == 0) return;

        // (Des)Activate the menu
        if (Input.GetButtonDown(menuInput))
        {
            LDJ_UIManager.Instance.PauseMenu();
            return;
        }

        // Returns if the character isn't controllable
        if (!isControllable) return;

        // Get the horizontal (X axis) & vertical (Z axis) movements of the player
        float _horizontal = Input.GetAxis(horizontalInput);
        float _vertical = Input.GetAxis(verticalInput);

        // Makes the player move in the direction of the horizontal & vertical movement if this movement isn't null
        Vector3 _direction = new Vector3(_horizontal, 0, _vertical);
        if (_direction != Vector3.zero)
        {
            Move(_direction);

            if (!animator.GetBool("IsRunning"))
            {
                // Sound
                //AkSoundEngine.PostEvent("Play_Footsteps", gameObject);
            }

            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        // Check to grab object
        if (Input.GetButtonDown(interactInput))
        {
            InteractObject();
        }

        // Try to throw selected object
        else if (Input.GetButtonDown(throwInput) || (throwAxisToInput.Convert() != 0))
        {
            ThrowObject();
        }

        // Scroll in the inventory
        float _inventoryScroll = Input.GetAxis(inventoryScrollAxis);

        if (Input.GetButtonDown(inventoryPlusInput) || _inventoryScroll > 0)
        {
            selectedObject = LDJ_UIManager.Instance.ScrollSelectedItem(true);
        }
        else if (Input.GetButtonDown(inventoryMinusInput) || _inventoryScroll < 0)
        {
            selectedObject = LDJ_UIManager.Instance.ScrollSelectedItem(false);
        }

        // If a goblin is near the player position
        if (false)
        {

        }
        else
        {
            // Get the horizontal & vertical movement of the player look
            float _lookX = Input.GetAxis(lookXInput);
            float _lookY = Input.GetAxis(lookYInput);

            // Let the player aims with the look movement
            // Set the look movement in X & Y
            _lookX = Mathf.Abs(_lookX) >= .05f ? _lookX : 0;
            _lookY = Mathf.Abs(_lookY) >= .05f ? _lookY : 0;

            // If the movement has changed, update the aims cursor position
            if ((lookX != _lookX || _lookY != lookY) && (new Vector2(_lookX, _lookY) != Vector2.zero))
            {
                lookX = _lookX;
                lookY = _lookY;
            }
        }

        // Set the position of the aims cursor
        aimsCursor.transform.position = Vector3.Lerp(aimsCursor.transform.position, transform.position + (new Vector3(lookX, .25f, lookY).normalized * 5), Time.deltaTime * 5);
    }

    // Makes the player move in a given direction
    private void Move(Vector3 _direction)
    {
        // Get the new desired positon
        Vector3 _newPosition = Vector3.Lerp(transform.position, transform.position + _direction, Time.deltaTime * (speed - (speed / ((maxWeight * 1.75f) / (weight <= 1 ? 1 : weight)))));

        // If there is nothing that bother the character's movement, move him to the desired direction
        if (Physics.OverlapBox(new Vector3(_newPosition.x, transform.position.y, transform.position.z) + collider.center, halfExtents, collider.transform.rotation, whatCollide, QueryTriggerInteraction.Ignore).Length > 0)
        {
            _newPosition = new Vector3(transform.position.x, _newPosition.y, _newPosition.z);
        }
        if (Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y, _newPosition.z) + collider.center, halfExtents, collider.transform.rotation, whatCollide, QueryTriggerInteraction.Ignore).Length > 0)
        {
            _newPosition = new Vector3(_newPosition.x, _newPosition.y, transform.position.z);
        }

        // Makes the character look the direction he's going
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_direction), Time.deltaTime * 5f);

        // Moves the character
        transform.position = _newPosition;
    }

    /// <summary>
    /// Sell all the character inventory
    /// </summary>
    public void SellInventory()
    {
        // Get the currency for all objects, then remove them
        foreach (LDJ_ObjectCharacteristics _object in objects)
        {
            // Set the weight
            weight -= _object.Weight;

            // Get the currency
            currency += _object.CurrencyValue;

            // Get the new selected object
            LDJ_ObjectCharacteristics _newSelected = LDJ_UIManager.Instance.RemoveInventoryItem(_object);
            selectedObject = _newSelected;

            // Destroy the removed object
            if (_object.Instance)
            {
                Destroy(_object.Instance.gameObject);
            }
        }

        // Clear the inventory
        objects.Clear();

        Debug.Log("Sell Inventory !");
    }

    // Set if the player is controllable or not when a menu is opened or closed
    private void SetControllableAfterMenu(bool _doOpenMenu)
    {
        // If the player is alive, set its control
        if (health > 0)
        {
            isControllable = !_doOpenMenu;
        }
    }

    // Set the player invulnerable during a certain amount of time
    private IEnumerator SetInvulnerability()
    {
        // Creates the invulnerability timer & enable this one
        float _timer = 0;
        isInvulnerable = true;

        // Set the character flash between red & white every 0.1 seconds
        while (_timer < invulnerabilityTime)
        {
            foreach (Material _material in materials)
            {
                _material.color = Color.red;
            }

            yield return new WaitForSeconds(.1f);

            _timer += Time.deltaTime;

            foreach (Material _material in materials)
            {
                _material.color = Color.white;
            }

            yield return new WaitForSeconds(.1f);

            _timer += Time.deltaTime;
        }

        // Set invulnerability off
        isInvulnerable = false;
    }

    // Makes the character throw the selected object
    private void ThrowObject()
    {
        // Throw the selected objet if there is one
        if (selectedObject != null)
        {
            // If the instance is null, instantiate it before throwing
            if (selectedObject.Instance == null)
            {
                // If there is no instance and nothing to instantiate, return
                if(selectedObject.ResourcePrefabName == string.Empty)
                {
                    return;
                }

                // Else, instantiate the element
                selectedObject.Instance = Instantiate(Resources.Load<LDJ_Object>(selectedObject.ResourcePrefabName));
            }
            // Enable it, set its position to the character one and give it velocity
            selectedObject.Instance.Throw(transform.position, (new Vector3(lookX, 0, lookY).normalized * throwForceVelocity) + (Vector3.up * throwForceVelocity / 10), strength);

            // Set the rotation of the object
            selectedObject.Instance.transform.forward = new Vector3(lookX, 0, lookY);

            // Remove the thrown object from the list
            objects.Remove(selectedObject);

            // Set the weight
            weight -= selectedObject.Weight;

            // Get the new selected object
            LDJ_ObjectCharacteristics _newSelected = LDJ_UIManager.Instance.RemoveInventoryItem(selectedObject);
            selectedObject = _newSelected;

            // Animator set
            animator.SetTrigger("Throw");

            transform.forward = new Vector3(lookX, 0, lookY);
        }
    }

    /// <summary>
    /// Maeks the player take damages
    /// </summary>
    public void TakeDamage()
    {
        // If the player shouldn't be hit, return
        if (isInvulnerable || !isControllable) return;

        // Decreases the health
        Health--;

        // Event call
        OnTakeDamage?.Invoke();

        if (!isControllable) return;

        // Animator set
        animator.SetTrigger("Hurt");

        // Sound
        //AkSoundEngine.PostEvent("Play_Hurt", gameObject);

        // Start the invulnerability coroutine
        StartCoroutine(SetInvulnerability());
    }

    /// <summary>
    /// Upgrade the player statistics on a given type
    /// </summary>
    /// <param name="_type">Statistic type to upgrade</param>
    public void Upgrade(UpgradeType _type, int _price)
    {
        currency -= _price;

        switch (_type)
        {
            case UpgradeType.Health:
                maxHealth++;
                health = maxHealth;
                break;
            case UpgradeType.Inventory:
                maxObjectAmount++;
                break;
            case UpgradeType.Speed:
                speed++;
                break;
            case UpgradeType.Weight:
                maxWeight++;
                break;
            default:
                break;
        }

        Debug.Log("Upgrade => " + _type);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Sert this as singleton or destroy this
        if (!Instance) Instance = this;
        else Destroy(this);

        // Get the required component references if not having them
        if (!collider) collider = GetComponent<BoxCollider>();
        if (!rigidbody) rigidbody = GetComponent<Rigidbody>();
        if (!aimsCursor) aimsCursor = GetComponentInChildren<SpriteRenderer>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        // Get alls materials on the character
        if (materials.Count == 0)
        {
            materials = GetComponentsInChildren<SkinnedMeshRenderer>().Select(m => m.material).ToList();
            materials.AddRange(GetComponentsInChildren<MeshRenderer>().Select(m => m.material).ToList());
        }

        // Get the half extents of the box collider to use in overlap
        halfExtents = Vector3.Scale(collider.size, collider.transform.lossyScale) / 2;
    }

    private void OnDestroy()
    {
        // Remove the static instance if this is this
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmos()
    {
        // Draw the gizmo arround the detection box
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, detectionBoxExtents * 2);
        Gizmos.color = Color.white;
    }

    // Use this for initialization
    void Start ()
    {
        // Set the camera target as this transform
        if (!LDJ_CameraBehaviour.Instance || !LDJ_UIManager.Instance) return; 
        LDJ_CameraBehaviour.Instance.SetTarget(transform);

        // Set the player's health
        LDJ_UIManager.Instance.SetHealth(Health);

        // Set the aims cursor orientation & position
        aimsCursor.transform.forward = Camera.main.transform.forward;
        lookY = -1;

        // Lock the cursor inside the game window
        Cursor.lockState = CursorLockMode.Confined;

        // Set the player as non controllable when a menu is open
        LDJ_UIManager.Instance.OnMenuOpened += SetControllableAfterMenu;

        // Opens the main menu on start
        LDJ_UIManager.Instance.MainMenu();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Executes actions depending on player's inputs
        InputActions();
	}
    #endregion
    #endregion
}

public class AxisToInput
{
    /* AxisToInput :
     * 
     * Class to convert an axis into an input
    */

    #region Fields / Accessors
    // The name of the axis to convert
    public string AxisName
    {
        get; private set;
    }

    // The last value of the input
    public int LastValue
    {
        get; private set;
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new axis to input converter
    /// </summary>
    /// <param name="_axisName">Name of the axis to convert</param>
    public AxisToInput(string _axisName)
    {
        AxisName = _axisName;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Converts the axis to an input
    /// </summary>
    /// <returns>Returns 0 if the axis wasn't pressed or if its value is the same as previously ; returns 1 or -1 otherwise depending on the axis value</returns>
    public int Convert()
    {
        // Get the value of the axis
        int _value = Mathf.RoundToInt(Input.GetAxis(AxisName));

        // If the value is different from the last one, set it as last and returns it
        if (_value != LastValue)
        {
            LastValue = _value;

            return _value;
        }
        // Else, returns 0
        else
        {
            return 0;
        }
    }
    #endregion
}
