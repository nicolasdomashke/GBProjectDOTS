using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial class TriggerEventSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    struct TriggerEventJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<PlayerTag> PlayerTagLookup;
        [ReadOnly] public ComponentLookup<CollectableTag> CollectableTagLookup;
        [ReadOnly] public ComponentLookup<CollectableInfo> CollectableInfoLookup;
        public EntityCommandBuffer ecb;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool entityAIsPlayer = PlayerTagLookup.HasComponent(entityA);
            bool entityBIsPlayer = PlayerTagLookup.HasComponent(entityB);

            bool entityAIsCollectable = CollectableTagLookup.HasComponent(entityA);
            bool entityBIsCollectable = CollectableTagLookup.HasComponent(entityB);

            if (entityAIsPlayer && entityBIsCollectable)
            {
                ecb.DestroyEntity(entityB);

                var collectableInfo = CollectableInfoLookup[entityA];
                collectableInfo.collectableCounter++;
                ecb.SetComponent(entityA, collectableInfo);
                //Debug.Log(collectableInfo.collectableCounter);
            }
            else if (entityBIsPlayer && entityAIsCollectable)
            {
                ecb.DestroyEntity(entityA);

                var collectableInfo = CollectableInfoLookup[entityB];
                collectableInfo.collectableCounter++;
                ecb.SetComponent(entityB, collectableInfo);
                //Debug.Log(collectableInfo.collectableCounter);
            }
        }
    }

    protected override void OnUpdate()
    {
        var simulationSingleton = GetSingleton<SimulationSingleton>();
        var ecb = ecbSystem.CreateCommandBuffer();

        var job = new TriggerEventJob
        {
            PlayerTagLookup = GetComponentLookup<PlayerTag>(true),
            CollectableTagLookup = GetComponentLookup<CollectableTag>(true),
            CollectableInfoLookup = GetComponentLookup<CollectableInfo>(true),
            ecb = ecb
        };

        Dependency = job.Schedule(simulationSingleton, Dependency);
        ecbSystem.AddJobHandleForProducer(Dependency);
    }
}