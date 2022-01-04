using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Contract;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;
using UserManagement.Infrastructure.Files;
using UserManagement.Models;

namespace UserManagement.Controllers
{
    public class BulkInsertController : Controller
    {
        private readonly IBulkDataImportService<MemberBulkImportVM> _service;

        public BulkInsertController(IBulkDataImportService<MemberBulkImportVM> service)
        {
            _service = service;
        }
        // GET
        public async Task<IActionResult> Index(string id)
        {
            const string folderPath = "Logs/Csv";
            var csvUtility = new MemberBulkValidCsvUtility(id);
            csvUtility.Configure(new CsvConfiguration() { CsvLogPath = folderPath});
            var validData = csvUtility.Read(null);
            var inValidCSVUtility = new MemberBulkInvalidCsvUtility(id);
            inValidCSVUtility.Configure(new CsvConfiguration() { CsvLogPath = folderPath });
            var inValidModels = inValidCSVUtility.Read(null);
            var message = "No data found to import. Please try again";
            var validModels = Enumerable.Empty<MemberBulkValid>();
            if (validData != null)
            {
                var modelsList = validData.ToList();
                var result = await _service.ImportData(modelsList, folderPath);
                if (result.IsFailure)
                {
                    message = result.Error;
                }
                else
                {
                    message = string.Empty;
                    validModels = result.Value;
                }
            }
            var bulkModel = new BulkInsertValidInvalidVM()
            {
                ValidModels = validModels?.ToList(),
                InValidModels = inValidModels?.ToList()
            };
            ViewBag.Message = message;
            return View(bulkModel);
        }
    }
}