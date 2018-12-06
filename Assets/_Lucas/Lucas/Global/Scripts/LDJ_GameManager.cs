using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LDJ_GameManager : MonoBehaviour
{
    /* LDJ_GameManager :
	*
	* Manages all the game behaviour
    * 
    * Objectives :
    *   • Core :
    *       - 
    *   
    *   • Polish :
    *       - 
    *       
    * Dones :
    *   • ... Empty
	*/

    #region Events

    #endregion

    #region Fields / Accessors
    // The canvas of the game
    [SerializeField] private Canvas canvas = null;

    // The event system of the scene
    [SerializeField] private EventSystem eventSystem = null;
    public EventSystem EventSystem
    {
        get { return eventSystem; }
    }

    // The last selected game object
    [SerializeField] private GameObject lastSelected = null;

    // The current Player of the scene
    [SerializeField] private LDJ_Player player = null;

    [SerializeField] private Button randomUpgradeObject = null;
    [SerializeField] private Button specificUpgradeObject = null;

    // The text of the random upgrade information
    [SerializeField] private TextMeshProUGUI randomUpgradeText;

    // The text displaying th money
    [SerializeField] private TextMeshProUGUI moneyAmount = null;
    #endregion

    #region Singleton
    // The singleton instance of this script
    public static LDJ_GameManager Instance = null;
    #endregion

    #region Methods
    #region Original Methods
    private void GetReferences()
    {
        // Get the event system
        if (!eventSystem)
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }
        // Get the canvas
        if (!canvas)
        {
            canvas = FindObjectOfType<Canvas>();
        }
        // Get the player
        if (!player)
        {
            player = FindObjectOfType<LDJ_Player>();
        }
    }

    /// <summary>
    /// Loads a level
    /// </summary>
    /// <param name="_levelID">ID of the level to load</param>
    public void LoadLevel(int _levelID)
    {
        Time.timeScale = 1;
        randomUpgradeObject.interactable = true;
        specificUpgradeObject.interactable = true;

        Cursor.visible = true;

        SceneManager.LoadScene(_levelID);
    }

    // Methods for the end map interaction
    public void PlayerEatAll()
    {
        if (player)
        {
            player.Eat();
        }
    }
    public void PlayerSell()
    {
        if (player)
        {
            player.SellInventory();

            moneyAmount.text = player.Currency.ToString();

            randomUpgradeObject.interactable = player.Currency >= 5;
            specificUpgradeObject.interactable = player.Currency >= 10;
        }
    }
    public void PlayerRandomUpgrade()
    {
        if (player)
        {
            UpgradeType _type = (UpgradeType)Random.Range(0, System.Enum.GetNames(typeof(UpgradeType)).Length);

            randomUpgradeText.text = string.Format(randomUpgradeText.text, _type);
            player.Upgrade(_type, 5);

            moneyAmount.text = player.Currency.ToString();
        }
    }
    public void PlayerSpecificUpgrade(int _type)
    {
        if (player)
        {
            player.Upgrade((UpgradeType)_type, 10);

            moneyAmount.text = player.Currency.ToString();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Reselect the last button if needed
    private void ReselectButton()
    {
        // If not having an event system, return
        if (!eventSystem) return;

        // If a game object is selected, set it as last selected
        if (eventSystem.currentSelectedGameObject != null)
        {
            lastSelected = eventSystem.currentSelectedGameObject;
        }
        // If not, set the last selected as it if it's not null
        else if (lastSelected != null)
        {
            eventSystem.SetSelectedGameObject(lastSelected);
        }
    }

    /// <summary>
    /// Set a button as the actually selected
    /// </summary>
    /// <param name="_gameObject">Game object to select</param>
    public void SetSelectedButton(GameObject _gameObject)
    {
        // If not having an event system, return
        if (!eventSystem) return;

        // Set the object as selected
        eventSystem.SetSelectedGameObject(_gameObject);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Set the singelton instance
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        // Get the references
        GetReferences();
    }

    private void FixedUpdate()
    {
        // Reselect the last button if needed
        ReselectButton();
    }

    private void OnDestroy()
    {
        // If this was the singleton instance, set it as null
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Use this for initialization
    void Start ()
    {
        // Set it, the canvas, the UI Manager & the event system, to don't destroy it on load
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(canvas);
        DontDestroyOnLoad(LDJ_UIManager.Instance.gameObject);
        DontDestroyOnLoad(EventSystem.gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        GetReferences();
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
    #endregion
    #endregion
}
