using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

public partial class AnimSystem : SystemBase
{
    protected override void OnCreate()
    {

    }

    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerTag>().ForEach((ref LocalTransform transform, ref PhysicsVelocity velocity, in PlayerAnim animator) =>
        {
            animator.animObject.transform.position = transform.Position;
            animator.animObject.transform.rotation = transform.Rotation;
            animator.anim.SetFloat("Speed", ((Vector3)velocity.Linear).magnitude);
        }).WithoutBurst().Run();
        

        Entities.WithNone<PlayerTag>().ForEach((Entity e, in PlayerAnim animator) =>
        {
            animator.anim.SetBool("IsDead", true);
            EntityManager.RemoveComponent<PlayerAnim>(e);
        }).WithStructuralChanges().Run();

        Entities.WithAll<EnemyTag>().ForEach((ref LocalTransform transform, ref PhysicsVelocity velocity, in EnemyAnim animator) =>
        {
            animator.animObject.transform.position = transform.Position;
            animator.animObject.transform.rotation = transform.Rotation;
            animator.anim.SetFloat("Speed", ((Vector3)velocity.Linear).magnitude);
        }).WithoutBurst().Run();
        

        Entities.WithNone<EnemyTag>().ForEach((Entity e, in EnemyAnim animator) =>
        {
            animator.anim.SetBool("IsDead", true);
            EntityManager.RemoveComponent<PlayerAnim>(e);
        }).WithStructuralChanges().Run();
    }

    protected override void OnDestroy()
    {
        
    }
}
