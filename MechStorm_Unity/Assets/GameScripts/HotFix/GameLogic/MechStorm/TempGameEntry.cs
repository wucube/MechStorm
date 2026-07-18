using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using MechStorm.Battle;
using MechStorm.Battle.Data;
using MechStorm.Battle.Diagnostics;
using MechStorm.Battle.Units;
using MechStorm.Presentation.Board;
using MechStorm.Presentation.Controllers;
using MechStorm.Presentation.Input;
using MechStorm.Presentation.Units;
using TEngine;
using UnityEngine;
using Vector2Int = MechStorm.Battle.Foundation.Vector2Int;

namespace MechStorm.Presentation
{
    public class TempGameEntry : MonoBehaviour
    {
        private const int PlayerAUnitId = 1;
        private const int EnemyAUnitId = 2;

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
        [SerializeField]
        private LayerMask _battleInputLayerMask = Physics.DefaultRaycastLayers;

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

            if (marker.TryGetComponent<Collider>(out var markerCollider))
            {
                markerCollider.enabled = false;
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
            var mech = new MechData(1, "Training Mech", new BasicAttackData(10, 2, 5), 100, 4);
            _playerAUnit = factory.Create(PlayerAUnitId, pilot, mech, new Vector2Int(1, 1));
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
            var mech = new MechData(2, "Training Enemy Mech", new BasicAttackData(8, 1, 3), 100, 3);

            _enemyAUnit = factory.Create(
                EnemyAUnitId, pilot, mech,
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

            var inputAction = _presentationController.Tick(out var actionUnit);
            var actionResult = _presentationController.LastActionResult;
            switch (inputAction)
            {
                case BattleInputAction.CurrentUnitSelected:
                    Log.Info(
                        $"[MechStorm] Selected current unit " +
                        $"{actionUnit.Pilot.Name}.");
                    break;
                case BattleInputAction.CurrentUnitMoved:
                    Log.Info(
                        $"[MechStorm] {actionResult.ActorUnit.Pilot.Name} moved to " +
                        $"GridPosition: {actionResult.PositionAfter.Value}");
                    break;
                case BattleInputAction.TargetAttacked:
                    RefreshHealthBar(actionResult.TargetUnit);
                    Log.Info(
                        $"[MechStorm] {actionResult.ActorUnit.Pilot.Name} attacked " +
                        $"{actionResult.TargetUnit.Pilot.Name}, " +
                        $"HP={actionResult.DurabilityBefore}->{actionResult.DurabilityAfter}/" +
                        $"{actionResult.TargetUnit.Mech.MaxDurability}");
                    if (actionResult.DurabilityAfter <= 0)
                    {
                        Log.Info($"[MechStorm] {actionResult.TargetUnit.Pilot.Name} was destroyed.");
                    }
                    break;
                case BattleInputAction.ActionRejected:
                    Log.Warning(
                        $"[MechStorm] Battle input rejected: " +
                        $"{_presentationController.LastRejectionReason}");
                    break;
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

            var targetUnit = GetDebugAttackTarget();
            if (targetUnit == null)
            {
                Log.Warning(
                    $"[MechStorm] No debug attack target for faction " +
                    $"{_battleSession.CurrentFaction}.");
                return;
            }

            var result = _battleSession.AttackTargetCombatUnit(targetUnit);
            if (!result.IsSuccess)
            {
                Log.Warning($"[MechStorm] Debug attack rejected: {result.FailureReason}");
                return;
            }

            RefreshHealthBar(result.TargetUnit);
            Log.Info(
                $"[MechStorm] {result.ActorUnit.Pilot.Name} attacked " +
                $"{result.TargetUnit.Pilot.Name}, " +
                $"HP={result.DurabilityBefore}->{result.DurabilityAfter}/" +
                $"{result.TargetUnit.Mech.MaxDurability}");

            if (result.DurabilityAfter <= 0)
            {
                Log.Info($"[MechStorm] {result.TargetUnit.Pilot.Name} was destroyed.");
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
            var result = _presentationController != null
                ? _presentationController.EndCurrentUnitAction()
                : _battleSession.EndCurrentUnitAction();
            Log.Info(
                $"[MechStorm] {result.ActorUnit.Pilot.Name} ended action, " +
                $"Next={result.NextCombatUnit.Pilot.Name}, " +
                $"Round={result.RoundNumberBefore}->{result.RoundNumberAfter}, " +
                $"Faction={result.FactionBefore}->{result.FactionAfter}");
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

        public void ExportBattleDebugJsonForDebug()
        {
            if (_battleSession == null)
            {
                Log.Error("[MechStorm] Cannot export battle JSON before BattleSession is ready.");
                return;
            }

            ExportBattleDebugJsonAsync().Forget();
        }

        private async UniTask ExportBattleDebugJsonAsync()
        {
            try
            {
                var snapshot = _battleSession.CreateSnapshot();
                var json = BattleDebugJsonSerializer.Serialize(snapshot, _battleSession.ActionLogs);
                var directoryPath = GetBattleDebugDirectoryPath();
                Directory.CreateDirectory(directoryPath);

                var fileName = $"battle_{DateTime.Now:yyyyMMdd_HHmmss_fff}.json";
                var filePath = Path.Combine(directoryPath, fileName);
                await File.WriteAllTextAsync(filePath, json);

                Log.Info($"[MechStorm] Battle debug JSON exported to: {filePath}");
            }
            catch (Exception exception)
            {
                Log.Error($"[MechStorm] Failed to export battle debug JSON: {exception}");
            }
        }

        private static string GetBattleDebugDirectoryPath()
        {
#if UNITY_EDITOR
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Logs", "BattleDebug"));
#else
            return Path.Combine(Application.persistentDataPath, "BattleDebug");
#endif
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

            if (!_playerA.TryGetComponent<Collider>(out var playerACollider) ||
                !_enemyA.TryGetComponent<Collider>(out var enemyACollider))
            {
                Log.Error("[MechStorm] Combat unit input requires unit colliders.");
                return;
            }

            var combatUnitColliders =
                new Dictionary<Collider, CombatUnit>
                {
                    [playerACollider] = _playerAUnit,
                    [enemyACollider] = _enemyAUnit,
                };
            _boardInputter = new BattleBoardInputter(
                camera,
                boardCollider,
                _coordConverter,
                combatUnitColliders,
                _battleInputLayerMask.value);
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

            var combatUnitVisuals =
                new Dictionary<CombatUnit, CombatUnitVisual>
                {
                    [_playerAUnit] = _playerAVisual,
                    [_enemyAUnit] = _enemyAVisual,
                };
            _presentationController = new BattlePresentationController(_battleSession, combatUnitVisuals, _boardInputter);
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
