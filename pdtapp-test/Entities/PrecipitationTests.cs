

using pdtapp.Entities;

namespace pdtapp_test.Entities
{
    public class PrecipitationTests
    {
        [Fact]
        public void CreateSucceeds()
        {
            var result = Record.Exception(() => new Precipitation());
            Assert.Null(result);
        }
    }
}