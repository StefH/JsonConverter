//-----------------------------------------------------------------------
// <copyright file="<file>.cs" company="The Outercurve Foundation">
//    Copyright (c) 2011, The Outercurve Foundation.
//
//    Licensed under the MIT License (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.opensource.org/licenses/mit-license.php
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
// <author>Nathan Totten (ntotten.com), Jim Zimmerman (jimzimmerman.com) and Prabir Shrestha (prabir.me)</author>
// <website>https://github.com/facebook-csharp-sdk/simple-json</website>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;
using JsonConverter.SimpleJson;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleJson.Tests;

#if NUNIT
    using TestClass = NUnit.Framework.TestFixtureAttribute;
    using TestMethod = NUnit.Framework.TestAttribute;
    using TestCleanup = NUnit.Framework.TearDownAttribute;
    using TestInitialize = NUnit.Framework.SetUpAttribute;
    using ClassCleanup = NUnit.Framework.TestFixtureAttribute;
    using ClassInitialize = NUnit.Framework.TestFixtureAttribute;
    using NUnit.Framework;
#else
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
#endif
#endif

using SimpleJson = JsonConverter.SimpleJson.SimpleJson;
using JsonArray = JsonConverter.SimpleJson.JsonArray;
using JsonObject = JsonConverter.SimpleJson.JsonObject;

[TestClass]
public class DeserializeObjectTests
{
    public class ObjProp
    {
        public string PropTypeKnown { get; set; }
        public object PropTypeUnknown { get; set; }
    }

    [TestMethod]
    public void ReadIndented()
    {
        string input = @"{
  ""CPU"": ""Intel"",
  ""Drives"": [
    ""DVD read/writer"",
    ""500 gigabyte hard drive""
  ]
}";
        object obj = SimpleJson.DeserializeObject(input);

        Assert.IsInstanceOfType(obj, typeof(IDictionary<string, object>));
        Assert.IsInstanceOfType(obj, typeof(JsonObject));

        var root = (IDictionary<string, object>)obj;

        Assert.AreEqual(2, root.Count);

        Assert.IsTrue(root.ContainsKey("CPU"));
        Assert.AreEqual("Intel", root["CPU"]);

        Assert.IsTrue(root.ContainsKey("Drives"));

        Assert.IsInstanceOfType(root["Drives"], typeof(IList<object>));
        Assert.IsInstanceOfType(root["Drives"], typeof(JsonArray));

        var drives = (IList<object>)root["Drives"];

        Assert.AreEqual(2, drives.Count);

        Assert.IsInstanceOfType(drives[0], typeof(string));
        Assert.AreEqual("DVD read/writer", drives[0]);
        Assert.IsInstanceOfType(drives[1], typeof(string));
        Assert.AreEqual("500 gigabyte hard drive", drives[1]);
    }

    [TestMethod]
    [ExpectedException(typeof(SerializationException), "Invalid JSON string")]
    public void UnexpectedEndOfString()
    {
        var result = SimpleJson.DeserializeObject("hi");
    }


    [TestMethod]
    public void ReadNullTerminiatorString()
    {
        var result = SimpleJson.DeserializeObject("\"h\0i\"");

        Assert.AreEqual("h\0i", result);
    }

    [TestMethod]
    public void EmptyObjectDeserialize()
    {
        var result = SimpleJson.DeserializeObject("{}");
        Assert.IsInstanceOfType(result, typeof(IDictionary<string, object>));
        var dict = (IDictionary<string, object>) result;
        Assert.AreEqual(0, dict.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(SerializationException), "Invalid JSON string")]
    public void UnexpectedEndOfHex()
    {
        var result = SimpleJson.DeserializeObject(@"'h\u006");
    }


    [TestMethod]
    [ExpectedException(typeof(SerializationException), "Invalid JSON string")]
    public void UnexpectedEndOfControlCharacter()
    {
        var result = SimpleJson.DeserializeObject(@"'h\");
    }

    [TestMethod]
    [ExpectedException(typeof(SerializationException), "Invalid JSON string")]
    public void UnexpectedEndWhenParsingUnquotedProperty()
    {
        var result = SimpleJson.DeserializeObject(@"{aww");
    }

    [TestMethod]
    public void ParsingQuotedPropertyWithControlCharacters()
    {
        var result = SimpleJson.DeserializeObject("{\"hi\r\nbye\":1}");

        Assert.IsInstanceOfType(result, typeof(IDictionary<string, object>));
        Assert.IsInstanceOfType(result, typeof(JsonObject));

        var dict = (IDictionary<string, object>)result;

        foreach (KeyValuePair<string, object> pair in dict)
        {
            Assert.AreEqual(@"hi
bye", pair.Key);
        }
    }



    [TestMethod]
    public void ReadNewLineLastCharacter()
    {
        string input = @"{
  ""CPU"": ""Intel"",
  ""Drives"": [ 
    ""DVD read/writer"",
    ""500 gigabyte hard drive""
  ]
}" + '\n';

        object o = SimpleJson.DeserializeObject(input);
        Assert.IsNotNull(o);

        Assert.IsInstanceOfType(o, typeof(IDictionary<string, object>));

        var dict = (IDictionary<string, object>)o;

        Assert.AreEqual("Intel", dict["CPU"]);
    }



    [TestMethod]
#if NETFX_CORE
        [Ignore]
#else
    [Ignore("not part of the json standard.")]
#endif
    public void FloatingPointNonFiniteNumber()
    {
        string input = @"[
  NaN,
  Infinity,
  -Infinity
]";
        var o = SimpleJson.DeserializeObject(input);
    }

    [TestMethod]
    public void LongStringTests()
    {
        int length = 20000;
        string json = @"[""" + new string(' ', length) + @"""]";

        var o = SimpleJson.DeserializeObject(json);

        var a = (IList<object>)o;

        Assert.IsInstanceOfType(a[0], typeof(string));

        Assert.AreEqual(20000, ((string)a[0]).Length);
    }

    [TestMethod]
    public void EscapedUnicodeTests()
    {
        string json = @"[""\u003c"",""\u5f20""]";

        var o = SimpleJson.DeserializeObject(json);

        Assert.IsInstanceOfType(o, typeof(List<object>));

        var l = (List<object>)o;
        Assert.AreEqual(2, l.Count);

        Assert.AreEqual("<", l[0]);
        //Assert.AreEqual("24352", Convert.ToInt32(Convert.ToChar(l[0])));
    }

    [TestMethod]
    [ExpectedException(typeof(SerializationException), "Invalid JSON string")]
    public void MissingColon()
    {
        string json = @"{
    ""A"" : true,
    ""B"" ""hello"", // Notice the colon is missing
    ""C"" : ""bye""
}";

        var o = SimpleJson.DeserializeObject(json);
    }



    [TestMethod]
    [Ignore("not part of the json standard.")]
    public void ReadOctalNumber()
    {
        var json = @"[0372, 0xFA, 0XFA]";

        var o = SimpleJson.DeserializeObject(json);
    }

    [TestMethod]
    public void ReadUnicode()
    {
        string json = @"{""Message"":""Hi,I\u0092ve send you smth""}";

        var o = SimpleJson.DeserializeObject(json);

        Assert.IsInstanceOfType(o, typeof(IDictionary<string, object>));
        Assert.IsInstanceOfType(o, typeof(JsonObject));

        var dict = (IDictionary<string, object>)o;

        Assert.AreEqual(@"Hi,I" + '\u0092' + "ve send you smth", dict["Message"]);
    }


    [TestMethod]
    public void DeserializeUnicodeChar()
    {
        string json = "\"न\"";

        var o = SimpleJson.DeserializeObject(json);

        Assert.IsInstanceOfType(o, typeof(string));

        Assert.AreEqual("न", o);
    }

    [TestMethod]
#if NETFX_CORE
        [Ignore]
#else
    [Ignore("not part of the json standard.")]
#endif
    public void ReadHexidecimalWithAllLetters()
    {
        string json = @"{""text"":0xabcdef12345}";

        var o = SimpleJson.DeserializeObject(json);
    }

    [TestMethod]
    [ExpectedException(typeof(SerializationException), "Invalid JSON string")]
    public void ParseIncompleteArray()
    {
        var o = SimpleJson.DeserializeObject("[1");
    }

    [TestMethod]
    public void DeserializeSurrogatePair()
    {
        string json = "\"𩸽 is Arabesque greenling(fish) in japanese\"";
        var o = SimpleJson.DeserializeObject(json);

        Assert.IsInstanceOfType(o, typeof(string));

        Assert.AreEqual("𩸽 is Arabesque greenling(fish) in japanese", o);
    }

    [TestMethod]
    public void DeserializeEscapedSurrogatePair()
    {
        string json = "\"\\uD867\\uDE3D is Arabesque greenling(fish)\"";  // 𩸽
        var o = SimpleJson.DeserializeObject(json);
        Assert.IsInstanceOfType(o, typeof(string));
        Assert.AreEqual("\uD867\uDE3D is Arabesque greenling(fish)", o);
        Assert.AreEqual("𩸽 is Arabesque greenling(fish)", o);
    }

    [TestMethod]
    [ExpectedException(typeof(SerializationException), "Invalid JSON string")]
    public void DeserializeInvaildEscapedSurrogatePair()
    {
        string json = "\"\\uD867\\u0000 is Arabesque greenling(fish)\"";
        var o = SimpleJson.DeserializeObject(json);
    }

    [TestMethod]
    public void DeserializeKnownJsonObjectType()
    {
        var json = "{\"FirstName\":\"Bruno\",\"LastName\":\"Baïa\",\"Address\":{\"Country\":\"France\",\"City\":\"Toulouse\"}}";
        var result = SimpleJson.DeserializeObject<JsonObject>(json);

        Assert.IsNotNull(result);
        Assert.AreEqual("Bruno", result["FirstName"]);
        Assert.AreEqual("Baïa", result["LastName"]);
        var address = result["Address"] as JsonObject;
        Assert.AreEqual("France", address["Country"]);
        Assert.AreEqual("Toulouse", address["City"]);
    }

    [TestMethod]
    public void DeserializeKnownJsonObjectTypeWithMapping()
    {
        const string json = "{\"first_name\":\"Bruno\",\"LastName\":\"Mars\"}";
        var result = SimpleJson.DeserializeObject<Sample>(json, new MySerializationStrategy());

        Assert.IsNotNull(result);
        Assert.AreEqual("Bruno", result.FirstName);
        Assert.AreEqual("Mars", result.LastName);
    }

    private class MySerializationStrategy : PocoJsonSerializerStrategy
    {
        protected override string MapClrMemberNameToJsonFieldName(string jsonFieldName)
        {
            return jsonFieldName == "FirstName" ? "first_name" : jsonFieldName;
        }
    }

    public class Sample
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    [TestMethod]
    public void DeserializeKnownJsonArrayType()
    {
        var json = "[{\"name\":\"road cycling\",\"value\":11},{\"name\":\"football\",\"value\":9}]";
        var result = SimpleJson.DeserializeObject<JsonArray>(json);

        Assert.IsNotNull(result);
        foreach (JsonObject hobby in result)
        {
            Assert.IsNotNull(hobby["name"]);
            Assert.IsNotNull(hobby["value"]);
        }
    }

    [TestMethod]
    public void DeserializeDoubleQuotesCorrectly()
    {
        var json = "{\"message\":\"Hi \\\"Prabir\\\"\"}";
        var result = (IDictionary<string, object>)SimpleJson.DeserializeObject(json);

        Assert.AreEqual("Hi \"Prabir\"", result["message"]);
    }

    class ClassWithUri { public Uri url { get; set; } }

    [TestMethod]
    public void DeserializeUriCorrectly()
    {
        var json = "{\"url\":\"https://github.com/shiftkey/simple-json/issues/1\"}";
        var result = SimpleJson.DeserializeObject<ClassWithUri>(json);

        Assert.AreEqual(new Uri("https://github.com/shiftkey/simple-json/issues/1"), result.url);
    }

    [TestMethod]
    public void DeserializeInvalidUriCorrectly()
    {
        var json = "{\"url\":\"this is a broken uri\"}";
        var result = SimpleJson.DeserializeObject<ClassWithUri>(json);

        Assert.IsNull(result.url);
    }

    [TestMethod]
    public void DeserializeUnknownProperty()
    {
        var json = "{\"PropTypeKnown\":\"str\",\"PropTypeUnknown\":{\"unknown\":\"property\"}}";
        var result = SimpleJson.DeserializeObject<ObjProp>(json);

        Assert.IsNotNull(result);
        Assert.AreEqual("str", result.PropTypeKnown);

        Assert.IsInstanceOfType(result.PropTypeUnknown, typeof(IDictionary<string, object>));
        Assert.IsInstanceOfType(result.PropTypeUnknown, typeof(JsonObject));

        var dict = (IDictionary<string, object>)result.PropTypeUnknown;
        Assert.IsNotNull(dict);
        Assert.AreEqual(1, dict.Count);
        Assert.AreEqual("property", dict["unknown"]);
    }

    public class BaseClass
    {
        public string B { get; set; }
    }

    public class SomeClass : BaseClass
    {
        public string A { get; set; }
    }

    [TestMethod]
    public void DeserializePropertiesOnBaseClass()
    {
        var json = "{\"A\":\"first-value\",\"B\":\"another-value\"}";
        var result = SimpleJson.DeserializeObject<SomeClass>(json);

        Assert.IsNotNull(result);
        Assert.AreEqual("first-value", result.A);
        Assert.AreEqual("another-value", result.B);
    }
}