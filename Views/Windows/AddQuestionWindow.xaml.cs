using SecurityTest.Models.Entities;
using System.Windows;
using SecurityTest.Views.Pages.AdminPages;

namespace SecurityTest.Views.Windows
{
	/// <summary>
	/// Логика взаимодействия для AddQuestionWindow.xaml
	/// </summary>
	public partial class AddQuestionWindow : Window
	{
		private readonly Question _currentQuestion;
		private readonly PersonalCabinetAdminPage _currentPage;
		public AddQuestionWindow(Theme selectedTheme, PersonalCabinetAdminPage page)
		{
			InitializeComponent();
			_currentQuestion = new Question {Theme = selectedTheme};
			DataContext = _currentQuestion;
			_currentPage = page;
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			SecurityEntities.GetContext().Questions.Add(_currentQuestion);
			SecurityEntities.GetContext().SaveChanges();
			_currentPage.UpdateQuestionTable();
			Close();
		}
	}
}