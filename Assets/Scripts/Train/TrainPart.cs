using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrainPart : MonoBehaviour
{
    public TrainPartType Type;
    public Train Train;


    public abstract void Init();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum TrainPartType
{
    Wheel
}
