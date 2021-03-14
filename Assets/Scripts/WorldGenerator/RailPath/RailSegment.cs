using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSegment : MonoBehaviour
{
    public RailSettings Settings;
    public RailPathPoint FromPoint;
    public RailPathPoint ToPoint;
    public List<RailSegment> ConnectedSegments;
    public Vector3 SegmentVector;
    public float Angle;

    public void Init(RailPathPoint from, RailPathPoint to, RailSettings settings)
    {
        FromPoint = from;
        ToPoint = to;
        Settings = settings;

        SegmentVector = ToPoint.Position - FromPoint.Position;

        Angle = Vector2.SignedAngle(new Vector2(SegmentVector.x, SegmentVector.z), Vector2.up);
        Angle = (Angle > 180) ? Angle + 360 : Angle;

        FromPoint.Segments.Add(this);
        ToPoint.Segments.Add(this);
        ConnectedSegments = new List<RailSegment>();
    }

    public Vector3 GetWorldPositionAtDistance(float distance)
    {
        return Vector3.Lerp(FromPoint.Position, ToPoint.Position, distance / RailPathGenerator.RailSegmentLength);
    }

    /// <summary>
    /// Returns the segment in the direction when coming from "previousSegment"
    /// </summary>
    public RailSegment GetNextSegment(RailSegment previousSegment)
    {
        // can only handle straights
        foreach(RailSegment segment in ConnectedSegments)
        {
            if (segment != previousSegment) return segment;
        }
        return null;
    }
}
