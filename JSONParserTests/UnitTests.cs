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
            Assert.IsTrue(((JSONObject)json).Keys.SequenceEqual(new StringLiteral[] { "x" }));
            Assert.IsTrue(((JSONObject)json).Values.SequenceEqual(new Content[] { new DoubleLiteral(10.235151) }));
        }

        [Test]
        public void NullTest()
        {
            Content json = JSON.Parse("{\"x\":null}");
            Assert.IsTrue(json is JSONObject);
            Assert.IsTrue(((JSONObject)json).Keys.SequenceEqual(new StringLiteral[] { "x" }));
        }

        [Test]
        public void NestedObjectTest()
        {
            Content json = JSON.Parse("{\n  \"array\": [\n    1,\n    2,\n    3\n  ],\n  \"boolean\": true,\n  \"color\": \"gold\",\n  \"number\": 123,\n  \"object\": {\n    \"a\": \"b\",\n    \"c\": \"d\"\n  },\n  \"string\": \"Hello World\"\n}");
            Assert.IsTrue(json is JSONObject);
            Assert.IsTrue(((JSONObject)json).Keys.SequenceEqual(new StringLiteral[] { "array", "boolean", "color", "number", "object", "string" }));
        }
    }
}