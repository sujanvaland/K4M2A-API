//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Indexing.v3;
//using Google.Apis.Indexing.v3.Data;
//using Google.Apis.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SpiritualNetwork.API.Services
{
    public class GoogleIndexingService
    {
        //private readonly IndexingService _indexingService;

        //public GoogleIndexingService()
        //{
        //    GoogleCredential credential;
        //    using (var stream = new FileStream("your-service-account.json", FileMode.Open, FileAccess.Read))
        //    {
        //        credential = GoogleCredential.FromStream(stream)
        //            .CreateScoped(IndexingService.Scope.Indexing);
        //    }

        //    _indexingService = new IndexingService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential
        //    });
        //}

        //public async Task NotifyGoogle(string url)
        //{
        //    var request = new UrlNotification
        //    {
        //        Url = url,
        //        Type = "URL_UPDATED"
        //    };

        //    await _indexingService.UrlNotifications.Publish(request).ExecuteAsync();
        //}
    }
}
