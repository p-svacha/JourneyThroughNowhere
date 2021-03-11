﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrainPart : MonoBehaviour
{
    public TrainPartType Type;
    public Wagon Wagon;


    public abstract void Init(Wagon wagon);

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
    Wheel,
    Floor
}
