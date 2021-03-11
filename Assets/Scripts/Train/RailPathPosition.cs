using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RailPathPosition
{
    private const int LastSegmentsLength = 50;
    public List<RailSegment> LastSegments; // The last segments that the position has passed (LastSegments[0] = CurrentSegment)

    public RailSegment CurrentSegment;
    public float CurrentSegmentDistance; // The distance since the last path point

    public RailPathPosition(List<RailSegment> segments, float distance = 0f)
    {
        LastSegments = segments;
        CurrentSegment = LastSegments[0];
        CurrentSegmentDistance = distance;
    }

    // For temporary rail path position allocations
    public RailPathPosition(RailSegment segment, float distance)
    {
        CurrentSegment = segment;
        CurrentSegmentDistance = distance;
    }

    public Vector3 GetWorldPosition()
    {
        return CurrentSegment.GetWorldPositionAtDistance(CurrentSegmentDistance);
    }

    public void SetPosition(RailSegment segment, float distance)
    {
        if (segment != CurrentSegment)
        {
            CurrentSegment = segment;

            // Update last segments
            LastSegments.Insert(0, segment);
            if (LastSegments.Count > LastSegmentsLength) LastSegments.RemoveAt(LastSegments.Count - 1);

        }
        CurrentSegmentDistance = distance;
    }

    

    public void SetPosition(RailPathPosition position)
    {
        SetPosition(position.CurrentSegment, position.CurrentSegmentDistance);
    }

    /// <summary>
    /// Returns the segment and distance at a certain distance backwards from the positions front end.
    /// </summary>
    public RailPathPosition GetBackwardsPathPosition(float distance)
    {
        // Target position is on same segment
        if(distance <= CurrentSegmentDistance) return new RailPathPosition(CurrentSegment, CurrentSegmentDistance - distance);

        // Target position is not on same segment
        float distanceFromSegmentStart = distance - CurrentSegmentDistance;
        int segmentIndex = (int)(distanceFromSegmentStart / RailPathGenerator.RailSegmentLength);
        float restDistance = ((distanceFromSegmentStart / RailPathGenerator.RailSegmentLength) - segmentIndex) * RailPathGenerator.RailSegmentLength;

        segmentIndex++;
        restDistance = RailPathGenerator.RailSegmentLength - restDistance;

        RailPathPosition position = new RailPathPosition(new List<RailSegment>() { LastSegments[segmentIndex] }, restDistance);
        return position;
    }

}
