using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct RescueSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Rescuer>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Settings settings = SystemAPI.GetSingleton<Settings>();

        if (SystemAPI.Exists(settings.victimFound))
        {
            var rescuerQuery = SystemAPI.QueryBuilder().WithAllRW<LocalTransform, Rescuer>().Build();
            var robotQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Robot>().Build();

            var rescuerTransforms = rescuerQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);
            var robotTransforms = robotQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);
            var robotEntities = robotQuery.ToEntityArray(state.WorldUpdateAllocator);

            // Assuming there's only one rescuer for simplicity
            var rescuerPosition = rescuerTransforms[0].Position;

            // Finding the position of the robot that found the victim
            var lastRobotPosition = settings.lastRobotPosition;

            // Calculating the line between lastRobotPosition and rescuer
            var lineDirection = math.normalize(rescuerPosition - lastRobotPosition);

            // Finding robots closest to the line
            var closestRobots = new NativeList<RobotData>(state.WorldUpdateAllocator);
            var distanceThreshold = settings.distanceThreshold;
            var lineLength = math.distance(lastRobotPosition, rescuerPosition);

            for (int i=0; i<robotTransforms.Length; i++)
            {
                var robotPosition = robotTransforms[i].Position;
                var pointOnLine = ProjectPointOnLine(robotPosition, lastRobotPosition, lineDirection);
                var distanceToLine = math.distance(pointOnLine, robotPosition);

                // Check if the point is within the segment between lastRobotPosition and rescuerPosition
                var distanceToStart = math.distance(lastRobotPosition, pointOnLine);
                var distanceToEnd = math.distance(rescuerPosition, pointOnLine);
                if (distanceToLine < distanceThreshold && distanceToStart <= lineLength && distanceToEnd <= lineLength)
                {
                    closestRobots.Add(new RobotData { Transform = robotTransforms[i], RobotEntity = robotEntities[i] });
                }
            }

            // Perform bubble sort to sort the robots based on their distance from rescuerPosition
            for (int i = 0; i < closestRobots.Length - 1; i++)
            {
                for (int j = i + 1; j < closestRobots.Length; j++)
                {
                    var distanceA = math.distance(closestRobots[i].Transform.Position, rescuerPosition);
                    var distanceB = math.distance(closestRobots[j].Transform.Position, rescuerPosition);

                    if (distanceB < distanceA)
                    {
                        RobotData temp = closestRobots[i];
                        closestRobots[i] = closestRobots[j];
                        closestRobots[j] = temp;
                    }
                }
            }

            // Aggiungiamo anche la vittima alla fine della lista
            closestRobots.Add(new RobotData
            {
                RobotEntity = settings.victimFound,
                Transform = SystemAPI.GetComponent<LocalTransform>(settings.victimFound)
            });

            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(state.WorldUpdateAllocator);

            // Aggiungiamo il component dei Waypoint
            if (closestRobots.Length > 0)
            {
                for(int i=0; i<closestRobots.Length; i++)
                {
                    entityCommandBuffer.AddComponent(closestRobots[i].RobotEntity, new Waypoint
                    {
                        index = i,
                        pos = closestRobots[i].Transform.Position
                    });
                }

                entityCommandBuffer.Playback(state.EntityManager);
                state.Enabled = false;
            }
        }
    }

    // Project a point onto a line in 3D space
    private float3 ProjectPointOnLine(float3 point, float3 linePoint, float3 lineDirection)
    {
        var t = math.dot(point - linePoint, lineDirection);
        return linePoint + t * lineDirection;
    }
}

struct RobotData
{
    public LocalTransform Transform;
    public Entity RobotEntity;
}
