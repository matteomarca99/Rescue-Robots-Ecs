using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MovementAuthoring : MonoBehaviour
{
    public float3 movementVector;

    public class Baker : Baker<MovementAuthoring>
    {
        public override void Bake(MovementAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Movement
            {
                movementVector = authoring.movementVector
            });
        }
    }
}

public struct Movement : IComponentData
{
    public float3 movementVector;
}
