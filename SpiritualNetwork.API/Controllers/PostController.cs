using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Net;
using System.Text;
using Twilio.TwiML.Messaging;
using Azure;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Images;
using Azure.Identity;
using OpenAI.Images;
using System.ClientModel;
using System.Text.RegularExpressions;

namespace SpiritualNetwork.API.Controllers
{
   
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ApiBaseController
    {
        private readonly IPostService _postService;
        private readonly RabbitMQService _rabbitMQService;
        private readonly IHastTagService _hashtagService;
        public PostController(IPostService postService, RabbitMQService rabbitMQService, IHastTagService hashtagService)
        {
            _postService = postService;
            _rabbitMQService = rabbitMQService;
            _hashtagService = hashtagService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ExtractHashTag(string text)
        {
            var options = new RestClientOptions("https://k4m2aai.openai.azure.com")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/openai/deployments/gpt-4/chat/completions?api-version=2024-02-15-preview", Method.Post);
            request.AddHeader("api-key", GlobalVariables.OpenAPIKey);
            request.AddHeader("Content-Type", "application/json");

            // Construct the prompt
            string prompt = text; //$@"Generate a social media post with image on Shambhavi mudra meditation";

            // Build the message payload
            var promptRequest = new
            {
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                temperature = 0.7,
                top_p = 0.95,
                max_tokens = 800
            };

            // Add the serialized body
            request.AddStringBody(JsonConvert.SerializeObject(promptRequest), DataFormat.Json);

            // Send the request
            var response = await client.ExecuteAsync(request);
            AIPost aIPost = new AIPost();
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<PromptResponse>(response.Content);
                if (result != null && result.choices != null && result.choices.Count > 0)
                {
					aIPost.Content = result.choices[0].message.content;
				}
                else
                {
                    Console.WriteLine("No valid response from the API.");
                }

				string pattern = @"\[Image: (.*?)\]";
				Match match = Regex.Match(aIPost.Content, pattern);

				if (match.Success)
				{
					string imageDescription = match.Groups[1].Value;
					options = new RestClientOptions("https://k4m2aai.openai.azure.com")
				{
					MaxTimeout = -1,
				};
				client = new RestClient(options);
				request = new RestRequest("/openai/deployments/dall-e-3/images/generations?api-version=2024-02-01", Method.Post);
				request.AddHeader("api-key", GlobalVariables.OpenAPIKey);
				request.AddHeader("Content-Type", "application/json");
					var body = $@"{{
                        ""prompt"": ""{imageDescription}"",
                        ""size"": ""1024x1024"",
                        ""n"": 1
                    }}";
					request.AddStringBody(body, DataFormat.Json);
				response = await client.ExecuteAsync(request);
                
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var imgresult = JsonConvert.DeserializeObject<ImageResponse>(response.Content);
                    if (imgresult != null && imgresult.Data != null)
                    {
                        var url = imgresult.Data.FirstOrDefault()?.Url;
						aIPost.Url = url;
                    }
                    else
                    {
                        Console.WriteLine("No valid response from the API.");
                    }
                }
				}
				else
				{
					Console.WriteLine("No image description found.");
				}
				
			}
            else
            {
                Console.WriteLine($"API call failed: {response.Content}");
            }

            return Ok(aIPost);
        }

        [AllowAnonymous]
        [HttpGet(Name = "hastTag")]
        public async Task<JsonResponse> ExtractPostHashTag(int postId)
        {
            try
            {
                return await _hashtagService.ExtractPostHashTag(postId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }


        [HttpPost(Name = "PostUpload")]
        public async Task<JsonResponse> PostUpload(IFormCollection form)
        {

            try
            {
				// Validate if files were uploaded
				//if (form.Files.Count == 0)
				//{
				//	return new JsonResponse(400, false, "Fail", "No files uploaded.");
				//}

				// Convert form data to DTO
				var postDataDto = new PostDataDto();

				// Populate the form fields
				foreach (var key in form.Keys)
				{
					postDataDto.FormFields[key] = form[key];
				}

				// Handle file uploads
				foreach (var file in form.Files)
				{
					using (var memoryStream = new MemoryStream())
					{
						await file.CopyToAsync(memoryStream);
						var base64Content = Convert.ToBase64String(memoryStream.ToArray());

						// Add file info to the DTO
						postDataDto.Files.Add(new FileDataDto
						{
							FileName = file.FileName,
							Base64Content = base64Content
						});
					}
				}
                postDataDto.Topic = "post";
                postDataDto.UserUniqueId = user_unique_id;
                postDataDto.Username = username;
				// Produce a message
				await KafkaProducer.ProduceMessage("post", postDataDto);
				//var response = await _postService.InsertPost(postDataDto);
				return new JsonResponse(200,true,"Success");
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }


        [HttpPost(Name = "RePost")]
        public async Task<JsonResponse> RePost(ReactionReq req)
        {
            try
            {
                var response = await _postService.RePost(req.PostId, user_unique_id);
                return response;
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetAllPosts")]
        public async Task<JsonResponse> GetAllPosts(GetTimelineReq req)
        {
            try
            {
                return await _postService.GetAllPostsAsync(user_unique_id, req.PageNo, req.ProfileUserId,req.Type);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "UpdateViews")]
        public async Task<JsonResponse> UpdateViews(List<int> req)
        {
            try
            {
                return await _postService.UpdateViews(req);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "GetAllImgVideoLink")]
        public async Task<JsonResponse> GetAllImgVideoLink(GetTimelineReq req)
        {
            try
            {
                return await _postService.GetAllImgVideoLink(user_unique_id, req.PageNo, req.ProfileUserId, req.Type);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetPostById")]
        public async Task<JsonResponse> GetPostById(int postId)
        {
            try
            {
                return await _postService.GetPostById(user_unique_id,postId);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        

        [HttpPost(Name = "DeletePost")]
        public async Task<JsonResponse> DeletePost(DeletePostReq req)
        {
            try
            {
                return await _postService.DeletePostAsync(req.Id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "PostMentionList")]
        public async Task<JsonResponse> PostMentionList(MentionListReq req)
        {
            try
            {
                return await _postService.PostMentionList(req,user_unique_id);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost(Name = "ExtractMetaTags")]
        public async Task<JsonResponse> ExtractMetaTags(ExtractUrlMetaReq req)
        {
            var metaTags = Common.StringHelper.ExtractMetaTags(req.Url);
            return new JsonResponse(200, true, "Success", metaTags);
        }

        [HttpPost(Name = "BlockUnBlockPost")]
        public async Task<JsonResponse> BlockUnBlockPost(BlockUnBlockReq req)
        {
            try
            {
                return await _postService.BlockUnBlockPosts(req.PostId,user_unique_id);
            }
            catch (Exception ex) 
            {
                return new JsonResponse(200,true,"Fail",ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost(Name = "UpdatePost")]
        public JsonResponse UpdatePost()
        {
            try
            {
                _postService.UpdatePost();
                return new JsonResponse(200, true, "Success","");
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, true, "Success", "");
            }
        }

        [HttpGet(Name = "ChangeWhoCanReply")]
        public async Task<JsonResponse> ChangeWhoCanReply(int postId,int whoCanReply)
        {
            try
            {
                var post = await _postService.ChangeWhoCanReply(postId, whoCanReply);
                return new JsonResponse(200, true, "Success", post);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "PinUnpinPost")]
        public async Task<JsonResponse> PinUnpinPost(int postId)
        {
            try
            {
                var post = await _postService.PinUnpinPost(postId,user_unique_id);
                return new JsonResponse(200, true, "Success", post);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "RemoveLocationInfo")]
        public async Task<JsonResponse> RemoveLocationInfo()
        {
            try
            {
                await _postService.RemoveLocationInfo(user_unique_id);
                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost(Name = "ReportPost")]
        public async Task<JsonResponse> ReportPost(Report req)
        {
            try
            {
                await _postService.ReportPost(req, user_unique_id);
                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpGet(Name = "DeleteAllVideoPost")]
        public async Task<JsonResponse> DeleteAllVideoPost()
        {
            try
            {
                await _postService.DeleteAllVideoPost(user_unique_id);
                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }
    }
}
