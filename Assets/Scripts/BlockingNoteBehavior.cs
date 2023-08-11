using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using UnityEngine;

public class BlockingNoteBehavior : NoteBehavior
{
    protected Vector3 expectedPosition;

    private List<Vector3> attackTarget;

    public virtual void SetExpectedPosition(Vector3 expectedPosition)
    {
        this.expectedPosition = expectedPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayed) return;
        isPlayed = true;

        foreach (var i in attackTarget)
        {
            BulletController bullet = BulletSpawner.instance.GetBullet();
            bullet.transform.position = transform.position;
            bullet.transform.right = (i - transform.position).normalized;
            bullet.SetTarget(i);
        }

        StartCoroutine(PlayNote());
        if (transform.CompareTag("block"))
        {
            collision.transform.position = expectedPosition;
        }

        spriteRenderer.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

    }

    public void AddFloatingNote(Vector3 target)
    {
        attackTarget.Add(target);
    }

    public override void SetNote(NoteOnEvent noteOnEvent, NoteOffEvent noteOffEvent, double timeOff)
    {
        base.SetNote(noteOnEvent, noteOffEvent, timeOff);
        attackTarget = new List<Vector3>();

        attackTarget.Clear();

    }
}
