using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;

namespace kabuto
{
	public class PlayerBullet : GameEntity
	{
		public Vector2 Velocity = Vector2.Zero;
		
        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile BodySprite { get; set; }
        public float Radius { get { return 25.0f; } }

		public PlayerBullet (Vector2 spawnPos)
		{
			Logger.Debug("[PlayerBullet] Construct");
            BodySprite = Support.TiledSpriteFromFile("/Application/assets/balloon.png", 6, 12);
            this.AddChild(BodySprite);
            
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Bullet,
				owner = this,
				collider = BodySprite,
				center = () => GetCollisionCenter(BodySprite),
				radius = () => Radius,
			});
			
			const float SingleFrame = 1.0f / 60.0f;
			AddAnimation("Idle", new Support.AnimationAction(BodySprite, 0, 3, SingleFrame * 30, looping: true));
			AddAnimation("Explosion", new Support.AnimationAction(BodySprite, 4, 6, SingleFrame * 20, finishCallback:ExplosionAnimFinishCallback));
			SetAnimation(BodySprite, "Idle");
			
			Position = spawnPos;
			Scale *= 2.0f;
			
			this.Velocity = Vector2.UnitY * 320;
		}
		
		public override void CollideTo(GameEntity owner, Node collider)
		{
			base.CollideTo(owner, collider);
			
			Type type = owner.GetType();
			if (type == typeof(EnemyTurtle))
			{
				Logger.Debug("[PlayerBullet] Collied to Enemy");
				CollisionDatas.RemoveAll( (x) => x.owner==this );
				Velocity *= 0.3f;
				SetAnimation(BodySprite, "Explosion");
			}
		}
		
		void ExplosionAnimFinishCallback() {
			Game.Instance.RemoveQueue.Add(this);
		}
		
		public override void Tick(float dt)
		{
			base.Tick(dt);
			
			Position += Velocity * dt;

			if( Position.Y > Game.Instance.ScreenSize.Y ) {
				Logger.Debug("[PlayerBullet] Out of bounds");
				Game.Instance.RemoveQueue.Add( this );
			}
		}
	}
}

