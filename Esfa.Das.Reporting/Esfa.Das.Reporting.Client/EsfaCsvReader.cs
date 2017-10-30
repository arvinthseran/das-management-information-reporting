using System;
using CsvHelper;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Esfa.Das.Reporting.Client
{
    public static class EsfaCsvReader
    {
        public static List<T> ReadCSV<T>(string absolutePath) where T : struct
        {
            List<T> result = new List<T>();
            T value;
            using (TextReader fileReader = File.OpenText(absolutePath))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = true;
                while (csv.Read())
                {
                    for (int i = 0; csv.TryGetField<T>(i, out value); i++)
                    {
                        result.Add(value);
                    }
                }
            }
            return result;

        }
    }

    public class FcsExcelData
    {
        public long ukprn;

        public List<FcsExcelData> ReadFromFile(string absolutePath)
        {
            List<FcsExcelData> fcsdata = new List<FcsExcelData>();
            var ukprns = EsfaCsvReader.ReadCSV<long>(absolutePath);
            foreach(var ukprn in ukprns)
            {
                fcsdata.Add(new FcsExcelData { ukprn = ukprn });
            }

            return fcsdata;
        }
    }
}