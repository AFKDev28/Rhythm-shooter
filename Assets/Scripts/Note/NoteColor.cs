using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NoteColor", order = 1)]

public class NoteColor : ScriptableObject
{   
    public Color[] color;
    public Sprite[] sprites;
}
