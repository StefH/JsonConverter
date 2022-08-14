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

using JsonConverter.SimpleJson;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
#endif
#endif

    [TestClass]
    public class ToStringTests
    {
        [TestMethod]
        public void ToStringCallingSerializeObjectOnItself()
        {
            var x = new X { Y = "z" };

            var json = JsonConverter.SimpleJson.SimpleJson.SerializeObject(x);

            Assert.AreEqual("{\"Y\":\"z\"}", json);
        }

        class X
        {
            public string Y { get; set; }

            public override string ToString()
            {
                return JsonConverter.SimpleJson.SimpleJson.SerializeObject(this, new PocoJsonSerializerStrategy())!;
            }
        }
    }
}