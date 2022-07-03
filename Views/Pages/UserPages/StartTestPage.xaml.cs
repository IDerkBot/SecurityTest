using SecurityTest.Models;
using SecurityTest.Models.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SecurityTest.Views.Pages.UserPages
{
	/// <summary>
	/// Логика взаимодействия для StartTestPage.xaml
	/// </summary>
	public partial class StartTestPage : Page
	{
		private readonly Exam _currentExam;
		public StartTestPage(User selectedUser)
		{
			InitializeComponent();
			if(SecurityEntities.GetContext().Exams.Any(x => x.IDUser == selectedUser.ID))
				_currentExam = SecurityEntities.GetContext().Exams.Single(x => x.IDUser == selectedUser.ID);
			else
			{
				MessageBox.Show("Экзамен не назначен!");
				BtnStartTest.IsEnabled = false;
				return;
			}

			if (SecurityEntities.GetContext().Results.Any(x => x.IDUser == selectedUser.ID))
			{
				MessageBox.Show("Экзамен уже был проведен!");
				BtnStartTest.IsEnabled = false;
			}
			DataContext = _currentExam;
			TblTime.Text = _currentExam.TimeInSecond != null ? $"{_currentExam.TimeInSecond / 60} минут {_currentExam.TimeInSecond % 60} секунд" : "Неограниченно";
		}

		private void BtnStartTestMove_OnClick(object sender, RoutedEventArgs e)
		{
			BtnStartTest.IsEnabled = false;
			PageManager.Navigate(new TestPage(_currentExam));
		}

		private void StartTestPage_OnLoaded(object sender, RoutedEventArgs e)
		{
			
		}
	}
}