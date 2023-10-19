using System;
using Config;
using UnityEngine;

namespace PureImpl
{
    public class GameOfLife : MonoBehaviour
    {
        [SerializeField] private KeyCode _restartKey = KeyCode.R;
        [SerializeField] private GameConfig _config;

        private static readonly NeighbourFlags[] _neighboursToFlag = (NeighbourFlags[]) Enum.GetValues(typeof(NeighbourFlags));
        private GameInstance _instance;
        private Transform _parent;

        private void Start()
        {
            _parent = new GameObject("Graphics").transform;
            RecreateGame();
        }

        private void RecreateGame()
        {
            if (_instance != null)
            {
                for (int i = 0, end = _instance.CellCount; i < end; i++)
                {
                    var renderable = _instance.Renderables[i];
                    if (renderable.Renderer)
                        Destroy(renderable.Renderer.gameObject);
                }

                _instance.Dispose();
            }

            _instance = GameOfLifeInitialization.CreateInstance(_config);

            for (int i = 0, end = _instance.CellCount; i < end; i++)
            {
                var cellObj = new GameObject("Cell");
                var renderable = new Renderable
                {
                    Renderer = cellObj.AddComponent<MeshRenderer>(),
                    Filter = cellObj.AddComponent<MeshFilter>()
                };
                cellObj.transform.SetParent(_parent);
                var position = _instance.Position[i];
                cellObj.transform.position = _config.DrawOrigin + new Vector3(
                    position.x + position.x * _config.DrawWidthSpacing,
                    position.y + position.y * _config.DrawHeightSpacing);
                _instance.Renderables[i] = renderable;
            }
        }

        private void Update()
        {
            if (ShouldTriggerSimulation())
            {
                ApplyLifeNextSim();
                SimulateCells();
                UpdateCellGraphics();
            }

            UpdateCameraPosition();

            if (Input.GetKeyDown(_restartKey))
            {
                RecreateGame();
            }
        }

        private void ApplyLifeNextSim()
        {
            for (int i = 0, end = _instance.CellCount; i < end; i++)
            {
                _instance.IsLife[i] = _instance.IsLifeNextSim[i];
            }
        }

        private bool ShouldTriggerSimulation()
        {
            if (_config.SimulateByKey)
                return Input.GetKeyDown(_config.SimulateKey);

            _instance.SimulationInterval -= Time.deltaTime;
            if (_instance.SimulationInterval > 0)
                return false;

            _instance.SimulationInterval = _config.SimulationInterval;
            return true;
        }

        private void SimulateCells()
        {
            var neighboursArray = _instance.Neighbours;
            var isLifeArray = _instance.IsLife;
            var isLifeNextSimArray = _instance.IsLifeNextSim;
            
            for (int i = 0, end = _instance.CellCount; i < end; i++)
            {
                var neighbours = neighboursArray[i];
                var lifeNeighbours = 0;

                bool IsNeighbourLife(ref int neighbour) => neighbour != -1 && isLifeArray[neighbour];
                if (IsNeighbourLife(ref neighbours.N)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.NE)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.E)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.SE)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.S)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.SW)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.W)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.NW)) lifeNeighbours++;

                var neighboursTestFlags = isLifeArray[i] ? _config.LifeNeighboursToLive : _config.LifeNeighboursToBecomeLife;
                var neighboursCountFlag = _neighboursToFlag[lifeNeighbours];
                isLifeNextSimArray[i] = (neighboursTestFlags & neighboursCountFlag) != 0;
            }
        }

        private void UpdateCellGraphics()
        {
            for (int i = 0, end = _instance.CellCount; i < end; i++)
            {
                var renderable = _instance.Renderables[i];
                var material = _instance.IsLife[i] ? _config.LifeMaterial : _config.DeathMaterial;
                renderable.Renderer.material = material;
                renderable.Filter.mesh = _config.CellMesh;
            }
        }

        private void UpdateCameraPosition()
        {
            var width = _instance.GridWidth;
            var height = _instance.GridHeight;
            var cameraPosition = new Vector3
            (
                width / 2f + _config.DrawWidthSpacing * (width / 2f),
                height / 2f + _config.DrawHeightSpacing * (height / 2f),
                -_config.CameraDistance
            );
            Camera.main.transform.position = cameraPosition;
        }
    }
}