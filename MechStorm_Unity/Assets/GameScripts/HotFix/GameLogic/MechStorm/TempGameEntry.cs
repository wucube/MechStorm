using MechStorm.Battle.Combat;
using UnityEngine;

namespace MechStorm.Presentation
{
    public class TempGameEntry : MonoBehaviour
    {
        [SerializeField]
        private Transform _plane;
        private Transform _playerA;
        private Transform _playerB;
        private BattleBoardRenderer _boardRenderer;
        private GridCoordinateConverter _coordConverter;
        
        private CombatUnitFactory  _factory;
        private TurnStateMachine _turnStateMachine;
        
        
        void Start()
        {
            
        }
        
        
    }
}