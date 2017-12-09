using System;
using UnityEngine;

namespace Gridlike {

	/// <summary>
	/// Stores references to both the sprite the user wants to render on his grids and to their generated counterparts that
	/// are actually rendered. After generation, the sprites and importeSprites are identical (same image).
	/// </summary>
	[Serializable]
	public class TileSpriteInfo {

		/// <summary>
		/// The generated main sprite (sprite on the generated sprite sheet).
		/// </summary>
		public Sprite sprite;
		/// <summary>
		/// The generated subId sprites (sprite on the generated sprite sheet).
		/// </summary>
		public Sprite[] sprites;

		/// <summary>
		/// The user specified main sprite.
		/// </summary>
		public Sprite importedSprite;
		/// <summary>
		/// The user specified subId sprites.
		/// </summary>
		public Sprite[] importedSprites;
	}
}