using UnityEngine;

namespace Components
{
    public class GameConfig : MonoBehaviour
    {
        [Header("Rules")] 
        public int[] AliveNeighboursToBecomeAlive = {3};
        public int[] AliveNeighboursToLive = {2, 3};
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
        public Material AliveMaterial;
        public Material DeadMaterial;
        public Mesh CellMesh;
        public float CameraDistance = 15;
    }
}