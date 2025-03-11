using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using VeteranAnalyticsSystem.Models;

namespace VeteranAnalyticsSystem.Services
{
    public class CsvImporter
    {
        public List<Veteran> ImportVeterans(string filePath)
        {
            // For reading non-UTF8 encodings
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Configure CsvHelper
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true // specify that the first row is a header
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            // Register your ClassMap on the csv.Context
            csv.Context.RegisterClassMap<VeteranCsvMap>();

            var records = csv.GetRecords<Veteran>().ToList();
            return records;
        }
    }

    public sealed class VeteranCsvMap : ClassMap<Veteran>
    {
        public VeteranCsvMap()
        {
            // Example mappings (replace with your actual column headers)
            Map(m => m.FirstName).Name("Veteran First Name:");
            Map(m => m.LastName).Name("Veteran Last Name:");
            Map(m => m.Email).Name("Veteran E-mail Address");
            // ... etc.
        }
    }
}
