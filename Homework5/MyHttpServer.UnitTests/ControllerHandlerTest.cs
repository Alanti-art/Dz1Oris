using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyHttpServer.frameworks.core;

namespace MyHttpServer.Tests;

[TestClass]
[DoNotParallelize]
public class ControllerHandlerTests
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

        server = new HttpServer();
        server.ServerStart();
        client = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{port}/"), Timeout = TimeSpan.FromSeconds(4) };
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
    public async Task RootPathReturns400()
    {
        using HttpResponseMessage response = await client.GetAsync("");

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task AuthPathWithoutActionReturns400()
    {
        using HttpResponseMessage response = await client.GetAsync("auth");

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task UnknownControllerReturns404()
    {
        using HttpResponseMessage response = await client.GetAsync("unknown/login");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task UnknownActionReturns404()
    {
        using HttpResponseMessage response = await client.GetAsync("auth/not-found");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task ControllerRouteIgnoresCase()
    {
        var form = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("login", "test"),
            new KeyValuePair<string, string>("password", "123")
        });

        using HttpResponseMessage response = await client.PostAsync("AUTH/login", form);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task ActionRouteUsesExactCase()
    {
        using HttpResponseMessage response = await client.PostAsync("auth/LOGIN", new StringContent(""));

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task GetRouteIsFoundAndReturnsEmptyBody()
    {
        using HttpResponseMessage response = await client.GetAsync("auth/json");
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("", body);
    }

    [TestMethod]
    public async Task PostToGetOnlyActionReturns404()
    {
        using HttpResponseMessage response = await client.PostAsync("auth/html", new StringContent(""));

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task UnsupportedHttpMethodReturns404()
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, "auth/login");
        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task ExtraPathSegmentDoesNotChangeActionRoute()
    {
        var form = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("login", "test"),
            new KeyValuePair<string, string>("password", "123")
        });

        using HttpResponseMessage response = await client.PostAsync("auth/login/extra", form);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task QueryStringDoesNotReplaceFormData()
    {
        var form = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("login", "from-form"),
            new KeyValuePair<string, string>("password", "form-password")
        });

        string output = await PostAndCaptureOutput("auth/login?login=from-query&password=query-password", form);

        StringAssert.Contains(output, "Получен login: from-form; пароль form-password.");
    }

    [DataTestMethod]
    [DataRow("login=alice&password=secret", "Получен login: alice; пароль secret.")]
    [DataRow("login=Alice+Smith&password=a%2Bb", "Получен login: Alice Smith; пароль a+b.")]
    [DataRow("login=first&login=last&password=secret", "Получен login: last; пароль secret.")]
    [DataRow("login=&password=", "Получен login: ; пароль .")]
    [DataRow("login&password=secret", "Получен login: ; пароль secret.")]
    [DataRow("login=a=b&password=secret", "Получен login: ; пароль secret.")]
    [DataRow("", "Получен login: ; пароль .")]
    public async Task PostFormIsParsedAsExpected(string formBody, string expectedOutput)
    {
        using var form = new StringContent(formBody, Encoding.UTF8, "application/x-www-form-urlencoded");

        string output = await PostAndCaptureOutput("auth/login", form);

        StringAssert.Contains(output, expectedOutput);
    }

    [TestMethod]
    public async Task PostBodyIsParsedEvenWithJsonContentType_CurrentBehavior()
    {
        using var body = new StringContent("login=sent&password=as-text", Encoding.UTF8, "application/json");

        string output = await PostAndCaptureOutput("auth/login", body);

        StringAssert.Contains(output, "Получен login: sent; пароль as-text.");
    }

    private async Task<string> PostAndCaptureOutput(string path, HttpContent content)
    {
        TextWriter oldOutput = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            using HttpResponseMessage response = await client.PostAsync(path, content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        finally
        {
            Console.SetOut(oldOutput);
        }

        return output.ToString();
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}
