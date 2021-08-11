using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private IEnumerable<ResultModel<MemberBulkImportVM>> resultModels = new List<ResultModel<MemberBulkImportVM>>();

        public HomeController(IBulkDataImportService<MemberBulkImportVM> bulkDataImportService)
        {
            this.bulkDataImportService = bulkDataImportService;
        }
        // GET: HomeController
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult BulkImport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BulkImport(IFormFile formFile)
        {
            try
            {
                var stream = new MemoryStream();
                await formFile.CopyToAsync(stream);
                resultModels = await bulkDataImportService.ImportData(stream);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }
    }
}
