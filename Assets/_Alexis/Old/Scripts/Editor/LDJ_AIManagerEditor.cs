using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LDJ_AIManager))]
public class LDJ_AIManagerEditor : Editor
{
    LDJ_AIManager p_target; 

    private void OnEnable()
    {
        p_target = (LDJ_AIManager)target; 
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //GUITools.ActionButton("Add Point", AddPoint, Color.white, Color.black);
        //GUITools.ActionButton("Link all points", p_target.LinkAllPoints, Color.white, //Color.black);
        //GUITools.ActionButton("Get all points", GetAllPoints, Color.white, Color.black);
        //for (int i = 0; i < p_target.NavigationPoints.Count; i++)
        //{
        //    EditorGUILayout.BeginHorizontal();
        //    EditorGUILayout.LabelField($"Point n°{i + 1}");
        //    GUITools.ActionButton("Remove Point", RemovePoint, i, Color.white, Color.white);
        //    EditorGUILayout.EndHorizontal();
        //    EditorGUILayout.Space(); 
        //}
    }

    void AddPoint()
    {
        Vector3 _pos = p_target.NavigationPoints.Count == 0 ? p_target.transform.position : p_target.NavigationPoints.Last().Position; 
        if(p_target.NavigationPointPrefab.PrefabName == null || p_target.NavigationPointPrefab.PrefabName == string.Empty)
        {
            Debug.Log("Prefab Not found");
            return; 
        }
        LDJ_NavigationPoint _point = Instantiate(Resources.Load<LDJ_NavigationPoint>(p_target.NavigationPointPrefab.PrefabName) as LDJ_NavigationPoint, _pos, Quaternion.identity, p_target.transform);
        p_target.NavigationPoints.Add(_point); 
    }

    void RemovePoint(int _i)
    {
        LDJ_NavigationPoint _point = p_target.NavigationPoints[_i]; 
        p_target.NavigationPoints.RemoveAt(_i);
        DestroyImmediate(_point.gameObject); 
    }

    void GetAllPoints()
    {
        p_target.NavigationPoints.Clear();
        p_target.NavigationPoints = FindObjectsOfType<LDJ_NavigationPoint>().ToList(); 
    }
}
