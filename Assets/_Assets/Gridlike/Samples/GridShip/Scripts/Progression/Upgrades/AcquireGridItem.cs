using UnityEngine;
using System.Collections;

namespace Gridship {

	public class AcquireGridItem : Upgrade {

		int itemID;

		public AcquireGridItem(int itemId) {
			this.itemID = itemId;
		}

		public override string Name () {
			return "acquire item " + itemID;
		}

		public override string Description () {
			return "you've acquired the item " + itemID;
		}

		public override void Execute () {
			if (!character.availableGridItems.Contains (itemID)) {
				character.availableGridItems.Add (itemID);
			}
		}
	}
}