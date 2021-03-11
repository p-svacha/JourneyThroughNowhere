using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    private RailPathGenerator RailPathGenerator;
    private TerrainGenerator TerrainGenerator;

    // Start is called before the first frame update
    void Start()
    {
        TerrainGenerator = new TerrainGenerator();
        TerrainGenerator.DrawTestTerrain();

        RailPathGenerator = new RailPathGenerator();
        RailPathGenerator.GeneratePath(200);
        //RailPathGenerator.DebugPath();
        RailPathGenerator.DrawPath();

        TestSpawnTrain();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TestSpawnTrain()
    {
        GameObject trainObject = new GameObject("Train");
        Train train = trainObject.AddComponent<Train>();
        train.SetPosition(RailPathGenerator.RailSegments[4], 1f);

        Wagon wagon = train.AddWagon();

        TrainWheel wheel = TrainWheelGenerator.GenerateTrainWheel(new Vector3(4f, 4f, 4f));
        wagon.AddWheel(wheel, WheelPosition.FrontLeft);
    }
}
