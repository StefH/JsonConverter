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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleJson.Tests;

using JsonArray = JsonConverter.SimpleJson.JsonArray;

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

[TestClass]
public class JsonArrayTests
{
    [TestMethod]
    public void Clear()
    {
        var a = new JsonArray { 1 };
        Assert.AreEqual(1, a.Count);

        a.Clear();
        Assert.AreEqual(0, a.Count);
    }

    [TestMethod]
    public void Contains()
    {
        var v = 1;

        var a = new JsonArray { v };

        Assert.IsFalse(a.Contains(2));
        Assert.IsFalse(a.Contains(null));
        Assert.IsTrue(a.Contains(v));
    }

    [TestMethod]
    public void Remove()
    {
        object v = 1;
        var j = new JsonArray { v };

        Assert.AreEqual(1, j.Count);

        Assert.AreEqual(false, j.Remove(2));
        Assert.AreEqual(false, j.Remove(null));
        Assert.AreEqual(true, j.Remove(v));
        Assert.AreEqual(false, j.Remove(v));

        Assert.AreEqual(0, j.Count);
    }

    [TestMethod]
    public void IndexOf()
    {
        object v1 = 1;
        object v2 = 2;

        var j = new JsonArray { v1 };

        Assert.AreEqual(0, j.IndexOf(v1));

        j.Add(v2);
        Assert.AreEqual(0, j.IndexOf(v1));
        Assert.AreEqual(1, j.IndexOf(v2));
    }

    [TestMethod]
    public void RemoveAt()
    {
        object v1 = 1;
        object v2 = 2;
        object v3 = 3;

        var j = new JsonArray
        {
            v1,
            v2,
            v3
        };

        Assert.AreEqual(true, j.Contains(v1));
        j.RemoveAt(0);
        Assert.AreEqual(false, j.Contains(v1));

        Assert.AreEqual(true, j.Contains(v3));
        j.RemoveAt(1);
        Assert.AreEqual(false, j.Contains(v3));

        Assert.AreEqual(1, j.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), @"Index was out of range. Must be non-negative and less than the size of the collection. Parameter name: index")]
    public void RemoveAtOutOfRangeIndexShouldBeError()
    {
        new JsonArray().RemoveAt(0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), @"Index was out of range. Must be non-negative and less than the size of the collection. Parameter name: index")]
    public void RemoveNegativeIndexShouldBeError()
    {
        new JsonArray().RemoveAt(-1);
    }

    [TestMethod]
    public void Insert()
    {
        object v1 = 1;
        object v2 = 2;
        object v3 = 3;
        object v4 = 4;

        var j = new JsonArray
        {
            v1,
            v2,
            v3
        };

        j.Insert(1, v4);

        Assert.AreEqual(0, j.IndexOf(v1));
        Assert.AreEqual(1, j.IndexOf(v4));
        Assert.AreEqual(2, j.IndexOf(v2));
        Assert.AreEqual(3, j.IndexOf(v3));
    }

    [TestMethod]
    public void InsertShouldInsertAtZeroIndex()
    {
        object v1 = 1;
        object v2 = 2;

        var j = new JsonArray();

        j.Insert(0, v1);
        Assert.AreEqual(0, j.IndexOf(v1));

        j.Insert(0, v2);
        Assert.AreEqual(1, j.IndexOf(v1));
        Assert.AreEqual(0, j.IndexOf(v2));
    }

    [TestMethod]
    public void InsertNull()
    {
        var j = new JsonArray();
        j.Insert(0, null);

        Assert.AreEqual(null, j[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException),
        @"Index must be within the bounds of the List.
Parameter name: index")]
    public void InsertNegativeIndexShouldThrow()
    {
        new JsonArray().Insert(-1, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), @"Index must be within the bounds of the List.
Parameter name: index")]
    public void InsertOutOfRangeIndexShouldThrow()
    {
        new JsonArray().Insert(2, 1);
    }

    [TestMethod]
    public void Item()
    {
        object v1 = 1;
        object v2 = 2;
        object v3 = 3;
        object v4 = 4;

        var j = new JsonArray
        {
            v1,
            v2,
            v3
        };

        j[1] = v4;

        Assert.AreEqual(-1, j.IndexOf(v2));
        Assert.AreEqual(1, j.IndexOf(v4));
    }

    [TestMethod]
    public void Iterate()
    {
        var a = new JsonArray { 1, 2, 3, 4, 5 };

        var i = 1;
        foreach (var o in a)
        {
            Assert.AreEqual(i, o);
            ++i;
        }
    }
}