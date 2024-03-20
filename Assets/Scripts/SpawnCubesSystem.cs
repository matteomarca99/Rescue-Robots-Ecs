using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class SpawnCubesSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<Settings>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        Settings settings = SystemAPI.GetSingleton<Settings>();

        if (settings.simulationStarted)
        {
            this.Enabled = false;

            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(WorldUpdateAllocator);

            // Generiamo i soccorritori umani
            for (int i = 0; i < settings.numRescuers; i++)
            {
                Entity spawnedEntity = entityCommandBuffer.Instantiate(settings.rescuerPrefabEntity);

                // Generiamo posizioni casuali all'interno dell'area della simulazione
                float randomX = UnityEngine.Random.Range(-settings.simulationArea.x / 2f, settings.simulationArea.x / 2f);
                float randomZ = UnityEngine.Random.Range(-settings.simulationArea.y / 2f, settings.simulationArea.y / 2f);

                entityCommandBuffer.SetComponent(spawnedEntity, new LocalTransform
                {
                    Position = new float3(randomX, 0.6f, randomZ),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
            }

            NativeList<float3> victimTransforms = new NativeList<float3>(WorldUpdateAllocator);

            // Generiamo le vittime
            for (int i = 0; i < settings.numVictims; i++)
            {
                Entity spawnedEntity = entityCommandBuffer.Instantiate(settings.victimPrefabEntity);

                // Generiamo posizioni casuali all'interno dell'area della simulazione
                float randomX = UnityEngine.Random.Range(-settings.simulationArea.x / 2f, settings.simulationArea.x / 2f);
                float randomZ = UnityEngine.Random.Range(-settings.simulationArea.y / 2f, settings.simulationArea.y / 2f);

                float3 randomPos = new float3(randomX, 0.6f, randomZ);

                entityCommandBuffer.SetComponent(spawnedEntity, new LocalTransform
                {
                    Position = randomPos,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });

                victimTransforms.Add(randomPos);
            }

            // Generiamo i robot soccorritori
            for (int i = 0; i < settings.numRobots; i++)
            {
                float3 randomPosition = float3.zero;
                bool positionIsValid = false;

                while (!positionIsValid)
                {
                    // Generiamo posizioni casuali all'interno dell'area della simulazione
                    float randomX = UnityEngine.Random.Range(-settings.simulationArea.x / 2f, settings.simulationArea.x / 2f);
                    float randomZ = UnityEngine.Random.Range(-settings.simulationArea.y / 2f, settings.simulationArea.y / 2f);

                    randomPosition = new float3(randomX, 0.6f, randomZ);

                    // Controlla se la posizione generata è troppo vicina a una vittima
                    positionIsValid = true;
                    foreach (var victimTransform in victimTransforms)
                    {
                        if (math.distance(randomPosition, victimTransform) < settings.minDistanceBetweenRobotAndVictim)
                        {
                            // La posizione generata è troppo vicina a una vittima, quindi non è valida
                            positionIsValid = false;
                            break;
                        }
                    }
                }

                // Ora hai una posizione valida per il robot
                Entity spawnedEntity = entityCommandBuffer.Instantiate(settings.robotPrefabEntity);
                entityCommandBuffer.SetComponent(spawnedEntity, new LocalTransform
                {
                    Position = randomPosition,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });

                entityCommandBuffer.SetComponent(spawnedEntity, new Movement
                {
                    movementVector = new float3(UnityEngine.Random.Range(-1f, +1f), 0, UnityEngine.Random.Range(-1f, +1f))
                });
            }

            // Infine generiamo anche il terreno dove verranno posizionati i cubi, con le dimensioni dell'area
            Entity terrainEntity = entityCommandBuffer.Instantiate(settings.terrainPrefabEntity);

            entityCommandBuffer.SetComponent(terrainEntity, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            });

            // E lo adattiamo alle dimensioni dell'area
            entityCommandBuffer.AddComponent(terrainEntity, new PostTransformMatrix
            {
                Value = float4x4.Scale(settings.simulationArea.x / 10, 0, settings.simulationArea.y / 10)
            });

            entityCommandBuffer.Playback(EntityManager);
        }
    }
}
