using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSettings
{
    public float PlankWidth;
    public float PlankHeight;
    public float TrackWidth;
    public float TrackGap;
    public float TrackHeight;
    public float TotalHeight;

    public RailSettings(float plankWidth, float plankHeight, float trackWidth, float trackGap, float trackHeight)
    {
        PlankWidth = plankWidth;
        PlankHeight = plankHeight;
        TrackWidth = trackWidth;
        TrackGap = trackGap;
        TrackHeight = trackHeight;
        TotalHeight = PlankHeight + TrackHeight;
    }
}
