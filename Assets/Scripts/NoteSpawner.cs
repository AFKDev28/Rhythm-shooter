using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Pool;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Core;
using System.Linq;
using Unity.Mathematics;
using Melanchall.DryWetMidi.Common;

public class NoteSpawner : MonoBehaviour
{
    private IObjectPool<NoteBehavior> notePool;
    public int amountToPool;
    [SerializeField] private NoteBehavior noteBehaviorPrefab;

    private Vector2 extraSize;
    //private string midiFilePath = Path.Combine(Application.streamingAssetsPath, "GravityFalls.mid");
    private string midiFilePath = Path.Combine(Application.streamingAssetsPath, "AllFallDown.mid");
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

    void Start()
    {
        notePool = new ObjectPool<NoteBehavior>(() =>
        {
            return Instantiate(noteBehaviorPrefab);
        }, note => note.gameObject.SetActive(true)
        , note => note.gameObject.SetActive(false)
        , note => Destroy(note.gameObject)
        , false, 50);
        LoadDataFromMidiFile();
    }

    private void StartSpawn()
    {
        //reset atribute
        startTime = Time.time - 5.0f;
        canSpawn = true;
        currentNote = 0;
        ballVelocity = BallController.instance.rb.velocity;
        repeatTime = 0;
        //get the size of note and block
        Vector2 noteSize = noteBehaviorPrefab.GetComponent<BoxCollider2D>().size;
        Vector2 ballSize = BallController.instance.GetComponent<BoxCollider2D>().size;
        offSet = new Vector2((ballSize.x + noteSize.x) / 2, ballSize.y);
        lastBallPosition = BallController.instance.rb.position + ballVelocity * 5.0f;


        lastNoteTime = -1;
        isOddNote = false;
    }
    private void Update()
    {
        if (canSpawn)
        {
            while (currentNote < notes.Count && notes[currentNote].startTime < Time.time - startTime  )
            {
                if (notes[currentNote].type == NoteType.StaticBlock)
                {
                    // get note from pool

                    NoteData noteData = notes[currentNote];

                    NoteBehavior musicnote = notePool.Get();

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
                    while ((nextnote < notes.Count) && (notes[nextnote].startTime == noteData.startTime || notes[nextnote].type != NoteType.StaticBlock))
                    {
                        nextnote++;
                    }

                    

                    if (nextnote < notes.Count)
                    {

                        noteOffSet.y =  - ballVelocity.y * (notes[nextnote].startTime - noteData.startTime) + offSet.y;
                        noteOffSet.y = Mathf.Clamp(noteOffSet.y, 0, offSet.y);
                        Debug.Log(noteOffSet.y);
                    }

                    musicnote.SetExpectedPosition(lastBallPosition);
                    musicnote.transform.position = lastBallPosition + noteOffSet;


                    lastNoteTime = noteData.startTime;

                }

                else
                {

                }
                currentNote++;


            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            StartSpawn();
        }

        
    }

    private void LoadDataFromMidiFile()
    {
        MidiFile midiFile = MIDIManager.instance.GetMIDIFromFile(midiFilePath);
        tempoMap = midiFile.GetTempoMap();
        bool isFirstTrackTruck = true;
        notes = new List<NoteData>();
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
            notes.OrderBy(x => x.startTime);
            
            isFirstTrackTruck = false;
        }
    }


    private void KillNote(NoteBehavior note)
    {
        Destroy(note.gameObject);
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

