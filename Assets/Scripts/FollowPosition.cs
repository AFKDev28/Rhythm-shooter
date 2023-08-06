using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] private Transform followObject;

    private void FixedUpdate()
    {
        this.transform.position = new Vector3(followObject.position.x, followObject.position.y, this.transform.position.z);
    }
}
