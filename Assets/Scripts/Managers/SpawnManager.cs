
namespace TankGame
{
    public class SpawnManager : Singleton<SpawnManager>
    {
        public void PreloadPools()
        {
            ObjectPoolManager.Instance.Load("Prefabs/Bullet");
            ObjectPoolManager.Instance.Load("Prefabs/BulletExplosion");
        }

        public void UnloadPools()
        {
            ObjectPoolManager.Instance.Unload("Prefabs/Bullet");
            ObjectPoolManager.Instance.Unload("Prefabs/BulletExplosion");
        }
    }
}
