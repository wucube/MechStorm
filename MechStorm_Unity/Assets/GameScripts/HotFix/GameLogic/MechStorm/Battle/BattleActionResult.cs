using System;
using System.Collections.Generic;
using MechStorm.Battle.Foundation;

namespace MechStorm.Battle.Combat
{
    /// <summary>
    /// 一次战斗操作的执行结果。
    /// </summary>
    public sealed class BattleActionResult
    {
        /// <summary>
        /// 失败操作使用的空变化类型集合。
        /// </summary>
        private static readonly IReadOnlyList<BattleActionChangeType> _emptyChangeTypes = Array.Empty<BattleActionChangeType>();

        /// <summary>
        /// 本次战斗会话中的操作序号。
        /// </summary>
        public int Sequence { get; }

        /// <summary>
        /// 操作是否执行成功。
        /// </summary>
        public bool IsSuccess => FailureReason == BattleActionFailureReason.None;

        /// <summary>
        /// 本次尝试执行的操作类型。
        /// </summary>
        public BattleActionType ActionType { get; }

        /// <summary>
        /// 操作失败原因，成功时为 None。
        /// </summary>
        public BattleActionFailureReason FailureReason { get; }

        /// <summary>
        /// 执行操作的单位。
        /// </summary>
        public CombatUnit ActorUnit { get; }

        /// <summary>
        /// 操作目标单位，无目标时为 null。
        /// </summary>
        public CombatUnit TargetUnit { get; }

        /// <summary>
        /// 行动结束后的下一个单位，其他操作为 null。
        /// </summary>
        public CombatUnit NextCombatUnit { get; }

        /// <summary>
        /// 本次操作产生的有序战斗变化类型。
        /// </summary>
        public IReadOnlyList<BattleActionChangeType> ChangeTypes { get; }

        /// <summary>
        /// 移动前的位置，非移动操作为 null。
        /// </summary>
        public Vector2Int? PositionBefore { get; }

        /// <summary>
        /// 移动后的位置，非移动操作为 null。
        /// </summary>
        public Vector2Int? PositionAfter { get; }

        /// <summary>
        /// 受击前的耐久值，非攻击操作为 null。
        /// </summary>
        public int? DurabilityBefore { get; }

        /// <summary>
        /// 受击后的耐久值，非攻击操作为 null。
        /// </summary>
        public int? DurabilityAfter { get; }

        /// <summary>
        /// 行动结束前的回合数，其他操作为 null。
        /// </summary>
        public int? RoundNumberBefore { get; }

        /// <summary>
        /// 行动结束后的回合数，其他操作为 null。
        /// </summary>
        public int? RoundNumberAfter { get; }

        /// <summary>
        /// 行动结束前的当前阵营，其他操作为 null。
        /// </summary>
        public CombatFaction? FactionBefore { get; }

        /// <summary>
        /// 行动结束后的当前阵营，其他操作为 null。
        /// </summary>
        public CombatFaction? FactionAfter { get; }

        private BattleActionResult(int sequence, BattleActionType actionType, BattleActionFailureReason failureReason, CombatUnit actorUnit, CombatUnit targetUnit, CombatUnit nextCombatUnit,
            IReadOnlyList<BattleActionChangeType> changeTypes, Vector2Int? positionBefore = null, Vector2Int? positionAfter = null, int? durabilityBefore = null, int? durabilityAfter = null,
            int? roundNumberBefore = null, int? roundNumberAfter = null, CombatFaction? factionBefore = null, CombatFaction? factionAfter = null)
        {
            Sequence = sequence;
            ActionType = actionType;
            FailureReason = failureReason;
            ActorUnit = actorUnit;
            TargetUnit = targetUnit;
            NextCombatUnit = nextCombatUnit;
            ChangeTypes = changeTypes;
            PositionBefore = positionBefore;
            PositionAfter = positionAfter;
            DurabilityBefore = durabilityBefore;
            DurabilityAfter = durabilityAfter;
            RoundNumberBefore = roundNumberBefore;
            RoundNumberAfter = roundNumberAfter;
            FactionBefore = factionBefore;
            FactionAfter = factionAfter;
        }

        /// <summary>
        /// 创建移动成功结果。
        /// </summary>
        /// <param name="sequence">操作序号。</param>
        /// <param name="actorUnit">执行移动的单位。</param>
        /// <param name="positionBefore">移动前的位置。</param>
        /// <param name="positionAfter">移动后的位置。</param>
        public static BattleActionResult MoveSucceeded(int sequence, CombatUnit actorUnit, Vector2Int positionBefore, Vector2Int positionAfter)
        {
            if (actorUnit == null)
            {
                throw new ArgumentNullException(nameof(actorUnit));
            }

            return new BattleActionResult(sequence, BattleActionType.Move, BattleActionFailureReason.None, actorUnit,
                null, null, new[] { BattleActionChangeType.UnitMoved }, positionBefore, positionAfter);
        }

        /// <summary>
        /// 创建攻击成功结果。
        /// </summary>
        /// <param name="sequence">操作序号。</param>
        /// <param name="actorUnit">执行攻击的单位。</param>
        /// <param name="targetUnit">受到攻击的目标单位。</param>
        /// <param name="durabilityBefore">目标受击前的耐久值。</param>
        /// <param name="durabilityAfter">目标受击后的耐久值。</param>
        public static BattleActionResult AttackSucceeded(int sequence, CombatUnit actorUnit,
            CombatUnit targetUnit, int durabilityBefore, int durabilityAfter)
        {
            if (actorUnit == null)
            {
                throw new ArgumentNullException(nameof(actorUnit));
            }

            if (targetUnit == null)
            {
                throw new ArgumentNullException(nameof(targetUnit));
            }

            var changeTypes = durabilityAfter <= 0
                ? new[] { BattleActionChangeType.DamageApplied, BattleActionChangeType.UnitDied }
                : new[] { BattleActionChangeType.DamageApplied };

            return new BattleActionResult(sequence, BattleActionType.Attack, BattleActionFailureReason.None, actorUnit, targetUnit,
                null, changeTypes, durabilityBefore: durabilityBefore, durabilityAfter: durabilityAfter);
        }

        /// <summary>
        /// 创建单位行动结束结果。
        /// </summary>
        /// <param name="sequence">操作序号。</param>
        /// <param name="actorUnit">结束行动的单位。</param>
        /// <param name="nextCombatUnit">下一个行动单位。</param>
        /// <param name="roundNumberBefore">行动结束前的回合数。</param>
        /// <param name="roundNumberAfter">行动结束后的回合数。</param>
        /// <param name="factionBefore">行动结束前的当前阵营。</param>
        /// <param name="factionAfter">行动结束后的当前阵营。</param>
        public static BattleActionResult ActionEnded(int sequence, CombatUnit actorUnit, CombatUnit nextCombatUnit, int roundNumberBefore,
            int roundNumberAfter, CombatFaction factionBefore, CombatFaction factionAfter)
        {
            if (actorUnit == null)
            {
                throw new ArgumentNullException(nameof(actorUnit));
            }

            if (nextCombatUnit == null)
            {
                throw new ArgumentNullException(nameof(nextCombatUnit));
            }

            var changeTypes = new List<BattleActionChangeType>
            {
                BattleActionChangeType.UnitActionEnded
            };

            if (factionBefore != factionAfter)
            {
                changeTypes.Add(BattleActionChangeType.FactionTurnEnded);
            }

            if (roundNumberBefore != roundNumberAfter)
            {
                changeTypes.Add(BattleActionChangeType.RoundStarted);
            }

            if (factionBefore != factionAfter)
            {
                changeTypes.Add(BattleActionChangeType.FactionTurnStarted);
            }

            changeTypes.Add(BattleActionChangeType.CurrentUnitChanged);

            return new BattleActionResult(sequence, BattleActionType.EndUnitAction, BattleActionFailureReason.None, actorUnit, null,
                nextCombatUnit, changeTypes.ToArray(), roundNumberBefore: roundNumberBefore, roundNumberAfter: roundNumberAfter,
                factionBefore: factionBefore, factionAfter: factionAfter);
        }

        /// <summary>
        /// 创建操作失败结果。
        /// </summary>
        /// <param name="sequence">操作序号。</param>
        /// <param name="actionType">尝试执行的操作类型。</param>
        /// <param name="actorUnit">尝试执行操作的单位。</param>
        /// <param name="failureReason">操作失败原因。</param>
        public static BattleActionResult Failed(int sequence, BattleActionType actionType,
            CombatUnit actorUnit, BattleActionFailureReason failureReason)
        {
            if (actorUnit == null)
            {
                throw new ArgumentNullException(nameof(actorUnit));
            }

            if (failureReason == BattleActionFailureReason.None)
            {
                throw new ArgumentException("A failed action must provide a failure reason.", nameof(failureReason));
            }

            return new BattleActionResult(sequence, actionType, failureReason, actorUnit, null, null, _emptyChangeTypes);
        }
    }
}