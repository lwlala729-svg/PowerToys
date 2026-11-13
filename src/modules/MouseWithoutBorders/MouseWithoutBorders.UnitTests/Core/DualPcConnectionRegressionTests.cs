// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MouseWithoutBorders.Class;
using MouseWithoutBorders.Core;

namespace MouseWithoutBorders.UnitTests.Core;

[TestClass]
public sealed class DualPcConnectionRegressionTests
{
    [DataTestMethod]
    [DataRow((int)SocketStatus.Resolving, true)]
    [DataRow((int)SocketStatus.Connecting, true)]
    [DataRow((int)SocketStatus.Handshaking, true)]
    [DataRow((int)SocketStatus.Connected, true)]
    [DataRow((int)SocketStatus.NA, false)]
    [DataRow((int)SocketStatus.Error, false)]
    [DataRow((int)SocketStatus.Timeout, false)]
    [DataRow((int)SocketStatus.ForceClosed, false)]
    [DataRow((int)SocketStatus.InvalidKey, false)]
    public void ClientReconnectDoesNotStartDuringInFlightConnection(int status, bool expectedActive)
    {
        Assert.AreEqual(expectedActive, Common.IsActiveClientSocketStatus((SocketStatus)status));
    }

    [TestMethod]
    public void NameToIpMappingKeepsPolicyAndUserEntriesSeparated()
    {
        var mappings = SocketStuff.GetName2IpMappingLines(
            "POLICYHOST 198.51.100.20", "TARGETHOST 192.0.2.10");
        CollectionAssert.AreEqual(
            new[] { "POLICYHOST 198.51.100.20", "TARGETHOST 192.0.2.10" },
            mappings);
    }

    [TestMethod]
    public void NameToIpMappingAcceptsOnlyUserRules()
    {
        var mappings = SocketStuff.GetName2IpMappingLines(
            string.Empty, "TARGETHOST 192.0.2.10");
        CollectionAssert.AreEqual(
            new[] { "TARGETHOST 192.0.2.10" },
            mappings);
    }
}
