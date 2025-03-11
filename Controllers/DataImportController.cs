using Microsoft.AspNetCore.Mvc;
using System.IO;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Services;

namespace VeteranAnalyticsSystem.Controllers
{
    public class DataImportController : Controller
    {
        private readonly GratitudeAmericaDbContext _context;
        private readonly CsvImporter _csvImporter;

        public DataImportController(GratitudeAmericaDbContext context)
        {
            _context = context;
            _csvImporter = new CsvImporter();
        }

        // This action triggers the import from the CSV file.
        // In production, you might secure this endpoint with authorization.
        [HttpPost]
        public IActionResult ImportCsv()
        {
            // Construct the file path for the CSV file stored in the DataFiles folder.
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "DataFiles", "Master Applicant List.csv");

            // Import the records from CSV
            var veterans = _csvImporter.ImportVeterans(filePath);

            // Save the records to the database
            _context.Veterans.AddRange(veterans);
            _context.SaveChanges();

            return RedirectToAction("Index", "Veterans");
        }
    }
}
