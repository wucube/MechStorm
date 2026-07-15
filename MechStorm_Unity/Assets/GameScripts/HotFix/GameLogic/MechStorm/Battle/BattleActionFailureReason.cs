namespace MechStorm.Battle.Combat
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