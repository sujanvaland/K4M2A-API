using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities.CommonModel;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace SpiritualNetwork.API.Services
{
    public class ImageService : IImageService
    {
        public Task<JsonResponse> GetThumbNail(IFormFile file)
        {
            throw new NotImplementedException();
        }

        /*
public Task<JsonResponse> GetThumbNail(IFormFile file)
{
   using (var image = Image(file.OpenReadStream()))
   {
       int width = 500;
       int height = 500;

       // Calculate the aspect ratio
       float aspectRatio = (float)image.Width / image.Height;

       int newWidth = width;
       int newHeight = height;

       // Calculate new dimensions while maintaining aspect ratio
       if (image.Width > image.Height)
       {
           newHeight = (int)(height / aspectRatio);
       }
       else
       {
           newWidth = (int)(width * aspectRatio);
       }

       using (var thumbnail = new Bitmap(newWidth, newHeight))
       using (var graphic = Graphics.FromImage(thumbnail))
       {
           graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
           graphic.SmoothingMode = SmoothingMode.HighQuality;
           graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
           graphic.CompositingQuality = CompositingQuality.HighQuality;

           graphic.DrawImage(image, 0, 0, newWidth, newHeight);

           using (var stream = new MemoryStream())
           {
               // Save the thumbnail to a MemoryStream
               thumbnail.Save(stream, image.RawFormat);
               // Check the size of the thumbnail
               if (stream.Length > 30000) // Check if it exceeds 30KB (30,000 bytes)
               {
                   // If the thumbnail size is larger than 30KB, adjust quality or perform additional resizing
                   // This might affect image quality
                   // You can experiment with different quality levels to reduce the file size
                   thumbnail.Save(stream, image.RawFormat);
               }
               return stream.ToArray();
           }
       }
   }
   throw new NotImplementedException();
}

*/
        public Task<JsonResponse> GetThumbnailFile(byte[] bytearr)
        {
            throw new NotImplementedException();
        }
    }
}


