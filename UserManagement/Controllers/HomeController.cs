using FormHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Business.Validators;
using UserManagement.Contract;
using UserManagement.Contract.Validator;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;
using UserManagement.Extensions;

namespace UserManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBulkDataImportService<MemberBulkImportVM> bulkDataImportService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger<HomeController> logger;
        private readonly IBulkInsertValidator<MemberBulkImportVM> bulkInsertValidator;
        private IEnumerable<ResultModel<MemberBulkImportVM>> resultModels = new List<ResultModel<MemberBulkImportVM>>();

        public HomeController(IBulkDataImportService<MemberBulkImportVM> bulkDataImportService, IHostingEnvironment hostingEnvironment, ILogger<HomeController> logger, IBulkInsertValidator<MemberBulkImportVM> bulkInsertValidator)
        {
            this.bulkDataImportService = bulkDataImportService;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            this.bulkInsertValidator = bulkInsertValidator;
        }
        // GET: HomeController
        public ActionResult Index()
        {
            return RedirectToAction("BulkImport");
        }


        public ActionResult BulkImport()
        {
            logger.LogInformation("Bulk import called");
            return View();
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

            var result = await bulkInsertValidator.ValidateAsync(listModels);
            result.UpdateModelState(ModelState);

            return View(listModels);
        }


        /// <summary>
        /// This method is used to call from ajax through formhelper library
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ImportData(IList<MemberBulkImportVM> data)
        {
            if (data is null)
            {
                throw new System.ArgumentNullException(nameof(data));
            }

            ViewBag.States = await bulkDataImportService.GetStates();

            var result = await bulkInsertValidator.ValidateAsync(data);
            return new JsonResult(result.ToFormResult());
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