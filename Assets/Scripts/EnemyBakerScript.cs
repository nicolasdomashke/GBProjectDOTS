using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;
using Unity.Mathematics;
using System.Threading.Tasks;
using Zenject;

class EnemyBakerScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject shootEffect;
    public GameObject deathEffect;
    public GameObject animPrefab;
}

class EnemyBakerScriptBaker : Baker<EnemyBakerScript>
{
    public override void Bake(EnemyBakerScript authoring)
    {
        //Config projConfig = ProjectContext.Instance.Container.Resolve<Config>();

        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        ConfigLoad(entity, authoring);
        AddComponent(entity, new EnemyTag {deathEffect = GetEntity(authoring.deathEffect, TransformUsageFlags.Dynamic)});
        //AddComponent(entity, new Dashable {dashCD = projConfig.settings.enemyDashCD, dashDuration = projConfig.settings.enemyDashDuration, dashBoost = projConfig.settings.enemyDashBoost, dashCDCurrent = 0f, dashBoostCurrent = 0f});
        //AddComponent(entity, new Shooter {bullet = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic), shootEffect = GetEntity(authoring.shootEffect, TransformUsageFlags.Dynamic), gunCD = projConfig.settings.enemyGunCD, bulletSpeed = projConfig.settings.enemyBulletSpeed});
        var animObject = Object.Instantiate(authoring.animPrefab);
        AddComponentObject(entity, new EnemyAnim {animObject = animObject, anim = animObject.GetComponent<Animator>()});
        AddComponent(entity, new EvatuationData {distanceToClosestBullet = 0f});
        var capsuleCollider = Unity.Physics.CapsuleCollider.Create(
            new CapsuleGeometry
            {
                Radius = .5f,
                Vertex0 = new float3(0f, 1.3f, 0f),
                Vertex1 = new float3(0f, 0f, 0f),
            });
        AddComponent(entity, new PhysicsCollider {Value = capsuleCollider});
    }
    public async void ConfigLoad(Entity e, EnemyBakerScript authoring)
    {
        await ProjectContextLoad(e, authoring);
    }

    async Task ProjectContextLoad(Entity entity, EnemyBakerScript authoring)
    {
        Config projConfig = ProjectContext.Instance.Container.Resolve<Config>();
        AddComponent(entity, new Dashable {dashCD = projConfig.settings.enemyDashCD, dashDuration = projConfig.settings.enemyDashDuration, dashBoost = projConfig.settings.enemyDashBoost, dashCDCurrent = 0f, dashBoostCurrent = 0f});
        AddComponent(entity, new Shooter {bullet = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic), shootEffect = GetEntity(authoring.shootEffect, TransformUsageFlags.Dynamic), gunCD = projConfig.settings.enemyGunCD, bulletSpeed = projConfig.settings.enemyBulletSpeed});
    }
}

public struct RotationSpeed : IComponentData
{
    public float speed;
}
public struct EnemyTag : IComponentData {
    public Entity deathEffect;
}
public class EnemyAnim : IComponentData
{
    public GameObject animObject;
    public Animator anim;
}