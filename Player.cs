
using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace kabuto
{
	public class Player : GameEntity
    {
        public const float IdleAnimationSpeedThreshold = 1.25f;
		public const float MaximumWalkVelocity = 10.0f;
		
		public Vector2 Velocity = Vector2.Zero;
        public float AttackTime { get; set; }
		public float Health { get; set; }
		public int FootstepDelay { get; set; }
		public float Redius { get { return 32.0f; } }

        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile BodySprite { get; set; }
		
        public Player()
        {
            BodySprite = Support.TiledSpriteFromFile("/Application/assets/majo.png", 3, 4);
            this.AddChild(BodySprite);
            
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Player,
				owner = this,
				collider = BodySprite,
				center = () => GetCollisionCenter(BodySprite),
				radius = () => Redius,
			});
			
			const float SingleFrame = 1.0f / 60.0f;
			AddAnimation("Idle",	new Support.AnimationAction(BodySprite, 9, 11, SingleFrame * 30, looping: true));
			AddAnimation("Walk",	new Support.AnimationAction(BodySprite, 3, 5, SingleFrame * 60, looping: true));
			SetAnimation(BodySprite, "Idle");
			
			Position = new Vector2( Game.Instance.ScreenSize.X * 0.5f, Game.Instance.FloorHeight);
			Scale *= 2;
			AttackTime = -1.0f;
			Health = 5.0f;
        }
		
       	public override void Tick(float dt)
        {
        	base.Tick(dt);
            
            if (InvincibleTime <= 0.0f)
			{
	            // ground control (to major tom)
            	float axis = PlayerInput.LeftRightAxis();
            	Velocity += Vector2.UnitX * axis;

				if (System.Math.Abs(Velocity.X) > IdleAnimationSpeedThreshold)
				{
					if (CurrentAnimation == "Idle") {
						SetAnimation(BodySprite, "Walk");
						if( Velocity.X<0 ) {
							BodySprite.FlipU = true;
						}
						else {
							BodySprite.FlipU = false;
						}
					}

					if (FootstepDelay <= 0)
					{
						Support.SoundSystem.Instance.PlayNoClobber("player_walk.wav");
						FootstepDelay = 10;
					}
					FootstepDelay -= 1;
				}
				else
				{
					if (CurrentAnimation.StartsWith("Walk")) {
						SetAnimation(BodySprite, "Idle");
					}
				}
				
				if(axis==0) {
					Velocity = Velocity * (0.2f*dt);
				}
            }
			
			Velocity.X = FMath.Clamp(Velocity.X, -MaximumWalkVelocity, MaximumWalkVelocity);

			// Transform
            Position += Velocity;
            
            Position = new Vector2(
				FMath.Clamp(Position.X, 0, Game.Instance.ScreenSize.X - Redius*2),
				FMath.Clamp(Position.Y, 0, Game.Instance.ScreenSize.Y)
			);
            
			// cleanup infinitesmal float values
			if (System.Math.Abs(Velocity.X) < 0.001f)
				Velocity = new Vector2(0.0f, Velocity.Y);
			
            // Attacks
            if (AttackTime < 0.0f)
			{
				if (PlayerInput.AttackButton())
				{
					StartAttack();
				}
			}
			
			if (AttackTime > 0.0f)
			{
				AttackTime -= dt;
			}
            
			// walk animation speed based on velocity
			AnimationTable["Walk"].SetSpeed(System.Math.Abs(Velocity.X));
		}
		
		public override void DebugDraw() {
			base.DebugDraw();
			Game.Instance.DebugString.WriteLine("PL Vel: "+Velocity.ToString());
		}
		
		public void StartAttack()
		{
			Logger.Debug("StartAttack");
			
			//Game.Instance.ParticleEffects.AddParticle(GetCollisionCenter(BodySprite), Colors.White);
			//Game.Instance.ParticleEffects.AddParticlesBurst(1, GetCollisionCenter(BodySprite), Vector2.UnitY, Colors.Blue, 4.0f);
			//Game.Instance.ParticleEffects.AddParticlesTile("EnemyZombie", 0, BodySprite.FlipU, GetCollisionCenter(BodySprite), Vector2.UnitY, 1.0f);
			//Game.Instance.ParticleEffects.AddParticlesTile("EnemySlime", 0, BodySprite.FlipU, GetCollisionCenter(BodySprite), Vector2.UnitY, 1.0f);
			//Game.Instance.ParticleEffects.AddParticlesTile("EnemySlime", 0, BodySprite.FlipU, GetCollisionCenter(BodySprite) + Vector2.UnitX * 150.0f, Vector2.UnitY, 1.0f);
			//Game.Instance.ParticleEffects.AddParticlesCone(16, GetCollisionCenter(BodySprite), Vector2.UnitY, Colors.White, 1.0f);
			//DropCoinsWithAChanceOfHeart(GetCollisionCenter(BodySprite) + Vector2.UnitY * 160.0f, 4);
			
			AttackTime = 0.125f;
			
			var bullet = new PlayerBullet(Position);
			Game.Instance.World.AddChild(bullet);

			Support.SoundSystem.Instance.Play("player_sword_attack.wav");
		}
		
		public override void CollideFrom(GameEntity owner, Node collider)
		{
			base.CollideTo(owner, collider);
			
			Type type = owner.GetType();
			if (type == typeof(EnemyTurtle))
			{
				//collied enemy
			}
		}
		
	};
}