using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RailPathPosition
{
    public RailSegment Segment;
    public float Distance; // The distance on the segment (is always between 0 and RailPathGenerator.RailSegmentLength)

    public RailPathPosition(RailSegment segment, float distance)
    {
        Segment = segment;
        Distance = distance;
    }

    public Vector3 GetWorldPosition()
    {
        return Segment.GetWorldPositionAtDistance(Distance);
    }

    public void SetPosition(RailPathPosition position)
    {
        Segment = position.Segment;
        Distance = position.Distance;
    }

}
