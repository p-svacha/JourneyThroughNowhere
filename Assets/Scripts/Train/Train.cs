using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public RailSegment CurrentSegment; // The next path point the train will pass with its front end on the current track
    public float CurrentSegmentDistance; // The distance since the last path point

    public List<Wagon> Wagons = new List<Wagon>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(RailSegment segment, float distance)
    {
        CurrentSegment = segment;
        CurrentSegmentDistance = distance;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float ratio = CurrentSegmentDistance / RailPathGenerator.RailSegmentLength;
        Vector3 position = Vector3.Lerp(CurrentSegment.FromPoint.Position, CurrentSegment.ToPoint.Position, ratio);
        transform.position = position;
    }

    public Wagon AddWagon()
    {
        GameObject wagonObject = new GameObject("Wagon");
        Wagon wagon = wagonObject.AddComponent<Wagon>();
        wagon.InitWagon();
        Wagons.Add(wagon);
        wagon.transform.SetParent(transform);
        return wagon;
    }
}
