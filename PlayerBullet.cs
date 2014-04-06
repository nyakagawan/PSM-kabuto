using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;

namespace kabuto
{
	public class PlayerBullet : GameEntity
	{
		public Vector2 Velocity = Vector2.Zero;
		
        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile BodySprite { get; set; }
		

		public PlayerBullet (Vector2 spawnPos)
		{
			Logger.Debug("[PlayerBullet] Construct");
            BodySprite = Support.TiledSpriteFromFile("/Application/assets/bat_frames.png", 2, 2);
            this.AddChild(BodySprite);
            
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Player,
				owner = this,
				collider = BodySprite,
				center = () => GetCollisionCenter(BodySprite),
				radius = () => 14.0f,
			});
			
			Position = spawnPos;
			BodySprite.Pivot = BodySprite.TextureInfo.TextureSizef / 2.0f / 2.0f;//2x2 titled spriteだから div 4.0 してる
			Logger.Debug(BodySprite.TextureInfo.TextureSizef.ToString());
			Logger.Debug(BodySprite.Pivot.ToString());
			
			this.Velocity = Vector2.UnitY * 160;
		}
		
		public override void CollideTo(GameEntity owner, Node collider)
		{
			base.CollideTo(owner, collider);
			
			Type type = owner.GetType();
			if (type == typeof(EnemyTurtle))
			{
				Logger.Debug("[PlayerBullet] Collied to Enemy");
			}
		}
		
		float _Rotation = 0;
		public override void Tick(float dt)
		{
			base.Tick(dt);
			
			BodySprite.Rotation = Vector2.Rotation(_Rotation);
			Position += Velocity * dt;
			_Rotation += FMath.DegToRad*500*dt;
		}
	}
}

