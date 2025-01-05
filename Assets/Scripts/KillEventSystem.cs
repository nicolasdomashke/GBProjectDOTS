using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial class KillEventSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    struct KillEventJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<LocalTransform> LocalTransformLookup;
        [ReadOnly] public ComponentLookup<PlayerTag> PlayerTagLookup;
        [ReadOnly] public ComponentLookup<EnemyTag> EnemyTagLookup;
        [ReadOnly] public ComponentLookup<BulletTag> BulletTagLookup;
        public BufferLookup<LinkedEntityGroup> LinkedEntityGroupBuffer;
        public EntityCommandBuffer ecb;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool entityAIsPlayer = PlayerTagLookup.HasComponent(entityA);
            bool entityBIsPlayer = PlayerTagLookup.HasComponent(entityB);

            bool entityAIsEnemy = EnemyTagLookup.HasComponent(entityA);
            bool entityBIsEnemy = EnemyTagLookup.HasComponent(entityB);

            bool entityAIsBullet = BulletTagLookup.HasComponent(entityA);
            bool entityBIsBullet = BulletTagLookup.HasComponent(entityB);

            if (entityAIsBullet)
            {
                if (entityBIsPlayer)
                {
                    ecb.DestroyEntity(entityA);
                    var transform = LocalTransformLookup[entityB];
                    Entity particles = ecb.Instantiate(PlayerTagLookup[entityB].deathEffect);
                    ecb.SetComponent(particles, new LocalTransform {Position = transform.Position, Rotation = Quaternion.Euler(-90, 0, 0),  Scale = 1f});
                    ecb.SetComponent(entityB, new LocalTransform {Scale = 0f});
                    ecb.RemoveComponent<PlayerTag>(entityB);
                    Debug.Log("You died!");
                }
                else if (entityBIsEnemy)
                {
                    var transform = LocalTransformLookup[entityB];
                    Entity particles = ecb.Instantiate(EnemyTagLookup[entityB].deathEffect);
                    ecb.SetComponent(particles, new LocalTransform {Position = transform.Position, Rotation = Quaternion.Euler(-90, 0, 0),  Scale = 1f});
                    ecb.SetComponent(entityA, new LocalTransform {Scale = 0f});
                    ecb.RemoveComponent<EnemyTag>(entityA);
                    ecb.DestroyEntity(entityB);
                    Debug.Log("Enemy died!");
                }
                
            }
            else if (entityBIsBullet)
            {
                if (entityAIsPlayer)
                {
                    ecb.DestroyEntity(entityB);
                    var transform = LocalTransformLookup[entityA];
                    Entity particles = ecb.Instantiate(PlayerTagLookup[entityA].deathEffect);
                    ecb.SetComponent(particles, new LocalTransform {Position = transform.Position, Rotation = Quaternion.Euler(-90, 0, 0), Scale = 1f});
                    ecb.SetComponent(entityA, new LocalTransform {Scale = 0f});
                    ecb.RemoveComponent<PlayerTag>(entityA);
                    Debug.Log("You died!");
                }
                else if (entityAIsEnemy)
                {
                    var transform = LocalTransformLookup[entityA];
                    Entity particles = ecb.Instantiate(EnemyTagLookup[entityA].deathEffect);
                    ecb.SetComponent(particles, new LocalTransform {Position = transform.Position, Rotation = Quaternion.Euler(-90, 0, 0), Scale = 1f});
                    ecb.SetComponent(entityA, new LocalTransform {Scale = 0f});
                    ecb.RemoveComponent<EnemyTag>(entityA);
                    ecb.DestroyEntity(entityB);
                    Debug.Log("Enemy died!");
                }
            }
        }
    }

    protected override void OnUpdate()
    {
        var simulationSingleton = GetSingleton<SimulationSingleton>();
        var ecb = ecbSystem.CreateCommandBuffer();

        var job = new KillEventJob
        {
            LocalTransformLookup = GetComponentLookup<LocalTransform>(true),
            PlayerTagLookup = GetComponentLookup<PlayerTag>(true),
            EnemyTagLookup = GetComponentLookup<EnemyTag>(true),
            BulletTagLookup = GetComponentLookup<BulletTag>(true),
            LinkedEntityGroupBuffer = GetBufferLookup<LinkedEntityGroup>(true),
            ecb = ecb
        };

        Dependency = job.Schedule(simulationSingleton, Dependency);
        ecbSystem.AddJobHandleForProducer(Dependency);
    }
}