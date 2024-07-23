using AutoMapper;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using SpiritualNetwork.Entities.CommonModel;
using System.Data;

namespace SpiritualNetwork.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class ImportDataController : ApiBaseController
    {
        private readonly IRepository<Books> _bookRepository;
        private readonly IRepository<Movies> _movieRepository;
        private readonly IRepository<Gurus> _guruRepository;
        private readonly IRepository<Practices> _practiceRepository;
        private readonly IRepository<Experience> _experienceRepository;
        private readonly IFileService _fileService;

        public ImportDataController(
            IRepository<Books> bookRepository,
            IRepository<Movies> movieRepository,
            IRepository<Gurus> guruRepository,
            IRepository<Practices> practiceRepository,
            IRepository<Experience> experienceRepository,
            IFileService fileService)
        {
            _bookRepository = bookRepository;
            _movieRepository = movieRepository;
            _guruRepository = guruRepository;
            _practiceRepository = practiceRepository;
            _experienceRepository = experienceRepository;
            _fileService = fileService;
        }

        [AllowAnonymous]
        [HttpPost(Name = "UploadFileBook")]
        public async Task<JsonResponse> UploadFileBook(IFormFile form, string path)
        {
            try
            {
                var response = await _fileService.UploadFileForSuggestion(form, "images/"+path+"/");
                return new JsonResponse(200, true, "success", response);
            }
            catch (Exception ex)
            {
                return new JsonResponse(200, false, "Fail", ex.Message);
            }
        }

        [HttpPost("UploadSuggestion")]
        public async Task<IActionResult> UploadSuggestion(IFormFile file, string type)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        });

                        var dataTable = dataSet.Tables[4];

                        switch (type.ToLower())
                        {
                            case "book":
                                var booksList = (from DataRow row in dataTable.Rows
                                                 select new Books
                                                 {
                                                     BookName = row["BookName"].ToString() ?? "",
                                                     Author = row["Author"].ToString() ?? "",
                                                     BookImg = row["BookImg"].ToString() ?? ""
                                                 }).ToList();
                                await _bookRepository.InsertRangeAsync(booksList);
                                break;

                            case "movie":
                                var movieList = (from DataRow row in dataTable.Rows
                                                 select new Movies
                                                 {
                                                     MovieImg = row["MovieImg"].ToString() ?? "",
                                                     MovieName = row["MovieName"].ToString() ?? ""
                                                 }).ToList();
                                await _movieRepository.InsertRangeAsync(movieList);
                                break;

                            case "guru":
                                var guruList = (from DataRow row in dataTable.Rows
                                                select new Gurus
                                                {
                                                    GuruImg = row["GuruImg"].ToString() ?? "",
                                                    GuruName = row["GuruName"].ToString() ?? ""
                                                }).ToList();
                                await _guruRepository.InsertRangeAsync(guruList);
                                break;

                            case "practice":
                                var practiceList = (from DataRow row in dataTable.Rows
                                                    select new Practices
                                                    {
                                                        PracticeImg = row["PracticeImg"].ToString() ?? "",
                                                        PracticeName = row["PracticeName"].ToString() ?? ""
                                                    }).ToList();
                                await _practiceRepository.InsertRangeAsync(practiceList);
                                break;

                            case "experience":
                                var ExperienceList = (from DataRow row in dataTable.Rows
                                                      select new Experience
                                                      {
                                                          ExperienceImg = row["ExperienceImg"].ToString() ?? "",
                                                          ExperienceName = row["ExperienceName"].ToString() ?? "",
                                                      }).ToList();

                                await _experienceRepository.InsertRangeAsync(ExperienceList);
                                break;

                            default:
                                return BadRequest("Invalid type specified");
                        }
                    }
                }

                return Ok("Data inserted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

    }
}
