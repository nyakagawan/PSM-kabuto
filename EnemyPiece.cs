using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;

namespace kabuto
{
	public class EnemyPiece : GameEntity
	{
		public float LifeTime { get; set; }
		public Vector2 TargetPosition { get; set; }
		
        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Sprite { get; set; }
		
		MoveTo MoveToAction { get; set; }
		
		public EnemyPiece ( Vector2 pos, Vector2 targetPos, float lifeTime )
		{
			Position = pos;
			TargetPosition = targetPos;
			LifeTime = lifeTime;
			
            Sprite = Support.TiledSpriteFromFile("/Application/assets/bomb.png", 6, 10);
            this.AddChild(Sprite);
			
			const float SingleFrame = 1.0f / 60.0f;
			AddAnimation("Idle", new Support.AnimationAction(Sprite, 0, 6, SingleFrame * 30, looping: false));
			SetAnimation(Sprite, "Idle");
			
			Logger.Debug("[EnemyPiece] pos:{0}, target:{1}", Position.ToString(), TargetPosition.ToString());
		
			var mv = new MoveTo(TargetPosition, LifeTime);
			MoveToAction = mv;
			this.RunAction(mv);
		}
		
		public override void Tick (float dt)
		{
			base.Tick (dt);
			
			if( MoveToAction.IsRunning==false ) {
				Game.Instance.RemoveQueue.Add(this);
			}
		}
	}
}
