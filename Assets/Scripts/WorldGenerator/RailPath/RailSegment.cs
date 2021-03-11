using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSegment : MonoBehaviour
{
    public RailSettings Settings;
    public RailPathPoint FromPoint;
    public RailPathPoint ToPoint;
    public Vector3 SegmentVector;
    public float Angle;

    public void Init(RailPathPoint from, RailPathPoint to, RailSettings settings)
    {
        FromPoint = from;
        ToPoint = to;
        Settings = settings;

        SegmentVector = ToPoint.Position - FromPoint.Position;

        Angle = Vector2.SignedAngle(new Vector2(SegmentVector.x, SegmentVector.z), Vector2.up);
    } 
}
