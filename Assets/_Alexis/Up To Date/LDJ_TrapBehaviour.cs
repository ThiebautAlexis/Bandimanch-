using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LDJ_TrapBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider _collider)
    {
        LDJ_GoblinAgent _goblin; 
        if(_goblin =_collider.GetComponent<LDJ_GoblinAgent>())
        {
            _goblin.HitAgent(); 
        }
    }

}
