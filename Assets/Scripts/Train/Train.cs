using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Train : MonoBehaviour
{
    private const float WagonConnectionLength = 2.5f;

    public RailPathPosition RailPosition;
    private const int UpcomingSegmentsLength = 30;
    public List<RailSegment> UpcomingSegments; // The upcoming segments the train will pass on its current path (UpcomingSegments[0] = CurrentSegment)

    public List<Wagon> Wagons = new List<Wagon>();

    public float Velocity = 2; // kph

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Velocity != 0)
        {
            float mps = Velocity / 3.6f;
            float distance = Time.deltaTime * mps;
            RailPathPosition nextPosition = null;
            if (distance > 0) nextPosition = GetForwardsPathPosition(distance);
            else if(distance < 0) nextPosition = RailPosition.GetBackwardsPathPosition(distance);
            SetPosition(nextPosition);
        }
    }

    public void Init(List<RailSegment> segments)
    {
        RailPosition = new RailPathPosition(segments, 0f);
        UpcomingSegments = new List<RailSegment>();
        UpcomingSegments.Add(RailPosition.CurrentSegment);
        UpdateUpcomingSegments();
        UpdatePosition();
    }

    public void SetPosition(RailPathPosition newPos)
    {
        if (newPos.CurrentSegment != RailPosition.CurrentSegment)
        {
            UpcomingSegments.RemoveAt(0);
            UpdateUpcomingSegments();
        }
        RailPosition.SetPosition(newPos);
        

        
        float tmpDistance = 0f;
        foreach (Wagon w in Wagons)
        {
            RailPathPosition wagonPosition = RailPosition.GetBackwardsPathPosition(tmpDistance);
            w.SetPosition(wagonPosition);
            tmpDistance += Wagon.Length + WagonConnectionLength;
        }
        

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float ratio = RailPosition.CurrentSegmentDistance / RailPathGenerator.RailSegmentLength;
        Vector3 position = Vector3.Lerp(RailPosition.CurrentSegment.FromPoint.Position, RailPosition.CurrentSegment.ToPoint.Position, ratio);
        transform.position = position;
    }

    public Wagon AddWagon()
    {
        GameObject wagonObject = new GameObject("Wagon");
        Wagon wagon = wagonObject.AddComponent<Wagon>();

        float wagonDistance = Wagons.Sum(x => Wagon.Length) + (Wagons.Count * WagonConnectionLength);
        RailPathPosition wagonPosition = RailPosition.GetBackwardsPathPosition(wagonDistance);
        List<RailSegment> wagonSegments = new List<RailSegment>();
        int segmentIndex = RailPosition.LastSegments.IndexOf(wagonPosition.CurrentSegment);
        for (int i = segmentIndex; i < RailPosition.LastSegments.Count; i++) wagonSegments.Add(RailPosition.LastSegments[i]);
        wagon.InitWagon(this, wagonSegments, wagonPosition.CurrentSegmentDistance);


        Wagons.Add(wagon);
        wagon.transform.SetParent(transform);
        wagon.transform.localPosition = Vector3.zero;

        UpdatePosition();

        return wagon;
    }

    private void UpdateUpcomingSegments()
    {
        while (UpcomingSegments.Count < UpcomingSegmentsLength)
        {
            RailSegment furthest = null;
            RailSegment secondFurthest = null;
            if (UpcomingSegments.Count == 1)
            {
                furthest = RailPosition.CurrentSegment;
                secondFurthest = RailPosition.LastSegments[1];
            }
            else
            {
                furthest = UpcomingSegments[UpcomingSegments.Count - 1];
                secondFurthest = UpcomingSegments[UpcomingSegments.Count - 2];
            }

            UpcomingSegments.Add(furthest.GetNextSegment(secondFurthest));
        }
    }

    /// <summary>
    /// Returns the segment and distance at a certain distance forwards from the positions front end.
    /// </summary>
    private RailPathPosition GetForwardsPathPosition(float distance)
    {
        // Target position is on same segment
        if (distance < RailPathGenerator.RailSegmentLength - RailPosition.CurrentSegmentDistance) return new RailPathPosition(RailPosition.CurrentSegment, RailPosition.CurrentSegmentDistance + distance);

        // Target position is not on same segment
        float realDistance = distance - (RailPathGenerator.RailSegmentLength - RailPosition.CurrentSegmentDistance);
        int segmentIndex = (int)(realDistance / RailPathGenerator.RailSegmentLength);
        float restDistance = ((realDistance / RailPathGenerator.RailSegmentLength) - segmentIndex) * RailPathGenerator.RailSegmentLength;

        segmentIndex++;

        RailPathPosition position = new RailPathPosition(new List<RailSegment>() { UpcomingSegments[segmentIndex] }, restDistance);
        return position;
    }
}
