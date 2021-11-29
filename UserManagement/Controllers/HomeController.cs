using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FormHelper;
using Newtonsoft.Json;
using UserManagement.Contract;
using UserManagement.Contract.Validator;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;
using UserManagement.Extensions;
using UserManagement.Infrastructure.Files;

namespace UserManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBulkDataImportService<MemberBulkImportVM> _bulkDataImportService;
        private readonly ILogger<HomeController> _logger;
        private readonly IBulkInsertValidator<MemberBulkImportVM> _bulkInsertValidator;

        public HomeController(IBulkDataImportService<MemberBulkImportVM> bulkDataImportService, ILogger<HomeController> logger, IBulkInsertValidator<MemberBulkImportVM> bulkInsertValidator)
        {
            this._bulkDataImportService = bulkDataImportService;
            this._logger = logger;
            this._bulkInsertValidator = bulkInsertValidator;
        }
        // GET: HomeController
        public ActionResult Index()
        {
            return RedirectToAction("BulkImport");
        }


        public ActionResult BulkImport()
        {
            _logger.LogInformation("Bulk import called");
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
            const string folderPath = "Logs/Csv";
            if (!(Directory.Exists(folderPath)))
            {
                Directory.CreateDirectory(folderPath);
            }

            var models = await _bulkDataImportService.CreateModels(stream);
            ViewBag.States = await _bulkDataImportService.GetStates();
            var listModels = models.ToList();

            var result = await _bulkInsertValidator.ValidateAsync(listModels);
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

            ViewBag.States = await _bulkDataImportService.GetStates();

            var result = await _bulkInsertValidator.ValidateAsync(data);
            var formResult = result.ToFormResult();
            if (!formResult.IsSucceed) return new JsonResult(formResult);
            
            var sessionId = Guid.NewGuid();
            const string folderPath = "Logs/Csv";
            var csvUtility = new MemberBulkImportVmCsvUtility(Convert.ToString(sessionId));
            csvUtility.Configure(new CsvConfiguration() { CsvLogPath = folderPath});
            csvUtility.Write(data);
             
            return FormResult.CreateSuccessResult("Sucess", $"BulkInsert/Index/{Convert.ToString(sessionId)}", 1000);

        }

        public async Task<IActionResult> GetDistricts(string stateId)
        { 
            var data = await _bulkDataImportService.GetDistrict(stateId);
            return new JsonResult(data);
        }
        public async Task<IActionResult> GetCities(string stateId,string districtId)
        {
            var data = await _bulkDataImportService.GetCities(stateId,districtId);
            return new JsonResult(data);
        }
    }
}