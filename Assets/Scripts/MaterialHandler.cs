using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialHandler : MonoBehaviour
{
    public Material DefaultMaterial;

    [Header("Rails")]
    public Color RailTrackColor;
    public Color RailPlankColor;

    [Header("Terrain")]
    public Color TerrainColor;

    public static MaterialHandler Instance
    {
        get
        {
            return GameObject.Find("MaterialHandler").GetComponent<MaterialHandler>();
        }
    }
}
