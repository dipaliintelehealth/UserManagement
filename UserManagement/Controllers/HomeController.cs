using FormHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            return View();
        }
        private async Task CheckValidation(IList<MemberBulkImportVM> models)
        {
            var validator = new MemberBulkImportVMValidator();
            for (int i = 0; i < models.Count; i++)
            {
                var validationResult = validator.Validate(models[i]);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        string key = $"[{i}].{item.PropertyName}";
                        ModelState.AddModelError(key, item.ErrorMessage);
                    }
                }
            }
        }

        [HttpPost, FormValidator]
        public async Task<IActionResult> ImportData(IList<MemberBulkImportVM> data)
        {
            if (data is null)
            {
                throw new System.ArgumentNullException(nameof(data));
            }

            ViewBag.States = await bulkDataImportService.GetStates();

            if (!ModelState.IsValid)
            {
                return FormResult.CreateErrorResult("An error occured.");
            }
            return FormResult.CreateSuccessResult("Success");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BulkImport(IFormFile formFile)
        {
            if (formFile is null)
            {
                throw new System.ArgumentNullException(nameof(formFile));
            }

            var stream = new MemoryStream();
            await formFile.CopyToAsync(stream);
            string folderPath = "Logs/Csv";
            if (!(Directory.Exists(folderPath)))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            var path = folderPath;
            var models = await bulkDataImportService.CreateModels(stream);
            ViewBag.States = await bulkDataImportService.GetStates();
            var listModels = models.ToList();
            await CheckValidation(listModels);
            return View(listModels);
            //return RedirectToAction(nameof(this.ImportData));
        }

        public async Task<IActionResult> GetDistricts(string stateId)
        { 
            var data = await bulkDataImportService.GetDistrict(stateId);
            return new JsonResult(data);
        }
        public async Task<IActionResult> GetCities(string stateId,string districtId)
        {
            var data = await bulkDataImportService.GetCities(stateId,districtId);
            return new JsonResult(data);
        }
    }
}