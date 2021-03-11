using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wagon : MonoBehaviour
{
    public const float WheelHeight = 0.8f;
    public const float Length = 8f;
    public const float Width = 3f;
    public const float WheelInset = 1f;

    public Train Train;
    public RailPathPosition RailPosition;

    public Dictionary<WheelPosition, TrainWheel> Wheels;
    public WagonFloor Floor;

    public void InitWagon(Train train, List<RailSegment> segments, float distance)
    {
        Train = train;
        Wheels = new Dictionary<WheelPosition, TrainWheel>()
        {
            { WheelPosition.FrontLeft, null },
            { WheelPosition.FrontRight, null },
            { WheelPosition.RearLeft, null },
            { WheelPosition.RearRight, null },
        };
        RailPosition = new RailPathPosition(segments, distance);
    }

    public void AddWheel(TrainWheel wheel, WheelPosition position)
    {
        Wheels[position] = wheel;
        wheel.transform.SetParent(transform);
        wheel.Wagon = this;
        UpdatePosition();
    }

    public void AddFloor(WagonFloor floor)
    {
        Floor = floor;
        floor.transform.SetParent(transform);
        floor.Wagon = this;
        UpdatePosition();
    }

    public void SetPosition(RailPathPosition position)
    {
        RailPosition.SetPosition(position);
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float ratio = RailPosition.CurrentSegmentDistance / RailPathGenerator.RailSegmentLength;
        Vector3 realPosition = Vector3.Lerp(RailPosition.CurrentSegment.FromPoint.Position, RailPosition.CurrentSegment.ToPoint.Position, ratio);
        transform.position = realPosition;

        foreach (KeyValuePair<WheelPosition, TrainWheel> kvp in Wheels.Where(x => x.Value != null))
        {
            kvp.Value.transform.position = GetWheelPosition(kvp.Key);
            kvp.Value.transform.rotation = GetWheelRotation(kvp.Key);
        }

        if (Floor != null)
        {
            Floor.transform.position = GetFloorPosition();
            Floor.transform.rotation = GetFloorRotation();
        }
    }

    private Vector3 GetFloorPosition()
    {
        Vector3 heightOffset = new Vector3(0f, RailPosition.CurrentSegment.Settings.TotalHeight + WheelHeight, 0f);
        return RailPosition.GetWorldPosition() + heightOffset;
    }

    private Quaternion GetFloorRotation()
    {
        float frontAngle = RailPosition.CurrentSegment.Angle;
        RailPathPosition backPosition = RailPosition.GetBackwardsPathPosition(Length);
        float backAngle = backPosition.CurrentSegment.Angle;
        float avgAngle = (frontAngle + backAngle) / 2f;
        return Quaternion.Euler(0, avgAngle + 180, 0);
    }

    private Vector3 GetWheelPosition(WheelPosition position)
    {
        RailPathPosition wheelPosition = null;
        if (position == WheelPosition.FrontLeft || position == WheelPosition.FrontRight) wheelPosition = RailPosition.GetBackwardsPathPosition(WheelInset);
        if (position == WheelPosition.RearLeft || position == WheelPosition.RearRight) wheelPosition = RailPosition.GetBackwardsPathPosition(Length - WheelInset);

        Vector3 wheelCenterPosition = wheelPosition.CurrentSegment.GetWorldPositionAtDistance(wheelPosition.CurrentSegmentDistance);

        RailSettings railSettings = wheelPosition.CurrentSegment.Settings;
        float height = railSettings.TrackHeight + railSettings.PlankHeight + WheelHeight / 2;

        float wheelDistance = railSettings.TrackGap / 2;
        float wheelAngle = 0f;
        if (position == WheelPosition.FrontLeft || position == WheelPosition.RearLeft) wheelAngle = wheelPosition.CurrentSegment.Angle + 90;
        if (position == WheelPosition.FrontRight || position == WheelPosition.RearRight) wheelAngle = wheelPosition.CurrentSegment.Angle - 90;

        float offsetX = Mathf.Sin(Mathf.Deg2Rad * wheelAngle) * wheelDistance;
        float offsetZ = Mathf.Cos(Mathf.Deg2Rad * wheelAngle) * wheelDistance;
        Vector3 wheelOffset = new Vector3(offsetX, height, offsetZ);

        return wheelCenterPosition + wheelOffset;
    }

    private Quaternion GetWheelRotation(WheelPosition position)
    {
        RailPathPosition wheelPosition = null;
        if (position == WheelPosition.FrontLeft || position == WheelPosition.FrontRight) wheelPosition = RailPosition.GetBackwardsPathPosition(WheelInset);
        if (position == WheelPosition.RearLeft || position == WheelPosition.RearRight) wheelPosition = RailPosition.GetBackwardsPathPosition(Length - WheelInset);

        return Quaternion.Euler(90f, 90f + wheelPosition.CurrentSegment.Angle, 0f);
    }
}

public enum WheelPosition
{
    FrontLeft,
    FrontRight,
    RearLeft,
    RearRight
}
