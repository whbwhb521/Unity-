using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Testrun : MonoBehaviour
{
    public Transform TargetObject;
    void Start()
    {
        
    }
    void Update()
    {
        if (TargetObject != null)
        {
            GetComponent<NavMeshAgent>().destination = TargetObject.position;
        }
    }
}
