using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWheelGenerator
{
    public static TrainWheel GenerateTrainWheel(Vector3 position)
    {
        GameObject wheelObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheelObject.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
        wheelObject.transform.localScale = new Vector3(0.8f, 0.2f, 0.8f);
        wheelObject.transform.position = position;
        TrainWheel wheel = wheelObject.AddComponent<TrainWheel>();


        return wheel;
    }
}
