﻿using System.Collections;
using System.Collections.Generic;
using System; 
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random; 

/*[SCRIPT HEADER] LDJ_AIAgent 
 * Created by: Alexis Thiebaut
 * Date: 01/12/2018
 * Description: Système de floaking pour les ennemis
 * CHOSES A FAIRE:
 * -> Utiliser le nav mash agent d'Unity pour faire se déplacer le leader
 * -> Gérer mieux le path des autres goblins
 * 
 */
[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(BoxCollider))]
public class LDJ_AIAgent : MonoBehaviour
{
    #region Fields and properties
    [SerializeField] bool isLeader = false; 
    public bool IsLeader { get { return isLeader; } set { isLeader = value; } }

    [SerializeField] LDJ_AIAgent leaderAgent; 

    [SerializeField] Vector3 velocity;
    public Vector3 Velocity { get { return velocity; } }

    [SerializeField] float speed =0; 

    [SerializeField] float updateTick = .5f;

    List<Vector3> agentPath = new List<Vector3>();
    public bool canMove = false;

    int pathIndex = 0;
    #endregion

    #region Methods

    /// <summary>
    /// BEHAVIOUR OF THE BOID
    /// </summary>
    void BoidBehaviour()
    {
        if (!LDJ_AIManager.Instance.IsReady && !isLeader)
        {
            velocity = Vector3.zero;
            return;
        }
        #region LEADER
        if (isLeader)
        {
            if (LDJ_AIManager.Instance.IsCalculating) return;
            if(Vector3.Distance(LDJ_AIManager.Instance.TargetTransform.position, transform.position) < 5)
            {
                velocity = Vector3.zero;
                canMove = false; 
            }
            if (!canMove && !LDJ_AIManager.Instance.IsCalculating)
            {
                canMove = false;
                SetDestination();
                return;
            }
            if (agentPath.Count > 1)
            {
                if (Vector3.Distance(transform.position, agentPath[pathIndex]) < 5)
                {
                    pathIndex++;
                    LDJ_AIManager.Instance.ChangeSpeedWithPercentage(10); 
                }
                if(pathIndex == agentPath.Count)
                {
                    SetDestination(); 
                }
            }
            if(agentPath.Count == 0 && canMove)
            {
                velocity = Vector3.ClampMagnitude(GetDirectionFromPosition(LDJ_AIManager.Instance.TargetTransform.position), 1);
                return; 
            }
            if (canMove)
            {
                velocity = Vector3.ClampMagnitude(GetDirectionFromPosition(agentPath[pathIndex]), 1);
            }
            return; 
        }
        #endregion

        #region RegularAgent
        if(leaderAgent.Velocity == Vector3.zero)
        {
            velocity = Vector3.zero;
            return; 
        }
        // GET ALL AGENTS
        LDJ_AIAgent[] _agents = LDJ_AIManager.Instance.AllAgents.ToArray();
        if (_agents.Length < 2) return;

        //COHESION IS THE GLOBAL CENTER
        Vector3 _cohesion = Vector3.zero;
        //SEPARATION MOVE AGENT TO AVOID EACH OTHER
        Vector3 _separation = Vector3.zero;
        //ALIGNEMENT IS THE GENERAL DIRECTION
        Vector3 _alignement = Vector3.zero;

        Vector3 _direction;
        float _distance; 

        LDJ_AIAgent _a;

        //GET VARIOUS RANGE
        float _separationRange = LDJ_AIManager.Instance.SeparationRange;

        if(Vector3.Distance(transform.position, leaderAgent.transform.position) > LDJ_AIManager.Instance.SpawnRange)
        {
            transform.position = leaderAgent.transform.position - (Vector3.back * 1); 
        }
        //GET ALL OTHER AGENTS
        for (int i = 0; i < _agents.Length; i++)
        {
            _a = _agents[i];
            if (_a == this) continue;

            //GET DIRECTION AND DISTANCE BETWEEN POINTS
            _direction = GetDirectionFromPosition(_a.transform.position);
            _distance = Vector3.Distance(transform.position, _a.transform.position);

            //Alignement defined by the 

            if (_distance < _separationRange)
            {
                //REMOVE THE DIRECTION TO SEPARATE POINTS
                _separation -= _direction;
            }
        }
               
        //SAME THING FROM THE LEADER
        _direction = GetDirectionFromPosition(leaderAgent.transform.position);
        _distance = Vector3.Distance(transform.position, leaderAgent.transform.position);

        _alignement = leaderAgent.velocity;
        _alignement = Vector3.ClampMagnitude(_alignement, 1);
        _alignement *= LDJ_AIManager.Instance.AlignementMultiplicator;


        if (_distance < _separationRange)
        {
            //REMOVE THE DIRECTION TO SEPARATE POINTS
            _separation -= _direction;
        }
        
        //REMOVE THIS TO BE LESS COMPACT
        //_separation = _separation / _agents.Length;
        _separation = Vector3.ClampMagnitude(_separation, 1);
        _separation *= LDJ_AIManager.Instance.SeparationMultiplicator;


        //_cohesion = leaderAgent.transform.position;
        _cohesion = leaderAgent.transform.position;
        _cohesion = Vector3.ClampMagnitude(_cohesion - transform.position, 1);
        _cohesion *= LDJ_AIManager.Instance.CohesionMultiplicator;

        //SET THE VELOCITY
        velocity = Vector3.ClampMagnitude(_alignement + _separation + _cohesion, 1);
        #endregion
    }

    /// <summary>
    /// GET THE DIRECTION BETWEEN THE AGENT POSITION AND ANOTHER POSITION
    /// </summary>
    /// <param name="_pos">POSITION</param>
    /// <returns>DIRECTION BETWEEN TRANSFORM POSITION AND SELECTED POSITION</returns>
    Vector3 GetDirectionFromPosition(Vector3 _pos)
    {
        return (_pos - transform.position);
    }

    /// <summary>
    ///MOVE AGENT 
    /// </summary>
    void MoveAgent()
    {
        if (!LDJ_AIManager.Instance.IsReady ) return;
        velocity = Vector3.ClampMagnitude(velocity, 1);
        velocity.y = 0;
        //APPLY VELOCITY TO THE POSITION OF THE AGENT
        transform.position += velocity * Time.deltaTime * speed;
        transform.LookAt(transform.position + velocity); 
    }

    /// <summary>
    /// SET DESTINATION TO THE TARGET
    /// </summary>
    public void SetDestination()
    {
        if (LDJ_AIManager.Instance.IsCalculating || agentPath.Count > 0 || canMove) return;
        pathIndex = 0;
        agentPath.Clear();
        List<Vector3> _tmp = new List<Vector3>();
        if (LDJ_AIManager.Instance.CalculatePath(_tmp))
        {
            agentPath = _tmp;
            canMove = true;
        }
    }

    /// <summary>
    /// SET THE LEADER
    /// </summary>
    public void SetLeader()
    {
        leaderAgent = LDJ_AIManager.Instance.Leader;
        velocity = Vector3.zero; 
    }

    void SetSpeed(float _newSpeed) => speed = _newSpeed;
    #endregion

    #region UnityMethods
    private void OnCollisionEnter(Collision collision)
    {
        LDJ_Player _player = null;
        if (_player = collision.transform.GetComponent<LDJ_Player>())
        {
            _player.TakeDamage();
        }
    }

    void Start()
    {
        LDJ_AIManager.Instance.OnSpeedModified += SetSpeed; 
        speed = LDJ_AIManager.Instance.LeaderSpeed;
        if(!IsLeader)
        {
            SetLeader();
        }
        InvokeRepeating("BoidBehaviour", Random.value * updateTick, updateTick);
    }

    void Update()
    {
        MoveAgent();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawLine(transform.position, transform.position + velocity);
        return; 
        if (!LDJ_AIManager.Instance) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, LDJ_AIManager.Instance.SeparationRange);
    }
    #endregion



}