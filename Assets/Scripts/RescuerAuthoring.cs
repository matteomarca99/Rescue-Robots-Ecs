using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RescuerAuthoring : MonoBehaviour
{
    class Baker : Baker<RescuerAuthoring>
    {
        public override void Bake(RescuerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Rescuer());
        }
    }
}

public struct Rescuer : IComponentData
{

}