using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[BurstCompile]
public partial class ChangeColorSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        ChangeColorJob changeColorSystemJob = new ChangeColorJob();

        changeColorSystemJob.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct ChangeColorJob : IJobEntity
    {
        public void Execute(ref URPMaterialPropertyBaseColor materialBaseColor, in Waypoint waypoint)
        {
            materialBaseColor.Value = new float4(Color.green.r, Color.green.g, Color.green.b, Color.green.a);
        }
    }
}