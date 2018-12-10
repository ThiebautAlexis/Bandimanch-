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
    // Buttons of the end map menu
    [SerializeField] private Button randomUpgradeObject = null;
    [SerializeField] private Button specificUpgradeObject = null;

    // The text of the random upgrade information
    [SerializeField] private TextMeshProUGUI randomUpgradeText;

    // The text displaying th money
    [SerializeField] private TextMeshProUGUI moneyAmount = null;

    [Header("Maps Start Points :")]
    // List containing all the maps start points
    [SerializeField] private List<Vector3> mapsStartPoints = new List<Vector3>();

    // Index of the current level
    [SerializeField] private int currentLevel = 0;
    #endregion

    #region Singleton
    // The singleton instance of this script
    public static LDJ_GameManager Instance = null;
    #endregion

    #region Methods
    #region Original Methods
    #region End Map Interactions
    /// <summary>
    /// Makes the player eat all food
    /// </summary>
    public void PlayerEatAll()
    {
        LDJ_Player.Instance.Eat();
    }
    /// <summary>
    /// Sold all the player's inventory
    /// </summary>
    public void PlayerSell()
    {
        LDJ_Player.Instance.SellInventory();

        moneyAmount.text = LDJ_Player.Instance.Currency.ToString();

        randomUpgradeObject.interactable = LDJ_Player.Instance.Currency >= 5;
        specificUpgradeObject.interactable = LDJ_Player.Instance.Currency >= 10;
    }
    /// <summary>
    /// Gives the player a random upgrade
    /// </summary>
    public void PlayerRandomUpgrade()
    {
        UpgradeType _type = (UpgradeType)Random.Range(0, System.Enum.GetNames(typeof(UpgradeType)).Length);

        randomUpgradeText.text = string.Format(randomUpgradeText.text, _type);
        LDJ_Player.Instance.Upgrade(_type, 5);

        moneyAmount.text = LDJ_Player.Instance.Currency.ToString();
    }
    /// <summary>
    /// Gives the player a specific upgrade
    /// </summary>
    /// <param name="_type">Int value of the UpgradeType enum to give</param>
    public void PlayerSpecificUpgrade(int _type)
    {
        LDJ_Player.Instance.Upgrade((UpgradeType)_type, 10);

        moneyAmount.text = LDJ_Player.Instance.Currency.ToString();
    }
    #endregion

    /// <summary>
    /// Triggers the end of this level
    /// </summary>
    public void EndLevel()
    {
        // If this is not the last level, triggers the end map menu
        if (currentLevel < mapsStartPoints.Count - 1)
        {
            LDJ_UIManager.Instance.EndMapMenu();
        }
        // If this is the last one, triggers the end of the game
        else
        {
            LDJ_UIManager.Instance.EndGameMenu();
        }
    }

    /// <summary>
    /// Get to the next level
    /// </summary>
    public void NextLevel()
    {
        // If the player can go the next level, take him there
        if (currentLevel < mapsStartPoints.Count - 1)
        {
            currentLevel++;

            LDJ_Player.Instance.transform.position = mapsStartPoints[currentLevel];
        }
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Starts the entiere game from the beginning
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(0);
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
    }

    private void FixedUpdate()
    {
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
    }

    // Update is called once per frame
    void Update ()
    {
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (Vector3 point in mapsStartPoints)
        {
            Gizmos.DrawSphere(point, .5f); 
        }

        
    }
    #endregion
    #endregion
}
