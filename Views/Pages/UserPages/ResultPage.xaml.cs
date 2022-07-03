using SecurityTest.Models;
using SecurityTest.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace SecurityTest.Views.Pages.UserPages
{
	/// <summary>
	/// Логика взаимодействия для ResultPage.xaml
	/// </summary>
	public partial class ResultPage : Page
	{
		public ResultPage(Exam exam, ResultContext results)
		{
			InitializeComponent();
			PageManager.SetTitle(Title);
			var currentExam = exam;
			var que = new List<Question>
			{
				SecurityEntities.GetContext().Answers.Single(x => x.ID == results.Answer1.ID).Question,
				SecurityEntities.GetContext().Answers.Single(x => x.ID == results.Answer2.ID).Question,
				SecurityEntities.GetContext().Answers.Single(x => x.ID == results.Answer3.ID).Question,
				SecurityEntities.GetContext().Answers.Single(x => x.ID == results.Answer4.ID).Question,
				SecurityEntities.GetContext().Answers.Single(x => x.ID == results.Answer5.ID).Question,
				SecurityEntities.GetContext().Answers.Single(x => x.ID == results.Answer6.ID).Question,
				SecurityEntities.GetContext().Answers.Single(x => x.ID == results.Answer7.ID).Question,
				SecurityEntities.GetContext().Answers.Single(x => x.ID == results.Answer8.ID).Question,
				SecurityEntities.GetContext().Answers.Single(x => x.ID == results.Answer9.ID).Question,
				SecurityEntities.GetContext().Answers.Single(x => x.ID == results.Answer10.ID).Question
			};
			
			var list = new List<ResultContextInfo>
			{
				new ResultContextInfo
				{
					Name = "Выбранные",
					Results = results
				},
				new ResultContextInfo
				{
					Name = "Правильные",
					Results = new ResultContext
					{
						Answer1 = que[0].Answers.Single(x => x.IsCorrect),
						Answer2 = que[1].Answers.Single(x => x.IsCorrect),
						Answer3 = que[2].Answers.Single(x => x.IsCorrect),
						Answer4 = que[3].Answers.Single(x => x.IsCorrect),
						Answer5 = que[4].Answers.Single(x => x.IsCorrect),
						Answer6 = que[5].Answers.Single(x => x.IsCorrect),
						Answer7 = que[6].Answers.Single(x => x.IsCorrect),
						Answer8 = que[7].Answers.Single(x => x.IsCorrect),
						Answer9 = que[8].Answers.Single(x => x.IsCorrect),
						Answer10 = que[9].Answers.Single(x => x.IsCorrect),
					}
				}
			};
			DgAnswers.ItemsSource = list;
			var currentAnswers = SecurityEntities.GetContext().ResultInfoes
				.Count(x => x.Result.IDUser == currentExam.IDUser && x.Answer.IsCorrect);
			TblCurrentAnswers.Text =
				$"Верных ответов: {currentAnswers}";
			TblPercent.Text = $"{currentAnswers * 10}%";
			var time = SecurityEntities.GetContext().Results.Single(x => x.IDUser == currentExam.IDUser).TimeInSecond;
			TblTime.Text = $"{time / 60} минут {time % 60} секунд";

			TblResult.Text = currentAnswers < 9 || exam.IsTime == true && time > exam.TimeInSecond ? "Тест не сдан" : "Тест сдан";
			TblResult.Foreground = currentAnswers < 9 || exam.IsTime == true && time > exam.TimeInSecond ? new SolidColorBrush(Color.FromRgb(237, 72, 48)) : new SolidColorBrush(Color.FromRgb(0, 150, 79));
			
		}
	}
}