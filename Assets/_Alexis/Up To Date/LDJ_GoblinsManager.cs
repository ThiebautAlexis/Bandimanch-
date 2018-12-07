using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 

[RequireComponent(typeof(BoxCollider))]
public class LDJ_GoblinsManager : MonoBehaviour
{
    #region Events
    public event Action OnHordeActivated;
    public event Action OnHordeHit;
    public event Action<float> OnPlayerHit; 
    public Action<float> OnSpeedModified; 
    #endregion

    #region Fields and Properties
    [Header("Spawning Informations")]
    [SerializeField] LDJ_GoblinAgent instanciedAgent;
    [SerializeField, Range(1, 250)] int instanceCounts = 10;
    [SerializeField, Range(1, 50)] float spawningRange = 5;
    [Header("Horde Informations")]
    [SerializeField] Transform hordeParent; 
    [SerializeField] Transform hordeTarget;
    [Header("Speed")]
    [SerializeField, Range(1, 50)] float minSpeed = 1;
    [SerializeField, Range(1, 50)] float maxSpeed = 50;
    [SerializeField, Range(1, 50)] float currentSpeed = 1;
    [SerializeField, Range(1, 10)] int decreasingSpeedValue = 10;
    [SerializeField, Range(1, 10)] int increasingSpeedValue = 5;
    [SerializeField, Range(1, 300)] int increasingSpeedRange = 25; 

    private LDJ_GoblinAgent leader;

    #endregion

    #region UnityMethods
    private void Start()
    {
        InvokeRepeating("IncreaseSpeed", increasingSpeedRange, increasingSpeedRange);     
    }

    private void Awake()
    {
        OnHordeActivated += InitHorde;
        if (!hordeTarget) hordeTarget = FindObjectOfType<LDJ_Player>().transform; 
    }

    private void OnTriggerEnter(Collider _coll)
    {
        if (_coll.GetComponent<LDJ_Player>())
            OnHordeActivated?.Invoke(); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.left * spawningRange)); 
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initialize the horde
    /// Instantiate every agent
    /// Set them a speed and a target
    /// Add Callback on events
    /// </summary>
    private void InitHorde()
    {
        if(instanciedAgent)
        {
            LDJ_GoblinAgent _goblin; 
            for (int i = 0; i < instanceCounts; i++)
            {
                _goblin = Instantiate(instanciedAgent, transform.position + new Vector3(Random.Range(0, spawningRange), 0, Random.Range(0, spawningRange)), Quaternion.identity, hordeParent ? hordeParent : null);
                OnSpeedModified += _goblin.SetSpeed;
                _goblin.OnAgentHit += HitHorde;
                _goblin.OnHitPlayer += HitPlayer; 
                OnPlayerHit += _goblin.StopMovement; 
                if (i == 0)
                {
                    leader = _goblin;
                    _goblin.InitAgent(currentSpeed, hordeTarget.transform);
                     
                }
                else 
                    _goblin.InitAgent(currentSpeed, leader.transform); 
            }
        }
        OnHordeActivated -= InitHorde; 
    }

    /// <summary>
    /// Call the method Change Speed with percentage with the increasingSpeedPercentages
    /// </summary>
    private void IncreaseSpeed()
    {
        ApplyNewSpeed(increasingSpeedValue); 
    }

    /// <summary>
    /// Change speed value using a percentage
    /// The value is calculated with a percentage based on the total speed range
    /// </summary>
    /// <param name="_percentage"></param>
    public void ApplyNewSpeed(float _value)
    {
        //_percentage = Mathf.Clamp(_percentage, -100, 100);
        //bool _increase = _percentage > 0;
        //float _value = ((maxSpeed - minSpeed) * Mathf.Abs(_percentage)) / 100;
        //if (_increase)
        //    currentSpeed += _value;
        //else
        currentSpeed += _value;
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
        OnSpeedModified?.Invoke(currentSpeed);
    }

    /// <summary>
    /// CALLED WHEN THE HORDE IS HIT 
    /// slow down the horde and every agent
    /// </summary>
    public void HitHorde()
    {
        OnHordeHit?.Invoke();
        ApplyNewSpeed(-decreasingSpeedValue);
    }

    /// <summary>
    /// Called when the player is hit
    /// Apply stop on each agent
    /// </summary>
    public void HitPlayer()
    {
        OnPlayerHit?.Invoke(minSpeed); 
    }
    #endregion 
}
