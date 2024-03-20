using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SettingsAuthoring : MonoBehaviour
{
    public bool simulationStarted;
    public GameObject robotPrefab;
    public GameObject rescuerPrefab;
    public GameObject victimPrefab;
    public GameObject terrainPrefab;
    public int numRobots;
    public int numRescuers;
    public int numVictims;
    public float minDistanceBetweenRobotAndVictim;
    public float distanceThreshold;
    public float2 simulationArea;

    public class Baker : Baker<SettingsAuthoring>
    {
        public override void Bake(SettingsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new Settings
            {
                simulationStarted = authoring.simulationStarted,
                robotPrefabEntity = GetEntity(authoring.robotPrefab, TransformUsageFlags.Dynamic),
                rescuerPrefabEntity = GetEntity(authoring.rescuerPrefab, TransformUsageFlags.Dynamic),
                victimPrefabEntity = GetEntity(authoring.victimPrefab, TransformUsageFlags.None),
                terrainPrefabEntity = GetEntity(authoring.terrainPrefab, TransformUsageFlags.None),
                numRobots = authoring.numRobots,
                numRescuers = authoring.numRescuers,
                numVictims = authoring.numVictims,
                minDistanceBetweenRobotAndVictim = authoring.minDistanceBetweenRobotAndVictim,
                distanceThreshold = authoring.distanceThreshold,
                victimFound = Entity.Null,
                lastRobotPosition = Vector3.zero,
                simulationArea = authoring.simulationArea
            });
        }
    }
}

public struct Settings : IComponentData
{
    public bool simulationStarted;
    public Entity robotPrefabEntity;
    public Entity rescuerPrefabEntity;
    public Entity victimPrefabEntity;
    public Entity terrainPrefabEntity;
    public int numRobots;
    public int numRescuers;
    public int numVictims;
    public float minDistanceBetweenRobotAndVictim;
    public float distanceThreshold;
    public Entity victimFound;
    public float3 lastRobotPosition;
    public float2 simulationArea;
}
