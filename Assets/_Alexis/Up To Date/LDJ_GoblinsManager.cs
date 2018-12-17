using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Random = UnityEngine.Random; 

[RequireComponent(typeof(BoxCollider))]
public class LDJ_GoblinsManager : MonoBehaviour
{
    #region Events
    public event Func<IEnumerator> OnHordeActivated;
    public event Action OnHordeHit;
    public event Action<float> OnPlayerHit; 
    public Action<float> OnSpeedModified; 
    #endregion

    #region Fields and Properties
    [Header("Spawning Informations")]
    [SerializeField] LDJ_GoblinAgent instanciedAgent;
    [SerializeField, Range(1, 250)] int instanceCounts = 10;
    [SerializeField] Vector3 spawnPosition; 
    [SerializeField, Range(1, 50)] float spawningRange = 5;
    [Header("Texture Informations")]
    [SerializeField] Texture2D[] goblinTextures; 
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
        if (_coll.GetComponent<LDJ_Player>() && OnHordeActivated != null)
            StartCoroutine(OnHordeActivated?.Invoke()); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(spawnPosition, .5f);
        Gizmos.DrawWireSphere(spawnPosition, spawningRange); 
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initialize the horde
    /// Instantiate every agent
    /// Set them a speed and a target
    /// Add Callback on events
    /// </summary>
    private IEnumerator InitHorde()
    {
        if(instanciedAgent)
        {
            LDJ_GoblinAgent _goblin; 
            for (int i = 0; i < instanceCounts; i++)
            {
                yield return new WaitForSeconds(Random.Range(.1f, .5f)); 
                _goblin = Instantiate(instanciedAgent, spawnPosition + new Vector3(Random.Range(0, spawningRange), 0, Random.Range(0, spawningRange)), Quaternion.identity, hordeParent ? hordeParent : null);
                _goblin.transform.localScale *= Random.Range(.5f, 1.5f);
                if(goblinTextures.Length > 0)
                {
                    _goblin.GetComponentsInChildren<Renderer>().ToList().ForEach(r => r.material.mainTexture = goblinTextures[Random.Range(0, goblinTextures.Length)]);
                }
                OnSpeedModified += _goblin.SetSpeed;
                _goblin.OnAgentHit += HitHorde;
                _goblin.OnHitPlayer += HitPlayer;
                if (LDJ_UIManager.Instance) _goblin.OnHitPlayer += LDJ_UIManager.Instance.ActivateFang;
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
