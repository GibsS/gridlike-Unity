using UnityEngine;
using System.Collections;

namespace Gridship {

	public class AcquireBow : Upgrade {

		public override string Name () {
			return "bow";
		}

		public override string Description () {
			return "you've acquired the bow. select it and left click to shoot";
		}

		public override void Execute () {
			character.AcquireBow ();
		}
	}
}