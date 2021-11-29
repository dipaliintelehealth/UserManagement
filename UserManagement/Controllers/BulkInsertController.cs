using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Contract;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;
using UserManagement.Infrastructure.Files;

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
            var csvUtility = new MemberBulkImportVmCsvUtility(id);
            csvUtility.Configure(new CsvConfiguration() { CsvLogPath = folderPath});
            var models = csvUtility.Read(null);
            var message = "No data found to import. Please try again";
            if (models != null)
            {
                var modelsList = models.ToList();
                var result = await _service.ImportData(modelsList, folderPath);
                if (result.IsSuccess)
                {
                    return View(result.Value);
                }

                message = result.Error;
            }

            ViewBag.Message = message;
            return View("Error");
        }
    }
}