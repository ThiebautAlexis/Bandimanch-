using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LDJ_UIInventoryItem : MonoBehaviour
{
    /* LJD_UIInventoryItem :
	*
	* [Description]
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
    // The button of the item
    [SerializeField] private Button button = null;
    public Button Button
    {
        get { return button; }
    }

    // The image of the item
    [SerializeField] private Image image = null;
    public Image Image
    {
        get { return image; }
    }

    // The object reference of the item
    [SerializeField] private List<LDJ_ObjectCharacteristics> objectsReferences = new List<LDJ_ObjectCharacteristics>();
    public List<LDJ_ObjectCharacteristics> ObjectsReferences
    {
        get { return objectsReferences; }
    }

    // The text indicating the item stack amount
    [SerializeField] private TextMeshProUGUI stackAmountText = null;
    public TextMeshProUGUI StackAmountText
    {
        get { return stackAmountText; }
    }
    // The text indicating the weight of the item
    [SerializeField] private TextMeshProUGUI weightText = null;
    public TextMeshProUGUI WeightText
    {
        get { return weightText; }
    }
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// Adds an item to the list
    /// </summary>
    /// <param name="_object">Object reference to add</param>
    public void AddItem(LDJ_ObjectCharacteristics _object)
    {
        // If the list is empty, initializes it
        if (objectsReferences == null || objectsReferences.Count == 0)
        {
            Init(_object);
            return;
        }

        // If the object reference is already in it or not the same than the the first already containing, return
        if (objectsReferences.Contains(_object) || (_object.ResourcePrefabName != objectsReferences[0].ResourcePrefabName))
        {
            Debug.Log("Return");
            return;
        }

        // Adds the object to the list
        objectsReferences.Add(_object);

        // Set the texts indications
        stackAmountText.text = (int.Parse(stackAmountText.text) + 1).ToString();
        weightText.text = (int.Parse(weightText.text) + _object.Weight).ToString();
    }

    /// <summary>
    /// Initializes the UI inventory item basing on an object reference
    /// </summary>
    /// <param name="_object">Object reference to initializes with</param>
    public void Init(LDJ_ObjectCharacteristics _object)
    {
        // Creates a new list of object characteristics and add it a value
        objectsReferences = new List<LDJ_ObjectCharacteristics>(1) { _object };

        // Set the sprite and the texts indications
        image.sprite = _object.Sprite;
        stackAmountText.text = 1.ToString();
        weightText.text = _object.Weight.ToString();
    }
    /// <summary>
    /// Initializes the UI inventory item based on an item reference
    /// </summary>
    /// <param name="_reference"></param>
    public void Init(LDJ_UIInventoryItem _reference)
    {
        // Set the obejcts references
        objectsReferences = _reference.objectsReferences;

        // Set the sprite of the item
        image.sprite = _reference.image.sprite;
        // Set the object amount
        stackAmountText.text = _reference.objectsReferences.Count.ToString();

        // Set the weight of each element
        weightText.text = _reference.objectsReferences[0].Weight.ToString();
        foreach (LDJ_ObjectCharacteristics _object in _reference.objectsReferences)
        {
            if (_object != _reference.objectsReferences[0])
            {
                weightText.text = (int.Parse(weightText.text) + _object.Weight).ToString();
            }
        }
    }

    /// <summary>
    /// Removes an item from the list
    /// </summary>
    /// <param name="_object">Object reference to remove</param>
    /// <returns>Returns true if the object should be destroyed</returns>
    public bool RemoveItem(LDJ_ObjectCharacteristics _object)
    {
        // Remove the reference if it is contained
        if (objectsReferences.Contains(_object))
        {
            objectsReferences.Remove(_object);
        }

        // If there is no object reference, destroy this
        if (objectsReferences == null || objectsReferences.Count == 0)
        {
            return true;
        }

        // Set the texts indications
        stackAmountText.text = (int.Parse(stackAmountText.text) - 1).ToString();
        weightText.text = (int.Parse(weightText.text) - _object.Weight).ToString();

        return false;
    }
    #endregion

    #region Unity Methods
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
