using System.Collections;
using System.Collections.Generic;
using System; 
using System.Linq; 
using UnityEngine;
using Random = UnityEngine.Random;


/*[SCRIPT HEADER] LDJ_AIManager 
 * Created by: Alexis Thiebaut
 * Date: 01/12/2018
 * Description:
 * A FAIRE 
 * PATHFINDING
 */

[RequireComponent(typeof(BoxCollider))]
public class LDJ_AIManager : MonoBehaviour
{
    public event Action<float> OnSpeedModified;
    public event Action OnHordeHit;  

    #region Fields And Properties
    public static LDJ_AIManager Instance;
    [Header("Leader and agents")]
    [SerializeField] LDJ_AIAgent leader; 
    public LDJ_AIAgent Leader { get { return leader; } }

    [SerializeField] Transform hordeParent; 

    private List<LDJ_AIAgent> allAgents = new List<LDJ_AIAgent>(); 
    public List<LDJ_AIAgent> AllAgents { get { return allAgents; } }
    [SerializeField] bool isReady = false; 
    public bool IsReady { get { return isReady; } }

    [Header("Spawning Agents")]
    [SerializeField, Range(10, 500)] int agentCount = 25;
    [SerializeField, Range(5, 30)] float spawnRange = 10;
    public float SpawnRange { get { return spawnRange; } } 

    [Header("Target")]
    [SerializeField] Transform targetTransform; 
    public Transform TargetTransform { get { return targetTransform; } }

    #region Cohesion
    [Header("Cohesion")]
    [SerializeField, Range(1,50)] float cohesionMultiplicator = 1;
    public float CohesionMultiplicator { get { return cohesionMultiplicator; } }
    #endregion

    #region Separation
    [Header("Separation")]
    [SerializeField, Range(1,10)] float separationRange = 5;
    public float SeparationRange { get { return separationRange; } }

    [SerializeField, Range(1,50)] float separationMultiplicator = 1;
    public float SeparationMultiplicator { get { return separationMultiplicator; } }

    #endregion

    #region Alignement
    [Header("Alignement")]
    [SerializeField, Range(1,50)] float alignementMultiplicator = 1;
    public float AlignementMultiplicator { get { return alignementMultiplicator; } }
    #endregion

    #region Speed
    [Header("Speed")]
    [SerializeField, Range(1, 20)] float minSpeed = 1;
    [SerializeField, Range(1, 20)] float maxSpeed = 20; 
    [SerializeField] float leaderSpeed = 1;
    public float LeaderSpeed { get { return leaderSpeed; } }

    [SerializeField, Range(0,100)] int decreasingPercentage = 5;
    #endregion

    #region Pathfinding
    [Header("PathFinding")]
    [SerializeField] LDJ_NavigationPoint navigationPointPrefab;
    public LDJ_NavigationPoint NavigationPointPrefab { get { return navigationPointPrefab; } }

    [SerializeField] List<LDJ_NavigationPoint> navigationPoints = new List<LDJ_NavigationPoint>();
    public List<LDJ_NavigationPoint> NavigationPoints { get { return navigationPoints; } set { navigationPoints = value; } }

    bool isCalculating = false; 
    public bool IsCalculating { get { return isCalculating; } }
    #endregion
    #endregion

    #region Methods
    /// <summary>
    /// Get every linked point from the last entry until reaching the starting position
    /// </summary>
    /// <param name="_astarValues"></param>
    /// <param name="_destination"></param>
    List<Vector3> BuildPath(Dictionary<LDJ_NavigationPoint, LDJ_NavigationPoint> _astarValues, Vector3 _destination)
    {
        LDJ_NavigationPoint _currentPoint = _astarValues.Last().Key;
        List<Vector3> _navigationPath = new List<Vector3>();
        _navigationPath.Add(_destination);
        while (_currentPoint != _astarValues.First().Key)
        {
            _navigationPath.Add(_currentPoint.Position);
            _currentPoint = _astarValues[_currentPoint];
        }
        _navigationPath.Add(_currentPoint.Position);
        _navigationPath.Reverse();
        return _navigationPath;
    }

    /// <summary>
    /// CALCULATING PATH USING A*
    /// </summary>
    /// <returns></returns>
    public bool CalculatePath(List<Vector3> _path)
    {
        isCalculating = true;
        //GET POINTS
        LDJ_NavigationPoint _startingNavPoint = navigationPoints.OrderBy(n => Vector3.Distance(leader.transform.position, n.Position)).FirstOrDefault();
        LDJ_NavigationPoint _destinationPoint = navigationPoints.OrderBy(n => Vector3.Distance(targetTransform.position, n.Position)).FirstOrDefault();
        if(_startingNavPoint == _destinationPoint)
        {
            _path = new List<Vector3>(); 
            return true; 
        }

        //LISTS AND DICO
        List<LDJ_NavigationPoint> _openList = new List<LDJ_NavigationPoint>();
        Dictionary<LDJ_NavigationPoint, LDJ_NavigationPoint> _cameFrom = new Dictionary<LDJ_NavigationPoint, LDJ_NavigationPoint>();

        _openList.Add(_startingNavPoint);
        _startingNavPoint.HasBeenSelected = true;
        _startingNavPoint.HeuristicCostFromStart = 0;
        _cameFrom.Add(_startingNavPoint, _startingNavPoint);
        LDJ_NavigationPoint[] _linkedPoints;
        LDJ_NavigationPoint _currentPoint;
        while (_openList.Count > 0)
        {
            //Get the point with the best heuristic cost
            _currentPoint = GetBestPoint(_openList);
            //If this point is in the targeted triangle, 
            if (_currentPoint == _destinationPoint)
            {
                //add the destination point to the close list and set the previous point to the current point 
                //_cameFrom.Add(_destinationPoint, _currentPoint);

                //Clear all points selection state
                foreach (LDJ_NavigationPoint _point in navigationPoints)
                {
                    _point.HasBeenSelected = false;
                }
                //Build the path
                _path.AddRange(BuildPath(_cameFrom, targetTransform.position));
                isCalculating = false;
                return true;
            }
            _currentPoint.LinkPoint();
            //Get all linked points from the current point
            _linkedPoints = _currentPoint.LinkedPoints.ToArray();
            LDJ_NavigationPoint _parentPoint;
            for (int i = 0; i < _linkedPoints.Length; i++)
            {
                LDJ_NavigationPoint _linkedPoint = _linkedPoints[i];
                // If the linked points is not selected yet
                if (!_linkedPoint.HasBeenSelected)
                {
                    // Calculate the heuristic cost from start of the linked point
                    float _cost = _currentPoint.HeuristicCostFromStart + HeuristicCost(_currentPoint, _linkedPoint);
                    _linkedPoint.HeuristicCostFromStart = _cost;
                    if (!_openList.Contains(_linkedPoint) || _cost < _linkedPoint.HeuristicCostFromStart)
                    {
                        _parentPoint = _currentPoint;
                        // Set the heuristic cost from start for the linked point
                        _linkedPoint.HeuristicCostFromStart = _cost;
                        //Its heuristic cost is equal to its cost from start plus the heuristic cost between the point and the destination
                        _linkedPoint.HeuristicPriority = HeuristicCost(_linkedPoint, _destinationPoint) + _cost;
                        //Set the point selected and add it to the open and closed list
                        _linkedPoint.HasBeenSelected = true;
                        _openList.Add(_linkedPoint);
                        _cameFrom.Add(_linkedPoint, _parentPoint);
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Change speed value using a percentage
    /// The value is calculated with a percentage based on the total speed range
    /// </summary>
    /// <param name="_percentage"></param>
    public void ChangeSpeedWithPercentage(float _percentage)
    {
        _percentage = Mathf.Clamp(_percentage, -100, 100);
        bool _increase = _percentage > 0;
        float _value = ((maxSpeed - minSpeed) * Mathf.Abs(_percentage)) / 100;
        if (_increase)
            leaderSpeed += _value;
        else
            leaderSpeed -= _value;
        leaderSpeed = Mathf.Clamp(leaderSpeed, minSpeed, maxSpeed);
        OnSpeedModified?.Invoke(leaderSpeed);
    }

    /// <summary>
    /// Get the point with the best heuristic cost from a list 
    /// Remove this point from the list and return it
    /// </summary>
    /// <param name="_points">list where the points are</param>
    /// <returns>point with the best heuristic cost</returns>
    LDJ_NavigationPoint GetBestPoint(List<LDJ_NavigationPoint> _points)
    {
        int bestIndex = 0;
        for (int i = 0; i < _points.Count; i++)
        {
            if (_points[i].HeuristicPriority < _points[bestIndex].HeuristicPriority)
            {
                bestIndex = i;
            }
        }

        LDJ_NavigationPoint _bestNavPoint = _points[bestIndex];
        _points.RemoveAt(bestIndex);
        return _bestNavPoint;
    }

    /// <summary>
    /// CALCULATE THE HEURISTIC COST BETWEEN TWO POINTS
    /// HEURISTIC COST IS THE DISTANCE BETWEEN THE TWO POINTS
    /// </summary>
    /// <param name="_p1">point one</param>
    /// <param name="_p2">point two</param>
    /// <returns></returns>
    float HeuristicCost(LDJ_NavigationPoint _p1, LDJ_NavigationPoint _p2)
    {
        return Vector3.Distance(_p1.Position, _p2.Position);
    }

    /// <summary>
    /// CALLED WHEN THE HORDE IS HIT 
    /// </summary>
    public void HitHorde()
    {
        OnHordeHit?.Invoke();
        ChangeSpeedWithPercentage(-decreasingPercentage); 
    }

    /// <summary>
    /// Spawn enemies and set leader destination
    /// </summary>
    public void InitHorde()
    {
        SpawnAgents();
        isReady = true;
        leader.SetDestination();
    }

    public void LinkAllPoints() => NavigationPoints.ForEach(p => p.LinkPoint());

    /// <summary>
    /// Spawn a number of agents into the spawn range around the leader
    /// </summary>
    void SpawnAgents()
    {
        Vector3 _spawnPosition;
        for (int i = 0; i < agentCount; i++)
        {
            _spawnPosition = new Vector3(leader.transform.position.x + Random.Range(-spawnRange / 2, spawnRange / 2), leader.transform.position.y, leader.transform.position.z + Random.Range(-spawnRange / 2, spawnRange / 2));
            LDJ_AIAgent _agent = Instantiate(Resources.Load<LDJ_AIAgent>(leader.name) as LDJ_AIAgent, _spawnPosition, Quaternion.identity);
            if(hordeParent)_agent.transform.SetParent(hordeParent); 
            allAgents.Add(_agent); 
        }
        //Debug.Log("Sound");
       // AkSoundEngine.PostEvent("Play_Horde", gameObject);
    }   
    #endregion

    #region Unity Method
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(this);
        if (!targetTransform) targetTransform = FindObjectOfType<LDJ_Player>().transform; 
        if (leader == null) return;
        leader.IsLeader = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        LDJ_Player _player; 
        if(_player = collider.GetComponent<LDJ_Player>())
        {
            InitHorde(); 
        }
    }

    private void Start()
    {
    }
    #endregion
}
