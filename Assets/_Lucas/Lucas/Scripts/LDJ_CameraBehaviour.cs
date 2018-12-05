using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LDJ_CameraBehaviour : MonoBehaviour
{
    /* LDJ_CameraBehaviour :
	*
	* The Behaviour of the camera
    * 
    * Objectives :
    *   • Core :
    *       - ... It's empty there...
    *       
    *   • Polish :
    *       - Make the speed of the camera increase before being at its maximum value
    *       
    * Dones :
    *   • Core
    *       - Make the camera follow a target in the X & Z axis
    *       - Restricts camera movements between bounds
	*/

    #region Events

    #endregion

    #region Fields / Accessors
    [Header("Settings :")]
    // The target transform to follow
    [SerializeField] private Transform target = null;

    // The offset of the camera compared to the target
    [SerializeField] private Vector3 offset = Vector3.one;
    #region Bounds
    [Header("Bounds :")]
    // The bounds of the area where the camera can move
    [SerializeField] private float downBound = -1;
    [SerializeField] private float leftBound = -1;
    [SerializeField] private float rightBound = 1;
    [SerializeField] private float topBound = 1;
    #endregion

    [Header("Components & References :")]
    // The camera componentn attached to this game object
    [SerializeField] private new Camera camera = null;

    // List of all current meshes between the camera and the target
    [SerializeField] private Dictionary<MeshRenderer, float> cumbersomeMeshes = new Dictionary<MeshRenderer, float>();
    #endregion

    #region Singleton
    // The singleton instance of this script
    public static LDJ_CameraBehaviour Instance = null;
    #endregion

    #region Methods
    #region Original Methods
    // Makes the camera follow the transform target
    private void FollowTarget()
    {
        // Get the position compared to the target
        Vector3 _position = target.transform.position + offset;
        // Clamp the camera between bounds
        transform.position = new Vector3(Mathf.Clamp(_position.x, leftBound, rightBound), _position.y, Mathf.Clamp(_position.z, downBound, topBound));

        // Raycast from camera to target
        MeshRenderer[] _meshes = Physics.RaycastAll(transform.position, (target.transform.position - transform.position).normalized, Vector3.Distance(transform.position, target.transform.position)).Where(r => r.collider.GetComponent<MeshRenderer>() != null).Select(r => r.collider.GetComponent<MeshRenderer>()).ToArray();


        // Get all cumbersome meshes
        Dictionary<MeshRenderer, float> _newCumbersomeMeshes = cumbersomeMeshes;

        Color _meshColor = Color.white;

        // For each ray between the camera & the target, make them in transparancy
        foreach (MeshRenderer _mesh in _meshes)
        {
            if (_mesh && !cumbersomeMeshes.ContainsKey(_mesh))
            {
                _meshColor = _mesh.material.color;

                _newCumbersomeMeshes.Add(_mesh, _meshColor.a);

                _mesh.material.color = new Color(_meshColor.r, _meshColor.g, _meshColor.b, .25f);
            }
        }

        MeshRenderer[] _cumbersomeMeshes = cumbersomeMeshes.Select(c => c.Key).ToArray();

        // For each meshes no longer in between the camera and the target, set its transparancy back
        foreach (MeshRenderer _mesh in _cumbersomeMeshes)
        {
            if (!_meshes.Contains(_mesh))
            {
                _meshColor = _mesh.material.color;

                _mesh.material.color = new Color(_meshColor.r, _meshColor.g, _meshColor.b, cumbersomeMeshes[_mesh]);

                _newCumbersomeMeshes.Remove(_mesh);
            }
        }

        // Update the list of current cumbersome meshes
        cumbersomeMeshes = _newCumbersomeMeshes;
    }

    /// <summary>
    /// Sets the target to follow by the camera
    /// </summary>
    /// <param name="_target">Transform target to follow</param>
    public void SetTarget(Transform _target)
    {
        target = _target;
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Set the singleton instance as this if there is none or destroy it
        if (!Instance) Instance = this;
        else
        {
            Destroy(this);
        }

        // Get the camera component on this game object if needed
        if (!camera) camera = GetComponent<Camera>();
    }

    private void OnDestroy()
    {
        // Remove the static instance if this is this
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmos()
    {
        // Draws the bounds of the camera
        Gizmos.color = Color.yellow;

        // Draws the line delimiting the bounds
        Gizmos.DrawLine(new Vector3(leftBound, transform.position.y, downBound), new Vector3(rightBound, transform.position.y, downBound));
        Gizmos.DrawLine(new Vector3(leftBound, transform.position.y, topBound), new Vector3(rightBound, transform.position.y, topBound));
        Gizmos.DrawLine(new Vector3(leftBound, transform.position.y, downBound), new Vector3(leftBound, transform.position.y, topBound));
        Gizmos.DrawLine(new Vector3(rightBound, transform.position.y, downBound), new Vector3(rightBound, transform.position.y, topBound));

        // Draws the sphere at the bounds corners
        Gizmos.DrawSphere(new Vector3(leftBound, transform.position.y, downBound), .1f);
        Gizmos.DrawSphere(new Vector3(rightBound, transform.position.y, downBound), .1f);
        Gizmos.DrawSphere(new Vector3(leftBound, transform.position.y, topBound), .1f);
        Gizmos.DrawSphere(new Vector3(rightBound, transform.position.y, topBound), .1f);

        Gizmos.color = Color.white;
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        FollowTarget();
	}
    #endregion
    #endregion
}
