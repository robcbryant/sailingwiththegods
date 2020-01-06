using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Quizzes
{
	abstract class Quiz
	{
		public abstract string Name { get; }
		public abstract void Start(Action onComplete);
	}

	public static class QuizSystem
	{
		static Dictionary<string, Quiz> _quizzes = new Dictionary<string, Quiz>();

		static void Add(Quiz quiz) {
			_quizzes.Add(quiz.Name, quiz);
		}

		static QuizSystem() {
			Add(new ClashingRocksQuiz());
		}

		static Quiz GetQuiz(string name) => _quizzes.ContainsKey(name) ? _quizzes[name] : null;

		public static void StartQuiz(string name, Action onComplete) {
			var quiz = GetQuiz(name);
			if (quiz != null) {
				quiz.Start(onComplete);
			}
		}
	}
}
