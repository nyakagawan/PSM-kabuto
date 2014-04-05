using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace kabuto
{
	public class Layer : Node
    {
    }

	static public class EntryPoint
    {
        public static void Run(string[] args)
        {
            Sce.PlayStation.HighLevel.GameEngine2D.Director.Initialize( 1024*4 );

			Game.Instance = new Game();
            var game = Game.Instance;
            
            Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.RunWithScene(game.Scene,true);
            
			System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            while (true)
            {
            	timer.Start();
                SystemEvents.CheckEvents();

                //Sce.PlayStation.HighLevel.GameEngine2D.Camera.DrawDefaultGrid(32.0f);

                Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.GL.SetBlendMode(BlendMode.Normal);
                Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.Update();
                Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.Render();
				
                game.FrameUpdate();
                
            	timer.Stop();
                long ms = timer.ElapsedMilliseconds;
                //Console.WriteLine("ms: {0}", (int)ms);
            	timer.Reset();

                Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.GL.Context.SwapBuffers();
                Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.PostSwap();
            }
        }
    }
}

static class Program
{
	static void Main(string[] args) {
		kabuto.EntryPoint.Run(args);
	}
}
