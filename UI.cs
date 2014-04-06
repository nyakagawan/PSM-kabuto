
using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace kabuto
{
	public class UI : Sce.PlayStation.HighLevel.GameEngine2D.Node
	{
		public void TitleMode()
		{
//			Support.MusicSystem.Instance.Play("game_title_screen.mp3");
		}
		
		public void GameMode()
		{
//			Support.MusicSystem.Instance.Stop("game_title_screen.mp3");
		}
	}
}
