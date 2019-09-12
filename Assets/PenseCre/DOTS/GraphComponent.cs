using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct GraphComponent : IComponentData
{
    public static int function;
    public static int resolution;
    public static float speed;
    public static float offset;
    public Vector2Int id;
    public float scale;
}
