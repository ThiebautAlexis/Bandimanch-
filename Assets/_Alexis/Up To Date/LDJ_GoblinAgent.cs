using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(BoxCollider))]
public class LDJ_GoblinAgent : MonoBehaviour
{
    #region Events 
    Func<IEnumerator> RandomRadiusCallback;
    public event Action OnHitPlayer;
    public event Action OnAgentHit; 
    #endregion

    #region Fields and properties 
    private NavMeshAgent agent;
    private Transform target;
    [Header("Avoidance Settings")]
    [SerializeField, Range(.1f, 5)] float avoidanceRangeMin = .1f;
    [SerializeField, Range(.1f, 5)] float avoidanceRangeMax = 5;
    [SerializeField, Range(1, 10)] float radiusChangementTime = 5; 
    private float currentAvoidanceRange;
    private float maxAvoidanceRangeValue;
    [Header("StroppingSettings")]
    [SerializeField, Range(.1f, 5f)] float startingTimer = 1; 

    private bool canMove = true; 
    #endregion

    #region Methods
    /// <summary>
    /// Init the agent with a speed and a target to chase
    /// </summary>
    /// <param name="_baseSpeed">base speed of the agent</param>
    /// <param name="_target">target of the agent</param>
    public void InitAgent(float _baseSpeed, Transform _target)
    {
        if (!agent) agent = GetComponent<NavMeshAgent>(); 
        agent.speed = _baseSpeed;
        target = _target;
    }

    /// <summary>
    /// Set a new speed to the agent
    /// </summary>
    /// <param name="_newSpeed"></param>
    public void SetSpeed(float _newSpeed)
    {
        agent.speed = _newSpeed;
    }

    /// <summary>
    /// Ping pong between a min and max values 
    /// </summary>
    private void UpdateAgentRadius()
    {
        if (!canMove) return; 
        currentAvoidanceRange = Mathf.Clamp(Mathf.PingPong(Time.time, maxAvoidanceRangeValue), avoidanceRangeMin, avoidanceRangeMax);
        agent.radius = currentAvoidanceRange; 
    }

    /// <summary>
    /// Get a random value for the maximum radius of the agent
    /// </summary>
    /// <returns></returns>
    IEnumerator GetRandomRadius()
    {
        maxAvoidanceRangeValue = Random.Range(avoidanceRangeMin, avoidanceRangeMax);
        yield return new WaitForSeconds(Random.Range(1, radiusChangementTime));
        StartCoroutine(RandomRadiusCallback()); 
        yield break;
    }

    /// <summary>
    /// Calculate the path and move to the target
    /// </summary>
    void MoveToTarget()
    {
        if (!agent || !target) return; 
        if(!canMove)
        {
            if (!agent.isStopped) agent.isStopped = true;
            return; 
        }
        if (agent.isStopped) agent.isStopped = false; 
        //if(agent.CalculatePath(target.position, agent.path))
        agent.SetDestination(target.position); 
    }
    
    /// <summary>
    /// Called when the agent is hit by an object
    /// </summary>
    public void HitAgent()
    {
        Debug.Log("Agent Hit"); 
        OnAgentHit?.Invoke(); 
    }

    /// <summary>
    /// Called when the player is hit
    /// </summary>
    /// <param name="_playerHit"></param>
    void HitPlayer(LDJ_Player _playerHit)
    {
        OnHitPlayer?.Invoke();
        _playerHit.TakeDamage();
    }

    /// <summary>
    /// Stop the horde
    /// Reset it after
    /// </summary>
    /// <param name="_speed"></param>
    public void StopMovement(float _speed)
    {
        canMove = false;
        SetSpeed(_speed); 
        StartCoroutine(ResetMovement()); 
    }

    public void PauseMovement(bool _pauseState) => canMove = !_pauseState;

    /// <summary>
    /// Set the bool can move to true after X seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetMovement()
    {
        yield return new WaitForSeconds(startingTimer);
        canMove = true;
        yield break; 
    }
    #endregion

    #region UnityMethods
    private void Awake()
    {
        RandomRadiusCallback += GetRandomRadius;
        LDJ_UIManager.Instance.OnMenuOpened += PauseMovement; 
    }
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentAvoidanceRange = avoidanceRangeMin;
        StartCoroutine(GetRandomRadius()); 
    }
    private void Update()
    {
        MoveToTarget(); 
        UpdateAgentRadius();
    }
    private void OnCollisionEnter(Collision _collision)
    {
        if (!canMove) return; 
        LDJ_Player _player = null;
        if (_player = _collision.transform.GetComponent<LDJ_Player>())
        {
            HitPlayer(_player); 
        }
    }
    private void OnDrawGizmos()
    {
        if (!target) return; 
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, target.position); 
    }
    #endregion
}
