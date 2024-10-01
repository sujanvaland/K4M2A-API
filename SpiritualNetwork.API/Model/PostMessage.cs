namespace SpiritualNetwork.API.Model
{
	public class PostDataDto
	{
		public Dictionary<string, string> FormFields { get; set; } = new Dictionary<string, string>();
		public List<FileDataDto> Files { get; set; } = new List<FileDataDto>();
		public string Topic { get; set; }
		public int UserUniqueId { get; set; }
		public string Username { get; set; }
	}

	public class FileDataDto
	{
		public string FileName { get; set; }
		public string Base64Content { get; set; }
	}
}
