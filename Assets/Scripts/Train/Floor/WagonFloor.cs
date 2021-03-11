using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonFloor : TrainPart
{
    public const float FloorHeight = 0.2f;

    public override void Init(Wagon wagon)
    {
        Wagon = wagon;
        Type = TrainPartType.Floor;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
