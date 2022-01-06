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
            var result = await _service.AddDataFromTemporaryStorage(id);
            if(result.IsFailure)
            {
                ViewBag.Message = result.Error;
                return View("Error");
            }
           
            return View(result.Value);
        }
    }
}