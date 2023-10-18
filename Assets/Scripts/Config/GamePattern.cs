using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "Config/GamePattern", fileName = "GamePattern")]
    public class GamePattern : ScriptableObject
    {
        [Header("Random")]
        public bool UseRandom;
        public bool UseSeed;
        public int Seed;
        [Range(0f, 1f)] public float LifeProbability;

        [Header("Predefined")]
        public Vector2Int[] LifeCells;
    }
}