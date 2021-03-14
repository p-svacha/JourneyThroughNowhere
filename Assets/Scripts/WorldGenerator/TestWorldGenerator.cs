using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWorldGenerator : MonoBehaviour
{
    public CharacterController Player;

    private RailPathGenerator RailPathGenerator;
    private TerrainGenerator TerrainGenerator;

    // Start is called before the first frame update
    void Start()
    {
        TerrainGenerator = new TerrainGenerator();

        RailPathGenerator = new RailPathGenerator(TerrainGenerator);
        RailPathGenerator.GeneratePath(300);
        RailPathGenerator.DebugPath();
        RailPathGenerator.DrawPath();

        TestSpawnTrain();

        Player.transform.position = new Vector3(0f, TerrainGenerator.GetElevation(new Vector2(0f, 0f)) + 2f, 0f);
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
