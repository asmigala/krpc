using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using KRPC.Server.Net;

namespace KRPC.Test.Server.Net
{
    [TestFixture]
    public class NetworkInformationTest
    {
        [Test]
        public void NetworkAdapters ()
        {
            List<IPAddress> addresses = NetworkInformation.GetLocalIPAddresses ().ToList ();
            Assert.IsTrue (addresses.Contains (IPAddress.Loopback));
            foreach (var address in addresses) {
                Console.WriteLine (address);
            }
        }

        [Test]
        [Ignore]
        public void GetLoopbackSubnetMask ()
        {
            Assert.AreEqual ("", NetworkInformation.GetSubnetMask (IPAddress.Loopback).ToString ());
        }
    }
}