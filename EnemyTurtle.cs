using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace kabuto
{
	public class EnemyBase : GameEntity {
	}
	
	public class EnemyTurtle : EnemyBase
    {
        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Sprite { get; set; }
        public Support.AnimationAction FlyAnimation { get; set; }
		
		public Vector2 Velocity = Vector2.Zero;
		public Vector2 MaximumVelocity = Vector2.Zero;
		public Vector2 MinimumVelocity = Vector2.Zero;
		Vector2 Direction = Vector2.One;
		int GroundPingPongCount { get; set; }
		int WallPingPongCount { get; set; }
        
        public EnemyTurtle()
        {
//			Logger.Debug("EnemyTurtle Construct begin");
            Sprite = Support.TiledSpriteFromFile("/Application/assets/koumori.png", 3, 4);
//            Sprite = Support.TiledSpriteFromFile("/Application/assets/bat_frames.png", 2, 2);
			AddChild(Sprite);
            
			const float SingleFrame = 1.0f / 60.0f;
			FlyAnimation = new Support.AnimationAction(Sprite, 9, 11, SingleFrame * 30, looping: true);
//			FlyAnimation = new Support.AnimationAction(Sprite, 0, 4, 0.3f, looping: true);
            
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Enemy,
				owner = this,
				collider = Sprite,
				center = () => GetCollisionCenter(Sprite) + new Vector2( 0, 10 ),
				radius = () => 20.0f,
			});
			
			Sprite.RunAction(FlyAnimation);
			this.Scale *= 2;
			MaximumVelocity = new Vector2( 300, 200 );
			MinimumVelocity = new Vector2( 30, 30 );
			
//			Logger.Debug("EnemyTurtle Construct end");
        }

        public override void Tick(float dt)
        {
            Sprite.Visible = false;
			Game.Instance.SpriteBatch.Register(
				SpriteBatchType.Bat,
				this.Position,
				Sprite.TileIndex2D,
				Sprite.FlipU,
				Vector2.One * 2
				);
			
        	base.Tick(dt);
			
			if (InvincibleTime > 0.0f)
				return;
				
			//add gravity
			Velocity += Vector2.UnitY * -100.0f * dt;
			
			//reduce velocity
//			Velocity *= 0.95f;

			//clamp velocity
			Velocity = Vector2.Clamp( Velocity, MaximumVelocity * -1, MaximumVelocity );
			if( GroundPingPongCount>0 ) {
				if(Velocity.X>=-MinimumVelocity.X && Velocity.X<=MinimumVelocity.X) {
					Velocity.X = MinimumVelocity.X * (Velocity.X<0 ? -1 : 1);
				}
				if(Velocity.Y>=-MinimumVelocity.Y && Velocity.X<=MinimumVelocity.Y) {
					Velocity.Y = MinimumVelocity.Y * (Velocity.Y<0 ? -1 : 1);
				}
			}
			
			Position += Velocity * dt;
        }
		
		public override void TakeDamage(float damage, Vector2? source)
		{
			base.TakeDamage(damage, source);
//			SpawnDamageParticles(GetCollisionCenter(Sprite), (Vector2)source, damage, Support.Color(108, 71, 22));
//			MoveTime = 0.0f;
//			MoveDelay = 2.0f;
//			Sprite.StopAllActions();
//			Sprite.RunAction(FlyAnimation);
//			Support.SoundSystem.Instance.Play("bat_take_damage.wav");
		}
		
		public override void Die(Vector2? source, float damage)
		{
			base.Die(source, damage);
//			Vector2 offset = (GetCollisionCenter(Sprite) - (Vector2)source);
//			if (offset.LengthSquared() > 0.0f)
//				offset = offset.Normalize() * 4.0f;	
//			Game.Instance.ParticleEffects.AddParticlesTile("EnemyBat", Support.GetTileIndex(Sprite), Sprite.FlipU, GetCollisionCenter(Sprite), offset + Vector2.UnitY * 4.0f, damage * 2.0f);
//			Support.SoundSystem.Instance.PlayNoClobber("bat_die.wav");
//			DropCoinsWithAChanceOfHeart(GetCollisionCenter(Sprite), 5);
		}
	}
}