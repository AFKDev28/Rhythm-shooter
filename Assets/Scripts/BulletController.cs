using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public static float raycastLength = 0.2f;
     public static float movSpeed = 10.0f;
    [SerializeField] LayerMask raycastMask;

    protected Action<BulletController> _killAction;
    protected Vector3 target;
    public void InIt(Action<BulletController> killAction)
    {
        _killAction = killAction;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * raycastLength);
     
    }
    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }
    private void Update()
    {
        transform.position +=  transform.right * movSpeed * Time.deltaTime;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, transform.right,raycastLength, raycastMask);
        if(raycastHit.collider != null && raycastHit.collider.transform.position == target)
        {
            if (raycastHit.collider.TryGetComponent(out FloatingNoteBehavior valueInfo) )
            {
               
                valueInfo.Play();
                _killAction(this);

            }
        }
    }
}
