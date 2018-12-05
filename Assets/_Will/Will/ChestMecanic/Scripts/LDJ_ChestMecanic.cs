using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDJ_ChestMecanic : MonoBehaviour
{
    #region F/P
    [SerializeField]
    GameObject fxToPlay;
    [SerializeField]
    GameObject chestToAnim;
    [SerializeField]
    bool isOpen = false;
    public bool IsOpen { get { return isOpen; } }
    #region Object To Drop
    [SerializeField]
    GameObject rootPlaceToDrop;
    [SerializeField]
    List<GameObject> allPossibleObjectToDrop = new List<GameObject>();
    [SerializeField]
    float powerKick = 1f;
    #endregion
    #endregion

    #region F/P
    void Init()
    {
        fxToPlay.SetActive(false);
    }
    void ChooseAndDropObject()
    {
        if (!IsOpen) return;
        if (allPossibleObjectToDrop.Count <= 0)
        {
            Debug.Log("List Empty");
            return;
        }
        Debug.Log("take it" + allPossibleObjectToDrop.Count);
        GameObject _objectToDrop = allPossibleObjectToDrop[Random.Range(0,allPossibleObjectToDrop.Count)];
        Rigidbody _rb = Instantiate(_objectToDrop, rootPlaceToDrop.transform.position , Quaternion.identity).GetComponent<Rigidbody>();
        _rb.velocity = (Vector3.forward * -powerKick/2) + (Vector3.up * powerKick);

        //AkSoundEngine.PostEvent("Play_Chest", gameObject);
    }
    public void MakeInteraction()
    {
        chestToAnim.GetComponent<Animator>().SetTrigger("PlayAnimation");
        fxToPlay.SetActive(true);
        isOpen = true;
        ChooseAndDropObject();
    }

    #endregion

    #region UniMeths
    private void Start()
    {
        Init();
    }
    #endregion
}
