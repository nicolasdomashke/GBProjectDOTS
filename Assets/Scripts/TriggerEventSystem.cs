using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial class TriggerEventSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem ecbSystem;
    private PlayerStats playerStats;
    private int localCollectablesCounter;
    private Settings config;
    private bool useGoogleDrive;
    private string jsonString;

    protected override void OnCreate()
    {
        config = Resources.Load<Settings>("Config");
        useGoogleDrive = config.useCloudStorage;
        localCollectablesCounter = 0;

        ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

        if (useGoogleDrive) {
            jsonString = GoogleDriveTools.DownloadFile("Stats.json");
        }
        else
        {
            jsonString = PlayerPrefs.GetString("Stats"); 
        }
        Debug.Log(jsonString);
        if (jsonString.Equals(""))
        {
            playerStats = new PlayerStats();
        }
        else
        {
            playerStats = JsonUtility.FromJson<PlayerStats>(jsonString);
        }
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
                //Debug.Log($"{collectableInfo.collectableCounter}");
            }
            else if (entityBIsPlayer && entityAIsCollectable)
            {
                ecb.DestroyEntity(entityA);

                var collectableInfo = CollectableInfoLookup[entityB];
                collectableInfo.collectableCounter++;
                ecb.SetComponent(entityB, collectableInfo);
                //Debug.Log($"{collectableInfo.collectableCounter}");
            }
        }
    }

    protected override void OnUpdate()
    {
        var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
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

        var stats = SystemAPI.GetSingleton<CollectableInfo>();
        localCollectablesCounter = stats.collectableCounter;
    }

    protected override void OnDestroy()
    {
        playerStats.collectablesFound += localCollectablesCounter;
        jsonString = JsonUtility.ToJson(playerStats);
        Debug.Log(jsonString);
        if (useGoogleDrive)
        {
            GoogleDriveTools.Upload(jsonString);
        }
        else
        {
            PlayerPrefs.SetString("Stats", jsonString);
        } 
    }
}