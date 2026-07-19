namespace MechStorm.Presentation.Board
{
    /// <summary>
    /// 棋盘格的表现高亮类型。
    /// </summary>
    public enum BattleCellHighlightType
    {
        /// <summary>可移动范围。</summary>
        Move,

        /// <summary>普通攻击覆盖范围。</summary>
        Attack,

        /// <summary>移动与攻击范围重叠。</summary>
        MoveAndAttack,

        /// <summary>合法攻击目标所在格。</summary>
        ValidTarget
    }
}