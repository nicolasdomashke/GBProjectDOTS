using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct MoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (localTransform, movementSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MovementSpeed>>())
        {
            localTransform.ValueRW = localTransform.ValueRW.RotateY(movementSpeed.ValueRO.speed * SystemAPI.Time.DeltaTime);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
