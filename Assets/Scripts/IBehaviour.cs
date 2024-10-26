using Unity.Entities;
public interface IBehaviour
{
    public float Evaluate(Entity entity, EntityManager entityManager, float dTime);
    public void Execute(Entity entity, EntityManager entityManager, float dTime);
}
