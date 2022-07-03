using SecurityTest.Models;
using SecurityTest.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SecurityTest.Views.Pages.UserPages
{
	/// <summary>
	/// Логика взаимодействия для TestPage.xaml
	/// </summary>
	public partial class TestPage : Page
	{
		private int _count;
		private readonly Test _currentTest;
		private readonly Exam _currentExam;
		private Theme _currentTheme;
		private Question _currentQue;
		private readonly Result _currentResult;
		private readonly ResultContext _currentResults;
		private List<Answer> _currentAnswers;
		private readonly List<Question> _useQuestions;
		private readonly List<int> _que = new List<int> { 1, 1, 2, 3, 3, 4, 4, 5, 5, 6 };

		#region Load

		private bool _load = true;
		public TestPage(Exam setExam)
		{
			InitializeComponent();
			_currentTest = SecurityEntities.GetContext().Tests.Single(x => setExam.Test.Title == x.Title);
			_currentExam = setExam;
			_useQuestions = new List<Question>(10);
			GetTheme(_currentTest);
			_currentResult = new Result { Test = _currentTest, User = setExam.User };
			_currentResults = new ResultContext();
			DgAnswers.Items.Add(_currentResults);
			PageManager.SetTitle(Title);
			var timer = new Timer();
			if (_currentExam.IsTime == true)
			{
				timer.Interval = 1000;
				timer.Elapsed += TimerOnElapsed;
				timer.Start();
			}
		}

		private int _second;
		private bool _isMessage;
		private void TimerOnElapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				_second++;
				if (_second > _currentExam.TimeInSecond) TblTime.Foreground = new SolidColorBrush(Color.FromRgb(237, 72, 48));
				if (_second > _currentExam.TimeInSecond - 60 && _isMessage == false)
				{
					_isMessage = true;
					MessageBox.Show("Осталась одна минута!");
				}
				TblTime.Text = _second % 60 < 10 ? $"{_second / 60}:0{_second % 60}" : $"{_second / 60}:{_second % 60}";
			});
		}

		#endregion

		#region Buttons

		private void BtnPrevious_OnClick(object sender, RoutedEventArgs e)
		{
			_count--;
			_currentQue = _useQuestions[_count];
			GetAnswers();
			Movement();
			MoveSelect(-1);
		}

		private void BtnNext_OnClick(object sender, RoutedEventArgs e)
		{
			_count++;
			_currentQue = _useQuestions[_count];
			GetAnswers();
			Movement();
			MoveSelect(1);
		}

		private void BtnResults_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				_currentResult.TimeInSecond = _second;
				SecurityEntities.GetContext().Results.Add(_currentResult);
				SecurityEntities.GetContext().ResultInfoes.AddRange(new List<ResultInfo>
					{
						new ResultInfo { Answer = _currentResults.Answer1, Question = _currentResults.Answer1.Question, Result = _currentResult },
						new ResultInfo { Answer = _currentResults.Answer2, Question = _currentResults.Answer2.Question, Result = _currentResult },
						new ResultInfo { Answer = _currentResults.Answer3, Question = _currentResults.Answer3.Question, Result = _currentResult },
						new ResultInfo { Answer = _currentResults.Answer4, Question = _currentResults.Answer4.Question, Result = _currentResult },
						new ResultInfo { Answer = _currentResults.Answer5, Question = _currentResults.Answer5.Question, Result = _currentResult },
						new ResultInfo { Answer = _currentResults.Answer6, Question = _currentResults.Answer6.Question, Result = _currentResult },
						new ResultInfo { Answer = _currentResults.Answer7, Question = _currentResults.Answer7.Question, Result = _currentResult },
						new ResultInfo { Answer = _currentResults.Answer8, Question = _currentResults.Answer8.Question, Result = _currentResult },
						new ResultInfo { Answer = _currentResults.Answer9, Question = _currentResults.Answer9.Question, Result = _currentResult },
						new ResultInfo { Answer = _currentResults.Answer10, Question = _currentResults.Answer10.Question, Result = _currentResult },
					}
				);
				SecurityEntities.GetContext().SaveChanges();
				PageManager.Navigate(new ResultPage(_currentExam, _currentResults));
			}
			catch (Exception exception)
			{
				MessageBox.Show($"{exception.Message}\nОшибка: \n{exception.InnerException}");
			}
		}

		#endregion

		#region Select Event

		private void LvAnswers_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_lvAnswersUpdate)
			{
				_lvAnswersUpdate = false;
				return;
			}

			var selectAnswer = LvAnswers.SelectedItem as Answer;
			_currentResults.GetType().GetProperty($"Answer{_count + 1}")?.SetValue(_currentResults, selectAnswer);
			DgAnswers.Items.Clear();
			DgAnswers.Items.Add(_currentResults);
			if (_count + 1 != 10) return;
			if (_currentResults.Answer1 != null && _currentResults.Answer2 != null && _currentResults.Answer3 != null &&
					_currentResults.Answer4 != null && _currentResults.Answer5 != null && _currentResults.Answer6 != null &&
					_currentResults.Answer7 != null && _currentResults.Answer8 != null && _currentResults.Answer9 != null &&
					_currentResults.Answer10 != null)
			{
				BtnResults.IsEnabled = true;
			}
		}

		#endregion

		#region Methods

		private bool _lvAnswersUpdate;

		private void Movement()
		{
			if (_count + 1 >= 10)
			{
				BtnNext.IsEnabled = false;
				return;
			}
			if (_count + 1 < 10 && BtnNext.IsEnabled == false)
			{
				BtnNext.IsEnabled = true;
			}
			if (_count == 0)
			{
				BtnPrevious.IsEnabled = false;
				return;
			}
			if (_count != 0 && BtnPrevious.IsEnabled == false)
				BtnPrevious.IsEnabled = true;
		}

		private void MoveSelect(int indexMove)
		{
			var borders = GSelectedQue.Children.Cast<Border>().ToList();
			if (indexMove < 0)
			{
				borders[_count + 1].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
				borders[_count].Background = new SolidColorBrush(Color.FromRgb(0, 150, 70));
			}
			else if (indexMove > 0)
			{
				borders[_count - 1].Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
				borders[_count].Background = new SolidColorBrush(Color.FromRgb(0, 150, 70));
			}

			GSelectedQue.Children.Clear();
			borders.ForEach(x => GSelectedQue.Children.Add(x));
		}

		private bool IsFourPower => _currentTest.Title.ToLower() == "4 разряд";

		private void GetTheme(Test test)
		{
			if (IsFourPower)
			{
				var tickets = SecurityEntities.GetContext().Themes.Where(x => x.IDTest == test.ID).ToList();
				var random = new Random();
				_currentTheme = tickets[random.Next(1, tickets.Count)];
			}

			for (var i = 0; i < 10; i++)
				GetQue(i);

			_currentQue = _useQuestions[_count];
			GetAnswers();
		}

		private void GetQue(int theme)
		{
			if (!IsFourPower)
			{
				var themes = SecurityEntities.GetContext().Themes.Where(x => x.IDTest == _currentTest.ID).ToList();
				_currentTheme = themes[_que[theme] - 1];
			}
			var questions = SecurityEntities.GetContext().Questions.Where(x => x.Theme.ID == _currentTheme.ID).ToList();
			var questionsFilter = questions.Where(x => _useQuestions.All(y => y.ID != x.ID)).ToList();
			_currentQue = questionsFilter.Count != 1 ? questionsFilter[new Random().Next(0, questionsFilter.Count)] : questionsFilter[0];
			_useQuestions.Add(_currentQue);
		}

		private void GetAnswers()
		{
			_currentAnswers = SecurityEntities.GetContext().Answers.Where(x => x.IDQuestion == _currentQue.ID).ToList();
			SetValues();
		}

		private void SetValues()
		{
			if (_load)
			{
				_lvAnswersUpdate = false;
				_load = false;
			}
			else
				_lvAnswersUpdate = true;
			TblQue.Text = _currentQue.Ask;
			LvAnswers.ItemsSource = _currentAnswers;
		}

		#endregion
	}
}
