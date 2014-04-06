
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
		
		public Vector2 Velocity = Vector2.Zero;
        public float AttackTime { get; set; }
		public float Health { get; set; }
		public int FootstepDelay { get; set; }

        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile BodySprite { get; set; }
        public string CurrentAnimation { get; set; }
        public Dictionary<string, Support.AnimationAction> AnimationTable { get; set; }
		
        public Player()
        {
            BodySprite = Support.TiledSpriteFromFile("/Application/assets/bat_frames.png", 2, 2);
            this.AddChild(BodySprite);
            
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Player,
				owner = this,
				collider = BodySprite,
				center = () => GetCollisionCenter(BodySprite),
				radius = () => 14.0f,
			});
			
			const float SingleFrame = 1.0f / 60.0f;
			AnimationTable = new Dictionary<string, Support.AnimationAction>() {
				{ "Idle", new Support.AnimationAction(BodySprite, 0, 4, SingleFrame * 30, looping: true) },
				{ "Walk", new Support.AnimationAction(BodySprite, 0, 4, SingleFrame * 60, looping: true) },
			};
			
			Position = new Vector2(200.0f, 200.0f);
			AttackTime = -1.0f;
			Health = 5.0f;
			
			SetAnimation("Idle");
        }
		
		public void TickTransform(float dt)
		{
            Position += Velocity;
            
            Position = new Vector2(
				FMath.Clamp(Position.X, -260.0f, 1030.0f),
				FMath.Max(Position.Y, Game.Instance.FloorHeight)
			);
            Position = new Vector2(Position.X, FMath.Max(Position.Y, Game.Instance.FloorHeight));
            
			// cleanup infinitesmal float values
			if (System.Math.Abs(Velocity.X) < 0.0001f)
				Velocity = new Vector2(0.0f, Velocity.Y);
		}
		
       	public override void Tick(float dt)
        {
			TickTransform(dt);
			
        	base.Tick(dt);
            
            if (InvincibleTime <= 0.0f)
			{
	            // ground control (to major tom)
            	float axis = PlayerInput.LeftRightAxis();
            	Velocity += Vector2.UnitX * axis;

				if (System.Math.Abs(Velocity.X) > IdleAnimationSpeedThreshold)
				{
					if (CurrentAnimation == "Idle")
						SetAnimation("Walk");

					if (FootstepDelay <= 0)
					{
						Support.SoundSystem.Instance.PlayNoClobber("player_walk.wav");
						FootstepDelay = 10;
					}
					FootstepDelay -= 1;
				}
				else
				{
					if (CurrentAnimation == "Walk")
						SetAnimation("Idle");
				}
				
				if(axis==0) {
					Velocity = Velocity * (0.2f*dt);
				}
            }

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
		
		public void SetAnimation(string animation)
		{
			if (CurrentAnimation != null)
				BodySprite.StopAction(AnimationTable[CurrentAnimation]);
				
			CurrentAnimation = animation;
			BodySprite.RunAction(AnimationTable[animation]);
			AnimationTable[animation].Reset();
			
			//Console.WriteLine("SetAnimation(): {0}", animation);
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
			
			SetAnimation("Attack");
			AttackTime = 0.125f;

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