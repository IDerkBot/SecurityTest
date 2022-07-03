using SecurityTest.Models.Entities;
using System.Windows;
using SecurityTest.Views.Pages.AdminPages;

namespace SecurityTest.Views.Windows
{
	/// <summary>
	/// Логика взаимодействия для EditAnswerWindow.xaml
	/// </summary>
	public partial class EditAnswerWindow : Window
	{
		private readonly PersonalCabinetAdminPage _currentPage;
		public EditAnswerWindow(Answer selectedAnswer, PersonalCabinetAdminPage page)
		{
			InitializeComponent();
			DataContext = selectedAnswer;
			_currentPage = page;
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			SecurityEntities.GetContext().SaveChanges();
			_currentPage.UpdateAnswersTable();
			Close();
		}
	}
}