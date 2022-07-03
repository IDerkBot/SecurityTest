using SecurityTest.Models;
using SecurityTest.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using RadioButton = System.Windows.Controls.RadioButton;
using Word = Microsoft.Office.Interop.Word;

namespace SecurityTest.Views.Pages.AdminPages
{
	/// <summary>
	/// Логика взаимодействия для UserCardPage.xaml
	/// </summary>
	public partial class UserCardPage : Page
	{
		private readonly UserProfile _currentUser;
		private List<ResultInfo> _currentResultInfo;
		public UserCardPage(UserInfo selectedUser)
		{
			InitializeComponent();
			_currentUser = new UserProfile { User = selectedUser.User, UserInfo = selectedUser };
			DataContext = _currentUser;
		}

		private void UserCardPage_OnLoaded(object sender, RoutedEventArgs e)
		{
			if (_currentUser == null) return;
			if (SecurityEntities.GetContext().Results.Any(x => x.IDUser == _currentUser.User.ID))
			{
				GetResultTable();
			}

			BtnSetTest.IsEnabled = !SecurityEntities.GetContext().Exams.Any(x => x.IDUser == _currentUser.User.ID);
			if (!SecurityEntities.GetContext().Exams.Any(x => x.IDUser == _currentUser.User.ID)) return;

			var exam = SecurityEntities.GetContext().Exams.Single(x => x.IDUser == _currentUser.User.ID);
			TblExamInfo.Text = !exam.Test.Title.ToLower().Contains("для") ? $"Экзамен: для {exam.Test.Title.ToLower()}а назначен" : $"Экзамен: {exam.Test.Title.ToLower()} назначен";

			if (!SecurityEntities.GetContext().Results.Any(x => x.IDUser == _currentUser.User.ID)) return;
			var result = SecurityEntities.GetContext().Results.Single(x => x.IDUser == _currentUser.User.ID);
			TblExamInfo.Text = !exam.Test.Title.ToLower().Contains("для") ? $"Экзамен: для {result.Test.Title.ToLower()}а проведен" : $"Экзамен: {exam.Test.Title.ToLower()} проведен";
		}

		#region Tab1

		private void BtnGeneratePassword_OnClick(object sender, RoutedEventArgs e)
		{
			TbPassword.Text = Membership.GeneratePassword(6, 1);
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog { Filter = @"Image File (*.jpg, *.png)|*.jpg;*.png" };
			if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
			var image = ImageManager.CroppedToBitmapImage(ofd.FileName);
			_currentUser.UserInfo.Image = ImageManager.CroppedToBytes(image);
			MainImage.Source = image;
		}

		private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
		{
			SecurityEntities.GetContext().SaveChanges();
			MessageBox.Show("Данные изменены");
		}

		private void BtnDeleteUser_OnClick(object sender, RoutedEventArgs e)
		{
			if(MessageBox.Show("Вы действительно хотите удалить данного пользователя?", "Удаление пользователя", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
			SecurityEntities.GetContext().Users.Remove(_currentUser.User);
			SecurityEntities.GetContext().UserInfoes.Remove(_currentUser.UserInfo);
			
			SecurityEntities.GetContext().SaveChanges();
			PageManager.Navigate(new PersonalCabinetAdminPage());
		}

		#endregion

		#region Tab2

		private string _currentTest;

		private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
		{
			TbTime.Visibility = CbTime.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
		}

		private void TbTime_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			if (TbTime.Text.Any(char.IsLetter)) TbTime.Text = TbTime.Text.Substring(0, TbTime.Text.Length - 1);
			TbTime.CaretIndex = TbTime.Text.Length;
		}

		private void RbSelectTest_OnChecked(object sender, RoutedEventArgs e)
		{
			_currentTest = ((RadioButton)sender).Content.ToString();
		}

		private void BtnSetTest_OnClick(object sender, RoutedEventArgs e)
		{
			BtnSetTest.IsEnabled = false;
			if (_currentTest == null)
			{
				MessageBox.Show("Вы не выбрали тест");
				return;
			}
			var isTime = CbTime.IsChecked == true;
			var time = TbTime.Text;
			int? timeInSecond;
			if (isTime)
			{
				if (time.Contains(":"))
					timeInSecond = int.Parse(time.Split(':')[0]) * 60 + int.Parse(time.Split(':')[1]);
				else
					timeInSecond = int.Parse(time);
			}
			else timeInSecond = null;

			var exam = new Exam
			{
				ID = 0,
				IDTest = SecurityEntities.GetContext().Tests.Single(x => x.Title == _currentTest).ID,
				IDUser = _currentUser.User.ID,
				IsTime = isTime,
				TimeInSecond = timeInSecond,
			};
			SecurityEntities.GetContext().Exams.Add(exam);
			SecurityEntities.GetContext().SaveChanges();
			TblExamInfo.Text = "Экзамен назначен";
		}

		#endregion

		#region Tab3

		private void BtnExamList_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				new Thread(() =>
				{
					try
					{
						// Создали объект ворда
						var app = new Word.Application();

						//Открываем файл
						//app.Documents.Open($@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Template.docx");
						app.Documents.Open($@"{AppDomain.CurrentDomain.BaseDirectory}\Views\Resources\Documents\Template.docx");

						//Заменяем слова
						if (_currentUser.User.Exams.ToList()[0].Test.Title.Contains("руководителей"))
							FindAndReplace(app, GetDictionaryManager());
						else
							FindAndReplace(app, GetDictionary());

						if (!Directory.Exists(
							    $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\"))
							Directory.CreateDirectory(
								$@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\");

						var culture = new CultureInfo("ru-RU");
						//Сохраняем файл
						//app.Visible = true;
						app.ActiveDocument.SaveAs(
							$@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\{_currentUser.User.ID}-{_currentUser.UserInfo.DateExam?.ToString(culture.DateTimeFormat.ShortDatePattern)}.docx");
						app.ActiveDocument.Close();
						app.Quit();
						Process.Start(
							$@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\{_currentUser.User.ID}-{_currentUser.UserInfo.DateExam?.ToString(culture.DateTimeFormat.ShortDatePattern)}.docx");
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Ошибка {ex.Message}\n\n\nВнутреннее исключение{ex.InnerException}");
					}
					
				}).Start();
			}
			catch (Exception exception)
			{
				MessageBox.Show($"Ошибка {exception.Message}\n\n\nВнутреннее исключение{exception.InnerException}");
			}

		}

		private Dictionary<string, string> GetDictionary()
		{
			var culture = new CultureInfo("ru-RU");
			var results = SecurityEntities.GetContext().ResultInfoes.Where(x => x.Result.IDUser == _currentUser.User.ID).ToList();
			var resInfo = SecurityEntities.GetContext().ResultInfoes.ToList();
			var dict = new Dictionary<string, string>
			{
				["[#ID#]"] = _currentUser.User.ID.ToString(),
				["[#DATE#]"] = _currentUser.UserInfo.DateExam?.ToString(culture.DateTimeFormat.ShortDatePattern),
				["[#TIME#]"] = "",
				["[#FULLNAME#]"] = _currentUser.User.Fullname,
				["[#DATEBIRTH#]"] = _currentUser.UserInfo.DateBirth?.ToString(culture.DateTimeFormat.ShortDatePattern),
				["[#QUE1#]"] = _currentResultInfo[0].Question.Ask,
				["[#QUE2#]"] = _currentResultInfo[1].Question.Ask,
				["[#QUE3#]"] = _currentResultInfo[2].Question.Ask,
				["[#QUE4#]"] = _currentResultInfo[3].Question.Ask,
				["[#QUE5#]"] = _currentResultInfo[4].Question.Ask,
				["[#QUE6#]"] = _currentResultInfo[5].Question.Ask,
				["[#QUE7#]"] = _currentResultInfo[6].Question.Ask,
				["[#QUE8#]"] = _currentResultInfo[7].Question.Ask,
				["[#QUE9#]"] = _currentResultInfo[8].Question.Ask,
				["[#QUE10#]"] = _currentResultInfo[9].Question.Ask,
				["[#ANSWER00#]"] = $"1. {_currentResultInfo[0].Question.Answers.ToList()[0].Content}",
				["[#ANSWER01#]"] = $"2. {_currentResultInfo[0].Question.Answers.ToList()[1].Content}",
				["[#ANSWER02#]"] = $"3. {_currentResultInfo[0].Question.Answers.ToList()[2].Content}",
				["[#ANSWER10#]"] = $"1. {_currentResultInfo[1].Question.Answers.ToList()[0].Content}",
				["[#ANSWER11#]"] = $"2. {_currentResultInfo[1].Question.Answers.ToList()[1].Content}",
				["[#ANSWER12#]"] = $"3. {_currentResultInfo[1].Question.Answers.ToList()[2].Content}",
				["[#ANSWER20#]"] = $"1. {_currentResultInfo[2].Question.Answers.ToList()[0].Content}",
				["[#ANSWER21#]"] = $"2. {_currentResultInfo[2].Question.Answers.ToList()[1].Content}",
				["[#ANSWER22#]"] = $"3. {_currentResultInfo[2].Question.Answers.ToList()[2].Content}",
				["[#ANSWER30#]"] = $"1. {_currentResultInfo[3].Question.Answers.ToList()[0].Content}",
				["[#ANSWER31#]"] = $"2. {_currentResultInfo[3].Question.Answers.ToList()[1].Content}",
				["[#ANSWER32#]"] = $"3. {_currentResultInfo[3].Question.Answers.ToList()[2].Content}",
				["[#ANSWER40#]"] = $"1. {_currentResultInfo[4].Question.Answers.ToList()[0].Content}",
				["[#ANSWER41#]"] = $"2. {_currentResultInfo[4].Question.Answers.ToList()[1].Content}",
				["[#ANSWER42#]"] = $"3. {_currentResultInfo[4].Question.Answers.ToList()[2].Content}",
				["[#ANSWER50#]"] = $"1. {_currentResultInfo[5].Question.Answers.ToList()[0].Content}",
				["[#ANSWER51#]"] = $"2. {_currentResultInfo[5].Question.Answers.ToList()[1].Content}",
				["[#ANSWER52#]"] = $"3. {_currentResultInfo[5].Question.Answers.ToList()[2].Content}",
				["[#ANSWER60#]"] = $"1. {_currentResultInfo[6].Question.Answers.ToList()[0].Content}",
				["[#ANSWER61#]"] = $"2. {_currentResultInfo[6].Question.Answers.ToList()[1].Content}",
				["[#ANSWER62#]"] = $"3. {_currentResultInfo[6].Question.Answers.ToList()[2].Content}",
				["[#ANSWER70#]"] = $"1. {_currentResultInfo[7].Question.Answers.ToList()[0].Content}",
				["[#ANSWER71#]"] = $"2. {_currentResultInfo[7].Question.Answers.ToList()[1].Content}",
				["[#ANSWER72#]"] = $"3. {_currentResultInfo[7].Question.Answers.ToList()[2].Content}",
				["[#ANSWER80#]"] = $"1. {_currentResultInfo[8].Question.Answers.ToList()[0].Content}",
				["[#ANSWER81#]"] = $"2. {_currentResultInfo[8].Question.Answers.ToList()[1].Content}",
				["[#ANSWER82#]"] = $"3. {_currentResultInfo[8].Question.Answers.ToList()[2].Content}",
				["[#ANSWER90#]"] = $"1. {_currentResultInfo[9].Question.Answers.ToList()[0].Content}",
				["[#ANSWER91#]"] = $"2. {_currentResultInfo[9].Question.Answers.ToList()[1].Content}",
				["[#ANSWER92#]"] = $"3. {_currentResultInfo[9].Question.Answers.ToList()[2].Content}",
				["[#TIMEEXAM#]"] = _currentResultInfo[0].Result.TimeInSecond == 0 ? "Неограниченно" : $"{_currentResultInfo[0].Result.TimeInSecond / 60} минут {_currentResultInfo[0].Result.TimeInSecond % 60} секунд",
				["[#CORRECT#]"] = resInfo.Count(x => x.Result.IDUser == _currentResultInfo[0].Result.IDUser && x.Answer.IsCorrect).ToString(),
				["[#NOCORRECT#]"] = (10 - resInfo.Count(x => x.Result.IDUser == _currentResultInfo[0].Result.IDUser && x.Answer.IsCorrect)).ToString(),
				["[#PERCENT#]"] = $"{resInfo.Count(x => x.Result.IDUser == _currentResultInfo[0].Result.IDUser && x.Answer.IsCorrect) * 10}%",
				["[#RESULT#]"] = resInfo.Count(x => x.Result.IDUser == _currentResultInfo[0].Result.IDUser && x.Answer.IsCorrect) < 9 || _currentUser.User.Exams.ToList()[0].IsTime == true && _currentResultInfo[0].Result.TimeInSecond > _currentUser.User.Exams.ToList()[0].TimeInSecond ? "Тест не сдан" : "Тест сдан",
				["[#ANSWER0#]"] = $"{_currentResultInfo[0].Answer.Number}({_currentResultInfo[0].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER1#]"] = $"{_currentResultInfo[1].Answer.Number}({_currentResultInfo[1].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER2#]"] = $"{_currentResultInfo[2].Answer.Number}({_currentResultInfo[2].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER3#]"] = $"{_currentResultInfo[3].Answer.Number}({_currentResultInfo[3].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER4#]"] = $"{_currentResultInfo[4].Answer.Number}({_currentResultInfo[4].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER5#]"] = $"{_currentResultInfo[5].Answer.Number}({_currentResultInfo[5].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER6#]"] = $"{_currentResultInfo[6].Answer.Number}({_currentResultInfo[6].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER7#]"] = $"{_currentResultInfo[7].Answer.Number}({_currentResultInfo[7].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER8#]"] = $"{_currentResultInfo[8].Answer.Number}({_currentResultInfo[8].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER9#]"] = $"{_currentResultInfo[9].Answer.Number}({_currentResultInfo[9].Question.Answers.Single(x => x.IsCorrect).Number})",
			};
			return dict;
		}
		private Dictionary<string, string> GetDictionaryManager()
		{
			var culture = new CultureInfo("ru-RU");
			var results = SecurityEntities.GetContext().ResultInfoes.Where(x => x.Result.IDUser == _currentUser.User.ID).ToList();
			var resInfo = SecurityEntities.GetContext().ResultInfoes.ToList();
			var dict = new Dictionary<string, string>
			{
				["[#ID#]"] = _currentUser.User.ID.ToString(),
				["[#DATE#]"] = _currentUser.UserInfo.DateExam?.ToString(culture.DateTimeFormat.ShortDatePattern),
				["[#TIME#]"] = "",
				["[#FULLNAME#]"] = _currentUser.User.Fullname,
				["[#DATEBIRTH#]"] = _currentUser.UserInfo.DateBirth?.ToString(culture.DateTimeFormat.ShortDatePattern),
				["[#QUE1#]"] = _currentResultInfo[0].Question.Ask,
				["[#QUE2#]"] = _currentResultInfo[1].Question.Ask,
				["[#QUE3#]"] = _currentResultInfo[2].Question.Ask,
				["[#QUE4#]"] = _currentResultInfo[3].Question.Ask,
				["[#QUE5#]"] = _currentResultInfo[4].Question.Ask,
				["[#QUE6#]"] = _currentResultInfo[5].Question.Ask,
				["[#QUE7#]"] = _currentResultInfo[6].Question.Ask,
				["[#QUE8#]"] = _currentResultInfo[7].Question.Ask,
				["[#QUE9#]"] = _currentResultInfo[8].Question.Ask,
				["[#QUE10#]"] = _currentResultInfo[9].Question.Ask,
				["[#ANSWER00#]"] = $"1. {_currentResultInfo[0].Question.Answers.ToList()[0].Content}",
				["[#ANSWER01#]"] = $"2. {_currentResultInfo[0].Question.Answers.ToList()[1].Content}",
				["[#ANSWER02#]"] = "",
				["[#ANSWER10#]"] = $"1. {_currentResultInfo[1].Question.Answers.ToList()[0].Content}",
				["[#ANSWER11#]"] = $"2. {_currentResultInfo[1].Question.Answers.ToList()[1].Content}",
				["[#ANSWER12#]"] = "",
				["[#ANSWER20#]"] = $"1. {_currentResultInfo[2].Question.Answers.ToList()[0].Content}",
				["[#ANSWER21#]"] = $"2. {_currentResultInfo[2].Question.Answers.ToList()[1].Content}",
				["[#ANSWER22#]"] = "",
				["[#ANSWER30#]"] = $"1. {_currentResultInfo[3].Question.Answers.ToList()[0].Content}",
				["[#ANSWER31#]"] = $"2. {_currentResultInfo[3].Question.Answers.ToList()[1].Content}",
				["[#ANSWER32#]"] = "",
				["[#ANSWER40#]"] = $"1. {_currentResultInfo[4].Question.Answers.ToList()[0].Content}",
				["[#ANSWER41#]"] = $"2. {_currentResultInfo[4].Question.Answers.ToList()[1].Content}",
				["[#ANSWER42#]"] = "",
				["[#ANSWER50#]"] = $"1. {_currentResultInfo[5].Question.Answers.ToList()[0].Content}",
				["[#ANSWER51#]"] = $"2. {_currentResultInfo[5].Question.Answers.ToList()[1].Content}",
				["[#ANSWER52#]"] = "",
				["[#ANSWER60#]"] = $"1. {_currentResultInfo[6].Question.Answers.ToList()[0].Content}",
				["[#ANSWER61#]"] = $"2. {_currentResultInfo[6].Question.Answers.ToList()[1].Content}",
				["[#ANSWER62#]"] = "",
				["[#ANSWER70#]"] = $"1. {_currentResultInfo[7].Question.Answers.ToList()[0].Content}",
				["[#ANSWER71#]"] = $"2. {_currentResultInfo[7].Question.Answers.ToList()[1].Content}",
				["[#ANSWER72#]"] = "",
				["[#ANSWER80#]"] = $"1. {_currentResultInfo[8].Question.Answers.ToList()[0].Content}",
				["[#ANSWER81#]"] = $"2. {_currentResultInfo[8].Question.Answers.ToList()[1].Content}",
				["[#ANSWER82#]"] = "",
				["[#ANSWER90#]"] = $"1. {_currentResultInfo[9].Question.Answers.ToList()[0].Content}",
				["[#ANSWER91#]"] = $"2. {_currentResultInfo[9].Question.Answers.ToList()[1].Content}",
				["[#ANSWER92#]"] = "",
				["[#TIMEEXAM#]"] = _currentResultInfo[0].Result.TimeInSecond == 0 ? "Неограниченно" : $"{_currentResultInfo[0].Result.TimeInSecond / 60} минут {_currentResultInfo[0].Result.TimeInSecond % 60} секунд",
				["[#CORRECT#]"] = resInfo.Count(x => x.Result.IDUser == _currentResultInfo[0].Result.IDUser && x.Answer.IsCorrect).ToString(),
				["[#NOCORRECT#]"] = (10 - resInfo.Count(x => x.Result.IDUser == _currentResultInfo[0].Result.IDUser && x.Answer.IsCorrect)).ToString(),
				["[#PERCENT#]"] = $"{resInfo.Count(x => x.Result.IDUser == _currentResultInfo[0].Result.IDUser && x.Answer.IsCorrect) * 10}%",
				["[#RESULT#]"] = resInfo.Count(x => x.Result.IDUser == _currentResultInfo[0].Result.IDUser && x.Answer.IsCorrect) < 9 || _currentUser.User.Exams.ToList()[0].IsTime == true && _currentResultInfo[0].Result.TimeInSecond > _currentUser.User.Exams.ToList()[0].TimeInSecond ? "Тест не сдан" : "Тест сдан",
				["[#ANSWER0#]"] = $"{_currentResultInfo[0].Answer.Number}({_currentResultInfo[0].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER1#]"] = $"{_currentResultInfo[1].Answer.Number}({_currentResultInfo[1].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER2#]"] = $"{_currentResultInfo[2].Answer.Number}({_currentResultInfo[2].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER3#]"] = $"{_currentResultInfo[3].Answer.Number}({_currentResultInfo[3].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER4#]"] = $"{_currentResultInfo[4].Answer.Number}({_currentResultInfo[4].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER5#]"] = $"{_currentResultInfo[5].Answer.Number}({_currentResultInfo[5].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER6#]"] = $"{_currentResultInfo[6].Answer.Number}({_currentResultInfo[6].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER7#]"] = $"{_currentResultInfo[7].Answer.Number}({_currentResultInfo[7].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER8#]"] = $"{_currentResultInfo[8].Answer.Number}({_currentResultInfo[8].Question.Answers.Single(x => x.IsCorrect).Number})",
				["[#ANSWER9#]"] = $"{_currentResultInfo[9].Answer.Number}({_currentResultInfo[9].Question.Answers.Single(x => x.IsCorrect).Number})",
			};
			return dict;
		}

		private void FindAndReplace(Word._Application app, Dictionary<string, string> words)
		{
			try
			{
				var missing = Type.Missing;
				foreach (var item in words)
				{
					var find = app.Selection.Find;
					find.Text = item.Key;
					find.Replacement.Text = item.Value.Length >= 255 ? $"{item.Value.Substring(0, 250)}..." : item.Value;
					object wrap = Word.WdFindWrap.wdFindContinue;
					object replace = Word.WdReplace.wdReplaceAll;

					find.Execute(Type.Missing, false, false, false, missing, false, true, wrap, false, missing, replace);
				}

				File.WriteAllBytes(
					$@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\{_currentUser.User.ID}.jpg",
					_currentUser.UserInfo.Image);

				foreach (var s in app.ActiveDocument.Shapes.Cast<Word.Shape>()
					         .Where(s => s.AlternativeText.ToLower().Contains("1.jpg")))
				{
					s.Fill.UserPicture(
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\{_currentUser.User.ID}.jpg");
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show($"Ошибка {exception.Message}\n\n\nВнутреннее исключение{exception.InnerException}");
			}
		}

		private void DgResults_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (DgResults.SelectedItem is ResultTable selectedResult)
			{
				TblNumber.Text = selectedResult.Number.ToString();
				TbValue.Text = selectedResult.Select.Number.ToString();
				TblCurrent.Text = selectedResult.Current.Number.ToString();
			}
		}

		private void BtnResultEdit_OnClick(object sender, RoutedEventArgs e)
		{
			var result = SecurityEntities.GetContext().ResultInfoes.Where(x => x.Result.IDUser == _currentUser.User.ID)
				.ToList()[int.Parse(TblNumber.Text) - 1];
			var value = int.Parse(TbValue.Text);
			var answer = SecurityEntities.GetContext().Answers
				.Single(x => x.IDQuestion == result.IDQuestion && x.Number == value);
			result.Answer = answer;
			SecurityEntities.GetContext().SaveChanges();
			GetResultTable();
		}

		private void GetResultTable()
		{
			_currentResultInfo = SecurityEntities.GetContext().ResultInfoes.Where(x => x.Result.IDUser == _currentUser.User.ID).ToList();
			var list = new List<ResultTable>()
				{
					new() { Number = 1, Select = _currentResultInfo[0].Answer, Current = _currentResultInfo[0].Question.Answers.First(x => x.IsCorrect), Theme = _currentResultInfo[0].Question.Theme },
					new() { Number = 2, Select = _currentResultInfo[1].Answer, Current = _currentResultInfo[1].Question.Answers.Single(x => x.IsCorrect), Theme = _currentResultInfo[1].Question.Theme },
					new() { Number = 3, Select = _currentResultInfo[2].Answer, Current = _currentResultInfo[2].Question.Answers.Single(x => x.IsCorrect), Theme = _currentResultInfo[2].Question.Theme },
					new() { Number = 4, Select = _currentResultInfo[3].Answer, Current = _currentResultInfo[3].Question.Answers.Single(x => x.IsCorrect), Theme = _currentResultInfo[3].Question.Theme },
					new() { Number = 5, Select = _currentResultInfo[4].Answer, Current = _currentResultInfo[4].Question.Answers.Single(x => x.IsCorrect), Theme = _currentResultInfo[4].Question.Theme },
					new() { Number = 6, Select = _currentResultInfo[5].Answer, Current = _currentResultInfo[5].Question.Answers.Single(x => x.IsCorrect), Theme = _currentResultInfo[5].Question.Theme },
					new() { Number = 7, Select = _currentResultInfo[6].Answer, Current = _currentResultInfo[6].Question.Answers.Single(x => x.IsCorrect), Theme = _currentResultInfo[6].Question.Theme },
					new() { Number = 8, Select = _currentResultInfo[7].Answer, Current = _currentResultInfo[7].Question.Answers.Single(x => x.IsCorrect), Theme = _currentResultInfo[7].Question.Theme },
					new() { Number = 9, Select = _currentResultInfo[8].Answer, Current = _currentResultInfo[8].Question.Answers.Single(x => x.IsCorrect), Theme = _currentResultInfo[8].Question.Theme },
					new() { Number = 10, Select = _currentResultInfo[9].Answer, Current = _currentResultInfo[9].Question.Answers.Single(x => x.IsCorrect), Theme = _currentResultInfo[9].Question.Theme },
				};
			DgResults.ItemsSource = list;
		}

		#endregion

		private void BtnExamListOpen_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				new Thread(() =>
				{
					try
					{
						// Создали объект ворда
						var app = new Word.Application();

						//Открываем файл
						//app.Documents.Open($@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Template.docx");
						app.Documents.Open($@"{AppDomain.CurrentDomain.BaseDirectory}\Views\Resources\Documents\Template.docx");

						//Заменяем слова
						if (_currentUser.User.Exams.ToList()[0].Test.Title.Contains("руководителей"))
							FindAndReplace(app, GetDictionaryManager());
						else
							FindAndReplace(app, GetDictionary());

						if (!Directory.Exists(
							    $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\"))
							Directory.CreateDirectory(
								$@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\");

						var culture = new CultureInfo("ru-RU");
						//Сохраняем файл
						app.Visible = true;
						//app.ActiveDocument.SaveAs(
						//	$@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\{_currentUser.User.ID}-{_currentUser.UserInfo.DateExam?.ToString(culture.DateTimeFormat.ShortDatePattern)}.docx");
						//app.ActiveDocument.Close();
						//app.Quit();
						//Process.Start(
						//	$@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\{_currentUser.User.ID}-{_currentUser.UserInfo.DateExam?.ToString(culture.DateTimeFormat.ShortDatePattern)}.docx");
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Ошибка {ex.Message}\n\n\nВнутреннее исключение{ex.InnerException}");
					}

				}).Start();
			}
			catch (Exception exception)
			{
				MessageBox.Show($"Ошибка {exception.Message}\n\n\nВнутреннее исключение{exception.InnerException}");
			}
		}
	}
}