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
		
        public float FloorHeight = 80.0f;
        public float WorldScale = 1.0f;
        
		public Vector2 TitleCameraCenter { get; set; }
		public Vector2 CameraTarget { get; set; }
		
		public int WaveCount { get; set; }
		public bool PlayerDead { get; set; }
		public Vector2 ScreenSize { get { return new Vector2(960.0f, 544.0f); } }
		
		public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile LightShafts { get; set; }
		public Sce.PlayStation.HighLevel.GameEngine2D.ActionBase EnemySpawnerLoop { get; set; }
		
		public SpriteBatch SpriteBatch;

        public Game()
        {
//			Director.Instance.DebugFlags |= DebugFlags.Navigate; // press left alt + mouse to navigate in 2d space
			Director.Instance.DebugFlags |= DebugFlags.DrawGrid;
			Director.Instance.DebugFlags |= DebugFlags.DrawContentWorldBounds;
			Director.Instance.DebugFlags |= DebugFlags.DrawContentLocalBounds;
			Director.Instance.DebugFlags |= DebugFlags.DrawTransform;
			Director.Instance.DebugFlags |= DebugFlags.DrawPivot;

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
            var bg_forest = Support.SpriteFromFile("/Application/assets/background_back.png");
            var fg_log = Support.SpriteFromFile("/Application/assets/background_front.png");

            bg_forest.Position = new Vector2(-160.0f, 0.0f);
			fg_log.Position = new Vector2(-160.0f, 0.0f);
			
			bg_forest.Pivot = new Vector2(bg_forest.TextureInfo.TextureSizef.X * 0.5f, 0.0f);
            fg_log.Pivot = new Vector2(fg_log.TextureInfo.TextureSizef.X * 0.5f, 0.0f);
			
            Background.AddChild(bg_forest);
            Foreground.AddChild(fg_log);
			
			UI.TitleMode();
			
			Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Schedule(Scene, TickTitle, 0.0f, false);
			
			World.AdHocDraw += this.DrawWorld;
        }
        
		public void TickTitle(float dt)
		{
			((Camera2D)Scene.Camera).Center = TitleCameraCenter;
			
			//LightShafts.Color.A = FMath.Sin(Game.Instance.UI.FrameCount * 0.01f) * 0.15f;
			
			if (PlayerInput.AnyButton())
			{
	            Player = new Player();
	            World.AddChild(Player);
				World.AddChild(SpriteBatch);
	            
	            // proper enemies
	            /*
	            if (false)
				{
		            World.AddChild(new EnemySpawner() { Position = new Vector2(-10.0f, FloorHeight), SpawnRate = 3.0f, SpawnCounter = 3.0f, Type = 0, Total = -1, });
		            World.AddChild(new EnemySpawner() { Position = new Vector2(970.0f, FloorHeight), SpawnRate = 4.0f, SpawnCounter = 4.0f, Type = 0, Total = -1, });
		            World.AddChild(new EnemySpawner() { Position = new Vector2(120.0f, 460.0f), SpawnRate = 8.0f, SpawnCounter = 5.0f, Type = 1, Total = -1, });
		            World.AddChild(new EnemySpawner() { Position = new Vector2(310.0f, 460.0f), SpawnRate = 20.0f, SpawnCounter = 20.0f, Type = 2, Total = -1, });
		            World.AddChild(new EnemySpawner() { Position = new Vector2(960.0f, 460.0f), SpawnRate = 40.0f, SpawnCounter = 22.0f, Type = 2, Total = -1, });
		            World.AddChild(new EnemySpawner() { Position = new Vector2(-10.0f, 460.0f), SpawnRate = 40.0f, SpawnCounter = 14.0f, Type = 2, Total = -1, });
				}
				*/
	            
				// test enemies
				/*
				if (false)
				{
		            World.AddChild(new EnemySpawner() { Position = new Vector2(200.0f, FloorHeight), SpawnRate = 6.0f, SpawnCounter = 6.0f, Type = 3, Total = -1 });
		            //World.AddChild(new EnemySpawner() { Position = new Vector2(200.0f, FloorHeight), SpawnRate = 3.0f, SpawnCounter = 6.0f, Type = 3, Total = -1 });
				}
				*/
				
	            // test high enemy count
				//for (int i = 0; i < 300; ++i)
					//World.AddChild(new EnemySlime() { Position = new Vector2(200.0f + i, 150.0f + (i % 30)) });
					
				Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Unschedule(Scene, this.TickTitle);
				Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Schedule(Scene, this.TickGame, 0.0f, false);

//				Support.SoundSystem.Instance.Play("game_press_start.wav");

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
#if false
			if (EnemySpawnerLoop != null)
				return;

			Game.Instance.WaveCount = 0;

			Sequence waves = new Sequence();
			waves.Add(new CallFunc(() => World.AddChild(new EnemyWave(0.0f))));
			waves.Add(new DelayTime() { Duration = 45.0f });
			waves.Add(new CallFunc(() => { Game.Instance.WaveCount += 1; }));
			
			EnemySpawnerLoop = new RepeatForever() { InnerAction = waves };
			World.RunAction(EnemySpawnerLoop);
#else
			//こうもりを出したい
			var type = 0;
			var spawn_rate = 0;
			var total = 1;
			var spawner = new EnemySpawner() {
				SpawnCounter = 0,
				SpawnRate = spawn_rate,
				Type = type,
				Total = total,
//				Position = new Vector2(-300.0f + Game.Instance.Random.NextFloat() * 1500.0f, 600.0f),
//				Position = new Vector2(200, 250),
				Position = new Vector2(50,50),
			};
			Logger.Debug("swapned");
			
			World.AddChild(spawner);
			Logger.Debug("added child");
#endif
		}

		public void StopEnemySpawning()
		{
#if false
			if (EnemySpawnerLoop == null)
				return;

			World.StopAction(EnemySpawnerLoop);
			EnemySpawnerLoop = null;
#endif
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
