using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDJ_MeatTree : MonoBehaviour
{
    /* LDJ_MeatTree :
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
    // Does the tree have meat on it ?
    [SerializeField] private bool hasMeat = true;
    public bool HasMeat
    {
        get { return hasMeat; }
    }
    // The anchor of all tree hams
    [SerializeField] private GameObject hamAnchor = null;

    // The meat object to instantiate
    [SerializeField] private LDJ_ObjectCharacteristics ham = null;
    public LDJ_ObjectCharacteristics Ham
    {
        get { return ham; }
    }
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// Get the meat on the tree
    /// </summary>
    /// <returns>Returns a meat object characteristics</returns>
    public LDJ_ObjectCharacteristics TakeMeat()
    {
        hamAnchor.SetActive(false);
        hasMeat = false;

        return ham;
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
