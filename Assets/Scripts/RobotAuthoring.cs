using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RobotAuthoring : MonoBehaviour
{
    class Baker : Baker<RobotAuthoring>
    {
        public override void Bake(RobotAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Robot());
        }
    }
}

public struct Robot : IComponentData
{

}

public struct Waypoint : IComponentData
{
    public int index;
    public float3 pos;
}