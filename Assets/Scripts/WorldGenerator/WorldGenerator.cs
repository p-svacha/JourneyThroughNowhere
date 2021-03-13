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
        RailPathGenerator.GeneratePath(1000);
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

        List<RailSegment> segments = new List<RailSegment>();
        for(int i = 0; i < 30; i++) segments.Insert(0, RailPathGenerator.RailSegments[i]);
        train.Init(segments);

        for (int i = 0; i < 4; i++)
        {
            Wagon wagon = train.AddWagon();

            wagon.AddWheel(TrainWheelGenerator.GenerateTrainWheel(), WheelPosition.FrontLeft);
            wagon.AddWheel(TrainWheelGenerator.GenerateTrainWheel(), WheelPosition.FrontRight);
            wagon.AddWheel(TrainWheelGenerator.GenerateTrainWheel(), WheelPosition.RearLeft);
            wagon.AddWheel(TrainWheelGenerator.GenerateTrainWheel(), WheelPosition.RearRight);

            wagon.AddFloor(WagonFloorGenerator.GenerateWagonFloor());
        }
    }
}
