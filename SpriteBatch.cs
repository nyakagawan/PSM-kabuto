
using System.Collections.Generic;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace kabuto
{
	public enum SpriteBatchType
	{
		Bat,
	};

	public class SpriteBatch : Node
	{
		public struct SpriteItem
		{
			public Vector2 position;
			public Vector2i tile;
			public bool flip_u;
		};

		public List<SpriteTile> Sprites = new List<SpriteTile>();
		public List<SpriteItem> Bats = new List<SpriteItem>();

		public SpriteBatch()
		{
			PrecacheSprites();

			Sprites.Clear();
			Sprites.Add(Support.TiledSpriteFromFile("/Application/assets/bat_frames.png", 2, 2));

			AdHocDraw += this.DrawBatch;
		}

		public void PrecacheSprites()
		{
			Support.PrecacheTiledSprite("/Application/assets/bat_frames.png", 2, 2);
		}

		public void Register(SpriteBatchType type, Vector2 position, Vector2i tile, bool flip_u)
		{
			List<SpriteItem> list = null;

			switch (type)
			{
				case SpriteBatchType.Bat: list = Bats; break;
			}

			list.Add(new SpriteItem() { position = position, tile = tile, flip_u = flip_u });
		}

		public void DrawBatch()
		{
			DrawList(Bats, Sprites[(int)SpriteBatchType.Bat]);
			//DrawList(Gauges, Sprites[(int)SpriteBatchType.Gauge]);
			//DrawList(Panels, Sprites[(int)SpriteBatchType.Panel]);
		}

		public void DrawList(List<SpriteItem> items, SpriteTile sprite)
		{
			Director.Instance.GL.SetBlendMode( sprite.BlendMode );
			sprite.Shader.SetColor( ref sprite.Color );
            sprite.Shader.SetUVTransform( ref Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.UV_TransformFlipV );

			Director.Instance.SpriteRenderer.BeginSprites(sprite.TextureInfo, sprite.Shader, items.Count);

			for (int i = 0; i < items.Count; ++i)
			{
				SpriteItem item = items[i];
				sprite.Quad.T = item.position;
				sprite.TileIndex2D = item.tile;
				Director.Instance.SpriteRenderer.FlipU = item.flip_u;
				Director.Instance.SpriteRenderer.FlipV = sprite.FlipV;
				TRS copy = sprite.Quad;
				Director.Instance.SpriteRenderer.AddSprite( ref copy, sprite.TileIndex2D );
			}

			Director.Instance.SpriteRenderer.EndSprites(); 

			items.Clear();
		}
	};
}
