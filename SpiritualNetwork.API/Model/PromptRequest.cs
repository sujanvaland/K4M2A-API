namespace SpiritualNetwork.API.Model
{
	public class Content
	{
		public string type { get; set; }
		public string text { get; set; }
	}

	public class PromptMessage
	{
		public string role { get; set; }
		public List<Content> content { get; set; }
	}

	public class PromptRequest
	{
		public List<PromptMessage> messages { get; set; }
		public double temperature { get; set; }
		public double top_p { get; set; }
		public int max_tokens { get; set; }
	}


}
