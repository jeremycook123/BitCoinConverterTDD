using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace BitcoinConverter.Code
{
    public class BitcoinConverterService
    {
        private HttpClient client;
        private const string version = "v1.0.3";

        public BitcoinConverterService()
        {
            //test comment2
            this.client = new HttpClient();
        }

        public BitcoinConverterService(HttpClient httpClient)
        {
            this.client = httpClient;
        }

        public async Task<double> GetExchangeRate(string currency)
        {
            var url = "https://api.coindesk.com/v1/bpi/currentprice/NZD.json";

            string html = await this.client.GetStringAsync(url);
            var jsonDoc = JsonDocument.Parse(System.Text.Encoding.ASCII.GetBytes(html));
            var rate = jsonDoc.RootElement.GetProperty("bpi").GetProperty(currency).GetProperty("rate");

            return Double.Parse(rate.GetString());
        }

        public async Task<double> ConvertToNZD(double coins)
        {
            return await GetExchangeRate("NZD", coins);
        }

        public async Task<double> ConvertToUSD(double coins)
        {
            return await GetExchangeRate("USD", coins);
        }

        private async Task<double> GetExchangeRate(string currency, double coins)
        {
            double result;

            try
            {
                result = await GetExchangeRate(currency) * coins;
            }
            catch(Exception)
            {
                result = 0;
            }

            return result;
        }

        public string GetVersion()
        {
            return version;
        }
    }
}
