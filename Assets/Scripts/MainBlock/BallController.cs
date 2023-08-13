using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof( Rigidbody2D))]
public class BallController : MonoBehaviour
{

    public static BallController instance { get; private set; }

    public Rigidbody2D rb;
    public BoxCollider2D collider2D;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        rb.velocity = new Vector2(4, 4);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.CompareTag("block"))
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);

    }
}
