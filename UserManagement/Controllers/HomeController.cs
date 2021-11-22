using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Business.Validators;
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
        private static IList<MemberBulkImportVM> _models;
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

       [HttpGet]
        public ActionResult CreateBulkImportGet()
        {
            var validator = new MemberBulkImportVMValidator();
            foreach (var result in _models)
            {
                var validationResult = validator.Validate(result);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                    }
                }
            }
           
            return View("BulkImportVM", _models);
        }

        [HttpPost]
        public ActionResult CreateBulkImport(IList<MemberBulkImportVM> bulkImportVM)
        {
            if (!ModelState.IsValid)
            {
                return View("BulkImportVM", bulkImportVM);
            }
            return View("BulkImportVM", bulkImportVM);
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
            var models = await bulkDataImportService.CreateModels(stream);
            _models = models.ToList();
            return RedirectToAction("CreateBulkImportGet");
           // resultModels = await bulkDataImportService.ImportData(stream, path);
           // return View(resultModels);

        }
    }
}