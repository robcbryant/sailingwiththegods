using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Quizzes
{
	class ClashingRocksQuiz : Quiz
	{
		public override string Name => "clashing_rocks";

		protected override string Title => "Clashing Rocks";
		protected override string Image => "stop_9";

		public override void Start(Action onComplete) {
			base.Start(onComplete);

			Question(
				"You’re at the clashing rocks: oh no! do you remember your instructions from Phineus? Choose wisely – or you will be smashed to epic smithereens.",
				new[] {
					new ButtonViewModel { Label = "Pray to Athena", OnClick = Dead },
					new ButtonViewModel { Label = "Apologize to Zeus", OnClick = Dead },
					new ButtonViewModel { Label = "Release a trembling dove", OnClick = Win },
					new ButtonViewModel { Label = "Row as if your lives and fame and ship depended on it", OnClick = Dead },
					new ButtonViewModel { Label = "Ask the ship’s seer to perform a sacrifice to Poseidon", OnClick = Dead }
				}
			);
		}

		private void Dead() {
			Message("Smashed, shattered, drowned – you’re dead.", GameOver);
		}

		private void Win() {
			Message(Globals.Quests.NextSegment.descriptionOfQuest, Complete);
		}
	}
}
