using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Quizzes
{
	class GoldenFleeceQuiz : Quiz
	{
		public override string Name => "golden_fleece";

		protected override string Title => "The Golden Fleece";

		const string FirstImage = "stop_16";
		const string SecondImage = "stop_17";

		private string _image = FirstImage;
		protected override string Image => _image;

		public override void Start(Action onComplete) {
			base.Start(onComplete);

			Question1();
		}

		void Question1() {
			Question(
				"You have arrived at the land of the Golden Fleece! This can be a place for excellent clout or sudden death. You’ll need to accomplish tasks in each of the following categories. But you must proceed in proper order! If you make the wrong choice, either in sequence or in the task, you’ll have to crawl back to the ship as best you can (if you’re not already dead). Do you want to get the fleece?",
				new[] {
					new ButtonViewModel { Label = "Yes", OnClick = Question2 },
					new ButtonViewModel { Label = "No", OnClick = SilentEscape }
				}
			);
		}

		void Question2() {
			Question(
				"You’ll try diplomacy first: Who from your crew should be your spokesman to connect with the people of this land?",
				new[] {
					new ButtonViewModel { Label = "Polydeukes", OnClick = () => Dead("Oops, everybody gets killed!") },
					new ButtonViewModel { Label = "Mopsos", OnClick = () => CloseAndDo("A good seer but a terrible negotiator. You are all thrown in a dungeon until a sympathetic guard lets you creep back to your ship in shame. Better sail away!", LowerCloutAndMoney) },
					new ButtonViewModel { Label = "The sons of Phrixus", OnClick = () => CloseAndDo("Well done, the sons of Phrixus call this place home! Now you have to plow a field with dragon’s teeth. This, we call success.", Question3) },
					new ButtonViewModel { Label = "Delion Autolycus and Phlogius, friends of Herakles", OnClick = () => CloseAndDo("They are men of muscle more than persuasion: you escape, but with a loss of clout", LowerClout) },
					new ButtonViewModel { Label = "Orpheus", OnClick = () => CloseAndDo("You escape, while everyone in the palace is entranced by his magical song", NoPunishment) },
				}
			);
		}

		void Question3() {
			_image = SecondImage;
			Question(
				"Now time to cash in on your good looks so you can get a magical bride. Aphrodite has helped you with the arrow of Eros, but Medea is still in torment: you are asking her to betray her father and fatherland, and she lives still in the house of her father, sequestered away from the wide world on which you’ve written your name and fortune. You must promise her something beyond your dazzling charms: is it...",
				new[] {
					new ButtonViewModel { Label = "All the clout-iness of becoming your bride and fellow traveler, a fine reputation among the Greeks", OnClick = () => CloseAndDo("Well done! Send out the wedding invitations. You now have a magician on your side.", Win) },
					new ButtonViewModel { Label = "A nice tapestry given you by the last woman you left brokenhearted on a beach (Hypsipyle)", OnClick = () => CloseAndDo("Jason, this is so the wrong re-gifting. Take that cloak and head for the ship – no fleece for you!", NoPunishment) },
					new ButtonViewModel { Label = "All the wealth from your ship’s stores", OnClick = () => CloseAndDo("Jason, she lives in the King’s palace in golden Colchis. She takes your stores, mocks your foolishness, and you are reduced to the clout level of local goatherd.  Baaaaaaad decision.", LowerCloutToGoatherd) },
					new ButtonViewModel { Label = "Orpheus", OnClick = () => Dead("you could have thought she would be interested, but this offers slender comfort to her emotional anguish. After a nice night of feasting she has you executed by the dragon at dawn.") },
				}
			);
		}

		private void LowerCloutAndMoney() {
			LowerClout();
			Globals.GameVars.playerShipVariables.ship.currency = Mathf.Max(Globals.GameVars.playerShipVariables.ship.currency - 1000, 0);
		}

		private void LowerClout() {
			Globals.GameVars.AdjustPlayerClout(-1);
		}

		private void LowerCloutToGoatherd() {
			// lower as far as we can without hitting 0. because of the multiplier, this is super gross :(
			Globals.GameVars.AdjustPlayerClout(-Mathf.CeilToInt(Globals.GameVars.playerShipVariables.ship.playerClout / 100f) + 1);
		}

		private void NoPunishment() {
			// intentionally does nothing
		}

		// aborts the quiz temporarily. you can try it again by clicking on the port to dock again
		private void SilentEscape() {
			HideAnyScreens();
		}

		private void CloseAndDo(string message, Action impact) {
			HideAnyScreens();
			Message(message, () => {
				HideAnyScreens();
				impact?.Invoke();
			});
		}

		private void Dead(string message) {
			Message(message, GameOver);
		}

		private void Win() {
			Globals.GameVars.AdjustPlayerClout(10);
			Message("Time to sow the dragon’s teeth, fight the earthborn men, and really really make Aeetes mad – because you have prepared yourself as per Medea’s instructions, and her magic and guidance do not fail.  You retire for the day from your labors: Aeetes burns with rage and Medea with anxiety, still torn to leave her homeland with the man she loves.  She comes to your ship and offers to get the fleece for you; you accept, Jason, and follow her to the grove where she puts the serpent to sleep with magical songs and drugs. This lets you, Jason, snatch the fleece from the oak over the body of the snoozing beastie.  The fleece is shiny, you are glowing, the sun is rising, and you have accomplished your quest! But now Aeetes is waking up... better run to the ship and catch a good strong wind!", Complete);
		}
	}
}
