using System;
using Newtonsoft.Json;

namespace VatTask
{
    public class CountryRates
    {
        public string name;
        public string code;
        public string country_code;
        public Period[] periods;

        [JsonIgnore]
        public Decimal? currStandardVat;
    }
}
