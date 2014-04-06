using System.Collections.Generic;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace kabuto
{
	public class GameEntity : Sce.PlayStation.HighLevel.GameEngine2D.Node
	{
		public float InvincibleTime { get; set; }
		public int FrameCount { get; set; }
		
        public string CurrentAnimation { get; set; }
        public Dictionary<string, Support.AnimationAction> AnimationTable { get; set; }
		
		public List<EntityCollider.CollisionEntry> CollisionDatas;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GameEntity()
		{
			Sce.PlayStation.HighLevel.GameEngine2D.Scheduler.Instance.Schedule(this, Tick, 0.0f, false);
			AdHocDraw += this.DebugDraw;
			CollisionDatas = new List<EntityCollider.CollisionEntry>();
			AnimationTable = new Dictionary<string, Support.AnimationAction>();
		}
		
		protected void SetAnimation(SpriteTile sprite, string animation)
		{
			Common.Assert( AnimationTable.ContainsKey(animation), "animation["+animation+"] does not exists." );
			if (CurrentAnimation != null)
				sprite.StopAction(AnimationTable[CurrentAnimation]);
				
			CurrentAnimation = animation;
			sprite.RunAction(AnimationTable[animation]);
			AnimationTable[animation].Reset();
		}
		
		protected void AddAnimation(string name, Support.AnimationAction animAct ) {
			AnimationTable.Add( name, animAct );
		}
		
		public static Vector2 GetCollisionCenter(Node node)
		{
			Bounds2 bounds = new Bounds2();
			node.GetlContentLocalBounds(ref bounds);
			Vector2 center = node.LocalToWorld(bounds.Center);
			return center;
		}
		
		public virtual void DebugDraw()
		{
			foreach (EntityCollider.CollisionEntry c in CollisionDatas)
			{
				if (c.owner != null)
				{
					Director.Instance.GL.ModelMatrix.Push();
					Director.Instance.GL.ModelMatrix.SetIdentity();
					Director.Instance.DrawHelpers.DrawCircle(c.center(), c.radius(), 32);
					Director.Instance.GL.ModelMatrix.Pop();
				}
			}
		}
		
		public virtual void Tick(float dt)
		{
			FrameCount += 1;
			
			InvincibleTime -= dt;
			InvincibleTime = System.Math.Max(0.0f, InvincibleTime);
			
			if (InvincibleTime <= 0.0f)
			{
				foreach(var c in CollisionDatas)
				{
					if (c.owner != null)
						Game.Instance.Collider.Add(c);
				}
			}
		}
		
		public virtual void CollideTo(GameEntity owner, Node collider) {
//			Logger.Debug("CollideTo");
		}
		public virtual void CollideFrom(GameEntity owner, Node collider) {
//			Logger.Debug("CollideFrom");
		}
		
		public void SpawnDamageParticles(Vector2 position, Vector2 source, float damage, Vector4 color)
		{				
			Vector2 dir = position - source;
			if (dir.LengthSquared() > 0.0f)
				dir = dir.Normalize();
			dir *= 0.25f;
			int particles = (int)(damage * 4.0f);
			float jitter = 1.5f * damage;
//			Game.Instance.ParticleEffects.AddParticlesBurst(particles, position, dir * damage * 4.0f + Vector2.UnitY * 2.0f, color, jitter, 1.0f);
		}
		
		public virtual void TakeDamage(float damage, Vector2? source)
		{
			Die(source, damage);
		}
		
		public virtual void Die(Vector2? source, float damage)
		{
			Game.Instance.World.RemoveChild(this, true);
		}
	};
	
	/// <summary>
	/// PlayerBulletやEenmeyPieceといった、Enemyを殺すオブジェクトの基底クラス
	/// </summary>
	public class EnemyKiller : GameEntity
	{
		public int Generation { get; set; }
	}
}
