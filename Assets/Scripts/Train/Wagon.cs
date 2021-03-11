using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wagon : MonoBehaviour
{
    private float WheelHeight = 1f;
    private float Length = 12f;
    private float Width = 6f;

    public List<TrainPart> TrainParts = new List<TrainPart>();
    public Dictionary<WheelPosition, TrainWheel> Wheels;

    public void InitWagon()
    {
        Wheels = new Dictionary<WheelPosition, TrainWheel>()
        {
            {WheelPosition.FrontLeft, null },
            {WheelPosition.FrontRight, null },
            {WheelPosition.RearLeft, null },
            {WheelPosition.RearRight, null },
        };
    }

    public void AddWheel(TrainWheel wheel, WheelPosition position)
    {
        Wheels[position] = wheel;
        wheel.transform.SetParent(transform);
        wheel.transform.localPosition = GetLocalWheelPosition(position);
    }

    private Vector3 GetLocalWheelPosition(WheelPosition position)
    {
        switch (position)
        {
            case WheelPosition.FrontLeft:
                return new Vector3(-Width / 2, WheelHeight / 2, Length / 2);

            default:
                return Vector3.zero;
        }
    }
}

public enum WheelPosition
{
    FrontLeft,
    FrontRight,
    RearLeft,
    RearRight
}
