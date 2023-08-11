using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingNoteBehavior : NoteBehavior
{
    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public void Play()
    {
        if(isPlayed)  return;
        isPlayed = true;
        StartCoroutine(PlayNote());
        spriteRenderer.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

    }

}
