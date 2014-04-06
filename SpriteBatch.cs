
using System.Collections.Generic;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace kabuto
{
	public enum SpriteBatchType : int
	{
		Bat = 0,
		Bomb,
		Max
	};

	public class SpriteBatch : Node
	{
		public struct SpriteItem
		{
			public Vector2 position;
			public Vector2i tile;
			public Vector2 scale;
			public bool flip_u;
		};
		
		class TiledSpriteLoadParam {
			public string filename;
			public int x;
			public int y;
			public TiledSpriteLoadParam(string fn, int _x, int _y) {
				filename = fn;
				x = _x;
				y = _y;
			}
		};
		List<TiledSpriteLoadParam> _TiledSpriteLoadParams = new List<TiledSpriteLoadParam>();

		public List<SpriteTile> Sprites = new List<SpriteTile>();
		public List<SpriteItem>[] SpriteItemLists = new List<SpriteItem>[(int)SpriteBatchType.Max];

		public SpriteBatch()
		{
			//バッチスプライトが増える場合はこいつに追加
//			_TiledSpriteLoadParams.Add( new TiledSpriteLoadParam("/Application/assets/bat_frames.png", 2, 2) );
			_TiledSpriteLoadParams.Add( new TiledSpriteLoadParam("/Application/assets/koumori.png", 3, 4) );
			_TiledSpriteLoadParams.Add( new TiledSpriteLoadParam("/Application/assets/bomb.png", 6, 10) );
			
			PrecacheSprites();

			Sprites.Clear();
			for(int i=0; i<_TiledSpriteLoadParams.Count; i++) {
				var ps = _TiledSpriteLoadParams[i];
				Sprites.Add( Support.TiledSpriteFromFile( ps.filename, ps.x, ps.y ) );
			}

			AdHocDraw += this.DrawBatch;
		}

		public void PrecacheSprites()
		{
			for(int i=0; i<_TiledSpriteLoadParams.Count; i++) {
				var ps = _TiledSpriteLoadParams[i];
				Support.PrecacheTiledSprite( ps.filename, ps.x, ps.y );
			}
		}

		public void Register(SpriteBatchType type, Vector2 position, Vector2i tile, bool flip_u) {
			Register( type, position, tile, flip_u, Vector2.One);
		}
		public void Register(SpriteBatchType type, Vector2 position, Vector2i tile, bool flip_u, Vector2 scale)
		{
			if(SpriteItemLists[(int)type]==null) {
				SpriteItemLists[(int)type] = new List<SpriteItem>();
			}
			var list = SpriteItemLists[(int)type];
			var item = new SpriteItem();
			item.position = position;
			item.tile = tile;
			item.flip_u = flip_u;
			item.scale = scale;
			list.Add(item);
		}

		public void DrawBatch()
		{
			for(int i=0; i<SpriteItemLists.Length; i++) {
				if(SpriteItemLists[i]!=null) {
					DrawList(SpriteItemLists[i], Sprites[i]);
				}
			}
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
				copy.S *= item.scale;
				Director.Instance.SpriteRenderer.AddSprite( ref copy, sprite.TileIndex2D );
			}

			Director.Instance.SpriteRenderer.EndSprites(); 

			items.Clear();
		}
	};
}
