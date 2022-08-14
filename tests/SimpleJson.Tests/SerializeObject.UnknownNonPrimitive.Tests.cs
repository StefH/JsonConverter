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

using JsonConverter.SimpleJson;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleJson.Tests
{
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

    [TestClass]
    public class SerializeObject_UnknownNonPrimitive_Tests
    {
        private class Dog
        {
            public string Name { get; set; }
        }

        public class ObjProp
        {
            public string PropTypeKnown { get; set; }
            public object PropTypeUnknown { get; set; }
        }

        [TestMethod]
        public void CanSerializeClassInstanceExample()
        {
            const string expected = @"{""Name"":""spot""}";
            var dog = new Dog { Name = "spot" };

            var json = SimpleJson.SerializeObject(dog);

            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void CanSerializeAnonymousObject()
        {
            const string expected = @"{""name"":""spot""}";
            var instance = new { name = "spot" };

            var json = SimpleJson.SerializeObject(instance);

            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void CanSerializeAnonymousObjectWithNumbers()
        {
            // todo: make json encode smaller by removing spaces after ,
            const string expected = @"{""quantity"":8902,""cost"":45.33,""value"":-0.01063}";
            var instance = new
            {
                quantity = 8902,
                cost = 45.33,
                value = -1.063E-02
            };

            var json = SimpleJson.SerializeObject(instance);
            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void CanSerializeWithAnonymousTypes()
        {
            const string expected = @"{""array"":{""quantity"":8902,""cost"":45.33,""value"":-0.01063}}";
            var instance = new
            {
                array = new { quantity = 8902, cost = 45.33, value = -1.063E-02 }
            };

            var json = SimpleJson.SerializeObject(instance);
            Assert.AreEqual(expected, json);
        }

        [TestMethod]
        public void CanSerializeWithDatesThatHasMilliSecondAsNonZero()
        {
            var instance = new
            {
                now = new DateTime(2004, 1, 20, 5, 3, 6, 12, DateTimeKind.Utc)
            };

            var json = SimpleJson.SerializeObject(instance);
            Assert.IsNotNull(json);
            Assert.AreEqual("{\"now\":\"2004-01-20T05:03:06.012Z\"}", json);
        }

        [TestMethod]
        public void CanSerializeWithDatesThatHasMilliSecondAsZero()
        {
            var instance = new
            {
                now = new DateTime(2004, 1, 20, 5, 3, 6, 0, DateTimeKind.Utc)
            };

            var json = SimpleJson.SerializeObject(instance);
            Assert.IsNotNull(json);
            Assert.AreEqual("{\"now\":\"2004-01-20T05:03:06Z\"}", json);
        }

        [TestMethod]
        public void CanSerializeUnknownProperty()
        {
            var instance = new ObjProp { PropTypeKnown = "str", PropTypeUnknown = new { unknown = "property" } };

            var json = SimpleJson.SerializeObject(instance);
            Assert.AreEqual("{\"PropTypeKnown\":\"str\",\"PropTypeUnknown\":{\"unknown\":\"property\"}}", json);
        }

        [TestMethod]
        public void CanSerializeUnknownPropertyUsingNamingConvention()
        {
            var instance = new ObjProp { PropTypeKnown = "str", PropTypeUnknown = new { unknown = "property" } };
            var json = SimpleJson.SerializeObject(instance, new MyJsonSerializerStrategy());
            Assert.AreEqual("{\"prop_type_known\":\"str\",\"prop_type_unknown\":{\"unknown\":\"property\"}}", json);
        }

        [TestMethod]
        public void CanRoundTrip()
        {
            var instance = new ObjProp { PropTypeKnown = "str", PropTypeUnknown = new { unknown = "property" } };
            var json = SimpleJson.SerializeObject(instance, new MyJsonSerializerStrategy());
            Assert.AreEqual("{\"prop_type_known\":\"str\",\"prop_type_unknown\":{\"unknown\":\"property\"}}", json);
            var result = SimpleJson.DeserializeObject<ObjProp>(json, new MyJsonSerializerStrategy());
            Assert.AreEqual("str", result.PropTypeKnown);
        }

        private class MyJsonSerializerStrategy : PocoJsonSerializerStrategy
        {
            protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
            {
                return clrPropertyName == "PropTypeKnown" ? "prop_type_known" : clrPropertyName == "PropTypeUnknown" ? "prop_type_unknown" : clrPropertyName;
            }
        }
    }
}