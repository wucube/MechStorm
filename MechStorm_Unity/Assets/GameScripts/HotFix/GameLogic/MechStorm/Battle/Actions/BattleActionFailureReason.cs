namespace MechStorm.Battle.Actions
{
    public enum BattleActionFailureReason
    {
        None,
        InvalidMoveTarget,
        TargetNotAdjacent,
        SameFactionTarget,
        TargetAlreadyDead,
        ActorCannotAct
    }
}