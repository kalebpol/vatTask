using System;
using System.IO;
using System.Linq;
using System.Net;

namespace VatTask
{
    class Program
    {   
        public static void PrintCountryRate(CountryRates countryRate)
        {
            Console.WriteLine("{0} ({1}), standard rate: {2}", countryRate.name, countryRate.country_code, countryRate.currStandardVat);
        }

        static void Main(string[] args)
        {
            // default values, could specify through command line arguments
            string vatUri = "http://jsonvat.com";
            DateTime requestedDate = DateTime.Now;
            int nLowestVat = 3;
            int nHighestVat = 3;
            //--------------------------------------------------------------------//

            // download and deserialize vat data
            VatData vatData;
            var webClient = new WebClient();
            try
            {
	            using (var stream = new MemoryStream(webClient.DownloadData(new Uri(vatUri))))
	            {
		            vatData = Utilities.Deserialize<VatData>(stream);	
	            }
            }
            catch(Exception ex)
            {
	            Console.WriteLine(ex.Message);
	            return;
            }
            //--------------------------------------------------------------------//

            if (vatData.rates == null)
            {
                Console.WriteLine("No Data!");
                return;
            }

            // get current standard vat rate for requested date
            foreach (var countryRate in vatData.rates.Where(x => x.periods != null))
            {
                var currPeriod = countryRate.periods.Where(x => x.effective_from <= requestedDate)
                                        .OrderByDescending(x => x.effective_from)
                                        .FirstOrDefault();
                if (currPeriod != null)
                    countryRate.currStandardVat = currPeriod.rates.standard;
            }

            var highestVat = (  from countryRate in vatData.rates
	                            where countryRate.currStandardVat != null
	                            orderby countryRate.currStandardVat descending
	                            select countryRate
                             ).Take(nHighestVat).ToList();

            var lowestVat = (   from countryRate in vatData.rates
                                where countryRate.currStandardVat != null
                                orderby countryRate.currStandardVat ascending
                                select countryRate
                             ).Take(nLowestVat).ToList();
            //--------------------------------------------------------------------//

            // print results
            Console.WriteLine("On {0},", requestedDate.ToShortDateString());
            Console.WriteLine("The following EU countries have the highest VAT rate:");
            foreach (var countryRate in highestVat)
               PrintCountryRate(countryRate);

            Console.WriteLine();
            Console.WriteLine("The following EU countries have the lowest VAT rate:");
            foreach (var countryRate in lowestVat)
                PrintCountryRate(countryRate);

            Console.WriteLine();
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
