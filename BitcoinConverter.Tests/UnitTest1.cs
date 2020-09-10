using BitcoinConverter.Code;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BitcoinConverter.Tests
{
    public class BitcoinConverterShould
    {
        private const string RESPONSE_JSON = @"{""bpi"":{""USD"":{""code"":""USD"",""rate"":""10,095.9106"",""description"":""United States Dollar"",""rate_float"":10095.9106},""NZD"":{""code"":""NZD"",""rate"":""15,095.5670"",""description"":""New Zealand Dollar"",""rate_float"":15095.567}}}";

        public BitcoinConverterShould(){
        }

        private BitcoinConverterService GetBitcoinConverterService() {
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(RESPONSE_JSON),
            };

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            var converter = new BitcoinConverterService(httpClient);

            return converter;
        }

        [Theory]
        [InlineData(1,15095.5670)]
        [InlineData(2,30191.1340)]
        public async void ConvertBitcoinsToNZD(double input, double expected)
        {
            //act
            var amount = await GetBitcoinConverterService().ConvertToNZD(input);

            //assert
            Assert.Equal(expected, amount);
        }

        [Theory]
        [InlineData(1,10095.9106)]
        [InlineData(2,20191.8212)]
        [InlineData(2.5,25239.7765)]
        public async void ConvertBitcoinsToUSD(double input, double expected)
        {
            //act
            var amount = await GetBitcoinConverterService().ConvertToUSD(input);

            //assert
            Assert.Equal(expected, amount);
        }

        [Theory]
        [InlineData("NZD",15095.5670)]
        [InlineData("USD",10095.9106)]
        public async void GetExchangeRate(string input, double expected)
        {
            //act
            var rate = await GetBitcoinConverterService().GetExchangeRate(input);

            //assert
            Assert.Equal(expected, rate);
        }

        [Fact]
        public async void ReturnZeroWhenServiceUnavailable(){
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = new StringContent("problems..."),
            };

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            var converter = new BitcoinConverterService(httpClient);

                        //act
            var amount = await converter.ConvertToUSD(1);

            //assert
            Assert.Equal(0, amount);
        }
    }
}
