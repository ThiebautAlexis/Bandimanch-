using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LDJ_NavigationPoint : MonoBehaviour
{
    #region Fields and properties
    public string PrefabName { get { return gameObject.name; } } 
    public Vector3 Position { get { return transform.position; } }
    public List<LDJ_NavigationPoint> LinkedPoints = new List<LDJ_NavigationPoint>();
    public int Layer { get { return gameObject.layer; } }

    public bool HasBeenSelected { get; set; }

    private float heuristicPriority;
    public float HeuristicPriority
    {
        get
        {
            return heuristicPriority = HeuristicCostFromStart + HeuristicCostToDestination;
        }
        set
        {
            heuristicPriority = value;
            HeuristicCostToDestination = heuristicPriority - HeuristicCostFromStart;
        }
    }

    public float HeuristicCostFromStart { get; set; }
    public float HeuristicCostToDestination { get; set; }
    #endregion

    #region Methods
    public void LinkPoint()
    {
        LinkedPoints.Clear(); 
        RaycastHit _hitPoint;
        LDJ_NavigationPoint _point; 
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out _hitPoint))
        {
            _point = _hitPoint.collider.GetComponent<LDJ_NavigationPoint>();
            if (_point)
            {
                LinkedPoints.Add(_point);
                _point = null; 
            }
        }
        if (Physics.Raycast(new Ray(transform.position, -transform.forward), out _hitPoint))
        {
            _point = _hitPoint.collider.GetComponent<LDJ_NavigationPoint>();
            if (_point)
            {
                LinkedPoints.Add(_point);
                _point = null;
            }
        }
        if (Physics.Raycast(new Ray(transform.position, -transform.right), out _hitPoint))
        {
            _point = _hitPoint.collider.GetComponent<LDJ_NavigationPoint>();
            if (_point)
            {
                LinkedPoints.Add(_point);
                _point = null;
            }
        }
        if (Physics.Raycast(new Ray(transform.position, transform.right), out _hitPoint))
        {
            _point = _hitPoint.collider.GetComponent<LDJ_NavigationPoint>();
            if (_point)
            {
                LinkedPoints.Add(_point);
                _point = null;
            }
        }
    }

    private void Start()
    {
        LinkPoint(); 
    }
    #endregion

    #region UnityMethods
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Position, 5f);
        for (int i = 0; i < LinkedPoints.Count; i++)
        {
            Gizmos.DrawLine(Position, LinkedPoints[i].Position); 
        }
        
    }
    #endregion
}
