using SecurityTest.Models.Entities;

namespace SecurityTest.Models
{
	public class ResultContext
	{
		public Answer Answer1 { get; set; }
		public Answer Answer2 { get; set; }
		public Answer Answer3 { get; set; }
		public Answer Answer4 { get; set; }
		public Answer Answer5 { get; set; }
		public Answer Answer6 { get; set; }
		public Answer Answer7 { get; set; }
		public Answer Answer8 { get; set; }
		public Answer Answer9 { get; set; }
		public Answer Answer10 { get; set; }
	}

	public class ResultContextInfo
	{
		public string Name { get; set; }
		public ResultContext Results { get; set; }
	}

	public class ResultTable
	{
		public int Number { get; set; }
		public Answer Select { get; set; }
		public Answer Current { get; set; }
		public Theme Theme { get; set; }
	}
}
