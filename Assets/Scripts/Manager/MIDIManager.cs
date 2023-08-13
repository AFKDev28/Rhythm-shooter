using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
public class MIDIManager : MonoBehaviour
{
    //Output Device to play midi note
    private const string OutputDeviceName = "Microsoft GS Wavetable Synth";
    private OutputDevice _outputDevice;

    //full track of the midi
    private Playback _playback;


    private void Awake()
    {
            InitializeOutputDevice();
    }


    public MidiFile GetMIDIFromFile(string filePath)
    {
        MidiFile  midiFile = MidiFile.Read(filePath);
        //ReadNoteFromMIDI();
        return midiFile;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Releasing playback and device...");

        if (_playback != null)
        {
            _playback.Dispose();
        }

        if (_outputDevice != null)
            _outputDevice.Dispose();

        Debug.Log("Playback and device released.");
    }


    private void InitializeOutputDevice()
    {
        Debug.Log($"Initializing output device [{OutputDeviceName}]...");

        var allOutputDevices = OutputDevice.GetAll();
        if (!allOutputDevices.Any(d => d.Name == OutputDeviceName))
        {
            var allDevicesList = string.Join(Environment.NewLine, allOutputDevices.Select(d => $"  {d.Name}"));
            Debug.Log($"There is no [{OutputDeviceName}] device presented in the system. " +
                $"Here the list of all device:{Environment.NewLine}{allDevicesList}");
            return;
        }

        _outputDevice = OutputDevice.GetByName(OutputDeviceName);
        Debug.Log($"Output device [{OutputDeviceName}] initialized.");
    }




    public void NoteOn(NoteOnEvent noteOnEvent)
    {
        _outputDevice?.SendEvent(noteOnEvent);
    }

    public void NoteOff(NoteOffEvent noteOffEvent)
    {
        _outputDevice?.SendEvent(noteOffEvent);
    }

}
