using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TrainWheelGenerator
{
    public static TrainWheel GenerateTrainWheel()
    {
        GameObject wheelObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheelObject.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
        wheelObject.transform.localScale = new Vector3(0.8f, 0.2f, Wagon.WheelHeight);
        TrainWheel wheel = wheelObject.AddComponent<TrainWheel>();
        wheel.Init(null);


        return wheel;
    }
}
