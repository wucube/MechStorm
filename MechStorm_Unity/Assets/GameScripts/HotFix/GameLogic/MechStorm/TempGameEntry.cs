using System.Collections.Generic;
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

        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private int _debugDamage = 10;

        private Transform _playerA;
        private CombatUnitVisual _playerAVisual;
        private Transform _enemyA;
        private CombatUnitVisual _enemyAVisual;
        private GridCoordinateConverter _coordConverter;
        private BattleBoardInputter _boardInputter;
        private BattlePresentationController _presentationController;
        private BattleSession _battleSession;
        private CombatUnit _playerAUnit;
        private CombatUnit _enemyAUnit;
        private Transform _playerAHealthBarAnchor;
        private Transform _enemyAHealthBarAnchor;
        private UnitHealthBarView _playerAHealthBarView;
        private UnitHealthBarView _enemyAHealthBarView;

        public bool IsBattleReady => _battleSession != null;

        public int CurrentRoundNumber => _battleSession?.CurrentRoundNumber ?? 0;

        public string CurrentFactionName =>
            _battleSession?.CurrentFaction.ToString() ?? "Not Ready";

        public string CurrentUnitName =>
            _battleSession?.CurrentCombatUnit.Pilot.Name ?? "Not Ready";

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

            CreateBoard();
            LogBoardValidation();
            CreateDebugMarkers();
            CreatePlayerA();
            CreateEnemyA();
            CreateBattleSession();
            CreateBoardInputter();
            CreatePresentationController();
        }

        void Update()
        {
            TickPresentationController();
            RefreshHealthBarFacing();
        }

        private void CreateBoard()
        {
            _coordConverter = new GridCoordinateConverter(_cellSize, _boardOrigin);
            var boardRenderer = new BattleBoardRenderer();
            boardRenderer.Initialize(_boardWidth, _boardHeight, _cellSize, _boardOrigin, _plane);
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

        private void CreateDebugMarkers()
        {
            if (!_showBoardDebugMarkers)
            {
                return;
            }

            CreateDebugMarker("Grid(0,0)", new Vector2Int(0, 0), Color.green);
            CreateDebugMarker("Grid(Max)", new Vector2Int(_boardWidth - 1, _boardHeight - 1), Color.red);
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

        private void CreatePlayerA()
        {
            CreatePlayerAUnit();
            CreatePlayerAVisual();
            CreatePlayerAHealthBar();
        }

        private void CreatePlayerAUnit()
        {
            var factory = new CombatUnitFactory();
            var pilot = new PilotData(1, "Player A", 3);
            var mech = new MechData(1, "Training Mech", 10, 100, 3);

            _playerAUnit = factory.Create(pilot, mech, new Vector2Int(1, 1));
        }

        private void CreatePlayerAVisual()
        {
            var unitObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            unitObject.name = "PlayerA";
            unitObject.transform.localScale = new Vector3(_cellSize * 0.6f, _cellSize, _cellSize * 0.6f);

            var anchorObject = new GameObject("HealthBarAnchor");
            anchorObject.transform.SetParent(unitObject.transform, false);
            anchorObject.transform.localPosition = Vector3.up * 0.8f;
            anchorObject.transform.localScale = Vector3.one;
            _playerAHealthBarAnchor = anchorObject.transform;

            _playerA = unitObject.transform;
            _playerAVisual = new CombatUnitVisual(_playerA, _coordConverter, _playerA.localScale.y * 0.5f);
            _playerAVisual.RefreshPosition(_playerAUnit.Position);
            LogUnitStatus(_playerAUnit, _playerA.position);
        }

        private void CreatePlayerAHealthBar()
        {
            if (_playerAHealthBarAnchor == null || _playerAUnit == null)
            {
                Log.Error("[MechStorm] PlayerA health bar dependencies are not ready.");
                return;
            }

            var camera = _camera != null ? _camera : Camera.main;
            _playerAHealthBarView = new UnitHealthBarView(_playerAHealthBarAnchor, camera);
            RefreshPlayerAHealthBar();
            _playerAHealthBarView.RefreshFacing();
        }

        private void CreateEnemyA()
        {
            CreateEnemyAUnit();
            CreateEnemyAVisual();
            CreateEnemyAHealthBar();
        }

        private void CreateEnemyAUnit()
        {
            var factory = new CombatUnitFactory();
            var pilot = new PilotData(2, "Enemy A", 3);
            var mech = new MechData(2, "Training Enemy Mech", 10, 100, 3);

            _enemyAUnit = factory.Create(
                pilot,
                mech,
                new Vector2Int(_boardWidth - 1, _boardHeight - 1));
        }

        private void CreateEnemyAVisual()
        {
            var unitObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            unitObject.name = "EnemyA";
            unitObject.transform.localScale = new Vector3(
                _cellSize * 0.6f,
                _cellSize,
                _cellSize * 0.6f);

            var anchorObject = new GameObject("HealthBarAnchor");
            anchorObject.transform.SetParent(unitObject.transform, false);
            anchorObject.transform.localPosition = Vector3.up * 0.8f;
            anchorObject.transform.localScale = Vector3.one;
            _enemyAHealthBarAnchor = anchorObject.transform;

            _enemyA = unitObject.transform;
            _enemyAVisual = new CombatUnitVisual(
                _enemyA,
                _coordConverter,
                _enemyA.localScale.y * 0.5f);
            _enemyAVisual.RefreshPosition(_enemyAUnit.Position);
            LogUnitStatus(_enemyAUnit, _enemyA.position);
        }

        private void CreateEnemyAHealthBar()
        {
            if (_enemyAHealthBarAnchor == null || _enemyAUnit == null)
            {
                Log.Error("[MechStorm] EnemyA health bar dependencies are not ready.");
                return;
            }

            var camera = _camera != null ? _camera : Camera.main;
            _enemyAHealthBarView = new UnitHealthBarView(
                _enemyAHealthBarAnchor,
                camera);
            RefreshEnemyAHealthBar();
            _enemyAHealthBarView.RefreshFacing();
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

        private void TickPresentationController()
        {
            if (_presentationController == null)
            {
                return;
            }

            if (_presentationController.Tick())
            {
                var currentUnit = _battleSession.CurrentCombatUnit;
                Log.Info(
                    $"[MechStorm] {currentUnit.Pilot.Name} moved to " +
                    $"GridPosition: {currentUnit.Position}");
            }
        }

        private void RefreshPlayerAHealthBar()
        {
            if (_playerAUnit == null || _playerAHealthBarView == null)
            {
                return;
            }

            _playerAHealthBarView.RefreshValue(
                _playerAUnit.MechRuntime.CurrentDurability,
                _playerAUnit.Mech.MaxDurability);
        }

        private void RefreshEnemyAHealthBar()
        {
            if (_enemyAUnit == null || _enemyAHealthBarView == null)
            {
                return;
            }

            _enemyAHealthBarView.RefreshValue(
                _enemyAUnit.MechRuntime.CurrentDurability,
                _enemyAUnit.Mech.MaxDurability);
        }

        private void RefreshHealthBarFacing()
        {
            _playerAHealthBarView?.RefreshFacing();
            _enemyAHealthBarView?.RefreshFacing();
        }

        public void ApplyDebugDamageToPlayerA()
        {
            if (_playerAUnit == null || _playerAHealthBarView == null)
            {
                Log.Error("[MechStorm] Cannot apply debug damage because PlayerA or health bar is missing.");
                return;
            }

            if (_debugDamage <= 0)
            {
                Log.Warning("[MechStorm] Debug damage must be greater than zero.");
                return;
            }

            if (_playerAUnit.MechRuntime.IsDestroyed)
            {
                Log.Info("[MechStorm] PlayerA is already destroyed.");
                return;
            }

            _playerAUnit.MechRuntime.TakeDamage(_debugDamage);
            RefreshPlayerAHealthBar();

            Log.Info(
                $"[MechStorm] PlayerA took {_debugDamage} damage, " +
                $"HP={_playerAUnit.MechRuntime.CurrentDurability}/{_playerAUnit.Mech.MaxDurability}");
        }

        public void AttackCurrentOpponentForDebug()
        {
            if (_battleSession == null)
            {
                Log.Error("[MechStorm] Cannot attack before BattleSession is ready.");
                return;
            }

            var attackerUnit = _battleSession.CurrentCombatUnit;
            var targetUnit = GetDebugAttackTarget();
            if (targetUnit == null)
            {
                Log.Warning(
                    $"[MechStorm] No debug attack target for faction " +
                    $"{_battleSession.CurrentFaction}.");
                return;
            }

            var durabilityBefore = targetUnit.MechRuntime.CurrentDurability;
            try
            {
                _battleSession.AttackTargetCombatUnit(targetUnit);
            }
            catch (System.ArgumentException exception)
            {
                Log.Warning($"[MechStorm] Debug attack rejected: {exception.Message}");
                return;
            }
            catch (System.InvalidOperationException exception)
            {
                Log.Warning($"[MechStorm] Debug attack rejected: {exception.Message}");
                return;
            }

            RefreshHealthBar(targetUnit);
            var durabilityAfter = targetUnit.MechRuntime.CurrentDurability;
            Log.Info(
                $"[MechStorm] {attackerUnit.Pilot.Name} attacked " +
                $"{targetUnit.Pilot.Name}, " +
                $"HP={durabilityBefore}->{durabilityAfter}/" +
                $"{targetUnit.Mech.MaxDurability}");

            if (targetUnit.IsDead())
            {
                Log.Info($"[MechStorm] {targetUnit.Pilot.Name} was destroyed.");
            }
        }

        public void EndCurrentUnitActionForDebug()
        {
            if (_battleSession == null)
            {
                Log.Error("[MechStorm] Cannot end unit action before BattleSession is ready.");
                return;
            }

            LogCurrentBattleState("Before End Action");
            _battleSession.EndCurrentUnitAction();
            LogCurrentBattleState("After End Action");
        }

        public void LogCurrentBattleStateForDebug()
        {
            if (_battleSession == null)
            {
                Log.Error("[MechStorm] Cannot log battle state before BattleSession is ready.");
                return;
            }

            LogCurrentBattleState("Current Battle State");
        }

        private CombatUnit GetDebugAttackTarget()
        {
            return _battleSession.CurrentFaction switch
            {
                CombatFaction.TeamA => _enemyAUnit,
                CombatFaction.TeamB => _playerAUnit,
                _ => null,
            };
        }

        private void RefreshHealthBar(CombatUnit combatUnit)
        {
            if (ReferenceEquals(combatUnit, _playerAUnit))
            {
                RefreshPlayerAHealthBar();
                return;
            }

            if (ReferenceEquals(combatUnit, _enemyAUnit))
            {
                RefreshEnemyAHealthBar();
            }
        }

        private void LogCurrentBattleState(string label)
        {
            var currentUnit = _battleSession.CurrentCombatUnit;
            Log.Info(
                $"[MechStorm] {label}: " +
                $"Round={_battleSession.CurrentRoundNumber}, " +
                $"Faction={_battleSession.CurrentFaction}, " +
                $"Unit={currentUnit.Pilot.Name}/{currentUnit.Mech.Name}");
        }

        private void CreateBoardInputter()
        {
            var camera = _camera != null ? _camera : Camera.main;
            if (camera == null)
            {
                Log.Error("[MechStorm] BattleBoardInputter requires a camera.");
                return;
            }

            if (!_plane.TryGetComponent<Collider>(out var boardCollider))
            {
                Log.Error("[MechStorm] BattleBoardInputter requires a collider on the board plane.");
                return;
            }

            _boardInputter = new BattleBoardInputter(camera, boardCollider, _coordConverter);
        }

        private void CreatePresentationController()
        {
            if (_battleSession == null ||
                _playerAVisual == null ||
                _enemyAVisual == null ||
                _boardInputter == null)
            {
                Log.Error("[MechStorm] BattlePresentationController dependencies are not ready.");
                return;
            }

            var unitVisualsByCombatUnit =
                new Dictionary<CombatUnit, CombatUnitVisual>
                {
                    [_playerAUnit] = _playerAVisual,
                    [_enemyAUnit] = _enemyAVisual,
                };
            _presentationController = new BattlePresentationController(
                _battleSession,
                unitVisualsByCombatUnit,
                _boardInputter);
        }

        private void CreateBattleSession()
        {
            if (_playerAUnit == null || _enemyAUnit == null)
            {
                Log.Error("[MechStorm] BattleSession requires PlayerA and EnemyA.");
                return;
            }

            _battleSession = new BattleSession(
                _boardWidth,
                _boardHeight,
                new[] { _playerAUnit },
                new[] { _enemyAUnit });
            LogCurrentBattleState("Battle Started");
        }
    }
}
