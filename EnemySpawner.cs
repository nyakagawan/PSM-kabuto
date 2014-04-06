using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace kabuto
{
	public class EnemySpawner : GameEntity
    {
		class SpawnLevelSetting {
			public int SpawnCountForNextLevel = 0;
			public float SpawnPerSecond = 0;
			public int DisplayLimitCount = 0;
			
			public float SpawnSecAve {
				get { return SpawnPerSecond!=0 ? 1.0f / SpawnPerSecond : 1.0f; }
			}
			
			public SpawnLevelSetting(int fornext, float sps, int dispLim) {
				SpawnCountForNextLevel = fornext;
				SpawnPerSecond = sps;
				DisplayLimitCount = dispLim;
			}
		}
		
		public int MaximumLevel { get { return 10; } }
		
    	public float SpawnCounter;
		public float NextSpawnTime;
        public int Type;
        public int Total;
		public int CurrentLevel { get; set; }
		public int EnemyDeadCountTotal { get; set; }
		public int EnemyDeadCountCurLevel { get; set; }
		
		List<SpawnLevelSetting> _SpawnLevelSetting = new List<SpawnLevelSetting>();
		List<Vector2> _BasePositionList = new List<Vector2>();
		
		/// <summary>
		/// Initializes a new instance of the <see cref="kabuto.EnemySpawner"/> class.
		/// </summary>
		public EnemySpawner() {
			_SpawnLevelSetting = new List<SpawnLevelSetting> {
				new SpawnLevelSetting(0, 2.0f, 30),
				new SpawnLevelSetting(20, 2.5f, 20),
				new SpawnLevelSetting(20, 3.0f, 30),
				new SpawnLevelSetting( 0, 4.0f, 40),
			};
			
			var screenSize = Game.Instance.ScreenSize;
			_BasePositionList = new List<Vector2>() {
//				new Vector2() { X=200,				Y=200 },
//				new Vector2() { X=0,				Y=screenSize.Y+50 },
				new Vector2() { X=screenSize.X/2,	Y=screenSize.Y+50 },
//				new Vector2() { X=screenSize.X,		Y=screenSize.Y+50 },
			};

			CurrentLevel = 1;
			NextSpawnTime = MakeNextSpawnTime(CurrentLevel);
		}

		public void AddDeadEnemy() {
			EnemyDeadCountTotal ++ ;
			EnemyDeadCountCurLevel ++ ;

			var curSpawnSetting = _SpawnLevelSetting[CurrentLevel-1];
			if( curSpawnSetting.SpawnCountForNextLevel>0
			&&	EnemyDeadCountCurLevel>curSpawnSetting.SpawnCountForNextLevel
			&&	(CurrentLevel+1)<_SpawnLevelSetting.Count
			){
				//spawn level up
				CurrentLevel ++ ;
				EnemyDeadCountCurLevel = 0;
			}
		}
		
		Vector2 ChooseSpawnBasePos() {
			return _BasePositionList[ Game.Instance.Random.Next(_BasePositionList.Count) ];
		}
		
		float MakeNextSpawnTime(int curlevel) {
			var curSpawnSetting = _SpawnLevelSetting[curlevel-1];
			var time = curSpawnSetting.SpawnSecAve;
			time = time + ( time * 0.2f * Game.Instance.Random.NextSignedFloat() );
			return time;
		}

        public override void Tick(float dt)
        {
        	base.Tick(dt);

			if (Game.Instance.PlayerDead)
				return;
			
			if( SpawnCounter<NextSpawnTime ) {
	            SpawnCounter += dt;
			}
			
			var displayEnemyCount = Total - EnemyDeadCountTotal;
			var curSpawnSetting = _SpawnLevelSetting[CurrentLevel-1];
            if(	NextSpawnTime>0
			&&	SpawnCounter>=NextSpawnTime
			&&	displayEnemyCount < curSpawnSetting.DisplayLimitCount
			){
				SpawnCounter = 0;
				
                SpawnEnemy();
				
				//ちょっと生成時間にゆらぎを持たせる
				NextSpawnTime = MakeNextSpawnTime(CurrentLevel);
            }
        }

        void SpawnEnemy()
        {
			Vector2 pos = ChooseSpawnBasePos();
			Vector2 initVelocity = new Vector2( 200 * Game.Instance.Random.NextSignedFloat(), 0 );
//			Logger.Debug("[SpawnEnemy] pos:{0}, vel:{1}", pos.ToString(), initVelocity.ToString());
			
			EnemyBase enemy = null;
        	switch (Type)
			{
			case 0:
				enemy = new EnemyTurtle() {
					Position = pos,
					Velocity = initVelocity,
				};
				break;
			default:
				Common.Assert(false, "default break");
				break;
			}
			
			Game.Instance.AddQueue.Add(enemy);
			Total ++ ;
        }
		
		public override void DebugDraw() {
			base.DebugDraw();
			
			Game.Instance.DebugString.WriteLine(
				"EnemySpawn total:{0}, lv:{1}, cnt:{2}, next:{3}",
				EnemyDeadCountTotal, EnemyDeadCountCurLevel, SpawnCounter, NextSpawnTime
				);
		}
    }
}
