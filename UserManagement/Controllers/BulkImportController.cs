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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BulkImportController : ControllerBase
    {
        private readonly IBulkDataImportService<MemberBulkImportVM> _bulkDataImportService;

        public BulkImportController(IBulkDataImportService<MemberBulkImportVM> bulkDataImportService)
        {
            this._bulkDataImportService = bulkDataImportService;
        }
        // GET: api/<BulkImportController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<BulkImportController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BulkImportController>
        [HttpPost]
        public async Task<IEnumerable<MemberBulkImportVM>> Post(IFormFile formFile)
        {
            var stream = new MemoryStream();
            await formFile.CopyToAsync(stream);
            string folderPath = "Logs/Csv";
            if (!(Directory.Exists(folderPath)))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            var path = folderPath;
           var models= await _bulkDataImportService.CreateModels(stream);
           var result = await _bulkDataImportService.ImportData(models, path);
            return result.Value;
        }

        // PUT api/<BulkImportController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BulkImportController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
