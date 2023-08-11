using System;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class NoteBehavior : MonoBehaviour
{
    protected Action<NoteBehavior> _killAction;

    NoteOnEvent noteOnEvent;
    NoteOffEvent noteOffEvent;
    protected double timeOff;

    protected bool isPlayed = false;

    [SerializeField] protected List<Sprite> sprites;

    protected Collider2D collider;
    protected SpriteRenderer spriteRenderer;


    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void InIt(Action<NoteBehavior> killAction)
    {
        _killAction = killAction;
    }
    public virtual void SetNote(NoteOnEvent noteOnEvent, NoteOffEvent noteOffEvent, double timeOff)
    {
        isPlayed = false;
        this.noteOnEvent = noteOnEvent;
        this.noteOffEvent = noteOffEvent;
        this.timeOff = timeOff;
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count - 1)];
    }


    protected IEnumerator PlayNote()
    {
        MIDIManager.instance.NoteOn(noteOnEvent);
        yield return new WaitForSeconds((float)timeOff);
        MIDIManager.instance.NoteOff(noteOffEvent);

        yield return null;
        _killAction(this);
    }


}
