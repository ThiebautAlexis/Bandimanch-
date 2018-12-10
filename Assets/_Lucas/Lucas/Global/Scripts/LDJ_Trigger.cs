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
    [SerializeField] private TriggerType triggerType;
    public TriggerType TriggerType
    {
        get { return triggerType; }
    }
    #endregion

    #region Methods
    #region Original Methods

    #endregion

    #region Unity Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<LDJ_Player>())
        {
            switch (triggerType)
            {
                case TriggerType.EndLevel:
                    LDJ_UIManager.Instance.EndMapMenu();
                    break;
                case TriggerType.EndGame:
                    LDJ_UIManager.Instance.EndGameMenu();
                    break;
                case TriggerType.HordeTrigger:
                    // Activate horde
                    break;
                default:
                    break;
            }
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
