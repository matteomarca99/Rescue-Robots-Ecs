using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct RotatingMovingCubeAspect : IAspect
{

    public readonly RefRW<LocalTransform> localTransform;
    public readonly RefRW<Movement> movement;

    public void Move(float deltaTime, float2 areaSize)
    {
        // Se stiamo oltrepassando l'area predefinita, rimbalziamo invertendo la direzione
        if (localTransform.ValueRO.Position.x < -areaSize.x / 2f || localTransform.ValueRO.Position.x > areaSize.x / 2f)
        {
            movement.ValueRW.movementVector.x *= -1; // Inverti la direzione sull'asse x
        }
        if (localTransform.ValueRO.Position.z < -areaSize.y / 2f || localTransform.ValueRO.Position.z > areaSize.y / 2f)
        {
            movement.ValueRW.movementVector.z *= -1; // Inverti la direzione sull'asse z
        }

        // Applica il movimento
        localTransform.ValueRW = localTransform.ValueRO.Translate(movement.ValueRO.movementVector * deltaTime);
    }
}
