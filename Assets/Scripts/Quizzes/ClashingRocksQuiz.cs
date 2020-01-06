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

		private Action CompleteCallback;

		public override void Start(Action onComplete) {
			Debug.Log("Started clashing rocks quiz");

			// reset state
			CompleteCallback = onComplete;

			FirstStep();
		}

		private void FirstStep() {

			Globals.UI.Show<QuizScreen, QuizScreenModel>(new QuizScreenModel(
				title: "Clashing Rocks",
				message: "You’re at the clashing rocks: oh no! do you remember your instructions from Phineus? Choose wisely – or you will be smashed to epic smithereens.",
				icon: Resources.Load<Sprite>("stop_9"),
				choices: new ObservableCollection<ButtonViewModel> {
					new ButtonViewModel { Label = "Pray to Athena", OnClick = Dead },
					new ButtonViewModel { Label = "Apologize to Zeus", OnClick = Dead },
					new ButtonViewModel { Label = "Release a trembling dove", OnClick = Complete },
					new ButtonViewModel { Label = "Row as if your lives and fame and ship depended on it", OnClick = Dead },
					new ButtonViewModel { Label = "Ask the ship’s seer to perform a sacrifice to Poseidon", OnClick = Dead }
				}
			));

		}

		private void Dead() {
			Globals.UI.Hide<QuizScreen>();
			Globals.UI.Show<QuestScreen, QuizScreenModel>(new QuizScreenModel(
				title: "Clashing Rocks",
				message: "Smashed, shattered, drowned – you’re dead.",
				icon: Resources.Load<Sprite>("stop_9"),
				choices: new ObservableCollection<ButtonViewModel> {
					new ButtonViewModel { Label = "OK", OnClick = () => {
						Globals.UI.Hide<QuestScreen>();
						Globals.GameVars.isGameOver = true;
					} }
				}
			));
		}

		private void Complete() {

			Globals.UI.Hide<QuizScreen>();
			Globals.UI.Show<QuestScreen, QuizScreenModel>(new QuizScreenModel(
				title: "Clashing Rocks",
				message: Globals.Quests.NextSegment.descriptionOfQuest,
				icon: Resources.Load<Sprite>("stop_9"),
				choices: new ObservableCollection<ButtonViewModel> {
					new ButtonViewModel {
						Label = "OK",
						OnClick = () => {
							Globals.UI.Hide<QuestScreen>();
							CompleteCallback?.Invoke();
						}
					}
				}
			));

		}
	}
}
