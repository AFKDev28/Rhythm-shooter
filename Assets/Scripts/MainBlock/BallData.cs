using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BallData", order = 1)]
public class BallData : ScriptableObject
{
    public Vector2 velocity;
}