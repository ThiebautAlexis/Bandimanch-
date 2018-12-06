using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using AK.Wwise;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class LDJ_Object : MonoBehaviour
{
    /* LDJ_Object :
	*
	* The behaviour of the objects the player can grab to put in his bag
    * 
    * Objectives :
    *   • Core :
    *       - Inflict damages on contact
    *       - Different types of objects with properties
    *       - Sell / exchange
    *       - Use / consume
    *   
    *   • Polish :
    *       - 
    *       
    * Dones :
    *   • Core :
    *       - Grab
    *       - Throw
	*/

    #region Events

    #endregion

    #region Fields / Accessors
    [Header("Object Characteristics :")]
    // The object characteristics object linked
    [SerializeField] public LDJ_ObjectCharacteristics ObjectCharacteristics = new LDJ_ObjectCharacteristics();

    [Header("State :")]
    // Can the object be grabbed by the player
    [SerializeField] private bool canBeGrabbed = true;
    public bool CanBeGrabbed
    {
        get { return canBeGrabbed; }
    }
    // Can the object currently inflict damages
    [SerializeField] private bool canInflictDamages = false;
    public bool CanInflictDamages
    {
        get { return canInflictDamages; }
    }
    // Can the obejct be stacked with other objects of the same type in the inventory
    [SerializeField] private bool isStackable = true;
    public bool IsStackable
    {
        get { return isStackable; }
    }

    [Header("Statistics :")]
    // The bonus force given by the thrower
    [SerializeField] private int bonusForce = 0;
    public int BonusForce
    {
        get { return bonusForce; }
    }

    [Header("Components & References :")]
    // The box collider of the object
    [SerializeField] private new BoxCollider collider = null;
    public BoxCollider Collider
    {
        get { return collider; }
    }
    [SerializeField] private LayerMask whatIsHorde = new LayerMask();
    // The rigidbody of the object
    [SerializeField] private new Rigidbody rigidbody = null;
    public Rigidbody Rigidbody
    {
        get { return rigidbody; }
    }
    #endregion

    #region Methods
    #region Original Methods
    // Check if the horde is hit
    private void CheckHitHorde()
    {
        Collider[] _hit = new Collider[] { };

        if ((_hit = Physics.OverlapBox(transform.TransformPoint(collider.center), collider.size, Quaternion.identity, whatIsHorde)).Length > 0)
        {
            LDJ_AIManager.Instance.HitHorde();

            rigidbody.velocity /= 2;

            canInflictDamages = false;
        }
    }

    /// <summary>
    /// Throws the object from a position, with a velocty and a bonus force
    /// </summary>
    /// <param name="_throwPosition">Position from which the object is thrown</param>
    /// <param name="_velocity">Velocity to throw the object with</param>
    /// <param name="bonusForce">Bonus force of the throw</param>
    public void Throw(Vector3 _throwPosition, Vector3 _velocity, int _bonusForce)
    {
        gameObject.SetActive(true);
        transform.position = _throwPosition;
        rigidbody.isKinematic = false;
        rigidbody.velocity = _velocity;
        bonusForce = _bonusForce;
        canInflictDamages = true;

        // Sound
        switch (ObjectCharacteristics.ObjectType)
        {
            case ObjectType.Cooker:
                break;
            case ObjectType.Food:
                break;
            case ObjectType.Treasure:
                break;
            case ObjectType.Weapon:
                //AkSoundEngine.PostEvent("Play_Axe_Throw", gameObject);
                break;
            default:
                break;
        }
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Get the components references if needed
        if (!collider) collider = GetComponent<BoxCollider>();
        if (!rigidbody) rigidbody = GetComponent<Rigidbody>();

        // If the instance is not this, set it
        if (ObjectCharacteristics.Instance != this)
        {
            ObjectCharacteristics.Instance = this;
        }
    }

    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled
    private void FixedUpdate()
    {
        // If the object can inflict damages but has a velocity of zero, set it as cannot inflict damages anymore
        if (canInflictDamages)
        {
            if (rigidbody.velocity.z == 0)
            {
                canInflictDamages = false;
            }
            else
            {
                CheckHitHorde();
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
