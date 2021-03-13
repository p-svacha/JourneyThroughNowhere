using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wagon : MonoBehaviour
{
    public const float WheelHeight = 0.8f;
    public const float Length = 12f;
    public const float Width = 5f;
    public const float WheelInset = 1f;

    public Train Train;
    public float TargetAngle;
    private const float MaxAngleChangePerKph = 0.4f;

    public RailPathPosition RailPosition;
    public Vector3 FrameMoveVector;
    public float FrameMoveRotation;

    public Dictionary<WheelPosition, TrainWheel> Wheels;
    public WagonFloor Floor;

    public void InitWagon(Train train, RailPathPosition position)
    {
        Train = train;
        Wheels = new Dictionary<WheelPosition, TrainWheel>()
        {
            { WheelPosition.FrontLeft, null },
            { WheelPosition.FrontRight, null },
            { WheelPosition.RearLeft, null },
            { WheelPosition.RearRight, null },
        };
        RailPosition = position;
    }

    public void AddWheel(TrainWheel wheel, WheelPosition position)
    {
        Wheels[position] = wheel;
        wheel.transform.SetParent(transform);
        wheel.Wagon = this;
        UpdatePosition(false);
    }

    public void AddFloor(WagonFloor floor)
    {
        Floor = floor;
        floor.transform.SetParent(transform);
        floor.transform.localPosition = new Vector3(0f, RailPosition.Segment.Settings.TotalHeight + WheelHeight, 0f);
        floor.transform.localRotation = Quaternion.identity;
        floor.Wagon = this;
        UpdatePosition(false);
    }

    public void SetPosition(RailPathPosition position)
    {
        FrameMoveVector = position.GetWorldPosition() - RailPosition.GetWorldPosition();
        RailPosition.SetPosition(position);
        UpdatePosition(true);
    }

    private void UpdatePosition(bool smooth)
    {
        transform.position = RailPosition.GetWorldPosition();
        Quaternion preRotation = transform.rotation;
        transform.rotation = GetRotation(smooth);
        FrameMoveRotation = transform.rotation.eulerAngles.y - preRotation.eulerAngles.y;

        foreach (KeyValuePair<WheelPosition, TrainWheel> kvp in Wheels.Where(x => x.Value != null))
        {
            kvp.Value.transform.position = GetWheelPosition(kvp.Key);
            kvp.Value.transform.rotation = GetWheelRotation(kvp.Key);
        }
    }

    private Quaternion GetRotation(bool smooth)
    {
        float frontAngle = RailPosition.Segment.Angle;
        RailPathPosition backPosition = Train.GetBackwardsPathPosition(RailPosition, Length);
        float backAngle = backPosition.Segment.Angle;
        float avgAngle = (frontAngle + backAngle) / 2f;
        TargetAngle = avgAngle + 180;
        if (!smooth) return Quaternion.Euler(0f, TargetAngle, 0f);
        float currentAngle = transform.rotation.eulerAngles.y;
        float maxAngleChange = MaxAngleChangePerKph * Mathf.Abs(Train.VelocityKph) * Time.deltaTime;
        if (Mathf.Abs(TargetAngle - currentAngle) <= maxAngleChange) return Quaternion.Euler(0f, TargetAngle, 0f);
        else if (TargetAngle > currentAngle) return Quaternion.Euler(0f, currentAngle + maxAngleChange, 0f);
        else if (TargetAngle < currentAngle) return Quaternion.Euler(0f, currentAngle - maxAngleChange, 0f);
        Debug.Log("Target: " + TargetAngle + " / Current: " + currentAngle);
        throw new System.Exception();
    }

    private Vector3 GetWheelPosition(WheelPosition position)
    {
        RailPathPosition wheelPosition = null;
        if (position == WheelPosition.FrontLeft || position == WheelPosition.FrontRight) wheelPosition = Train.GetBackwardsPathPosition(RailPosition, WheelInset);
        if (position == WheelPosition.RearLeft || position == WheelPosition.RearRight) wheelPosition = Train.GetBackwardsPathPosition(RailPosition, Length - WheelInset);

        Vector3 wheelCenterPosition = wheelPosition.Segment.GetWorldPositionAtDistance(wheelPosition.Distance);

        RailSettings railSettings = wheelPosition.Segment.Settings;
        float height = railSettings.TrackHeight + railSettings.PlankHeight + WheelHeight / 2;

        float wheelDistance = railSettings.TrackGap / 2;
        float wheelAngle = 0f;
        if (position == WheelPosition.FrontLeft || position == WheelPosition.RearLeft) wheelAngle = wheelPosition.Segment.Angle + 90;
        if (position == WheelPosition.FrontRight || position == WheelPosition.RearRight) wheelAngle = wheelPosition.Segment.Angle - 90;

        float offsetX = Mathf.Sin(Mathf.Deg2Rad * wheelAngle) * wheelDistance;
        float offsetZ = Mathf.Cos(Mathf.Deg2Rad * wheelAngle) * wheelDistance;
        Vector3 wheelOffset = new Vector3(offsetX, height, offsetZ);

        return wheelCenterPosition + wheelOffset;
    }

    private Quaternion GetWheelRotation(WheelPosition position)
    {
        RailPathPosition wheelPosition = null;
        if (position == WheelPosition.FrontLeft || position == WheelPosition.FrontRight) wheelPosition = Train.GetBackwardsPathPosition(RailPosition, WheelInset);
        if (position == WheelPosition.RearLeft || position == WheelPosition.RearRight) wheelPosition = Train.GetBackwardsPathPosition(RailPosition, Length - WheelInset);

        return Quaternion.Euler(90f, 90f + wheelPosition.Segment.Angle, 0f);
    }
}

public enum WheelPosition
{
    FrontLeft,
    FrontRight,
    RearLeft,
    RearRight
}
