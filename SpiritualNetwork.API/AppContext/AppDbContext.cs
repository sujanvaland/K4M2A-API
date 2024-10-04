using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using static SpiritualNetwork.API.Model.TimelineModel;

namespace SpiritualNetwork.API.AppContext
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public DbSet<Invitation> Invitation {  get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequest { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }
        public DbSet<GlobalSetting> GlobalSetting { get; set; }
        public DbSet<PreRegisteredUser> PreRegisteredUsers { get; set; }
        public DbSet<OnBoardingQuestion> Question { get; set; }
        public DbSet<UserAnswers> UserAnswers { get; set; }
        public DbSet<AnswerOption> AnswerOption { get; set; }
        public DbSet<UserPost> UserPosts { get; set; }
        public DbSet<PostFiles> PostFiles { get; set; }
        public DbSet<Entities.File> Files { get; set; }
        public DbSet<UserFeed> UserFeeds { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Reaction> Reaction { get; set; }
        public DbSet<Model.TimelineModel.PostResponse> PostResponses { get; set; }
        public DbSet<Model.TimelineModel.CommentReposne> CommentResponses { get; set; }
        public DbSet<Model.TimelineModel.UserChatResponse> UserChatResponse { get; set; }
        public DbSet<ReactionResponse> Reactions { get; set; }
        public DbSet<UserFollowers> UserFollowers { get; set; }
        public DbSet<OnlineUsers> OnlineUsers { get; set; }
        public DbSet<ChatMessages> ChatMessages {  get; set; }
        public DbSet<UserMuteBlockList> UserMuteBlockLists { get; set; }
        public DbSet<BlockedPosts> BlockedPosts { get; set; }
        public DbSet<Books> Book { get; set; }
        public DbSet<Movies> Movie { get; set; }
        public DbSet<Gurus> Guru { get; set; }
        public DbSet<Practices> Practice { get; set; }
        public DbSet<Experience> Experience { get; set; }
        public DbSet<UserProfileSuggestion> UserProfileSuggestion { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<UserNotification> UserNotification { get; set; }
        public DbSet<UserSubcription> UserSubcription { get; set; }
        public DbSet<MessageGroupDetails> MessageGroupDetail { get; set; }
        public DbSet<GroupMember> GroupMember { get; set; }
        public DbSet<SnoozeUser> SnoozeUser { get; set; }
        public DbSet<UserNetwork> UserNetworks { get; set; }
        public DbSet<UserAttribute> UserAttribute { get; set; }
        public DbSet<Poll> Poll { get; set; }
        public DbSet<PollVote> PollVote { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<EventType> EventType { get; set; }
        public DbSet<EventSpeakers> EventSpeakers { get; set; }
        public DbSet<EventAttendee> EventAttendee { get; set; }
        public DbSet<EventComment> EventComments { get; set; }
        public DbSet<Community> Community { get; set; }
        public DbSet<CommunityMember> CommunityMembers { get; set; }
        public DbSet<CommunityRules> CommunityRules { get; set; }
        public DbSet<CommunityReportPost> CommunityReportPost { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceFAQ> ServicesFAQ { get; set; }
		public DbSet<EmailVerificationRequest> EmailOTPRequest { get; set; }
		public DbSet<ServiceImages> ServicesImages { get; set; }
		public DbSet<ReportEntity> Reports { get; set; }
		public DbSet<Tags> Tags { get; set; }
		public DbSet<DeviceToken> DeviceToken { get; set; }

		//public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<PostResponse>()
                .HasNoKey()
                .ToTable("PostResponse", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<CommentReposne>()
                .HasNoKey()
                .ToTable("CommentReposne", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<ReactionResponse>()
                .HasNoKey()
                .ToTable("ReactionResponse", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<UserChatResponse>()
                .HasNoKey()
                .ToTable("UserChatResponse", t => t.ExcludeFromMigrations());

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("Default"));
        }

    }
}
