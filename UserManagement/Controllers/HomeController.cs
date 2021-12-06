using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
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
        public async Task<ActionResult> BulkImport(IFormCollection form)
        {
            if (!form.Files.Any())
            {
                throw new System.ArgumentNullException(nameof(form.Files));
            }

            var stream = new MemoryStream();
            await form.Files[0].CopyToAsync(stream);
            const string folderPath = "Logs/Csv";
            if (!(Directory.Exists(folderPath)))
            {
                Directory.CreateDirectory(folderPath);
            }

            //XL read and transfer to models
            var selectedRecords = form["SelectRecord"];
            var models = await _bulkDataImportService.CreateModels(stream);


            ViewBag.States = await _bulkDataImportService.GetStates();
            var listModels = models.Take(Convert.ToInt32(selectedRecords)).ToList();

            // Validation of models after XL reading
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

            var result = await _bulkInsertValidator.ValidateAsync(data);
           
            if (!result.IsValid)
            {
                ViewBag.States = await _bulkDataImportService.GetStates();
                var formResult = result.ToFormResult();
                return new JsonResult(formResult);
            }
            
            var resultTemporaryStorage = await  _bulkDataImportService.AddToTemporaryStorage(data);
            return resultTemporaryStorage.IsFailure ? FormResult.CreateErrorResult(resultTemporaryStorage.Error) 
                : FormResult.CreateSuccessResult("Validation Successful...", $"BulkInsert/Index/{resultTemporaryStorage.Value}", 1000);
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