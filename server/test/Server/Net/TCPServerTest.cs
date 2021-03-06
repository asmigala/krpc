using System.Net;
using System.Net.Sockets;
using System.Linq;
using NUnit.Framework;
using KRPC.Server.Net;
using System;

namespace KRPC.Test.Server.Net
{
    [TestFixture]
    public class TCPServerTest
    {
        [Test]
        public void Simple ()
        {
            bool serverStarted = false;
            bool serverStopped = false;
            var server = new TCPServer ("Test", IPAddress.Loopback, 0);
            server.OnStarted += (s, e) => serverStarted = true;
            server.OnStopped += (s, e) => serverStopped = true;
            Assert.IsFalse (server.Running);
            server.Start ();
            Assert.IsTrue (server.Running);
            Assert.AreEqual (0, server.Clients.Count ());
            Assert.AreEqual (IPAddress.Loopback, server.Address);
            Assert.IsTrue (server.Port > 0);
            server.Stop ();
            Assert.IsFalse (server.Running);
            Assert.AreEqual (0, server.Clients.Count ());
            Assert.IsTrue (serverStarted);
            Assert.IsTrue (serverStopped);
            Assert.AreEqual (0, server.BytesRead);
            Assert.AreEqual (0, server.BytesWritten);
        }

        [Test]
        public void StartStop ()
        {
            int serverStarted = 0;
            int serverStopped = 0;
            var server = new TCPServer ("Test", IPAddress.Loopback, 0);
            server.OnStarted += (s, e) => serverStarted++;
            server.OnStopped += (s, e) => serverStopped++;
            Assert.IsFalse (server.Running);
            for (int i = 0; i < 5; i++) {
                server.Start ();
                Assert.IsTrue (server.Running);
                server.Stop ();
                Assert.IsFalse (server.Running);
            }
            Assert.AreEqual (5, serverStarted);
            Assert.AreEqual (5, serverStopped);
            Assert.AreEqual (0, server.BytesRead);
            Assert.AreEqual (0, server.BytesWritten);
        }

        [Test]
        public void ClientConnectAndDisconnect ()
        {
            var server = new TCPServer ("Test", IPAddress.Loopback, 0);

            bool clientRequestingConnection = false;
            bool clientConnected = false;
            bool clientDisconnected = false;
            server.OnClientRequestingConnection += (s, e) => {
                e.Request.Allow ();
                clientRequestingConnection = true;
            };
            server.OnClientConnected += (s, e) => clientConnected = true;
            server.OnClientDisconnected += (s, e) => clientDisconnected = true;

            server.Start ();

            var tcpClient = new TcpClient (server.Address.ToString (), server.Port);
            UpdateUntil (server, () => clientConnected);

            Assert.IsTrue (tcpClient.Connected);
            Assert.IsFalse (clientDisconnected);
            Assert.AreEqual (1, server.Clients.Count ());

            tcpClient.GetStream ().Close ();
            tcpClient.Close ();
            Assert.IsFalse (tcpClient.Connected);
            UpdateUntil (server, () => clientDisconnected);
            Assert.AreEqual (0, server.Clients.Count ());

            Assert.IsFalse (tcpClient.Connected);
            server.Stop ();

            Assert.IsTrue (clientRequestingConnection);
            Assert.IsTrue (clientConnected);
            Assert.IsTrue (clientDisconnected);

            Assert.AreEqual (0, server.BytesRead);
            Assert.AreEqual (0, server.BytesWritten);
        }

        [Test]
        public void StillPendingByDefault ()
        {
            var server = new TCPServer ("Test", IPAddress.Loopback, 0);

            bool clientRequestingConnection = false;
            server.OnClientRequestingConnection += (s, e) => clientRequestingConnection = true;
            server.OnClientConnected += (s, e) => Assert.Fail ();
            server.OnClientDisconnected += (s, e) => Assert.Fail ();

            server.Start ();

            var tcpClient = new TcpClient (server.Address.ToString (), server.Port);
            UpdateUntil (server, () => clientRequestingConnection);

            Assert.IsTrue (clientRequestingConnection);
            Assert.AreEqual (0, server.Clients.Count ());

            server.Stop ();
            tcpClient.Close ();

            Assert.AreEqual (0, server.BytesRead);
            Assert.AreEqual (0, server.BytesWritten);
        }

        [Test]
        public void StopDisconnectsClient ()
        {
            var server = new TCPServer ("Test", IPAddress.Loopback, 0);

            bool clientConnected = false;
            bool clientDisconnected = false;
            server.OnClientRequestingConnection += (s, e) => e.Request.Allow ();
            server.OnClientConnected += (s, e) => clientConnected = true;
            server.OnClientDisconnected += (s, e) => clientDisconnected = true;

            server.Start ();

            var tcpClient = new TcpClient (server.Address.ToString (), server.Port);
            UpdateUntil (server, () => clientConnected);

            Assert.IsFalse (clientDisconnected);
            Assert.AreEqual (1, server.Clients.Count ());

            server.Stop ();

            Assert.IsTrue (clientDisconnected);
            Assert.AreEqual (0, server.Clients.Count ());

            tcpClient.Close ();

            Assert.AreEqual (0, server.BytesRead);
            Assert.AreEqual (0, server.BytesWritten);
        }

        delegate bool BooleanPredicate ();
        // Calls server.Update repeatedly every 50 ms, until predicate is true
        // or up to a maximum number of iterations, after which point the test fails
        void UpdateUntil (KRPC.Server.IServer<byte, byte> server, BooleanPredicate predicate, int iterations = 10)
        {
            for (int i = 0; i < iterations; i++) {
                server.Update ();
                if (predicate ())
                    return;
                System.Threading.Thread.Sleep (50);
            }
            Assert.Fail ();
        }

        [Test]
        public void BindToAnyAddress ()
        {
            bool serverStarted = false;
            bool serverStopped = false;
            var server = new TCPServer ("Test", IPAddress.Any, 0);
            server.OnStarted += (s, e) => serverStarted = true;
            server.OnStopped += (s, e) => serverStopped = true;
            Assert.IsFalse (server.Running);
            server.Start ();
            Assert.IsTrue (server.Running);
            Assert.AreEqual (0, server.Clients.Count ());
            Assert.AreEqual (IPAddress.Any, server.Address);
            Assert.IsTrue (server.Port > 0);
            server.Stop ();
            Assert.IsFalse (server.Running);
            Assert.AreEqual (0, server.Clients.Count ());
            Assert.IsTrue (serverStarted);
            Assert.IsTrue (serverStopped);
            Assert.AreEqual (0, server.BytesRead);
            Assert.AreEqual (0, server.BytesWritten);
        }
    }
}

