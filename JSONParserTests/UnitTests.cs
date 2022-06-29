using System;

namespace JSONParserTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            JSONParser.JSON.Init();
        }

        [Test]
        public void Test1()
        {
            JSONParser.Content json = JSONParser.JSON.Parse("{}");
            Assert.NotNull(json);
        }
    }
}