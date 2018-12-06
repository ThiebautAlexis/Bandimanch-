using System;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Cooker,
    Food,
    Treasure,
    Weapon
}

[Serializable]
public class LDJ_ObjectCharacteristics
{
    /* LDJ_ObjectCharacteristics :
	*
	* The characteristics of an object
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
    [Header("References :")]
    // The name of the prefab to load by resources
    [SerializeField] private string resourcePrefabName = string.Empty;
    public string ResourcePrefabName
    {
        get { return resourcePrefabName; }
    }

    // The instance of this object
    [SerializeField] private LDJ_Object instance = null;
    public LDJ_Object Instance
    {
        get { return instance; }
        set
        {
            if (value != null)
            {
                value.ObjectCharacteristics = this;
            }
            instance = value;
        }
    }

    [Header("Characteristics :")]
    // The force of the object whern thown
    [SerializeField] private int force = 1;
    public int Force
    {
        get { return force; }
    }
    // The maximum stack amount of this object
    [SerializeField] private int maxStackAmount = 999;
    public int MaxStackAmount
    {
        get { return maxStackAmount; }
    }
    // The weight of the object
    [SerializeField] private int weight = 1;
    public int Weight
    {
        get { return weight; }
    }
    // The value of the object
    [SerializeField] private int currencyValue = 1;
    public int CurrencyValue
    {
        get { return currencyValue; }
    }

    [Header("Object Type Properties :")]
    // This object type
    [SerializeField] private ObjectType objectType = ObjectType.Weapon;
    public ObjectType ObjectType
    {
        get { return objectType; }
    }

    // The cooker restoration coefficient for total amount of food
    [SerializeField] private float cookerPower = 1.5f;
    public float CookerPower
    {
        get { return cookerPower; }
    }

    // The food restoration power
    [SerializeField] private int foodRestoration = 1;
    public int FoodRestoration
    {
        get { return foodRestoration; }
    }

    [Header("UI Sprite :")]
    // The sprite displayed for the object in UI
    [SerializeField] private Sprite sprite = null;
    public Sprite Sprite
    {
        get { return sprite; }
    }
    #endregion

    #region Methods

    #endregion
}
