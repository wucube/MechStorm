using MechStorm.Battle.Combat;
using TEngine;
using UnityEngine;
using Vector2Int = MechStorm.Battle.Foundation.Vector2Int;

namespace MechStorm.Presentation
{
    public class TempGameEntry : MonoBehaviour
    {
        [SerializeField]
        private Transform _plane;
        [SerializeField]
        private int _boardWidth = 5;
        [SerializeField]
        private int _boardHeight = 4;
        [SerializeField]
        private float _cellSize = 1f;
        [SerializeField]
        private Vector3 _boardOrigin = Vector3.zero;
        [SerializeField]
        private bool _showBoardDebugMarkers = true;

        private Transform _playerA;
        private Transform _playerB;
        private BattleBoardRenderer _boardRenderer;
        private CombatUnitVisual _playerAVisual;
        private GridCoordinateConverter _coordConverter;
        
        private CombatUnitFactory  _factory;
        private TurnStateMachine _turnStateMachine;

        
        void Start()
        {
            if (_plane == null)
            {
                Log.Error("[MechStorm] TempGameEntry requires a Plane transform.");
                return;
            }

            if (_boardWidth <= 0 || _boardHeight <= 0)
            {
                Log.Error("[MechStorm] Board width and height must be greater than zero.");
                return;
            }

            _coordConverter = new GridCoordinateConverter(_cellSize, _boardOrigin);
            _boardRenderer = new BattleBoardRenderer();
            _boardRenderer.Initialize(_boardWidth, _boardHeight, _cellSize, _boardOrigin, _plane);

            LogBoardValidation();

            if (_showBoardDebugMarkers)
            {
                CreateDebugMarker("Grid(0,0)", new Vector2Int(0, 0), Color.green);
                CreateDebugMarker("Grid(Max)", new Vector2Int(_boardWidth - 1, _boardHeight - 1), Color.red);
            }

            CreatePlayerAVisual();
        }

        private void LogBoardValidation()
        {
            var worldWidth = _boardWidth * _cellSize;
            var worldHeight = _boardHeight * _cellSize;
            var expectedScale = new Vector3(worldWidth / 10f, 1f, worldHeight / 10f);
            var expectedPosition = new Vector3(_boardOrigin.x + worldWidth / 2f, _boardOrigin.y, _boardOrigin.z + worldHeight / 2f);
            var firstCellCenter = _coordConverter.GridToWorld(new Vector2Int(0, 0));
            var lastCellCenter = _coordConverter.GridToWorld(new Vector2Int(_boardWidth - 1, _boardHeight - 1));

            Log.Info($"[MechStorm] Board expected scale: {expectedScale}, actual: {_plane.localScale}");
            Log.Info($"[MechStorm] Board expected position: {expectedPosition}, actual: {_plane.position}");
            Log.Info($"[MechStorm] Grid(0,0) center: {firstCellCenter}, Grid({_boardWidth - 1},{_boardHeight - 1}) center: {lastCellCenter}");
        }

        private void CreateDebugMarker(string markerName, Vector2Int gridPosition, Color color)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = $"BoardDebug_{markerName}";
            marker.transform.position = _coordConverter.GridToWorld(gridPosition) + Vector3.up * 0.05f;
            marker.transform.localScale = Vector3.one * Mathf.Min(_cellSize * 0.25f, 0.25f);

            if (marker.TryGetComponent<Renderer>(out var renderer))
            {
                renderer.material.color = color;
            }
        }

        private void CreatePlayerAVisual()
        {
            _factory = new CombatUnitFactory();

            var pilot = new PilotData(1, "Player A", 3);
            var mech = new MechData(1, "Training Mech", 10, 100, 3);
            var combatUnit = _factory.Create(pilot, mech, new Vector2Int(1, 1));

            var unitObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            unitObject.name = "PlayerA";
            unitObject.transform.localScale = new Vector3(_cellSize * 0.6f, _cellSize, _cellSize * 0.6f);

            _playerA = unitObject.transform;
            _playerAVisual = new CombatUnitVisual(_playerA, combatUnit, _coordConverter, _playerA.localScale.y * 0.5f);
            _playerAVisual.RefreshPosition();
            LogUnitStatus(combatUnit, _playerA.position);
        }

        private void LogUnitStatus(CombatUnit combatUnit, Vector3 worldPosition)
        {
            Log.Info(
                $"[MechStorm] Unit={combatUnit.Pilot.Name}/{combatUnit.Mech.Name}, " +
                $"Position=({combatUnit.Position.X},{combatUnit.Position.Y}), " +
                $"World={worldPosition}, " +
                $"HP={combatUnit.MechRuntime.CurrentDurability}/{combatUnit.Mech.MaxDurability}, " +
                $"AP={combatUnit.PilotRuntime.CurrentActionPoint}/{combatUnit.Pilot.MaxActionPoint}");
        }
    }
}
