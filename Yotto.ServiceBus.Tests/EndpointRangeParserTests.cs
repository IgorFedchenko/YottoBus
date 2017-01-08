using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Yotto.ServiceBus.Proxy.Helpers;

namespace Yotto.ServiceBus.Tests
{
    [TestFixture]
    public class EndpointRangeParserTests
    {
        [Test]
        public void ShouldParseSimpleEndpoints()
        {
            string ip = "192.168.0.1";
            int port = 8080;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port);

            List<IPEndPoint> result = EndpointsRangeParser.Parse(ip + ":" + port);

            Assert.AreEqual(endpoint, result.First());
        }

        [Test]
        public void ShouldParseRanges()
        {
            string ip = "192.168.[0-1].1";
            string port = "[8080-8081]";
            List<IPEndPoint> endpoints = new List<IPEndPoint>
            {
                 new IPEndPoint(IPAddress.Parse("192.168.0.1"), 8080),
                 new IPEndPoint(IPAddress.Parse("192.168.0.1"), 8081),
                 new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8080),
                 new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8081),
            };

            List<IPEndPoint> result = EndpointsRangeParser.Parse(ip + ":" + port);

            CollectionAssert.AreEquivalent(endpoints, result);
        }

        [Test]
        public void ShouldParseLists()
        {
            string ip = "192.168.[0,1].1";
            string port = "[8080,8081]";
            List<IPEndPoint> endpoints = new List<IPEndPoint>
            {
                 new IPEndPoint(IPAddress.Parse("192.168.0.1"), 8080),
                 new IPEndPoint(IPAddress.Parse("192.168.0.1"), 8081),
                 new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8080),
                 new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8081),
            };

            List<IPEndPoint> result = EndpointsRangeParser.Parse(ip + ":" + port);

            CollectionAssert.AreEquivalent(endpoints, result);
        }

        [Test]
        public void ShouldParseComplexPatterns()
        {
            string ip = "192.168.[0,1-2].1";
            string port = "[8080-8082,8081]";
            List<IPEndPoint> endpoints = new List<IPEndPoint>
            {
                 new IPEndPoint(IPAddress.Parse("192.168.0.1"), 8080),
                 new IPEndPoint(IPAddress.Parse("192.168.0.1"), 8081),
                 new IPEndPoint(IPAddress.Parse("192.168.0.1"), 8082),
                 new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8080),
                 new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8081),
                 new IPEndPoint(IPAddress.Parse("192.168.1.1"), 8082),
                 new IPEndPoint(IPAddress.Parse("192.168.2.1"), 8080),
                 new IPEndPoint(IPAddress.Parse("192.168.2.1"), 8081),
                 new IPEndPoint(IPAddress.Parse("192.168.2.1"), 8082),
            };

            List<IPEndPoint> result = EndpointsRangeParser.Parse(ip + ":" + port);

            CollectionAssert.AreEquivalent(endpoints, result);
        }

        [Test]
        public void ShouldValidateInput()
        {
            string invalidIp = "192.168.0.255";
            string validIp = "192.168.0.254";

            Assert.Throws<ArgumentException>(() => EndpointsRangeParser.Parse(invalidIp + ":2555"));
            Assert.Throws<ArgumentException>(() => EndpointsRangeParser.Parse(validIp + ":0"));
        }

        [Test]
        public void ShouldAcceptLocalhost()
        {
            string localhostEndpoint = "localhost:8888";

            CollectionAssert.AreEqual(new List<IPEndPoint>() { new IPEndPoint(IPAddress.Loopback, 8888) }, EndpointsRangeParser.Parse(localhostEndpoint));
        }
    }
}
