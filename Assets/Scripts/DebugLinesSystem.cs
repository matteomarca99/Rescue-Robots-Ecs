using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TargetingSystem))]
[BurstCompile]
public partial struct DebugLinesSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Settings>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var waypointsQuery = SystemAPI.QueryBuilder().WithAll<Waypoint>().Build();

        var waypoints = waypointsQuery.ToComponentDataArray<Waypoint>(state.WorldUpdateAllocator);

        if (waypointsQuery.CalculateEntityCount() > 0)
        {
            for(int i=0; i<waypointsQuery.CalculateEntityCount(); i++)
            {
                if (i < waypointsQuery.CalculateEntityCount() - 1)
                {
                    Debug.DrawLine(waypoints[i].pos, waypoints[i + 1].pos, Color.yellow);
                }
            }
        }
    }
}


