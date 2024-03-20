using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class VictimAuthoring : MonoBehaviour
{
    class Baker : Baker<VictimAuthoring>
    {
        public override void Bake(VictimAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Victim());
        }
    }
}

public struct Victim : IComponentData
{

}