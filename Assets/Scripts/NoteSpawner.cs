using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Pool;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Core;
using Unity.Mathematics;
using Melanchall.DryWetMidi.Common;
using Random = UnityEngine.Random;
using System.Collections.ObjectModel;

public class NoteSpawner : MonoBehaviour
{
    private IObjectPool<NoteBehavior> blNotePool;
    [SerializeField] private NoteBehavior blNotePrefab;

    private IObjectPool<NoteBehavior> flNotePool;
    [SerializeField] private NoteBehavior flNotePrefab;


    private string midiFilePath = Path.Combine(Application.streamingAssetsPath, "AllFallDown.mid");
    //private string midiFilePath = Path.Combine(Application.streamingAssetsPath, "AllFallDown.mid");
    //private string fileName = "AllFallDown.mid";

    TempoMap tempoMap;
    List<NoteData> notes;

    [SerializeField] private float noteVolume = 1.0f;

    private float startTime;
    private int currentNote;
    private bool canSpawn = false;
    private Vector2 ballVelocity;
    private Vector3 lastBallPosition;
    private float lastNoteTime;
    private bool isOddNote;
    private Vector2 offSet;
    private int repeatTime;
    BlockingNoteBehavior musicnote = null;

    private float floatnoteOffset;
    void Start()
    {
        blNotePool = new ObjectPool<NoteBehavior>(() =>
        {
            NoteBehavior note = Instantiate(blNotePrefab);
            note.InIt(KillNote);
            return note;
        }
            , note => note.gameObject.SetActive(true)
        , note => note.gameObject.SetActive(false)
        , note => Destroy(note.gameObject)
        , false, 50);

        flNotePool = new ObjectPool<NoteBehavior>(() =>
        {
            NoteBehavior note = Instantiate(flNotePrefab);
            note.InIt(KillNote);

            return note;
        }
            , note => note.gameObject.SetActive(true)
            , note => note.gameObject.SetActive(false)
            , note => Destroy(note.gameObject)
            , false, 50);

        LoadDataFromMidiFile();
    }

    private void StartSpawn()
    {
        //reset atribute
        startTime = Time.time - 3.0f;
        canSpawn = true;
        currentNote = 0;
        ballVelocity = BallController.instance.rb.velocity;
        repeatTime = 0;
        //get the size of note and block
        Vector2 noteSize = blNotePrefab.GetComponent<BoxCollider2D>().size;
        Vector2 ballSize = BallController.instance.GetComponent<BoxCollider2D>().size;
       float floatnoteSize = flNotePrefab.GetComponent<CircleCollider2D>().radius; 
        offSet = new Vector2((ballSize.x + noteSize.x) / 2, ballSize.y);

        floatnoteOffset = BulletController.raycastLength + floatnoteSize;

        lastBallPosition = BallController.instance.rb.position + ballVelocity * 3.0f;


        lastNoteTime = -1;
        isOddNote = false;
    }
    private void Update()
    {
        if (canSpawn)
        {
            SpawnNotes();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            StartSpawn();
        }
    }

    private void SpawnNotes()
    {
        while (currentNote < notes.Count && notes[currentNote].startTime < Time.time - startTime)
        {
            if (notes[currentNote].type == NoteType.StaticBlock)
            {
                // get note from pool

                NoteData noteData = notes[currentNote];
                
                musicnote = (BlockingNoteBehavior)blNotePool.Get();
                //set NoteOnEvent, NoteOff Event and playtime for note
                float NoteOnVolume = (int)noteData.note.Velocity * noteVolume;
                float NoteOffVolume = (int)noteData.note.OffVelocity * noteVolume;
                NoteOnEvent noteOnEvent = new NoteOnEvent(noteData.note.NoteNumber, (SevenBitNumber)math.clamp(0, 127, (int)NoteOnVolume));
                NoteOffEvent noteOffEvent = new NoteOffEvent(noteData.note.NoteNumber, (SevenBitNumber)math.clamp(0, 127, (int)NoteOffVolume));
                double playTime = noteData.note.TimeAs<MetricTimeSpan>(tempoMap).TotalSeconds;

                musicnote.SetNote(noteOnEvent, noteOffEvent, playTime);

                // Set Position for note
                Vector3 noteOffSet = Vector3.zero;
                if (lastNoteTime != noteData.startTime)
                {
                    musicnote.transform.tag = "block";

                    isOddNote = !isOddNote;
                    lastBallPosition += new Vector3(ballVelocity.x * (isOddNote ? 1 : -1), ballVelocity.y, 0) * (noteData.startTime - lastNoteTime);
                }
                else
                {
                    musicnote.transform.tag = "blockduplicated";
                }


                // caculates offset X

                noteOffSet.x = offSet.x * (isOddNote ? 1 : -1);

                // caculates offset Y
                int nextnote = currentNote;
                while (nextnote < notes.Count)
                {
                    if(notes[nextnote].startTime != noteData.startTime && notes[nextnote].type == NoteType.StaticBlock)
                    {
                        noteOffSet.y = -ballVelocity.y * (notes[nextnote].startTime - noteData.startTime) + offSet.y;
                        noteOffSet.y = Mathf.Clamp(noteOffSet.y, 0, offSet.y);
                        break;
                    }
                    nextnote++;
                    
                }

                musicnote.SetExpectedPosition(lastBallPosition);
                musicnote.transform.position = lastBallPosition + noteOffSet;

                lastNoteTime = noteData.startTime;
            }

            else if (notes[currentNote].type == NoteType.other) 
            {
                if (musicnote != null)
                {
                    NoteData noteData = notes[currentNote];

                    NoteBehavior floatingmusicnote = flNotePool.Get();
                    //set NoteOnEvent, NoteOff Event and playtime for note
                    float NoteOnVolume = (int)noteData.note.Velocity * noteVolume;
                    float NoteOffVolume = (int)noteData.note.OffVelocity * noteVolume;
                    NoteOnEvent noteOnEvent = new NoteOnEvent(noteData.note.NoteNumber, (SevenBitNumber)math.clamp(0, 127, (int)NoteOnVolume));
                    NoteOffEvent noteOffEvent = new NoteOffEvent(noteData.note.NoteNumber, (SevenBitNumber)math.clamp(0, 127, (int)NoteOffVolume));
                    double playTime = noteData.note.TimeAs<MetricTimeSpan>(tempoMap).TotalSeconds;

                    floatingmusicnote.SetNote(noteOnEvent, noteOffEvent, playTime);

                    // Set Position for note
                    float x = (isOddNote ? 1 : -1) * Random.Range(0.0f, 1.0f),
                        y = Random.Range(-1.0f, 1.0f);
                    Vector3 random = new Vector3((isOddNote ? 1 : -1) * Random.Range(0.0f, 1.0f),
                        y = Random.Range(-1.0f, 1.0f), 0).normalized;
                    floatingmusicnote.transform.position = musicnote.transform.position
                        + random * ((noteData.startTime - lastNoteTime) * BulletController.movSpeed + floatnoteOffset);

                    musicnote.AddFloatingNote(floatingmusicnote.transform.position);
                }
            }
            currentNote++;
        }
    }

   
    private void LoadDataFromMidiFile()
    {
        MidiFile midiFile = MIDIManager.instance.GetMIDIFromFile(midiFilePath);
        tempoMap = midiFile.GetTempoMap();
        bool isFirstTrackTruck = true;
        notes  = new List<NoteData>();
        notes.Clear();
        foreach (var tracktruck in midiFile.GetTrackChunks())
        {
            foreach (var note in tracktruck.GetNotes())
            {
                float totalTime = (float)note.TimeAs<MetricTimeSpan>(tempoMap).TotalSeconds;

                if (isFirstTrackTruck)
                {
                    notes.Add(new NoteData(NoteType.StaticBlock, note, totalTime));
                }
                else
                {
                    notes.Add(new NoteData(NoteType.other, note, totalTime));
                }
            }

            isFirstTrackTruck = false;
        }
        notes.Sort((x, y) => x.startTime.CompareTo(y.startTime));
    }


    private void KillNote(NoteBehavior note)
    {
        if(note.CompareTag("floatnote"))
            flNotePool.Release(note);
        else blNotePool.Release(note);

    }
}

public enum NoteType
{
    StaticBlock,
    other
}

public struct NoteData
{
    public NoteType type;
    public Note note;
    public float startTime;

    public NoteData(NoteType type, Note note, float startTime)
    {
        this.type = type;
        this.note = note;
        this.startTime = startTime;
    }
}

