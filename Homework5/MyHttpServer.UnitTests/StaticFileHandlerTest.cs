using MyHttpServer.frameworks.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class StaticFileHandlerTests
    {
        private string oldDirectory = "";
        private string testDirectory = "";
        private HttpServer server = null!;
        private HttpClient client = null!;
        private int port;

        [TestInitialize]
        public void SetUp()
        {
            oldDirectory = Environment.CurrentDirectory;
            testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(testDirectory, "static"));
            Environment.CurrentDirectory = testDirectory;

            port = GetFreePort();
            File.WriteAllText("settings.json", $"{{\"Host\":\"127.0.0.1\",\"Port\":\"{port}\",\"Path\":\"\"}}");
            File.WriteAllText(Path.Combine("static", "style.css"), "body { color: red; }");
            File.WriteAllText(Path.Combine("static", "404.html"), "Not found");
            server = new HttpServer();
            server.ServerStart();
            client = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{port}/") };
        }

        [TestCleanup]
        public void CleanUp()
        {
            client.Dispose();
            server.ServerStop();
            Environment.CurrentDirectory = oldDirectory;
            Directory.Delete(testDirectory, true);
        }

        [TestMethod]
        public async Task ExistingCssFileReturnsFileAndContentType()
        {
            using HttpResponseMessage response = await client.GetAsync("style.css");
            string body = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("text/css", response.Content.Headers.ContentType!.MediaType);
            Assert.AreEqual("body { color: red; }", body);
        }

        [TestMethod]
        public async Task MissingHtmlFileReturns404Page()
        {
            using HttpResponseMessage response = await client.GetAsync("missing.html");
            string body = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual("Not found", body);
        }

        [TestMethod]
        public async Task PathWithoutExtensionGoesToNextHandler()
        {
            using HttpResponseMessage response = await client.GetAsync("noextension");

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private static int GetFreePort()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }
    }

}
