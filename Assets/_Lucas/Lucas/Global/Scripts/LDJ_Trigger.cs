using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerType
{
    EndLevel,
    EndGame,
    HordeTrigger
}

[RequireComponent(typeof(BoxCollider))]
public class LDJ_Trigger : MonoBehaviour
{
    /* LDJ_Trigger :
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
    #endregion

    #region Methods
    #region Original Methods

    #endregion

    #region Unity Methods
    private void OnTriggerEnter(Collider other)
    {
        // If the player enters the zone, trigger the end of the level
        if (other.GetComponent<LDJ_Player>())
        {
            LDJ_GameManager.Instance.EndLevel();
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
