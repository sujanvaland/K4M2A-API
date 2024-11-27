﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using SpiritualNetwork.API.Migrations;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services
{
    public class HashTagService : IHastTagService
    {
        
        private readonly IRepository<UserPost> _userPostRepository;
        private readonly IRepository<HashTag> _hashTagRepository;
        private readonly INotificationService _notificationService;

        public HashTagService(IRepository<UserPost> userPostRepository, IRepository<HashTag> hashTagRepository, INotificationService notificationService)
        {
            _userPostRepository = userPostRepository;
            _hashTagRepository = hashTagRepository;
            _notificationService = notificationService;
        }

        public async Task<JsonResponse> ExtractPostHashTag(int postId)
        {
            var post = await _userPostRepository.Table.Where(x => x.Id == postId && x.IsDeleted == false).FirstOrDefaultAsync();


            if (post != null)
            {

                PostMessageContent postMessageContent = JsonConvert.DeserializeObject<PostMessageContent>(post.PostMessage);

                var options = new RestClientOptions("https://k4m2aai.openai.azure.com")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/openai/deployments/gpt-4/chat/completions?api-version=2024-02-15-preview", Method.Post);
                request.AddHeader("api-key", GlobalVariables.OpenAPIKey);
                request.AddHeader("Content-Type", "application/json");

                string description = postMessageContent.TextMsg ?? "";
                string images = string.Join(", ", postMessageContent.ImgUrl);
                string videos = string.Join(", ", postMessageContent.VideoUrl);

                // Construct the prompt
                string prompt = $@"You are a hashtag generator for social media posts. Extract hashtags from the following text based on its content, key phrases, and context. Provide the hashtags as a comma-separated list. ";
                if (!string.IsNullOrEmpty(description))
                {prompt += $"\n\nDescription: {description}";
                }

                if (postMessageContent.ImgUrl.Any())
                {prompt += $"\n\nImages: {images}";
                }

                if (postMessageContent.VideoUrl.Any())
                {prompt += $"\n\nVideos: {videos}";
                }

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

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<PromptResponse>(response.Content);
                    if (result != null && result.choices != null && result.choices.Count > 0)
                    {
                        var hashtags = result.choices[0].message.content;

                        var hashtagList = hashtags.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        List<string> postHashtags = new List<string>();

                        foreach( var tag in hashtagList)
                        {
                            postHashtags.Add(tag);
                            var checkHashTag = await _hashTagRepository.Table.Where(x => x.Name == tag).FirstOrDefaultAsync();

                            if(checkHashTag == null)
                            {
                                HashTag NewhashTag = new HashTag();
                                NewhashTag.Name = tag;
                                NewhashTag.Count = 1;
                                await _hashTagRepository.InsertAsync(NewhashTag);
                            }
                            else
                            {
                                checkHashTag.Count++;
                                await _hashTagRepository.UpdateAsync(checkHashTag);
                            }
                        }
                        var postMessage = JsonConvert.DeserializeObject<Post>(post.PostMessage);
                        postMessage.hashtag = postHashtags;
                        post.PostMessage = JsonConvert.SerializeObject(postMessage);
                        await _userPostRepository.UpdateAsync(post);


                        NodeAddPost NodePostId = new NodeAddPost();
                        NodePostId.Id = postId;
                        await _notificationService.SendPostToNode(NodePostId);

                        return new JsonResponse(200, true, "Success", hashtags);
                    }
                    else
                    {
                        return new JsonResponse(200, true, "No Valid HashTag Found", null);

                    }
                }
                else
                {
                    return new JsonResponse(200, true, "API call failed", response.Content);
                }
            }
            else
            {
                return new JsonResponse(404, false, "Post not found", null);
            }
        }

    }
}
