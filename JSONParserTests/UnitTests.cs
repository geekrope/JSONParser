using System;
using JSONParser;

namespace JSONParserTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void EmptyObject()
        {
            Content json = JSON.Parse("{}");
            Assert.IsTrue(json is JSONObject);
            Assert.IsTrue(((JSONObject)json).Count == 0);
        }

        [Test]
        public void DoubleTest()
        {
            Content json = JSON.Parse("{\"x\":10.235151}");
            Assert.IsTrue(json is JSONObject);
            Assert.IsTrue(((JSONObject)json).Keys.SequenceEqual(new Literal<string>[] { "x" }));
            Assert.IsTrue(((JSONObject)json).Values.SequenceEqual(new Content[] { new Literal<double>(10.235151) }));
        }

        [Test]
        public void NullTest()
        {
            Content json = JSON.Parse("{\"x\":null}");
            Assert.IsTrue(json is JSONObject);
            Assert.IsTrue(((JSONObject)json).Keys.SequenceEqual(new Literal<string>[] { "x" }));
        }

        [Test]
        public void NestedObjectTest()
        {
            Content json = JSON.Parse("{\n  \"array\": [\n    1,\n    2,\n    3\n  ],\n  \"boolean\": true,\n  \"color\": \"gold\",\n  \"number\": 123,\n  \"object\": {\n    \"a\": \"b\",\n    \"c\": \"d\"\n  },\n  \"string\": \"Hello World\"\n}");
            Assert.IsTrue(json is JSONObject);
            Assert.IsTrue(((JSONObject)json).Keys.SequenceEqual(new Literal<string>[] { "array", "boolean", "color", "number", "object", "string" }));
            Assert.IsTrue(((JSONArray)((JSONObject)json).Values[0]).Equals(new JSONArray(new Content[] { (Literal<double>)1, (Literal<double>)2, (Literal<double>)3 })));
        }
    }
}