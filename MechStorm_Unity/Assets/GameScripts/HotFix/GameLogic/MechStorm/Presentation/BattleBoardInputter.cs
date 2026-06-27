using System;
using UnityEngine;
using Vector2Int = MechStorm.Battle.Foundation.Vector2Int;

namespace MechStorm.Presentation
{
    public sealed class BattleBoardInputter
    {
        private const float MAX_RAY_DISTANCE = 1000f;

        private readonly Camera _camera;
        private readonly Collider _collider;
        private readonly GridCoordinateConverter _coordinateConverter;

        public BattleBoardInputter(Camera camera, Collider collider, GridCoordinateConverter coordinateConverter)
        {
            _camera = camera ? camera : throw new ArgumentNullException(nameof(camera));
            _collider = collider ? collider : throw new ArgumentNullException(nameof(collider));
            _coordinateConverter = coordinateConverter ?? throw new ArgumentNullException(nameof(coordinateConverter));
        }

        public bool Tick(out Vector2Int gridPosition, out Vector3 worldPosition)
        {
            gridPosition = default;
            worldPosition = default;

            if (!Input.GetMouseButtonDown(0))
            {
                return false;
            }

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!_collider.Raycast(ray, out var hitInfo, MAX_RAY_DISTANCE))
            {
                return false;
            }

            worldPosition = hitInfo.point;
            gridPosition = _coordinateConverter.WorldToGrid(worldPosition);

            return true;
        }
    }
}
