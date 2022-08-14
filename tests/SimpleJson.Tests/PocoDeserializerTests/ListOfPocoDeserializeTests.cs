﻿//-----------------------------------------------------------------------
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

namespace SimpleJson.Tests.PocoDeserializerTests;

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

public class ListOfPocoDeserializeTests
{

    [TestMethod]
    public void CorrectlyDeserializesListOfPoco()
    {
        string json = "{\"colleges\":[{\"id\": 16777217,\"value\":\"Harvard\"},{\"id\": 16777218,\"value\":\"Columbia\"}]}";

        var result = SimpleJson.DeserializeObject<autocomplete_data>(json);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.colleges);
        Assert.AreEqual(2, result.colleges.Count);

        var harvard = result.colleges[0];
        var columbia = result.colleges[1];

        Assert.IsNotNull(harvard);
        Assert.AreEqual(16777217, harvard.id);
        Assert.AreEqual("Harvard", harvard.value);
        Assert.IsNotNull(columbia);
        Assert.AreEqual(16777218, columbia.id);
        Assert.AreEqual("Columbia", columbia.value);
    }

    // ReSharper disable once InconsistentNaming
    class autocomplete_data
    {
        // ReSharper disable once InconsistentNaming
        public List<college> colleges { get; set; }
    }


    // ReSharper disable once InconsistentNaming
    class college
    {
        // ReSharper disable once InconsistentNaming
        public long id { get; set; }

        // ReSharper disable once InconsistentNaming
        public string value { get; set; }
    }
}