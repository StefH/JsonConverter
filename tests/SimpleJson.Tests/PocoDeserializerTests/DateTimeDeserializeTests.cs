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

namespace SimpleJson.Tests.PocoDeserializerTests;

using System;

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
public class DateTimeDeserializeTests
{
    [TestMethod]
    public void Test()
    {
        var json = "\"2004-01-20T05:03:06Z\"";

        var result = SimpleJson.DeserializeObject<DateTime>(json);
        Assert.AreEqual(2004, result.Year);
        Assert.AreEqual(1, result.Month);
        Assert.AreEqual(20, result.Day);
        Assert.AreEqual(5, result.Hour);
        Assert.AreEqual(3, result.Minute);
        Assert.AreEqual(6, result.Second);
        Assert.AreEqual(0, result.Millisecond);
        Assert.AreEqual(DateTimeKind.Utc, result.Kind);
    }

    [TestMethod]
    public void TestWithoutMilisecond()
    {
        var json = "{\"Value\":\"2004-01-20T05:03:06Z\"}";

        var result = SimpleJson.DeserializeObject<SerializeDateTimeTypeClass>(json)!.Value;
        Assert.AreEqual(2004, result.Year);
        Assert.AreEqual(1, result.Month);
        Assert.AreEqual(20, result.Day);
        Assert.AreEqual(5, result.Hour);
        Assert.AreEqual(3, result.Minute);
        Assert.AreEqual(6, result.Second);
        Assert.AreEqual(0, result.Millisecond);
        Assert.AreEqual(DateTimeKind.Utc, result.Kind);
    }

    [TestMethod]
    public void TestWithMilliSecond()
    {
        var json = "{\"Value\":\"2004-01-20T05:03:06.012Z\"}";

        var result = SimpleJson.DeserializeObject<SerializeDateTimeTypeClass>(json)!.Value;
        Assert.AreEqual(2004, result.Year);
        Assert.AreEqual(1, result.Month);
        Assert.AreEqual(20, result.Day);
        Assert.AreEqual(5, result.Hour);
        Assert.AreEqual(3, result.Minute);
        Assert.AreEqual(6, result.Second);
        Assert.AreEqual(12, result.Millisecond);
        Assert.AreEqual(DateTimeKind.Utc, result.Kind);
    }

    public class SerializeDateTimeTypeClass
    {
        public DateTime Value { get; set; }
    }
}

public class DateOffsetTimeDeserializeTests
{
    [TestMethod]
    public void TestWithMillisecond()
    {
        var json = "{\"Value\":\"2004-01-20T05:03:06.012Z\"}";

        var result = SimpleJson.DeserializeObject<SerializeDateTimeOffsetTypeClass>(json)!.Value;
        Assert.AreEqual(2004, result.Year);
        Assert.AreEqual(1, result.Month);
        Assert.AreEqual(20, result.Day);
        Assert.AreEqual(5, result.Hour);
        Assert.AreEqual(3, result.Minute);
        Assert.AreEqual(6, result.Second);
        Assert.AreEqual(12, result.Millisecond);
        Assert.AreEqual(TimeSpan.Zero, result.Offset);
    }

    public class SerializeDateTimeOffsetTypeClass
    {
        public DateTimeOffset Value { get; set; }
    }
}