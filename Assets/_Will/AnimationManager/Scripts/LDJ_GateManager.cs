using UnityEngine;

public class LDJ_GateManager : MonoBehaviour
{
    #region F/P
    #region Interuptor
    [SerializeField, Header("Interuptor")]
    GameObject interuptor;
    [SerializeField]
    int weightNeeded = 3;
    [SerializeField]
    bool isActif = false;
    [SerializeField]
    bool isHeavy = true;
    [SerializeField]
    bool isTrap = false;
    #endregion
    #region GizmoSettings
    [SerializeField, Header("Gizmo")]
    int radiusGizmo = 1;
    [SerializeField]
    float rangeGizmo = 1;
    [SerializeField]
    float heightGizmo = 1;
    [SerializeField]
    Color actifColor;
    [SerializeField]
    Color unactifColor;
    #endregion
    #region Player
    [SerializeField]
    LDJ_Player playerAdventurer;
    #endregion
    #region InteractibleObject
    [SerializeField,Header("Interactible object")]
    GameObject objectToInteract;
    #endregion
    #endregion

    #region Meths
    void GetInventoryWeight()
    {
        if (!playerAdventurer) return;
        if (!isTrap)
        {
            if (isHeavy) isActif = playerAdventurer.Weight >= weightNeeded ? true : false;
            if (!isHeavy) isActif = playerAdventurer.Weight <= weightNeeded ? true : false;
        }
        if (isTrap) isActif = true;
        if (isActif) MakeInteraction();
    }
    void MakeInteraction()
    {
        Debug.Log(isActif);
        objectToInteract.GetComponent<Animator>().SetTrigger("PlayAnimation");
    }
    #endregion

    #region UniMeths
    private void OnDrawGizmos()
    {
        if (!interuptor) return;
        Vector3 _Distance = new Vector3(0, heightGizmo, rangeGizmo);
        Gizmos.color = isActif? actifColor : unactifColor;
        Gizmos.DrawSphere(interuptor.transform.position+(_Distance), radiusGizmo);
        Gizmos.color = Color.white;
    }

    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider.GetComponent<LDJ_Player>())
        {
            Debug.Log("in");
            GetInventoryWeight();
        }
    }

    private void OnTriggerExit(Collider _collider)
    {
        if (!isActif) return;
        if (_collider.GetComponent<LDJ_Player>())
        {
            Debug.Log("out");
            isActif = false;
        }
    }

    private void Start()
    {
        if (!playerAdventurer) playerAdventurer = FindObjectOfType<LDJ_Player>();
    }
    #endregion
}