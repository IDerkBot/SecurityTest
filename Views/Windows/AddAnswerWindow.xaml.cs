using System.Collections.Generic;
using SecurityTest.Models.Entities;
using System.Windows;
using SecurityTest.Views.Pages.AdminPages;

namespace SecurityTest.Views.Windows
{
	/// <summary>
	/// Логика взаимодействия для AddAnswerWindow.xaml
	/// </summary>
	public partial class AddAnswerWindow : Window
	{
		private readonly Answer _currentAnswer;
		private readonly PersonalCabinetAdminPage _currentPage;
		public AddAnswerWindow(Question selectedQuestion, PersonalCabinetAdminPage page)
		{
			InitializeComponent();
			_currentAnswer = new Answer {Question = selectedQuestion};
			CbNumber.ItemsSource = new List<int> { 1,2,3 };
			DataContext = _currentAnswer;
			_currentPage = page;
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			SecurityEntities.GetContext().Answers.Add(_currentAnswer);
			SecurityEntities.GetContext().SaveChanges();
			_currentPage.UpdateQuestionTable();
			Close();
		}
	}
}