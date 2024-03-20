using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(SpawnCubesSystem))]
public partial struct TargetingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Robot>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity settingsEntity = SystemAPI.GetSingletonEntity<Settings>();
        RefRW<Settings> settingsComponent = SystemAPI.GetComponentRW<Settings>(settingsEntity);

        if (!SystemAPI.Exists(settingsComponent.ValueRO.victimFound))
        {
            var targetQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Victim>().Build();
            var kdQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Robot>().Build();

            var targetEntities = targetQuery.ToEntityArray(state.WorldUpdateAllocator);
            var targetTransforms =
                targetQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);

            var tree = new KDTree(targetEntities.Length, Allocator.TempJob, 64);

            // init KD tree
            for (int i = 0; i < targetEntities.Length; i++)
            {
                // NOTE - the first parameter is ignored, only the index matters
                tree.AddEntry(i, targetTransforms[i].Position);
            }

            state.Dependency = tree.BuildTree(targetEntities.Length, state.Dependency);

            var queryKdTree = new QueryKDTree
            {
                Tree = tree,
                TargetEntities = targetEntities,
                Scratch = default,
                TargetHandle = SystemAPI.GetComponentTypeHandle<Target>(),
                LocalTransformHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>(true),
                Settings = settingsComponent
            };
            state.Dependency = queryKdTree.ScheduleParallel(kdQuery, state.Dependency);

            state.Dependency.Complete();
            tree.Dispose();

        } else
        {
            state.Enabled = false;
        }
    }
}

[BurstCompile]
public struct QueryKDTree : IJobChunk
{
    [ReadOnly] public NativeArray<Entity> TargetEntities;
    public PerThreadWorkingMemory Scratch;
    public KDTree Tree;

    [NativeDisableUnsafePtrRestriction]
    public RefRW<Settings> Settings;

    public ComponentTypeHandle<Target> TargetHandle;
    [ReadOnly] public ComponentTypeHandle<LocalTransform> LocalTransformHandle;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
        in v128 chunkEnabledMask)
    {
        var targets = chunk.GetNativeArray(ref TargetHandle);
        var transforms = chunk.GetNativeArray(ref LocalTransformHandle);

        for (int i = 0; i < chunk.Count; i++)
        {
            if (!Scratch.Neighbours.IsCreated)
            {
                Scratch.Neighbours = new NativePriorityHeap<KDTree.Neighbour>(1, Allocator.Temp);
            }

            Scratch.Neighbours.Clear();

            float maxDistanceSquared = 2f * 2f;

            Tree.GetEntriesInRangeWithHeap(unfilteredChunkIndex, transforms[i].Position, maxDistanceSquared,
                ref Scratch.Neighbours);

            if (Scratch.Neighbours.Count > 0)
            {
                var nearest = Scratch.Neighbours.Peek().index;
                targets[i] = new Target { Value = TargetEntities[nearest] };

                Settings.ValueRW.victimFound = TargetEntities[nearest];
                Settings.ValueRW.lastRobotPosition = transforms[i].Position;
            }
            else
            {
                targets[i] = new Target { Value = Entity.Null };
            }
        }
    }
}

public struct PerThreadWorkingMemory
{
    [NativeDisableContainerSafetyRestriction]
    public NativePriorityHeap<KDTree.Neighbour> Neighbours;
}