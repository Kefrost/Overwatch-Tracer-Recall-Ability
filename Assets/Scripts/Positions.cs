using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positions : MonoBehaviour
{
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }

    public Positions(Vector3 position, Vector3 rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}
