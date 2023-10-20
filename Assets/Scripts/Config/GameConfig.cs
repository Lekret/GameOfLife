using Data;
using UnityEngine;

namespace Config
{
    public class GameConfig : MonoBehaviour
    {
        [Header("Rules")] 
        public NeighboursCount LifeNeighboursToBecomeLife = NeighboursCount.Three;
        public NeighboursCount LifeNeighboursToLive = NeighboursCount.Two | NeighboursCount.Three;
        public int GridWidth;
        public int GridHeight;

        [Header("Simulation")] 
        public KeyCode SimulateKey = KeyCode.Space;
        public bool SimulateByKey;
        public float SimulationInterval;
        public GamePattern StartPattern;
        
        [Header("Graphics")] 
        public float DrawWidthSpacing;
        public float DrawHeightSpacing;
        public Vector3 DrawOrigin = new(0.5f, 0.5f);
        public Material LifeMaterial;
        public Material DeathMaterial;
        public Mesh CellMesh;
        public float CameraDistance = 15;
    }
}