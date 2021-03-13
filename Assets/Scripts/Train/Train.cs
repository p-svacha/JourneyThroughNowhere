using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Train : MonoBehaviour
{
    private const float WagonConnectionLength = 2.5f;

    public RailPathPosition RailPosition;

    private const int LastSegmentsLength = 50;
    public List<RailSegment> LastSegments; // The last segments that the position has passed (LastSegments[0] = CurrentSegment)

    private const int UpcomingSegmentsLength = 30;
    public List<RailSegment> UpcomingSegments; // The upcoming segments the train will pass on its current path (UpcomingSegments[0] = CurrentSegment)

    public List<Wagon> Wagons = new List<Wagon>();

    public float VelocityKph;
    public float FrameDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (VelocityKph != 0)
        {
            float mps = VelocityKph / 3.6f;
            FrameDistance = Time.deltaTime * mps;
            RailPathPosition nextPosition = null;
            if (FrameDistance > 0) nextPosition = GetForwardsPathPosition(RailPosition, FrameDistance);
            else if (FrameDistance < 0) nextPosition = GetBackwardsPathPosition(RailPosition, -FrameDistance);
            SetPosition(nextPosition);
        }
        else SetPosition(RailPosition);
    }

    public void Init(List<RailSegment> lastSegments)
    {
        LastSegments = lastSegments;
        RailPosition = new RailPathPosition(LastSegments[0], 0f);
        UpcomingSegments = new List<RailSegment>();
        UpcomingSegments.Add(RailPosition.Segment);

        // Init upcoming segments
        while (UpcomingSegments.Count < UpcomingSegmentsLength)
        {
            RailSegment furthest = null;
            RailSegment secondFurthest = null;
            if (UpcomingSegments.Count == 1)
            {
                furthest = RailPosition.Segment;
                secondFurthest = LastSegments[1];
            }
            else
            {
                furthest = UpcomingSegments[UpcomingSegments.Count - 1];
                secondFurthest = UpcomingSegments[UpcomingSegments.Count - 2];
            }

            UpcomingSegments.Add(furthest.GetNextSegment(secondFurthest));
        }

        UpdatePosition();
    }

    public void SetPosition(RailPathPosition newPos)
    {
        // Train position
        if (newPos.Segment != RailPosition.Segment)
        {
            UpdateAdjacentSegments(newPos.Segment);
        }
        RailPosition.SetPosition(newPos);

        // Wagon positions
        float tmpDistance = 0f;
        foreach (Wagon w in Wagons)
        {
            RailPathPosition wagonPosition = GetBackwardsPathPosition(RailPosition, tmpDistance);
            w.SetPosition(wagonPosition);
            tmpDistance += Wagon.Length + WagonConnectionLength;
        }
        

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float ratio = RailPosition.Distance / RailPathGenerator.RailSegmentLength;
        Vector3 position = Vector3.Lerp(RailPosition.Segment.FromPoint.Position, RailPosition.Segment.ToPoint.Position, ratio);
        transform.position = position;
    }

    public Wagon AddWagon()
    {
        GameObject wagonObject = new GameObject("Wagon");
        Wagon wagon = wagonObject.AddComponent<Wagon>();

        float wagonDistance = Wagons.Sum(x => Wagon.Length) + (Wagons.Count * WagonConnectionLength);
        RailPathPosition wagonPosition = GetBackwardsPathPosition(RailPosition, wagonDistance);
        wagon.InitWagon(this, wagonPosition);


        Wagons.Add(wagon);
        wagon.transform.SetParent(transform);
        wagon.transform.localPosition = Vector3.zero;

        UpdatePosition();

        return wagon;
    }

    private void UpdateAdjacentSegments(RailSegment newSegment)
    {
        if (newSegment == RailPosition.Segment) throw new System.Exception("UpdateAdjacentSegments was called even though the current segment didn't change!");
        if (UpcomingSegments.Contains(newSegment) && LastSegments.Contains(newSegment)) throw new System.Exception("New segment found in both last and upcoming segments, that doesn't make any sense.");
        if (!UpcomingSegments.Contains(newSegment) && !LastSegments.Contains(newSegment)) throw new System.Exception("New segment was not found in either last or upcoming segments.");

        // Segment is an upcoming one => train going forwards
        if(UpcomingSegments.Contains(newSegment))
        {
            // Handle passed segments
            int segmentIndex = UpcomingSegments.IndexOf(newSegment);
            for (int i = 0; i < segmentIndex; i++)
            {
                UpcomingSegments.RemoveAt(0);
                LastSegments.Insert(0, UpcomingSegments[0]);
            }

            // Adjust length of last segments
            while(LastSegments.Count > LastSegmentsLength) LastSegments.RemoveAt(LastSegments.Count - 1);

            // Adjust length of upcoming segments
            while (UpcomingSegments.Count < UpcomingSegmentsLength)
            {
                RailSegment furthest = UpcomingSegments[UpcomingSegments.Count - 1];
                RailSegment secondFurthest = UpcomingSegments[UpcomingSegments.Count - 2];

                UpcomingSegments.Add(furthest.GetNextSegment(secondFurthest));
            }
        }

        // Segment is a past one => train going backwards
        else if(LastSegments.Contains(newSegment)) 
        {
            int segmentIndex = LastSegments.IndexOf(newSegment);
            for (int i = 0; i < segmentIndex; i++)
            {
                LastSegments.RemoveAt(0);
                UpcomingSegments.Insert(0, LastSegments[0]);
            }

            // Adjust length of last segments
            while (LastSegments.Count < LastSegmentsLength)
            {
                RailSegment last = LastSegments[LastSegments.Count - 1];
                RailSegment secondLast = LastSegments[LastSegments.Count - 2];

                LastSegments.Add(last.GetNextSegment(secondLast));
            }

            // Adjust length of upcoming segments
            while (UpcomingSegments.Count > UpcomingSegmentsLength) UpcomingSegments.RemoveAt(UpcomingSegments.Count - 1);
        }
    }

    /// <summary>
    /// Returns the segment and distance at a certain distance forwards from the source position
    /// </summary>
    public RailPathPosition GetForwardsPathPosition(RailPathPosition sourcePosition, float distance)
    {
        // Target position is on same segment
        if (distance < RailPathGenerator.RailSegmentLength - sourcePosition.Distance) return new RailPathPosition(sourcePosition.Segment, sourcePosition.Distance + distance);

        // Target position is not on same segment
        float distanceFromSegmentStart = distance - (RailPathGenerator.RailSegmentLength - sourcePosition.Distance);
        int nSegmentSkips = (int)(distanceFromSegmentStart / RailPathGenerator.RailSegmentLength);
        float restDistance = ((distanceFromSegmentStart / RailPathGenerator.RailSegmentLength) - nSegmentSkips) * RailPathGenerator.RailSegmentLength;

        nSegmentSkips++;

        int sourceSegmentIndex = UpcomingSegments.IndexOf(sourcePosition.Segment);
        RailPathPosition position = new RailPathPosition(UpcomingSegments[sourceSegmentIndex + nSegmentSkips], restDistance);
        return position;
    }

    /// <summary>
    /// Returns the segment and distance at a certain distance backwards from the source position
    /// </summary>
    public RailPathPosition GetBackwardsPathPosition(RailPathPosition sourcePosition, float distance)
    {
        // Target position is on same segment
        if (distance <= sourcePosition.Distance) return new RailPathPosition(sourcePosition.Segment, sourcePosition.Distance - distance);

        // Target position is not on same segment
        float distanceFromSegmentStart = distance - sourcePosition.Distance;
        int nSegmentSkips = (int)(distanceFromSegmentStart / RailPathGenerator.RailSegmentLength);
        float restDistance = ((distanceFromSegmentStart / RailPathGenerator.RailSegmentLength) - nSegmentSkips) * RailPathGenerator.RailSegmentLength;

        nSegmentSkips++;
        restDistance = RailPathGenerator.RailSegmentLength - restDistance;

        int sourceSegmentIndex = LastSegments.IndexOf(sourcePosition.Segment);
        RailPathPosition position = new RailPathPosition(LastSegments[sourceSegmentIndex + nSegmentSkips], restDistance);
        return position;
    }
}
