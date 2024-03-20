using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct HandleCubesSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Settings>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Settings settings = SystemAPI.GetSingleton<Settings>();

        if (!SystemAPI.Exists(settings.victimFound))
        {
            HandleCubesSystemJob handleCubesSystemJob = new HandleCubesSystemJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                simulationArea = settings.simulationArea
            };

            handleCubesSystemJob.ScheduleParallel();
        } else
        {
            state.Enabled = false;
        }
    }

    [BurstCompile]
    public partial struct HandleCubesSystemJob : IJobEntity
    {
        public float deltaTime;
        public float2 simulationArea;

        public void Execute(RotatingMovingCubeAspect rotatingMovingCubeAspect)
        {
            rotatingMovingCubeAspect.Move(deltaTime, simulationArea);
        }
    }
}