using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof( Rigidbody2D))]
public class BallController : MonoBehaviour
{

    public static BallController instance { get; private set; }

    public Rigidbody2D rb;
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
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        rb.velocity = new Vector2(2, 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);

    }
}
