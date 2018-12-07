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
    #endregion

    #region Fields and properties 
    private NavMeshAgent agent;
    private Transform target;
    [SerializeField, Range(.1f, 5)] float avoidanceRangeMin = .1f;
    [SerializeField, Range(.1f, 5)] float avoidanceRangeMax = 5;
    [SerializeField, Range(1, 10)] float radiusChangementTime = 5; 
    private float currentAvoidanceRange;
    private float maxAvoidanceRangeValue;
    #endregion

    #region Methods
    public void InitAgent(float _newSpeed, Transform _target)
    {
        if (!agent) agent = GetComponent<NavMeshAgent>(); 
        agent.speed = _newSpeed;
        target = _target;
    }

    public void SetSpeed(float _newSpeed)
    {
        agent.speed = _newSpeed;
    }

    private void UpdateAgentRadius()
    {
        currentAvoidanceRange = Mathf.Clamp(Mathf.PingPong(Time.time, maxAvoidanceRangeValue), avoidanceRangeMin, avoidanceRangeMax);
        agent.radius = currentAvoidanceRange; 
    }

    IEnumerator GetRandomRadius()
    {
        maxAvoidanceRangeValue = Random.Range(avoidanceRangeMin, avoidanceRangeMax);
        yield return new WaitForSeconds(Random.Range(1, radiusChangementTime));
        StartCoroutine(RandomRadiusCallback()); 
        yield break;
    }

    void MoveToTarget()
    {
        if (!agent || !target) return; 
        agent.SetDestination(target.position);
    }
    #endregion

    #region UnityMethods
    private void Awake()
    {
        RandomRadiusCallback += GetRandomRadius; 
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
        LDJ_Player _player = null;
        if (_player = _collision.transform.GetComponent<LDJ_Player>())
        {
            _player.TakeDamage();
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
