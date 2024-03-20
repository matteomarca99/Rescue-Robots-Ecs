using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(RescueSystem))]
public partial struct PathToVictimSystem : ISystem
{
    public int waypointIndex;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Rescuer>();
        waypointIndex = 0;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var rescuerEntity = SystemAPI.GetSingletonEntity<Rescuer>();
        var waypointsQuery = SystemAPI.QueryBuilder().WithAll<Waypoint>().Build();

        var waypoints = waypointsQuery.ToComponentDataArray<Waypoint>(state.WorldUpdateAllocator);

        if (waypointsQuery.CalculateEntityCount() > 0 && waypointIndex < waypointsQuery.CalculateEntityCount())
        {
            // Ottieni la posizione del robot di destinazione (primo waypoint)
            var destinationPosition = waypoints[waypointIndex].pos;

            // Ottieni la posizione attuale del rescuer
            var rescuerTransform = SystemAPI.GetComponentRW<LocalTransform>(rescuerEntity);

            var distanceToWaypoint = math.distance(rescuerTransform.ValueRO.Position, destinationPosition);

            // Se il rescuer è abbastanza vicino al waypoint corrente, passa al successivo
            if (distanceToWaypoint < 0.5f) // Imposta una soglia di distanza appropriata
            {
                // Passa al successivo waypoint del robot
                waypointIndex++;
            }
            else
            {
                // Calcola il vettore di traslazione per spostare il rescuer verso il robot di destinazione
                var translationVector = math.normalize(destinationPosition - rescuerTransform.ValueRO.Position) * 10f * SystemAPI.Time.DeltaTime;

                // Applica la traslazione alla posizione del rescuer
                rescuerTransform.ValueRW = rescuerTransform.ValueRO.Translate(translationVector);
            }
        }
    }
}
