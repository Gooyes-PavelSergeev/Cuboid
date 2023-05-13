using Gooyes.Tools;
using UnityEngine;

namespace Cubic
{
    public class CuboidSpawner : Singleton<CuboidSpawner>
    {
        [SerializeField] private Cuboid _cuboidPrefab;

        public Cuboid SpawnCuboid(SpawnConfig spawnConfig)
        {
            Cuboid cuboid = Instantiate(_cuboidPrefab);
            cuboid.Init(spawnConfig);
            return cuboid;
        }
    }

    public struct SpawnConfig
    {
        public SpawnSource spawnSource;
        public int power;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 force;
    }

    public enum SpawnSource
    {
        OtherCuboid,
        Sequence
    }
}
