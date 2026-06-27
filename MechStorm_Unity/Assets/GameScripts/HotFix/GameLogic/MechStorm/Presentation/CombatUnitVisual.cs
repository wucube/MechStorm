using System;
using MechStorm.Battle.Combat;
using UnityEngine;

namespace MechStorm.Presentation
{
    public sealed class CombatUnitVisual
    {
        private readonly Transform _unitTrans;
        private readonly CombatUnit _combatUnit;
        private readonly GridCoordinateConverter _coordConverter;
        private readonly float _visualHeightOffset;

        public CombatUnitVisual(Transform unitTrans, CombatUnit combatUnit, GridCoordinateConverter coordConverter, float visualHeightOffset = 0.5f)
        {
            _unitTrans = unitTrans ? unitTrans : throw new ArgumentNullException(nameof(unitTrans));
            _combatUnit = combatUnit ?? throw new ArgumentNullException(nameof(combatUnit));
            _coordConverter = coordConverter ?? throw new ArgumentNullException(nameof(coordConverter));
            _visualHeightOffset = visualHeightOffset;
        }

        public void RefreshPosition()
        {
            _unitTrans.position = _coordConverter.GridToWorld(_combatUnit.Position) + Vector3.up * _visualHeightOffset;
        }
    }
}
