using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Contract;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;

namespace UserManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBulkDataImportService<MemberBulkImportVM> bulkDataImportService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger<HomeController> logger;
        private IEnumerable<ResultModel<MemberBulkImportVM>> resultModels = new List<ResultModel<MemberBulkImportVM>>();

        public HomeController(IBulkDataImportService<MemberBulkImportVM> bulkDataImportService, IHostingEnvironment hostingEnvironment, ILogger<HomeController> logger)
        {
            this.bulkDataImportService = bulkDataImportService;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }
        // GET: HomeController
        public ActionResult Index()
        {
            return View(resultModels);
        }


        public ActionResult BulkImport()
        {
            logger.LogInformation("Bulk import called");
          // var d = 0;
           //var t = 1 / d;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BulkImport(IFormFile formFile)
        {
            var stream = new MemoryStream();
            await formFile.CopyToAsync(stream);
            string folderPath = "Logs/Csv";
            if (!(Directory.Exists(folderPath)))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            var path = folderPath;
            resultModels = await bulkDataImportService.ImportData(stream, path);
            return View(resultModels);
        }
    }
}