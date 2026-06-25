using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MechStorm.Presentation
{
    public class BattleBoardRenderer
    {
	    
		private Transform _plane;

		
		public void Initialize(int width, int height, float cellSize, Vector3 origin, Transform plane)
		{
			// 棋盘在 Unity 世界中的实际宽度和深度。
			float worldWidth = width * cellSize;
			float worldHeight = height * cellSize;
			_plane = plane;

			// Unity 默认 Plane 原始尺寸是 10x10，所以缩放值 = 目标尺寸 / 10。
			_plane.localScale = new Vector3(worldWidth / 10,1f, worldHeight / 10);

			// origin 是棋盘左下角，Plane 的 position 是中心点，所以需要加半个棋盘尺寸。
			_plane.position = new Vector3(origin.x + worldWidth / 2, origin.y, origin.z + worldHeight / 2);
		}
    }
}
