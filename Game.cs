using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace kabuto
{
	public class Game
    {
    	public static Game Instance;// = new Game();
    	
        public Sce.PlayStation.HighLevel.GameEngine2D.Scene Scene { get; set; }
        public Layer Background { get; set; }
        public Layer World { get; set; }
        public Layer EffectsLayer { get; set; }
        public Layer Foreground { get; set; }
        public Layer Curtains { get; set; }
        public Layer Interface { get; set; }
        
		public Random Random { get; set; }
        public Player Player { get; set; }
        public EntityCollider Collider { get; set; }
		public Support.ParticleEffectsManager ParticleEffects { get; set; }
		public Support.TextureTileMapManager TextureTileMaps { get; set; }
		public UI UI { get; set; }
        
		public List<GameEntity> AddQueue { get; set; }
		public List<GameEntity> RemoveQueue { get; set; }
		
        public float FloorHeight = 60.0f;
        public float WorldScale = 1.0f;
        
		public Vector2 TitleCameraCenter { get; set; }
		public Vector2 CameraTarget { get; set; }
		
		public bool PlayerDead { get; set; }
		public Vector2 ScreenSize { get { return new Vector2(960.0f, 544.0f); } }
		
		public SpriteBatch SpriteBatch;
		public DebugString DebugString { get; set; }
		
		public EnemySpawner EnemySpawner { get; set; }
		public ScoreManager ScoreManager { get; set; }

        public Game()
        {
//			Director.Instance.DebugFlags |= DebugFlags.Navigate; // press left alt + mouse to navigate in 2d space
//			Director.Instance.DebugFlags |= DebugFlags.DrawGrid;
			Director.Instance.DebugFlags |= DebugFlags.DrawContentWorldBounds;
			Director.Instance.DebugFlags |= DebugFlags.DrawContentLocalBounds;
//			Director.Instance.DebugFlags |= DebugFlags.DrawTransform;
//			Director.Instance.DebugFlags |= DebugFlags.DrawPivot;
			
			DebugString = new DebugString(Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.GL.Context);
            Scene = new Sce.PlayStation.HighLevel.GameEngine2D.Scene();
            Background = new Layer();
            World = new Layer();
            EffectsLayer = new Layer();
            Foreground = new Layer();
            Curtains = new Layer();
            Interface = new Layer();
            Random = new Random();
            Collider = new EntityCollider();
            ParticleEffects = new Support.ParticleEffectsManager();
            TextureTileMaps = new Support.TextureTileMapManager();
            UI = new UI();
			ScoreManager = new ScoreManager();

			SpriteBatch = new SpriteBatch();

			BuildTextureTileMaps();
            
			AddQueue = new List<GameEntity>();
			RemoveQueue = new List<GameEntity>();

            Scene.AddChild(Background);
            Scene.AddChild(World);
            Scene.AddChild(EffectsLayer);
            Scene.AddChild(Foreground);
            Scene.AddChild(Interface);
            Scene.AddChild(Curtains);
			
			Scene.Camera.SetViewFromViewport();
			
			// temporary: munge viewport to match vita + assets
			Vector2 ideal_screen_size = ScreenSize;
			Camera2D camera = Scene.Camera as Camera2D;
			camera.SetViewFromHeightAndCenter(ideal_screen_size.Y, ideal_screen_size / 2.0f);
			TitleCameraCenter = camera.Center;
			CameraTarget = TitleCameraCenter;
			
			if(ParticleEffects!=null)
				EffectsLayer.AddChild(ParticleEffects);
			Interface.AddChild(UI);
		
            // world
            var bg_forest = Support.SpriteFromFile("/Application/assets/background_test2.png");
			bg_forest.Scale *= 1;

            bg_forest.Position = new Vector2(0f, 0.0f);
			
            Background.AddChild(bg_forest);
			
			UI.TitleMode();
			
			Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Schedule(Scene, TickTitle, 0.0f, false);
			
			World.AdHocDraw += this.DrawWorld;
		}
		
		public void Draw() {
			if(DebugString!=null) {
				DebugString.Render();
			}
		}
        
		public void TickTitle(float dt)
		{
			((Camera2D)Scene.Camera).Center = TitleCameraCenter;
			
			//LightShafts.Color.A = FMath.Sin(Game.Instance.UI.FrameCount * 0.01f) * 0.15f;
			
//			if (PlayerInput.AnyButton())
			{
	            Player = new Player();
	            World.AddChild(Player);
				World.AddChild(SpriteBatch);
	            
				Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Unschedule(Scene, this.TickTitle);
				Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Schedule(Scene, this.TickGame, 0.0f, false);

				UI.GameMode();

				StartEnemySpawning();
			}
		}

		/// <summary>
		/// Game main update
		/// </summary>
		/// <param name='dt'>
		/// delta time
		/// </param>
		public void TickGame(float dt)
		{
			if (Player == null)
			{
				Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Unschedule(Scene, this.TickGame);
				Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Schedule(Scene, this.TickTitle, 0.0f, false);
				UI.TitleMode();

				StopEnemySpawning();
			}
			else
			{
#if false
				const float Border = 200.0f;
				const float Limit = 160.0f;
				
				// Pull camera to left/right if near screen edge
				Camera2D camera = (Camera2D)Director.Instance.CurrentScene.Camera;
				Vector2 world_position = Player.GetCollisionCenter(Player.BodySprite);
				CameraTarget = new Vector2(world_position.X, CameraTarget.Y);
				
				Vector2 offset = CameraTarget - camera.Center;
				if (offset.LengthSquared() > 0.0f)
				{
					float distance = offset.Length();
					if (distance > Border)
					{
						camera.Center += offset * 0.01f;
						camera.Center = new Vector2(
							FMath.Clamp(camera.Center.X, TitleCameraCenter.X - Limit, TitleCameraCenter.X + Limit),
							camera.Center.Y
						);
					}
				}

				// don't play if player is dead
				if (Player.Health > 0.0f)
					Support.MusicSystem.Instance.PlayNoClobber("game_game_music.mp3");
#endif
			}
		}

		public void StartEnemySpawning()
		{
			var spawner = new EnemySpawner();
			World.AddChild(spawner);
			EnemySpawner = spawner;
		}

		public void StopEnemySpawning()
		{
			EnemySpawner = null;
		}

		public void BuildTextureTileMaps()
		{
			TextureTileMaps.Add("EnemyBat", Support.TiledSpriteFromFile("/Application/assets/bat_frames.png", 2, 2).TextureInfo.Texture, 2, 2);
#if false
			TextureTileMaps.Add("Player", Support.TiledSpriteFromFile("/Application/assets/sir_awesome_frames.png", 4, 4).TextureInfo.Texture, 4, 4);
			TextureTileMaps.Add("EnemySlime", Support.TiledSpriteFromFile("/Application/assets/slime_green_frames.png", 4, 4).TextureInfo.Texture, 4, 4);
			TextureTileMaps.Add("EnemyRedSlime", Support.TiledSpriteFromFile("/Application/assets/slime_red_frames.png", 4, 6).TextureInfo.Texture, 4, 6);
			TextureTileMaps.Add("EnemyZombie", Support.TiledSpriteFromFile("/Application/assets/zombie_frames.png", 4, 2).TextureInfo.Texture, 4, 2);

			for (int i = 0; i < 32; ++i)
			{
				//Console.WriteLine("TestOffscreen: {0}", i);
				//TextureTileMaps.TestOffscreen("Player", Support.TiledSpriteFromFile("/Application/assets/sir_awesome_frames.png", 4, 4).TextureInfo.Texture);
			}

			//TextureTileMaps.Add("Player", Support.TiledSpriteFromFile("/Application/assets/sir_awesome_frames.png", 4, 4).TextureInfo.Texture, 1, 1);
			//TextureTileMaps.Add("EnemySlime", Support.TiledSpriteFromFile("/Application/assets/slime_green_frames.png", 4, 4).TextureInfo.Texture, 1, 1);
			//TextureTileMaps.Add("EnemyRedSlime", Support.TiledSpriteFromFile("/Application/assets/slime_red_frames.png", 4, 6).TextureInfo.Texture, 1, 1);
			//TextureTileMaps.Add("EnemyZombie", Support.TiledSpriteFromFile("/Application/assets/zombie_frames.png", 4, 2).TextureInfo.Texture, 1, 1);
			//TextureTileMaps.Add("EnemyBat", Support.TiledSpriteFromFile("/Application/assets/bat_frames.png", 2, 2).TextureInfo.Texture, 1, 1);
#endif
		}
		
		public void FramePreUpdate()
		{
			if(DebugString!=null) {
				DebugString.Clear();
				DebugString.WriteLine("Score:{0:D8}, Combo:{0:D3}", ScoreManager.GameScore, ScoreManager.HiCombo);
			}
		}
        
		// NOTE: no delta time, frame specific
		public void FrameUpdate()
		{
			Collider.Collide();
			foreach (GameEntity e in RemoveQueue)
				World.RemoveChild(e,true);
			foreach (GameEntity e in AddQueue)
				World.AddChild(e);
				
			RemoveQueue.Clear();
			AddQueue.Clear();
			
#if false
			// is player dead?
			if (PlayerDead)
			{
				if (PlayerInput.AnyButton())
				{
					// ui will transition to title mode
					World.RemoveAllChildren(true);
					Collider.Clear();
					PlayerDead = false;
					
					// hide UI and then null player to swap back to title
					UI.HangDownTarget = -1.0f;
					UI.HangDownSpeed = 0.175f;
					var sequence = new Sequence();
					sequence.Add(new DelayTime() { Duration = 0.4f });
					sequence.Add(new CallFunc(() => this.Player = null));
					World.RunAction(sequence);
				}
			}
#endif
		}
		
		public void DrawWorld()
		{
//			// debug
//			Director.Instance.GL.ModelMatrix.Push();
//			Director.Instance.GL.ModelMatrix.SetIdentity();
//			Director.Instance.DrawHelpers.DrawCircle(TitleCameraCenter, 30.0f, 32);
//			Director.Instance.GL.ModelMatrix.Pop();
		}
		
		public void PlayerDied()
		{
			PlayerDead = true;
		}
    }
}
