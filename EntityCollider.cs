
using System.Collections.Generic;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;

namespace kabuto
{
	public class EntityCollider
	{
		public enum CollisionEntityType
		{
			Player,
			Enemy,
			EnemyPiece,
			Bullet,
			Item,
			Max
		}
		
		public delegate Vector2 GetCenterDelegate();
		public delegate float GetRadiusDelegate();
		// GetForceVector()?
		
		public struct CollisionEntry
		{
			public CollisionEntityType type;
			public GameEntity owner;
			public Node collider;
			public GetCenterDelegate center;
			public GetRadiusDelegate radius;
		}
		
		List<List<CollisionEntry>> typed_entries;
		
		public EntityCollider()
		{
			typed_entries = new List<List<CollisionEntry>>();
			for(int i=0; i<(int)CollisionEntityType.Max; i++) {
				typed_entries.Add(new List<CollisionEntry>());
			}
		}
		
		public void Add(CollisionEntityType type, GameEntity owner, Node collider, GetCenterDelegate center, GetRadiusDelegate radius)
		{	
			CollisionEntry entry = new CollisionEntry() { type = type, owner = owner, collider = collider, center = center, radius = radius };
			Add (entry);
		}
		
		public void Add(CollisionEntry entry)
		{
			List<CollisionEntry> entries = typed_entries[(int)entry.type];
			entries.Add(entry);
		}
		
		public void Collide()
		{
			//Bullet to ...
			Collide(
				CollisionEntityType.Bullet,
				new CollisionEntityType[] {
					CollisionEntityType.Enemy
				}
			);
			//EnemyPiece to ...
			Collide(
				CollisionEntityType.EnemyPiece,
				new CollisionEntityType[] {
					CollisionEntityType.Enemy
				}
			);
			
			Clear();
		}
		
		void Collide( CollisionEntityType toType, CollisionEntityType[] fromTypes ) {
			foreach(var a in typed_entries[(int)toType]) {
				var a_center = a.center();
				var a_rad = a.radius();
				
				for(int i=0; i<fromTypes.Length; i++) {
					foreach(var b in typed_entries[(int)fromTypes[i]]) {
						if( a.owner==b.owner ) {
							continue;
						}
						float r = a_rad + b.radius();
						
						Vector2 offset = b.center() - a_center;
						float lensqr = offset.LengthSquared();
						
						if (lensqr < r * r)
						{
							a.owner.CollideTo(b.owner, b.collider);
							b.owner.CollideFrom(a.owner, a.collider);
						}
					}
				}
			}
		}
		
		public void Clear()
		{
			foreach (var entries in typed_entries)
				entries.Clear();
		}
	}
}
