using System.Collections;
using Melanchall.DryWetMidi.Core;
using UnityEngine;

public class NoteBehavior : MonoBehaviour
{
    NoteOnEvent noteOnEvent;
    NoteOffEvent noteOffEvent;
    private double timeOff;


    public void SetNote(NoteOnEvent noteOnEvent, NoteOffEvent noteOffEvent , double timeOff)
    {
        this.noteOnEvent = noteOnEvent;
        this.noteOffEvent = noteOffEvent;
        this.timeOff = timeOff;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(PlayNote());
    }

    private IEnumerator PlayNote()
    {
        MIDIManager.instance.NoteOn(noteOnEvent);
        yield return new WaitForSeconds((float)timeOff);
        MIDIManager.instance.NoteOff(noteOffEvent);

        yield return null;
        Destroy(gameObject,3.0f);
    }
}
