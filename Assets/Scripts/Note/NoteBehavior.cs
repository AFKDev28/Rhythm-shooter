using System;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class NoteBehavior : MonoBehaviour
{
    [SerializeField] protected NoteColor noteColor;

    protected Action<NoteBehavior> _killAction;
    protected Action<NoteOnEvent> _NoteOnAction;
    protected Action<NoteOffEvent> _NoteOffAction;


    NoteOnEvent noteOnEvent;
    NoteOffEvent noteOffEvent;
    protected double timeOff;

    protected bool isPlayed = false;

    protected Collider2D collider;
    protected SpriteRenderer spriteRenderer;


    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void InIt(Action<NoteBehavior> killAction ,Action<NoteOnEvent> noteOnAction, Action<NoteOffEvent> noteOffAction )
    {
        _killAction = killAction;
        _NoteOnAction = noteOnAction;
        _NoteOffAction = noteOffAction;
    }
    public virtual void SetNote(NoteOnEvent noteOnEvent, NoteOffEvent noteOffEvent, double timeOff)
    {
        isPlayed = false;
        spriteRenderer.color = new Color(0.1f, 0.1f, 0.1f);

        this.noteOnEvent = noteOnEvent;
        this.noteOffEvent = noteOffEvent;
        this.timeOff = timeOff;
        spriteRenderer.sprite = noteColor.sprites[Random.Range(0, noteColor.sprites.Length - 1)];
    }


    protected IEnumerator PlayNote()
    {

        spriteRenderer.color = noteColor.color[Random.Range(0, noteColor.color.Length - 1)];

        _NoteOnAction(noteOnEvent);
        yield return new WaitForSeconds((float)timeOff);
        _NoteOffAction(noteOffEvent);
        yield return null;
        _killAction(this);
    }


}
