using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyHttpServer.frameworks.core;

namespace MyHttpServer.Tests;

[TestClass]
[DoNotParallelize]
public class HttpServerTests
{
    private string oldDirectory = "";
    private string testDirectory = "";
    private HttpServer? server;
    private int port;
    private bool serverStarted;

    [TestInitialize]
    public void SetUp()
    {
        oldDirectory = Environment.CurrentDirectory;
        testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(testDirectory, "static"));
        Environment.CurrentDirectory = testDirectory;

        port = GetFreePort();
        WriteSettings(port);
    }

    [TestCleanup]
    public void CleanUp()
    {
        if (serverStarted)
        {
            server!.ServerStop();
        }

        Environment.CurrentDirectory = oldDirectory;
        Directory.Delete(testDirectory, true);
    }

    [TestMethod]
    public void ConstructorWithMissingSettingsThrowsFileNotFoundException()
    {
        File.Delete("settings.json");

        Assert.ThrowsException<FileNotFoundException>(() => new HttpServer());
    }

    [TestMethod]
    public void ConstructorWithInvalidJsonThrowsJsonException()
    {
        File.WriteAllText("settings.json", "{");

        Assert.ThrowsException<JsonException>(() => new HttpServer());
    }

    [TestMethod]
    public async Task ServerStartsAndHandlesRequest()
    {
        StartServer();

        using var client = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{port}/") };
        using HttpResponseMessage response = await client.GetAsync("auth");

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task ServerHandlesMoreThanOneRequest()
    {
        StartServer();

        using var client = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{port}/") };
        using HttpResponseMessage firstResponse = await client.GetAsync("auth");
        using HttpResponseMessage secondResponse = await client.GetAsync("auth");

        Assert.AreEqual(HttpStatusCode.BadRequest, firstResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [TestMethod]
    public async Task StoppedServerDoesNotAcceptNewRequests()
    {
        StartServer();
        server!.ServerStop();
        serverStarted = false;

        using var client = new HttpClient
        {
            BaseAddress = new Uri($"http://127.0.0.1:{port}/"),
            Timeout = TimeSpan.FromSeconds(2)
        };

        try
        {
            using HttpResponseMessage response = await client.GetAsync("auth");
            Assert.Fail("Сервер ответил после остановки.");
        }
        catch (HttpRequestException)
        {
            // Соединение отклонено после остановки сервера.
        }
        catch (TaskCanceledException)
        {
            // Запрос не получил ответ до таймаута клиента.
        }
    }

    private void StartServer()
    {
        server = new HttpServer();
        server.ServerStart();
        serverStarted = true;
    }

    private static void WriteSettings(int port)
    {
        File.WriteAllText("settings.json",
            $"{{\"Host\":\"127.0.0.1\",\"Port\":\"{port}\",\"Path\":\"\"}}");
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}
