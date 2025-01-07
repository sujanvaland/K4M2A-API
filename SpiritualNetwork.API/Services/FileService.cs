using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Net;

namespace SpiritualNetwork.API.Services
{
    public class FileService : IFileService
    {
        private IRepository<Entities.File> _fileRepository;
        private IRepository<PostFiles> _postFiles;
        private IWebHostEnvironment _webHostEnvironment;

        public FileService(
            IRepository<Entities.File> attachmentRepository,
            IRepository<PostFiles> postFiles,
            IWebHostEnvironment webHostEnvironment)
        {
            _fileRepository = attachmentRepository;
            _postFiles = postFiles;
            _webHostEnvironment = webHostEnvironment;
        }

        private List<string> SaveFileToImagesFolder(byte[] fileContents, string filename)
        {
            string rootPath = _webHostEnvironment.ContentRootPath;
            string imagesFolderPath = System.IO.Path.Combine(rootPath, "posts/images");

            // Ensure 'images' directory exists, if not create it
            Directory.CreateDirectory(imagesFolderPath);

            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // Unique timestamp
            string tempfilename = "K4M2A_" + timestamp;
            string filePath = System.IO.Path.Combine(imagesFolderPath, tempfilename); // Your file name

            try
            {
                string url = filePath;
                List<string> urlstring = new List<string>();
                System.IO.File.WriteAllBytes(filePath, fileContents);
                urlstring.Add(tempfilename);
                urlstring.Add(filePath);
                urlstring.Add(filePath);
                return urlstring;
                // Console.WriteLine("File saved successfully.");
            }
            catch (Exception ex)
            {
                return null;
                // Console.WriteLine("Error saving file: " + ex.Message);
            }
        }

        public string UploadImagesToFtp(IFormFile imgfile, string filename, string path, string ftpServerUrl = "ftp://sg.storage.bunnycdn.com", string ftpUsername = "k4m2astorage", string ftpPassword = "5f4e266f-f398-4b43-8f60c3394169-faf3-4149")
        {
            try
            {
                // string remoteDirectory = "httpdocs/post/images"
                // Get the file bytes from the IFormFile
                using (MemoryStream ms = new MemoryStream())
                {

                    imgfile.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();

                    //var remoteDirectory = "httpdocs/" + path;
                    var remoteDirectory = path;
                    // Create a FTP request object
                    FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create($"{ftpServerUrl}/{remoteDirectory}/{filename}");

                    // Set credentials
                    ftpRequest.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                    // Specify the FTP command
                    ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                    // Write the file data to the request stream
                    using (Stream requestStream = ftpRequest.GetRequestStream())
                    {
                        requestStream.Write(fileBytes, 0, fileBytes.Length);
                    }

                    // Get FTP server's response
                    using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse())
                    {
                        return filename;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Entities.File>> UploadFile(IFormCollection form)
        {
            try
            {
                var path = form["path"].ToString();
                List<Entities.File> filearr = new List<Entities.File>();
                
                foreach (var item in form.Files)
                {
                    Entities.File file = new Entities.File();
                    byte[] TempContent;

                    using (var memory = new MemoryStream())
                    {
                        item.CopyTo(memory);
                        TempContent = memory.ToArray();
                    }


                    string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // Unique timestamp

                    string tempfilename = "K4M2A_" + timestamp + System.IO.Path.GetExtension(item.FileName);

                    string tempname = UploadImagesToFtp(item, tempfilename,path);

                    byte[] emptybyte = new byte[0];

                    file.ContentType = item.ContentType;
                    file.Content = emptybyte;
                    file.FileExtension = System.IO.Path.GetExtension(item.FileName);
                    file.FileName = tempfilename;
                    file.ThumbnailUrl = "https://k4m2a.b-cdn.net/" + path + tempfilename;
                    file.ActualUrl = "https://k4m2a.b-cdn.net/" + path + tempfilename;
                    filearr.Add(file);
                }

                await _fileRepository.InsertRangeAsync(filearr);
                return filearr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Entities.File>> UploadFileForSuggestion(IFormFile form, string path)
        {
            try
            {
                List<Entities.File> filearr = new List<Entities.File>();
               
                    Entities.File file = new Entities.File();
                    byte[] TempContent;

                    using (var memory = new MemoryStream())
                    {
                        form.CopyTo(memory);
                        TempContent = memory.ToArray();
                    }

                    string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // Unique timestamp

                    string tempfilename = "K4M2A_" + timestamp + System.IO.Path.GetExtension(form.FileName);

                    string tempname = UploadImagesToFtp(form, tempfilename, path);

                    byte[] emptybyte = new byte[0];

                    file.ContentType = form.ContentType;
                    file.Content = emptybyte;
                    file.FileExtension = System.IO.Path.GetExtension(form.FileName);
                    file.FileName = tempfilename;
                    file.ThumbnailUrl = "https://k4m2a.b-cdn.net/" + path + tempfilename;
                    file.ActualUrl = "https://k4m2a.b-cdn.net/" + path + tempfilename;
                    filearr.Add(file);

                await _fileRepository.InsertRangeAsync(filearr);
                return filearr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResponse> DeleteUploadedFile(int Id, string Url)
        {
            try
            {
                var data = await _fileRepository.Table.Where(x => x.Id == Id && x.ActualUrl == Url && x.IsDeleted == false).FirstOrDefaultAsync();

                if(data != null)
                {
                    await _fileRepository.DeleteAsync(data);
                    return new JsonResponse(200, true, "file deleted", null);
                }
                return new JsonResponse(200, true, "file not found", null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
