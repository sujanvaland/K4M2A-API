namespace SpiritualNetwork.API.Model
{
	
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class Choice
	{
		public ContentFilterResults content_filter_results { get; set; }
		public string finish_reason { get; set; }
		public int index { get; set; }
		public object logprobs { get; set; }
		public Message message { get; set; }
	}

	public class ContentFilterResults
	{
		public Hate hate { get; set; }
		public ProtectedMaterialCode protected_material_code { get; set; }
		public ProtectedMaterialText protected_material_text { get; set; }
		public SelfHarm self_harm { get; set; }
		public Sexual sexual { get; set; }
		public Violence violence { get; set; }
		public Jailbreak jailbreak { get; set; }
	}

	public class Hate
	{
		public bool filtered { get; set; }
		public string severity { get; set; }
	}

	public class Jailbreak
	{
		public bool filtered { get; set; }
		public bool detected { get; set; }
	}

	public class Message
	{
		public string content { get; set; }
		public string role { get; set; }
	}

	public class PromptFilterResult
	{
		public int prompt_index { get; set; }
		public ContentFilterResults content_filter_results { get; set; }
	}

	public class ProtectedMaterialCode
	{
		public bool filtered { get; set; }
		public bool detected { get; set; }
	}

	public class ProtectedMaterialText
	{
		public bool filtered { get; set; }
		public bool detected { get; set; }
	}

	public class PromptResponse
	{
		public List<Choice> choices { get; set; }
		public int created { get; set; }
		public string id { get; set; }
		public string model { get; set; }
		public string @object { get; set; }
		public List<PromptFilterResult> prompt_filter_results { get; set; }
		public string system_fingerprint { get; set; }
		public Usage usage { get; set; }
	}

	public class SelfHarm
	{
		public bool filtered { get; set; }
		public string severity { get; set; }
	}

	public class Sexual
	{
		public bool filtered { get; set; }
		public string severity { get; set; }
	}

	public class Usage
	{
		public int completion_tokens { get; set; }
		public int prompt_tokens { get; set; }
		public int total_tokens { get; set; }
	}

	public class Violence
	{
		public bool filtered { get; set; }
		public string severity { get; set; }
	}


}
