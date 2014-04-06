using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;

namespace kabuto
{
	public class EnemyPiece : GameEntity
	{
		public float LifeTime { get; set; }
		public Vector2 TargetPosition { get; set; }
        public float Radius { get { return 15.0f; } }
		
        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Sprite { get; set; }
		
		MoveTo MoveToAction { get; set; }
		
		public EnemyPiece ( Vector2 pos, Vector2 targetPos, float lifeTime )
		{
			Position = pos;
			TargetPosition = targetPos;
			LifeTime = lifeTime;
			
            Sprite = Support.TiledSpriteFromFile("/Application/assets/bomb.png", 6, 10);
            this.AddChild(Sprite);
            
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.EnemyPiece,
				owner = this,
				collider = Sprite,
				center = () => GetCollisionCenter(Sprite),
				radius = () => Radius,
			});
			
//			const float SingleFrame = 1.0f / 60.0f;
			AddAnimation("Idle", new Support.AnimationAction(Sprite, 0, 6, LifeTime, looping: false));
			SetAnimation(Sprite, "Idle");
			
			Logger.Debug("[EnemyPiece] pos:{0}, target:{1}", Position.ToString(), TargetPosition.ToString());
		
			var mv = new MoveTo(TargetPosition, LifeTime);
			MoveToAction = mv;
			this.RunAction(mv);
		}
		
		public override void Tick (float dt)
		{
            Sprite.Visible = false;
			base.Tick (dt);
			
			if( MoveToAction.IsRunning==false ) {
				Destroy();
			}
			
			Game.Instance.SpriteBatch.Register(
				SpriteBatchType.Bomb,
				this.Position,
				Sprite.TileIndex2D,
				Sprite.FlipU
				);
		}

		public override void CollideTo(GameEntity owner, Node collider)
		{
			base.CollideTo(owner, collider);
			
			Type type = owner.GetType();
			if (type == typeof(EnemyBase))
			{
//				Logger.Debug("[PlayerBullet] Collied to Enemy");
				Destroy();
			}
		}
		
		void Destroy() {
			MoveToAction.Stop();
			Game.Instance.RemoveQueue.Add(this);
			CollisionDatas.RemoveAll( (x) => x.owner==this );
		}
	}
}
