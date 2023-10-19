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
                foreach (var cell in _instance.Cells)
                {
                    if (cell.Renderable.Renderer)
                        Destroy(cell.Renderable.Renderer.gameObject);
                }
            }

            _instance = GameOfLifeInitialization.CreateInstance(_config);

            foreach (var cell in _instance.Cells)
            {
                var cellObj = new GameObject("Cell");
                var renderable = new Renderable
                {
                    Renderer = cellObj.AddComponent<MeshRenderer>(),
                    Filter = cellObj.AddComponent<MeshFilter>()
                };
                cellObj.transform.SetParent(_parent);
                cellObj.transform.position = _config.DrawOrigin + new Vector3(
                    cell.Position.x + cell.Position.x * _config.DrawWidthSpacing,
                    cell.Position.y + cell.Position.y * _config.DrawHeightSpacing);
                cell.Renderable = renderable;
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
            foreach (var cell in _instance.Cells)
            {
                cell.IsLife = cell.IsLifeNextSim;
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
            foreach (var cell in _instance.Cells)
            {
                var neighbours = cell.Neighbours;
                var lifeNeighbours = 0;

                bool IsNeighbourLife(ref Cell neighbour) => neighbour != null && neighbour.IsLife;
                if (IsNeighbourLife(ref neighbours.N)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.NE)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.E)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.SE)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.S)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.SW)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.W)) lifeNeighbours++;
                if (IsNeighbourLife(ref neighbours.NW)) lifeNeighbours++;

                var neighboursTestFlags = cell.IsLife ? _config.LifeNeighboursToLive : _config.LifeNeighboursToBecomeLife;
                var neighboursCountFlag = _neighboursToFlag[lifeNeighbours];
                cell.IsLifeNextSim = (neighboursTestFlags & neighboursCountFlag) != 0;
            }
        }

        private void UpdateCellGraphics()
        {
            foreach (var cell in _instance.Cells)
            {
                var renderable = cell.Renderable;
                var material = cell.IsLife ? _config.LifeMaterial : _config.DeathMaterial;
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