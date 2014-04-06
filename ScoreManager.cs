using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;

namespace kabuto
{
	public class ScoreManager
	{
		public int GameScore { get; set; }
		public int HiCombo { get; set; }
		
		public ScoreManager ()
		{
			GameScore = 0;
			HiCombo = 0;
		}
		
		public void AddScore(int baseScore, int killerGeneration)
		{
			int rate = (int)Math.Pow(2, killerGeneration);
			GameScore += baseScore * rate;
			if( killerGeneration>HiCombo ) {
				HiCombo = killerGeneration;
			}
		}
	}
}

