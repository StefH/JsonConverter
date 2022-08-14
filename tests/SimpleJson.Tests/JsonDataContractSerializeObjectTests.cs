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

[TestClass]
public class JsonDataContractSerializeObjectTests
{
    [DataContract]
    private class DataContractClass
    {
        public DataContractClass()
        {
            PrivatePropertyGetSetDataMemberWithoutName = "private without name";
            PrivatePropertyGetSetDataMemberWithName = "private with name";
            PrivatePropertyGetSetNoDataMember = "private no data member";
            PrivatePropertyGetSetIgnore = "ignore";
        }

        public readonly string ReadOnlyFieldWithoutDataMember = "public readonly without datamember";

        [DataMember]
        public readonly string ReadOnlyFieldDataMemberWithoutName = "public readonly data member without name";

        [DataMember(Name = "field_name")]
        public readonly string ReadOnlyFieldDataMemberWithName = "public readonly data member with name";

#pragma warning disable 0414
        [IgnoreDataMember]
        public readonly string ReadOnlyFieldIgnore = "public readonly ignore";

        private readonly string PrivateReadOnlyFieldWithoutDataMember = "private readonly without datamember";

        [DataMember]
        private readonly string PrivateReadOnlyFieldDataMemberWithoutName = "private readonly data member without name";

        [DataMember(Name = "private_field_name")]
        private readonly string PrivateReadOnlyFieldDataMemberWithName = "private readonly data member with name";

        [IgnoreDataMember]
        private readonly string PrivateReadOnlyFieldIgnore = "private readonly ignore";
#pragma warning restore 0414

        [DataMember]
        public string PropertyGetSetDataMemberWithoutName { get; set; }

        [DataMember]
        private string PrivatePropertyGetSetDataMemberWithoutName { get; set; }

        [DataMember(Name = "name")]
        public string PropertyGetSetDataMemberWithName { get; set; }

        [DataMember(Name = "private_name")]
        private string PrivatePropertyGetSetDataMemberWithName { get; set; }

        public string PropertyGetSetNoDataMember { get; set; }

        private string PrivatePropertyGetSetNoDataMember { get; set; }

        [IgnoreDataMember]
        public string PropertyGetSetIgnore { get; set; }

        [IgnoreDataMember]
        private string PrivatePropertyGetSetIgnore { get; set; }

        [DataMember]
        public string PropertyGetDataMemberWithoutName => "property get data member without name";

        [DataMember]
        private string PrivatePropertyGetDataMemberWithoutName => "property get data member without name";

        [DataMember(Name = "name_get")]
        public string PropertyGetDataMemberWithName => "property get data member with name";

        [DataMember(Name = "private_name_get")]
        private string PrivatePropertyGetDataMemberWithName => "property get data member with name";

        public string PropertyGetNoDataMember => "property get no data member";

        private string PrivatePropertyGetNoDataMember => "private property get no data member";

        [IgnoreDataMember]
        public string PropertyGetIgnore => "property get ignore";

        [IgnoreDataMember]
        private string PrivatePropertyGetIgnore => "private property get ignore";
    }

    private class NonDataContractClass
    {
    }

    private DataContractClass dataContractClass;

    public JsonDataContractSerializeObjectTests()
    {
        dataContractClass = new DataContractClass
        {
            PropertyGetSetDataMemberWithName = "name",
            PropertyGetSetDataMemberWithoutName = "nonname",
            PropertyGetSetNoDataMember = "no_datamember",
            PropertyGetSetIgnore = "ignored"
        };
    }
    /*
    [TestMethod]
    public void ContainsDataContractAttribute()
    {
        Assert.IsTrue(SimpleJson.GetAttribute(this.dataContractClass.GetType(), typeof(DataContractAttribute)) != null);
    }

    [TestMethod]
    public void DoesNotContainDataContractAttribute()
    {
        Assert.IsFalse(SimpleJson.GetAttribute(typeof(NonDataContractClass), typeof(DataContractAttribute)) != null);
    }

    [TestMethod]
    public void PropertyGetSetDataMemberWithoutNameTests()
    {
        var propertyInfo = typeof(DataContractClass).GetProperty("PropertyGetSetDataMemberWithoutName");

        var attr = SimpleJson.GetAttribute(propertyInfo, typeof(DataMemberAttribute));
        Assert.IsNotNull(attr);

        Assert.IsInstanceOf<DataMemberAttribute>(attr);
        var dataMemberAttribute = (DataMemberAttribute)attr;

        Assert.IsNull(dataMemberAttribute.Name);
    }

    [TestMethod]
    public void PropertyGetSetDataMemberWithNameTests()
    {
        var propertyInfo = typeof(DataContractClass).GetProperty("PropertyGetSetDataMemberWithName");

        var attr = SimpleJson.GetAttribute(propertyInfo, typeof(DataMemberAttribute));
        Assert.IsNotNull(attr);

        Assert.IsInstanceOf<DataMemberAttribute>(attr);
        var dataMemberAttribute = (DataMemberAttribute)attr;

        Assert.AreEqual("name", dataMemberAttribute.Name);
    }

    [TestMethod]
    public void PropertyGetSetNoDataMemberTests()
    {
        var propertyInfo = typeof(DataContractClass).GetProperty("PropertyGetSetNoDataMember");

        var attr = SimpleJson.GetAttribute(propertyInfo, typeof(DataMemberAttribute));
        Assert.IsNull(attr);
    }
    */

    [TestMethod]
    public void PublicReadOnlyFieldWithoutDataMemberShouldNotBePresent()
    {
        var json = SimpleJson.SerializeObject(dataContractClass, SimpleJson.DataContractJsonSerializerStrategy);

        var result = (IDictionary<string, object>)SimpleJson.DeserializeObject(json);

        Assert.IsFalse(result.ContainsKey("ReadOnlyFieldWithoutDataMember"));
    }

    [TestMethod]
    public void ReadOnlyFieldDataMemberWithoutNameShouldBePresent()
    {
        var json = SimpleJson.SerializeObject(dataContractClass, SimpleJson.DataContractJsonSerializerStrategy);

        var result = (IDictionary<string, object>)SimpleJson.DeserializeObject(json);

        Assert.IsTrue(result.ContainsKey("ReadOnlyFieldDataMemberWithoutName"));
    }

    [TestMethod]
    public void ReadOnlyFieldDataMemberWithNameShouldBePresent()
    {
        var json = SimpleJson.SerializeObject(dataContractClass, SimpleJson.DataContractJsonSerializerStrategy);

        var result = (IDictionary<string, object>)SimpleJson.DeserializeObject(json);

        Assert.IsTrue(result.ContainsKey("field_name"));
    }

    [TestMethod]
    public void ReadOnlyFieldIgnoreShouldNotBePresent()
    {
        var json = SimpleJson.SerializeObject(dataContractClass, SimpleJson.DataContractJsonSerializerStrategy);

        var result = (IDictionary<string, object>)SimpleJson.DeserializeObject(json);

        Assert.IsFalse(result.ContainsKey("ReadOnlyFieldIgnore"));
    }

    [TestMethod]
    public void PrivateReadOnlyFieldWithoutDataMemberShouldNotBePresent()
    {
        var json = SimpleJson.SerializeObject(dataContractClass, SimpleJson.DataContractJsonSerializerStrategy);

        var result = (IDictionary<string, object>)SimpleJson.DeserializeObject(json);

        Assert.IsFalse(result.ContainsKey("PrivateReadOnlyFieldWithoutDataMember"));
    }

    [TestMethod]
    public void PrivateReadOnlyFieldDataMemberWithoutNameShouldBePresent()
    {
        var json = SimpleJson.SerializeObject(dataContractClass, SimpleJson.DataContractJsonSerializerStrategy);

        var result = (IDictionary<string, object>)SimpleJson.DeserializeObject(json);

        Assert.IsTrue(result.ContainsKey("PrivateReadOnlyFieldDataMemberWithoutName"));
    }

    [TestMethod]
    public void PrivateReadOnlyFieldDataMemberWithNameShouldBePresent()
    {
        var json = SimpleJson.SerializeObject(dataContractClass, SimpleJson.DataContractJsonSerializerStrategy);

        var result = (IDictionary<string, object>)SimpleJson.DeserializeObject(json);

        Assert.IsTrue(result.ContainsKey("private_field_name"));
    }

    [TestMethod]
    public void PrivateReadOnlyFieldIgnoreShouldNotBePresent()
    {
        var json = SimpleJson.SerializeObject(dataContractClass, SimpleJson.DataContractJsonSerializerStrategy);

        var result = (IDictionary<string, object>)SimpleJson.DeserializeObject(json);

        Assert.IsFalse(result.ContainsKey("PrivateReadOnlyFieldIgnore"));
    }
}