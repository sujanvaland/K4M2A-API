using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpiritualNetwork.API.AppContext;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Common;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Net.Mail;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Linq;
using Npgsql;

namespace SpiritualNetwork.API.Services
{
    public class PostService : IPostService
    {
        private readonly IAttachmentService _attachmentService;
        private readonly INotificationService _notificationService;
        private readonly IRepository<UserPost> _userPostRepository;
        private IRepository<PostFiles> _postFiles;
        private readonly IRepository<Entities.File> _fileRepository;
        private readonly IRepository<Reaction> _reactionRepository;
        private readonly IRepository<BlockedPosts> _blockedPost;
        private readonly AppDbContext _context;
		//private readonly AppMSDbContext _msdbcontext;
		private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserSubcription> _userSubcriptionRepo;
        private readonly IPollService _pollService;
        private readonly IProfileService _profileService;
        private readonly IEventService _eventService;
        private readonly IGlobalSettingService _globalSettingService;
        private readonly IRepository<ReportEntity> _reportRepository;
        private readonly IRepository<UserInterest> _userInterestRepo;

        public PostService(IAttachmentService attachmentService,
            IRepository<UserSubcription> userSubcriptionRepo,
            INotificationService notificationService,
            IRepository<UserPost> userPostRepository,
            IRepository<PostFiles> postFiles,
            IRepository<Entities.File> filerepository,
            IRepository<Reaction> reactionRepository,
            IRepository<BlockedPosts> blockedPostRepository,
            IRepository<User> userRepository,
            IPollService pollService,
            AppDbContext context,
			//AppMSDbContext msdbcontext,
			IProfileService profileService,
            IEventService eventService,
            IGlobalSettingService globalSettingService,
            IRepository<ReportEntity> reportRepository,
            IRepository<UserInterest> userInterestRepo)
        {
            _blockedPost = blockedPostRepository;
            _userSubcriptionRepo = userSubcriptionRepo;
            _userPostRepository = userPostRepository;
            _notificationService = notificationService;
            _reactionRepository = reactionRepository;
            _fileRepository = filerepository;
            _context = context;
			//_msdbcontext = msdbcontext;
			_attachmentService = attachmentService;
            _userPostRepository = userPostRepository;
            _postFiles = postFiles;
            _userRepository = userRepository;
            _pollService = pollService;
            _profileService = profileService;
            _eventService = eventService;
            _globalSettingService = globalSettingService;
            _reportRepository = reportRepository;
            _userInterestRepo = userInterestRepo;
        }

        public async Task<UserPost> GetUserPostByPostId(int PostId)
        {
            try
            {
                var data = await _userPostRepository.Table.Where(x => x.Id == PostId).FirstOrDefaultAsync();
                return data;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> PostMentionList(MentionListReq req, int loginUserid)
        {
            try 
            {
                List<MentionBoxModel> mentionList = new List<MentionBoxModel>();
                foreach (var member in req.userName)
                {
                    var user = await _profileService.GetUserInfoBox(member, loginUserid);
                    MentionBoxModel infoBox = new MentionBoxModel();
                    infoBox.Id = user.Id;
                    infoBox.FullName = user.FirstName + " " + user.LastName;
                    infoBox.About = user.About == null ? "" : user.About;
                    infoBox.UserName = user.UserName;
                    infoBox.ProfileImgUrl = user.ProfileImg == null ? "" : user.ProfileImg;
                    infoBox.isPremium = user.IsPremium;
                    infoBox.isfollowing = user.IsFollowedByLoginUser;
                    infoBox.isPeer = user.IsFollowingLoginUser;
                    infoBox.IsBusinessAccount = user.IsBusinessAccount;
                    mentionList.Add(infoBox);
                }
                return new JsonResponse(200, true, "Success", mentionList);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> GetAllPostsAsync(int Id, int PageNo, int? ProfileUserId, string? Type)
        {
            try
            {
                if(Type == "community" && ProfileUserId > 0)
                {
                    SqlParameter userparam = new SqlParameter("@UserId", Id);
                    SqlParameter pageparam = new SqlParameter("@PageNo", PageNo);
                    SqlParameter communityIdparam = new SqlParameter("@CommunityId", ProfileUserId);
                    var Result = await _context.PostResponses
                        .FromSqlRaw("GetCommunityTimeLine @UserId,@PageNo,@CommunityId", userparam, pageparam, communityIdparam)
                        .ToListAsync();
                    return new JsonResponse(200, true, "Success", Result);
                }
                if (Type == "communitylatest" && ProfileUserId > 0)
                {
                    SqlParameter userparam = new SqlParameter("@UserId", Id);
                    SqlParameter pageparam = new SqlParameter("@PageNo", PageNo);
                    SqlParameter communityIdparam = new SqlParameter("@CommunityId", ProfileUserId);
                    var Result = await _context.PostResponses
                        .FromSqlRaw("GetLatestCommunityTimeLine @UserId,@PageNo,@CommunityId", userparam, pageparam, communityIdparam)
                        .ToListAsync();
                    return new JsonResponse(200, true, "Success", Result);
                }
                if (ProfileUserId > 0)
                {
					
					var userIdParam = new NpgsqlParameter("@userid", Id);
					var PageNoParam = new NpgsqlParameter("@PageNo", PageNo);
					var ProfileUserIdParam = new NpgsqlParameter("@ProfileUserId", ProfileUserId);

					var result = await _context.PostResponses
								  .FromSqlRaw("SELECT * FROM dbo.getProfileTimeLine(@userid,@PageNo,@ProfileUserId)", userIdParam, PageNoParam, ProfileUserIdParam)
								  .ToListAsync();
					return new JsonResponse(200, true, "Success", result);
                }
                else
                {
                    SqlParameter userparam = new SqlParameter("@UserId", Id);
                    SqlParameter pageparam = new SqlParameter("@PageNo", PageNo);
                    SqlParameter typeparam = new SqlParameter("@Type", Type);

                    var Result = await _context.PostResponses
                        .FromSqlRaw("GetTimeLine @UserId,@PageNo,@Type", userparam, pageparam, typeparam)
                        .ToListAsync();
                    return new JsonResponse(200, true, "Success", Result);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> UpdateViews(List<int> req)
        {
            try
            {
                var data = await _userPostRepository.Table
                .Where(x => req.Contains(x.Id))
                .ToListAsync();

                List<UserPost> list = new List<UserPost>();
                foreach (var item in data)
                {
                    if (!String.IsNullOrEmpty(item.PostMessage))
                    {
                        var postData = JsonSerializer.Deserialize<Post>(item.PostMessage);
                        postData.noOfViews = postData.noOfViews + 1;
                        var postMessageStr = JsonSerializer.Serialize(postData);
                        item.PostMessage = postMessageStr;
                    }
                    list.Add(item);
                }
                await _userPostRepository.UpdateRangeAsync(list);
                return new JsonResponse(200, true, "Success", list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 

        public async Task<JsonResponse> GetAllImgVideoLink(int Id, int PageNo, int? ProfileUserId, string? Type)
        {
            try
            {
                if (Type == "community" && ProfileUserId > 0)
                {
                    SqlParameter userparam = new SqlParameter("@UserId", Id);
                    SqlParameter pageparam = new SqlParameter("@PageNo", PageNo);
                    SqlParameter communityIdparam = new SqlParameter("@CommunityId", ProfileUserId);
                    var Result = await _context.PostResponses
                        .FromSqlRaw("GetCommunityTimeLine @UserId,@PageNo,@CommunityId", userparam, pageparam, communityIdparam)
                        .ToListAsync();
                    List<CommunityMediaModel> urlList = new List<CommunityMediaModel>();

                    foreach (var post in Result)
                    {
                        var postData = JsonSerializer.Deserialize<Post>(post.PostMessage);
                        
                        if(postData.url == "" && postData.imgUrl.Count == 0 && postData.videoUrl.Count == 0)
                        {
                            continue;
                        }

                        if(postData.url != null && postData.url != "")
                        {
                            var YTurl = new CommunityMediaModel();
                            YTurl.PostId = post.Id;
                            YTurl.UserId = post.PostUserId;
                            YTurl.YouTubeUrl = postData.url;
                            YTurl.CommunityId = ProfileUserId;
                            urlList.Add(YTurl);
                        }

                        if(postData.imgUrl != null && postData.imgUrl.Count > 0)
                        {
                            foreach(var img in postData.imgUrl)
                            {
                                var ImgUrl = new CommunityMediaModel();
                                ImgUrl.PostId = post.Id;
                                ImgUrl.UserId = post.PostUserId;
                                ImgUrl.ImgUrl = img;
                                ImgUrl.CommunityId = ProfileUserId;
                                urlList.Add(ImgUrl);
                            }
                        }

                        if(postData.videoUrl != null && postData.videoUrl.Count > 0)
                        {
                            foreach (var video in postData.videoUrl)
                            {
                                var VideoUrl = new CommunityMediaModel();
                                VideoUrl.PostId = post.Id;
                                VideoUrl.UserId = post.PostUserId;
                                VideoUrl.VideoUrl = video;
                                VideoUrl.CommunityId = ProfileUserId;
                                urlList.Add(VideoUrl);
                            }
                        }
                    }

                    return new JsonResponse(200, true, "Success", urlList);
                }

                return new JsonResponse(200, true, "Success", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResponse> GetPostById(int loginUserId,int postId)
        {
            try
            {
                var post = _userPostRepository.GetById(postId);
                if(post.IsDeleted)
                {
                    return new JsonResponse(200, false, "This Post was deleted by the Post Author", null);
                }
                var postIdParam = new NpgsqlParameter("@postId", postId);
                var userIdParam = new NpgsqlParameter("@requserId", loginUserId);
               
                var result = await _context.PostResponses
                              .FromSqlRaw("SELECT * FROM dbo.GetPostById(@postId, @requserId)", postIdParam, userIdParam)
                              .ToListAsync();
                return new JsonResponse(200, true, "Success", result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<JsonResponse> RePost(int PostId, int UserId)
        //{
        //    try
        //    {
        //        int reactionPostId = PostId;
        //        var isChildPost = await _userPostRepository.Table.Where(x => x.Id == PostId && x.ParentId > 0).FirstOrDefaultAsync();
        //        if(isChildPost != null)
        //        {
        //            reactionPostId = (int)isChildPost.ParentId;
        //        }
        //        var Reaction = await _reactionRepository.Table.Where(x=>x.PostId == reactionPostId && x.UserId == UserId && x.Type == "repost").FirstOrDefaultAsync();
        //        if(Reaction == null)
        //        {
        //            var data = await _userPostRepository.Table
        //            .Where(x => x.IsDeleted == false && x.Id == PostId)
        //            .FirstOrDefaultAsync();

        //            var filedata = await _postFiles.Table.Where(x => x.PostId == PostId).ToListAsync();

        //            var permiumcheck = _userSubcriptionRepo.Table.Where(x => x.UserId == UserId &&
        //                              x.PaymentStatus == "completed" && x.IsDeleted == false).FirstOrDefault();


        //            UserPost userPost = new UserPost();
        //            userPost.UserId = UserId;
        //            string type = "repost";
        //            userPost.Type = type;
        //            userPost.ParentId = PostId;
        //            userPost.PostMessage = "";
        //            await _userPostRepository.InsertAsync(userPost);

        //            var postData = JsonSerializer.Deserialize<Post>(data.PostMessage);
        //            postData.type = type;
        //            postData.id = userPost.Id;
        //            postData.noOfRepost += 1;
        //            if (permiumcheck != null)
        //            {
        //                postData.isPaid = true;
        //            }
        //            else
        //            {
        //                postData.isPaid = false;
        //            }
        //            userPost.PostMessage = JsonSerializer.Serialize(postData);
        //            await _userPostRepository.UpdateAsync(userPost);

        //            List<PostFiles> postFiles = new List<PostFiles>();

        //            foreach (var item in filedata)
        //            {
        //                PostFiles pf = new PostFiles();
        //                pf.PostId = userPost.Id;
        //                pf.FileId = item.FileId;

        //                postFiles.Add(pf);
        //            }

        //            await _postFiles.InsertRangeAsync(postFiles);

        //            if (type == "repost")
        //            {
        //                Reaction reaction = new Reaction();
        //                reaction.PostId = PostId;
        //                reaction.UserId = UserId;
        //                reaction.Type = "repost";
        //                await _reactionRepository.InsertAsync(reaction);
        //                await UpdateCount(PostId, "repost", 1);

        //                NotificationRes notification = new NotificationRes();
        //                notification.PostId = PostId;
        //                notification.ActionByUserId = UserId;
        //                notification.ActionType = "repost";
        //                notification.RefId1 = data.UserId.ToString();
        //                notification.RefId2 = "";
        //                notification.Message = "";
        //                await _notificationService.SaveNotification(notification);
        //            }

        //            return new JsonResponse(200, true, "Success", userPost);
        //        }
        //        else
        //        {
        //            int postToDelete = PostId;
        //            if (isChildPost == null)
        //            {
        //                //if it is orignal post then delete child post
        //                var data = await _userPostRepository.Table
        //                .Where(x => x.IsDeleted == false && x.ParentId == PostId && x.UserId == UserId)
        //                .FirstOrDefaultAsync();

        //                var filedata = await _postFiles.Table.Where(x => x.PostId == data.Id).ToListAsync();

        //                _userPostRepository.DeleteHard(data);
        //                _postFiles.DeleteHardRange(filedata);
        //                await UpdateCount(PostId, "repost", 0);
        //            }
        //            else
        //            {
        //                //if it is reposted post then delete that post
        //                var data = await _userPostRepository.Table
        //                .Where(x => x.IsDeleted == false && x.Id == PostId)
        //                .FirstOrDefaultAsync();

        //                var filedata = await _postFiles.Table.Where(x => x.PostId == data.Id).ToListAsync();

        //                _userPostRepository.DeleteHard(data);
        //                _postFiles.DeleteHardRange(filedata);
        //                await UpdateCount((int)isChildPost.ParentId, "repost", 0);
        //            }
        //            _reactionRepository.DeleteHard(Reaction);

                    

        //            return new JsonResponse(200, true, "Success", Reaction);
        //        }

               
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<JsonResponse> RePost(int PostId, int UserId)
        {
            var UserReaction = _reactionRepository.Table.Where(x=>x.PostId == PostId && x.UserId == UserId
            && x.Type == "repost").FirstOrDefault();
            
            if (UserReaction == null)
            {
                var newPostCount = await UpdateRepostCount(PostId, 1);
                var data = await _userPostRepository.Table
                .Where(x => x.IsDeleted == false && x.Id == PostId)
                .FirstOrDefaultAsync();

                var filedata = await _postFiles.Table.Where(x => x.PostId == PostId).ToListAsync();

                UserPost userPost = new UserPost();
                userPost.UserId = UserId;
                userPost.Type = "repost";
                userPost.ParentId = PostId;
                userPost.PostMessage = data.PostMessage;
                await _userPostRepository.InsertAsync(userPost);

                var newRepostPost = await _userPostRepository.Table
               .Where(x => x.IsDeleted == false && x.Id == userPost.Id)
               .FirstOrDefaultAsync();

                var postData = JsonSerializer.Deserialize<Post>(newRepostPost.PostMessage);
                postData.id = userPost.Id;
                postData.noOfRepost = 0;
                newRepostPost.PostMessage = JsonSerializer.Serialize(postData);
                await _userPostRepository.UpdateAsync(newRepostPost);

                List<PostFiles> postFiles = new List<PostFiles>();
                foreach (var item in filedata)
                {
                    PostFiles pf = new PostFiles();
                    pf.PostId = userPost.Id;
                    pf.FileId = item.FileId;
                    postFiles.Add(pf);
                }

                await _postFiles.InsertRangeAsync(postFiles);
                Reaction reaction = new Reaction();
                reaction.PostId = PostId;
                reaction.UserId = UserId;
                reaction.Type = "repost";
                await _reactionRepository.InsertAsync(reaction);
                

                NotificationRes notification = new NotificationRes();
                notification.PostId = PostId;
                notification.ActionByUserId = UserId;
                notification.ActionType = "repost";
                notification.RefId1 = data.UserId.ToString();
                notification.RefId2 = "";
                notification.Message = "";
                await _notificationService.SaveNotification(notification);
                   
                return new JsonResponse(200, true, "Success", userPost);
            }
            else
            {
                _reactionRepository.DeleteHard(UserReaction);

                var childPost = await _userPostRepository.Table
                        .Where(x => x.IsDeleted == false && x.Id == PostId && x.ParentId > 0 && x.UserId == UserId)
                        .FirstOrDefaultAsync();

                if(childPost != null)
                {
                    var filedata = await _postFiles.Table.Where(x => x.PostId == childPost.Id).ToListAsync();
                    _userPostRepository.DeleteHard(childPost);
                    _postFiles.DeleteHardRange(filedata);
                }
               
                await UpdateRepostCount((int)childPost.ParentId, 0);
                return new JsonResponse(200, true, "Success", childPost);
            }
        }
        public async Task<JsonResponse> UpdateCount(int PostId, string Type, int dir)
        {
            var data = await _userPostRepository.Table
                .Where(x => x.Id == PostId)
                .FirstOrDefaultAsync();

            var postData = JsonSerializer.Deserialize<Post>(data.PostMessage);

            if (Type == "like")
            {
                if(dir == 0)
                {
                    postData.noOfLikes -= postData.noOfLikes > 0 ? 1 : 0;
                }
                else if(dir == 1)
                {
                    postData.noOfLikes += 1;
                }
            }
            if(Type == "repost")
            {
                if (dir == 0 && postData.noOfRepost > 0)
                {
                    postData.noOfRepost -= 1;
                }
                else if(dir == 1)
                {
                    postData.noOfRepost += 1;
                }
            }
            if (Type == "comment")
            {
                if (dir == 0)
                {
                    postData.noOfComment -= postData.noOfComment > 0 ? 1 : 0;
                }
                else if (dir == 1)
                {
                    postData.noOfComment += 1;
                }
            }
            data.PostMessage = JsonSerializer.Serialize(postData);

            await _userPostRepository.UpdateAsync(data);

            return new JsonResponse(200, true, "Success", data);
        }

        public async Task<int> UpdateRepostCount(int PostId,int dir)
        {
            var data = await _userPostRepository.Table
                .Where(x => x.Id == PostId)
                .FirstOrDefaultAsync();

            var postData = JsonSerializer.Deserialize<Post>(data.PostMessage);

            if (dir == 0 && postData.noOfRepost > 0)
            {
                postData.noOfRepost -= 1;
            }
            else if (dir == 1)
            {
                postData.noOfRepost += 1;
            }
            data.PostMessage = JsonSerializer.Serialize(postData);

            await _userPostRepository.UpdateAsync(data);

            return (int)postData.noOfRepost;
        }

        public async Task<JsonResponse> InsertPost(PostDataDto postDataDto)
        {
            try
            {
                var user = _userRepository.GetById(postDataDto.UserUniqueId);

                var permiumcheck = _userSubcriptionRepo.Table.Where(x => x.UserId == postDataDto.UserUniqueId &&
                                   x.PaymentStatus == "completed" && x.IsDeleted == false).FirstOrDefault();
                var str = postDataDto.FormFields.ToList()[0].Value;
                var postData = JsonSerializer.Deserialize<Post>(str);
                if(postData == null)
                    return new JsonResponse(200, false, "Fail", "Bad Request");

                int pollId = 0;
                if (postData.poll != null )
                {
                    var poll = new Poll();
                    var polldata = JsonSerializer.Deserialize<PollRequest>(postData.poll);
                    poll.PollTitle = postData.textMsg;
                    poll.Choice1 = polldata.choice1;
                    poll.Choice2 = polldata.choice2;
                    poll.Choice3 = polldata.choice3;
                    poll.Choice4 = polldata.choice4;
                    poll.Day = Convert.ToInt32(polldata.day);
                    poll.Hour = Convert.ToInt32(polldata.hour);
                    poll.Minute = Convert.ToInt32(polldata.minute);
                    poll.CreatedBy = Convert.ToInt32(polldata.createdBy);
                    var pollresult = await _pollService.SavePoll(poll);
                    postData.pollId = pollresult.Id;
                    postData.poll = null;
                }

                UserPost userPost = new UserPost();
                userPost.ParentId = postData.parentId;
                userPost.UserId = user.Id;
                userPost.PostMessage = "";
                userPost.Type = postData.type;
                userPost.Latitude = postData.latitude;
                userPost.Longitude = postData.longitude;
                userPost.IsVideo = postData.videoUrl.Count > 0;
                await _userPostRepository.InsertAsync(userPost);

                if (permiumcheck != null)
                {
                    postData.isPaid = true;
                }
                else { postData.isPaid = false; }

                postData.id = userPost.Id;
                postData.createdBy = user.FirstName + " " + user.LastName;
                postData.userName = user.UserName;
                postData.profileImg = user.ProfileImg;
                postData.noOfComment = 0;
                postData.noOfLikes = 0;
                postData.noOfRepost = 0;
                postData.noOfViews = 0;
                postData.createdOn = DateTime.UtcNow.ToString();
                UploadPostResponse uploadPostResponse = new UploadPostResponse();
                uploadPostResponse.Post = userPost;

                if (postData.type == "comment")
                {
                    var parentPost = _userPostRepository.GetById(postData.parentId);
                    var postMessage = JsonSerializer.Deserialize<Post>(parentPost.PostMessage);
                    postMessage.noOfComment += 1;
                    var postMessageStr = JsonSerializer.Serialize(postMessage);
                    parentPost.PostMessage = postMessageStr;
                    _userPostRepository.Update(parentPost);
                }
                if (postDataDto.Files.Count == 0)
                {
                    userPost.PostMessage = JsonSerializer.Serialize(postData);
                    await _userPostRepository.UpdateAsync(userPost);
                }

				// Define maximum file size (e.g., 10 MB)
				const long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB

				if (postDataDto.Files.Count > 0)
				{
					List<IFormFile> formFiles = new List<IFormFile>();

					// Iterate through postDataDto.Files (Base64 encoded)
					foreach (var item in postDataDto.Files)
					{
						// Filter based on file type extension
						if (!(item.FileName.ToLower().EndsWith(".mp4") || item.FileName.ToLower().EndsWith(".avi")
							|| item.FileName.ToLower().EndsWith(".mov") || item.FileName.ToLower().EndsWith(".wmv")
							|| item.FileName.ToLower().EndsWith(".flv") || item.FileName.ToLower().EndsWith(".mkv")
							|| item.FileName.ToLower().EndsWith(".webm") || item.FileName.ToLower().EndsWith(".mpeg")
							|| item.FileName.ToLower().EndsWith(".mpg") || item.FileName.ToLower().EndsWith(".3gp")))
						{
							// Convert Base64 back to byte array
							byte[] fileBytes = Convert.FromBase64String(item.Base64Content);

							// Validate file size
							if (fileBytes.Length > MaxFileSizeInBytes)
							{
								throw new Exception($"The file {item.FileName} exceeds the maximum allowed size of {MaxFileSizeInBytes / (1024 * 1024)} MB.");
							}
							// Create a stream from the byte array
							var stream = new MemoryStream(fileBytes);

							// Create an IFormFile instance
							var formFile = new FormFile(stream, 0, fileBytes.Length, item.FileName, item.FileName)
                            {
                                Headers = new HeaderDictionary()
                            };
                            // Set the correct content type based on the file extension
                            formFile.ContentType = GetContentType(item.FileName);
							formFiles.Add(formFile);
						}
					}

					// Insert attachments (files)
					var uploadedFiles = await _attachmentService.InsertAttachment(formFiles);
					uploadPostResponse.Files = uploadedFiles;

					List<PostFiles> postFiles = new List<PostFiles>();
					postData.imgUrl = new List<string>();
					postData.pdfUrl = new List<string>();
					postData.thumbnailUrl = new List<string>();

					bool hasImageFile = false;
					bool hasVideoFile = false;

					// Process uploaded files
					foreach (var item in uploadedFiles)
					{
						PostFiles post = new PostFiles
						{
							PostId = userPost.Id,
							FileId = item.Id
						};
						postFiles.Add(post);

						// Handling images
						if (item.FileExtension.ToLower() == ".jpg" || item.FileExtension.ToLower() == ".jpeg"
							|| item.FileExtension.ToLower() == ".png" || item.FileExtension.ToLower() == ".gif"
							|| item.FileExtension.ToLower() == ".svg" || item.FileExtension.ToLower() == ".webp"
							|| item.FileExtension.ToLower() == ".bmp" || item.FileExtension.ToLower() == ".tiff")
						{
							hasImageFile = true;
							postData.imgUrl.Add(item.ActualUrl);
							postData.thumbnailUrl.Add(item.ThumbnailUrl);
						}

						// Handling PDFs
						if (item.FileExtension.ToLower() == ".pdf")
						{
							postData.pdfUrl.Add(item.ActualUrl);
						}

						// If you want to handle videos in the future, uncomment and handle them:
						/*
						if (item.FileExtension.ToLower() == ".mp4" || item.FileExtension.ToLower() == ".avi"
							|| item.FileExtension.ToLower() == ".mov" || item.FileExtension.ToLower() == ".wmv"
							|| item.FileExtension.ToLower() == ".flv" || item.FileExtension.ToLower() == ".mkv"
							|| item.FileExtension.ToLower() == ".webm" || item.FileExtension.ToLower() == ".mpeg"
							|| item.FileExtension.ToLower() == ".mpg" || item.FileExtension.ToLower() == ".3gp")
						{
							hasVideoFile = true;
							postData.videoUrl.Add(item.ActualUrl);
						}
						*/
					}

					// Save PostFiles to DB
					await _postFiles.InsertRangeAsync(postFiles);

					// Set IsVideo based on whether video files were uploaded
					userPost.IsVideo = postData.videoUrl.Count > 0;

					// Serialize PostMessage
					userPost.PostMessage = JsonSerializer.Serialize(postData);

					// Update the post
					await _userPostRepository.UpdateAsync(userPost);
				}
				else
				{
					uploadPostResponse.Files = new List<Entities.File>();
				}

                try
                {
                    NotificationRes notification = new NotificationRes();
                    notification.PostId = postData.id;
                    notification.ActionByUserId = user.Id;
                    notification.ActionType = postData.type;
                    notification.RefId1 = postData.parentId.ToString();
                    notification.RefId2 = "";
                    notification.Message = "";
                    notification.PushAttribute = postData.type == "post" ? "pushpostfollowing" : "pushcommentpost";
                    notification.EmailAttribute = postData.type == "post" ? "emailpostfollowing" : "emailcommentpost";
                    await _notificationService.SaveNotification(notification);
                }
                catch (Exception ex)
                {
                    //log to db
                }

                var message = (new
                {
                    PostId = uploadPostResponse.Post.Id,
                    UserUniqueId = postDataDto.UserUniqueId,
                    Topic = "hashtag"
                });
                
                await KafkaProducer.ProduceMessage("hashtag", message);

                //await KafkaProducer.ProduceMessage("hashtag", uploadPostResponse);

                return new JsonResponse(200, true, "Success", uploadPostResponse);

            }
            catch (Exception ex)
            {
				return new JsonResponse(500, false, "Fail", ex.Message);
			}

        }

		// Function to get MIME type based on file extension
		private string GetContentType(string fileName)
		{
			var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
			return extension switch
			{
				".jpg" => "image/jpeg",
				".jpeg" => "image/jpeg",
				".png" => "image/png",
				".gif" => "image/gif",
				".pdf" => "application/pdf",
				".doc" => "application/msword",
				".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
				_ => "application/octet-stream", // Default MIME type if unknown
			};
		}

		public async Task<JsonResponse> DeletePostAsync(int PostId)
        {
            try
            {
                var userpost = await _userPostRepository.GetByIdAsync(PostId);

                var postfile = await _postFiles.Table
                    .Where(x => x.IsDeleted == false && x.PostId == PostId)
                    .ToListAsync();

                var reactions = await _reactionRepository.Table
                    .Where(x => x.IsDeleted ==false && x.PostId == PostId)
                    .ToListAsync();  

                if (userpost != null)
                {
                    if (userpost.Type == "comment" && userpost.ParentId > 0 && userpost.ParentId.HasValue)
                    {
                        await UpdateCount(userpost.ParentId.Value, "comment", 0);
                       
                        if (userpost.ParentId.HasValue)
                        {
                            NodeAddPost NodePostId = new NodeAddPost();
                            NodePostId.Id = userpost.ParentId.Value;
                            await _notificationService.SendPostToNode(NodePostId);
                        }
                    }

                    await _userPostRepository.DeleteAsync(userpost);
                    _postFiles.DeleteHardRange(postfile);
                    _reactionRepository.DeleteHardRange(reactions);
                    // await _postFiles.DeleteRangeAsync(postfile);
                    //await _reactionRepository.DeleteRangeAsync(reactions);
                    return new JsonResponse(200, true, "Success",userpost);
                }

                return new JsonResponse(200, true, "Fail", null);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> BlockUnBlockPosts(int PostId, int UserId)
        {
            var data = await _blockedPost.Table
                .Where(post => post.UserId == UserId && post.PostId == PostId)
                .FirstOrDefaultAsync();

            if(data == null)
            {
                BlockedPosts blockedPosts = new BlockedPosts();
                blockedPosts.UserId = UserId;
                blockedPosts.PostId = PostId;
                await _blockedPost.InsertAsync(blockedPosts);
                return new JsonResponse(200,true,"Success",blockedPosts);
            }
            else
            {
                _blockedPost.DeleteHard(data);
                return new JsonResponse(200,true,"Success",data);
            }
        }

        public async Task<JsonResponse> UserPostInterest(PostInterestModel req, int LoginUserId)
        {
            var data = await _userInterestRepo.Table.Where(x=> x.PostId == req.PostId
                        && x.UserId == LoginUserId && x.PostUserId == req.PostUserId).FirstOrDefaultAsync();

            if (data == null)
            {
                var post = await _userPostRepository.Table.Where(x=> x.Id == req.PostId).FirstOrDefaultAsync();
                if (post == null)
                {
                    return new JsonResponse(200, false, "Post not found");
                }

                var postMessage = JsonSerializer.Deserialize<Post>(post.PostMessage);

                if (postMessage == null)
                {
                    return new JsonResponse(200, false, "PostMessage deserialization failed ");
                }

                UserInterest userInterest = new UserInterest();
                userInterest.PostUserId = req.PostUserId;
                userInterest.UserId = LoginUserId;
                userInterest.PostId = req.PostId;
                userInterest.ActionType = req.ActionType;
                userInterest.PostHashTag = (postMessage.hashtag != null && postMessage.hashtag.Count > 0) ? JsonSerializer.Serialize(postMessage.hashtag) : " "; 
                await _userInterestRepo.InsertAsync(userInterest);
                return new JsonResponse(200, true, "Success", userInterest);
            }
            else
            {
                data.ActionType = req.ActionType;
                await _userInterestRepo.UpdateAsync(data);
                return new JsonResponse(200, true, "Success", data);
            }
        }


        public void UpdatePost()
        {
            var data = _userPostRepository.Table.ToList();
            List<UserPost> posts = new List<UserPost>();
            foreach (var postItem in data)
            {
                var user = _userRepository.GetById(postItem.UserId);
                var permiumcheck =  _userSubcriptionRepo.Table.Where(x => x.UserId == postItem.UserId &&
                                    x.PaymentStatus == "completed" && x.IsDeleted == false).FirstOrDefault();
                var postMessage = JsonSerializer.Deserialize<Post>(postItem.PostMessage);
                postMessage.createdBy = user.FirstName + " " + user.LastName;
                postMessage.userName = user.UserName;
                postMessage.profileImg = user.ProfileImg;
                postMessage.type = postItem.Type;
                if (permiumcheck != null)
                {
                    postMessage.isPaid = true;
                }
                else { postMessage.isPaid = false; }
                postItem.PostMessage = JsonSerializer.Serialize(postMessage);
                posts.Add(postItem);
            }
            _userPostRepository.UpdateRange(posts);
        }

        public async Task<UserPost> ChangeWhoCanReply(int postId,int whoCanReply)
        {
            var data = await _userPostRepository.Table
                .Where(x => x.Id == postId)
                .FirstOrDefaultAsync();

            var postData = JsonSerializer.Deserialize<Post>(data.PostMessage);
            postData.whoCanReply = whoCanReply;
            var postMessageStr = JsonSerializer.Serialize(postData);
            data.PostMessage = postMessageStr;
            await _userPostRepository.UpdateAsync(data);
            return data;
        }

        public async Task<Reaction> PinUnpinPost(int postId, int UserId)
        {
            var data = await _reactionRepository.Table
                .Where(x => x.Type == "pin" && x.UserId == UserId)
                .FirstOrDefaultAsync();

            if(data != null)
            {
                if (data.PostId != postId)
                {
                    data.PostId = postId;
                    data.UserId = UserId;
                    _reactionRepository.Update(data);
                }
                else if (data.PostId == postId)
                {
                    _reactionRepository.DeleteHard(data);
                }
            }
            else
            {
                Reaction reaction = new Reaction();
                reaction.PostId = postId;
                reaction.UserId = UserId;
                reaction.Type = "pin";
                await _reactionRepository.InsertAsync(reaction);
            }
            
            return data;
        }

        public async Task RemoveLocationInfo(int UserId)
        {
            var data = await _userPostRepository.Table
                .Where(x => x.UserId == UserId)
                .ToListAsync();
            List<UserPost> list = new List<UserPost>();
            foreach (var item in data)
            {
                item.Latitude = "";
                item.Longitude = "";
                if (!String.IsNullOrEmpty(item.PostMessage))
                {
                    var postData = JsonSerializer.Deserialize<Post>(item.PostMessage);
                    postData.latitude = "";
                    postData.longitude = "";
                    var postMessageStr = JsonSerializer.Serialize(postData);
                    item.PostMessage = postMessageStr;
                }
                list.Add(item);
            }
            await _userPostRepository.UpdateRangeAsync(list);
        }

        public async Task DeleteAllVideoPost(int UserId)
        {
            var data = await _userPostRepository.Table
                .ToListAsync();
            List<UserPost> list = new List<UserPost>();
            foreach (var item in data)
            {
                if (!String.IsNullOrEmpty(item.PostMessage))
                {
                    var postData = JsonSerializer.Deserialize<Post>(item.PostMessage);
                    if(postData.videoUrl.Count() > 0)
                    {
                        list.Add(item);
                    }
                }
                
            }
            _userPostRepository.DeleteHardRange(list);
        }

        public void UpdatePost(List<string> hasttag, int postId)
        {
            var data = _userPostRepository.Table.ToList();
            List<UserPost> posts = new List<UserPost>();
            foreach (var postItem in data)
            {
                var user = _userRepository.GetById(postItem.UserId);
                var permiumcheck = _userSubcriptionRepo.Table.Where(x => x.UserId == postItem.UserId &&
                                    x.PaymentStatus == "completed" && x.IsDeleted == false).FirstOrDefault();
                var postMessage = JsonSerializer.Deserialize<Post>(postItem.PostMessage);
                postMessage.createdBy = user.FirstName + " " + user.LastName;
                postMessage.userName = user.UserName;
                postMessage.profileImg = user.ProfileImg;
                postMessage.type = postItem.Type;
                if (permiumcheck != null)
                {
                    postMessage.isPaid = true;
                }
                else { postMessage.isPaid = false; }
                postItem.PostMessage = JsonSerializer.Serialize(postMessage);
                posts.Add(postItem);
            }
            _userPostRepository.UpdateRange(posts);
        }


        public async Task ReportPost(Report req,int UserId)
        {
            var user = _userRepository.Table.Where(x => x.Id == UserId).FirstOrDefault();

            ReportEntity report = new ReportEntity();
            report.ActionUserId = UserId;
            report.ReportId = req.ReportId;
            report.ReportType = req.Type;
            report.Value = req.Value;
            report.Description = req.Content;
            await _reportRepository.InsertAsync(report);

            EmailRequest emailRequest = new EmailRequest();
            emailRequest.USERNAME = "Admin";

            if (req.Type == "post")
            {
                emailRequest.CONTENT1 = user.UserName + " Reported below post";
                emailRequest.CONTENT2 = "Reason: " + req.ReportPost.Value + "<br/>" + req.Content + "<br/>" + req.ReportPost.PostURl;
                emailRequest.CTATEXT = "";
                emailRequest.ToEmail = "anktkania2703@gmail.com";
                emailRequest.Subject = "Post Reported " + GlobalVariables.SiteName;
            }
            if(req.Type == "conversation")
            {
                emailRequest.CONTENT1 = user.UserName + " Reported below Conversation";
                emailRequest.CONTENT2 = "Reason: It's" + req.ReportConversation.Value + " Conversation. " +
                                        "<br/> Report To Details: <br/> UserId : " + req.ReportConversation.UserId+"," +
                                        "IsGroup "+ req.ReportConversation.IsGroup + " <br/>" + req.Content + "<br/>";
                emailRequest.CTATEXT = "";
                emailRequest.ToEmail = "anktkania2703@gmail.com";
                emailRequest.Subject = "Conversation Reported " + GlobalVariables.SiteName;
            }

            if (req.Type == "event")
            {
                emailRequest.CONTENT1 = user.UserName + " Reported below Event";
                emailRequest.CONTENT2 = "Reason: It's" + req.Value + " Event. " +
                                        "<br/> Report To Details: <br/> EventId : " + req.ReportId + "," +
                                        " <br/>" + req.Content + "<br/>";
                emailRequest.CTATEXT = "";
                emailRequest.ToEmail = "anktkania2703@gmail.com";
                emailRequest.Subject = "Event Reported " + GlobalVariables.SiteName;
            }

            SMTPDetails smtpDetails = new SMTPDetails();
            smtpDetails.Username = GlobalVariables.SMTPUsername;
            smtpDetails.Host = GlobalVariables.SMTPHost;
            smtpDetails.Password = GlobalVariables.SMTPPassword;
            smtpDetails.Port = GlobalVariables.SMTPPort;
            smtpDetails.SSLEnable = GlobalVariables.SSLEnable;
            var body = EmailHelper.SendEmailRequest(emailRequest, smtpDetails);

        }


        public void MigratePost()
        {
			//var data = _msdbcontext.UserInterest.ToList();
			//         _context.UserInterest.AddRange(data);
			//         _context.SaveChanges();

			//         var data1 = _msdbcontext.UserMuteBlockLists.ToList();
			//         _context.UserMuteBlockLists.AddRange(data1);
			//         _context.SaveChanges();

			//         var data2 = _msdbcontext.UserNetworks.ToList();
			//         _context.UserNetworks.AddRange(data2);
			//         _context.SaveChanges();

			//var data3 = _msdbcontext.UserNotification.ToList();
			//_context.UserNotification.AddRange(data3);
			//_context.SaveChanges();

			//var data4 = _msdbcontext.UserProfileSuggestion.ToList();
			//_context.UserProfileSuggestion.AddRange(data4);
			//_context.SaveChanges();

			//var data5 = _msdbcontext.UserSubcription.ToList();
			//_context.UserSubcription.AddRange(data5);
			//_context.SaveChanges();
		}

    }
}
