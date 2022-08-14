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

using SimpleJson.Tests.DataContractTests;

namespace SimpleJson.Tests.PocoJsonSerializerTests
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
#endif

    using SimpleJson = JsonConverter.SimpleJson.SimpleJson;

    [TestClass]
    public class PublicFieldsDeserializeTests
    {
        [TestMethod]
        public void DeserializesNullObjectCorrectly()
        {
            var json = "null";

            var result = (DataContractPublicFields)SimpleJson.DeserializeObject(json, typeof(DataContractPublicFields), SimpleJson.PocoJsonSerializerStrategy);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void DeserializesEmptyObjectCorrectly()
        {
            var json = "{}";
            var obj = new DataContractPublicFields();

            var result = (DataContractPublicFields)SimpleJson.DeserializeObject(json, typeof(DataContractPublicFields), SimpleJson.PocoJsonSerializerStrategy);

            Assert.IsNotNull(result);

            Assert.AreEqual(obj.NoDataMember, result.NoDataMember);
            Assert.AreEqual(obj.IgnoreDataMember, result.IgnoreDataMember);
            Assert.AreEqual(obj.DatMemberWithName, result.DatMemberWithName);
            Assert.AreEqual(obj.DataMemberWithoutName, result.DataMemberWithoutName);
        }

        [TestMethod]
        public void DeserializesCorrectly()
        {
            var json = "{\"DataMemberWithoutName\":\"1\",\"DatMemberWithName\":\"2\",\"IgnoreDataMember\":\"3\",\"NoDataMember\":\"4\"}";

            var result = (DataContractPublicFields)SimpleJson.DeserializeObject(json, typeof(DataContractPublicFields), SimpleJson.PocoJsonSerializerStrategy);

            Assert.IsNotNull(result);

            Assert.AreEqual("1", result.DataMemberWithoutName);
            Assert.AreEqual("2", result.DatMemberWithName);
            Assert.AreEqual("3", result.IgnoreDataMember);
            Assert.AreEqual("4", result.NoDataMember);
        }
    }
}