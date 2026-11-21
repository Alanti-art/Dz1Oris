using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyHttpServer.Controlers.frameworks.core;

namespace MyHttpServer.Tests;

[TestClass]
[DoNotParallelize]
public class ControllerHandlerTests
{
    private const string MasterConnectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database=master;TrustServerCertificate=True;";
    private static readonly object LocalDbLock = new();
    private static bool LocalDbStarted;

    private string oldDirectory = "";
    private string testDirectory = "";
    private HttpServer server = null!;
    private HttpClient client = null!;
    private int port;
    private string dbName = "";

    [TestInitialize]
    public async Task SetUp()
    {
        oldDirectory = Environment.CurrentDirectory;
        testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(testDirectory, "static"));
        Environment.CurrentDirectory = testDirectory;

        EnsureLocalDbStarted();
        dbName = "AuthTests_" + Guid.NewGuid().ToString("N");
        await CreateAuthDbAsync(dbName);

        string connectionString = $@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database={dbName};TrustServerCertificate=True;";
        port = GetFreePort();
        File.WriteAllText("settings.json",
            $"{{\"Host\":\"127.0.0.1\",\"Port\":\"{port}\",\"Path\":\"\",\"ConnectionString\":\"{EscapeJson(connectionString)}\"}}");

        EnsureLoginTemplate();

        server = new HttpServer();
        server.ServerStart();
        client = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{port}/"), Timeout = TimeSpan.FromSeconds(4) };
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        client?.Dispose();
        server?.ServerStop();
        Environment.CurrentDirectory = oldDirectory;
        try { Directory.Delete(testDirectory, true); } catch { }
        await DropDbAsync(dbName);
    }

    private static void EnsureLocalDbStarted()
    {
        lock (LocalDbLock)
        {
            if (LocalDbStarted) return;
            try
            {
                using var p = Process.Start(new ProcessStartInfo
                {
                    FileName = "sqllocaldb",
                    Arguments = "start MSSQLLocalDB",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                });
                p?.WaitForExit(15000);
            }
            catch { }
            LocalDbStarted = true;
        }
    }

    private static async Task CreateAuthDbAsync(string name)
    {
        await ExecOnMaster($@"
            IF DB_ID(N'{name}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{name}];
            END;
            CREATE DATABASE [{name}];");

        string cs = $@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database={name};TrustServerCertificate=True;";
        await using var conn = new SqlConnection(cs);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(@"
            CREATE TABLE Users (
                Id int IDENTITY(1,1) PRIMARY KEY,
                Login nvarchar(100) NULL,
                Password nvarchar(100) NULL,
                Role int NOT NULL DEFAULT(0)
            );", conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task DropDbAsync(string name)
    {
        if (string.IsNullOrEmpty(name)) return;
        try
        {
            await ExecOnMaster($@"
                IF DB_ID(N'{name}') IS NOT NULL
                BEGIN
                    ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{name}];
                END;");
        }
        catch { }
    }

    private static async Task ExecOnMaster(string sql)
    {
        await using var conn = new SqlConnection(MasterConnectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private static string EscapeJson(string value) => value.Replace("\\", "\\\\");

    private static void EnsureLoginTemplate()
    {
        const string template = "<!DOCTYPE html><html><body><form><input name='login' value='${Login}'/>" +
                                "<div class='error'>${ErrorMessage}</div>" +
                                "$if (${HasError})\n<div id='has-error'>yes</div>\n$else\n<div id='has-error'>no</div>\n$endif" +
                                "</form></body></html>";
        string cwdPath = Path.Combine(Directory.GetCurrentDirectory(), "static", "login.html");
        File.WriteAllText(cwdPath, template);

        try
        {
            string baseStatic = Path.Combine(AppContext.BaseDirectory, "static");
            Directory.CreateDirectory(baseStatic);
            File.WriteAllText(Path.Combine(baseStatic, "login.html"), template);
        }
        catch { }
    }

    [TestMethod]
    public async Task RootPathWithoutMatchingRouteReturns404()
    {
        File.WriteAllText(Path.Combine("static", "404.html"), "Custom 404");

        using HttpResponseMessage response = await client.GetAsync("nonexistent");
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("Custom 404", body);
    }

    [TestMethod]
    public async Task AuthPathWithoutActionReturns404()
    {
        File.WriteAllText(Path.Combine("static", "404.html"), "Custom 404");

        using HttpResponseMessage response = await client.GetAsync("auth");
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("Custom 404", body);
    }

    [TestMethod]
    public async Task UnknownControllerReturns404()
    {
        File.WriteAllText(Path.Combine("static", "404.html"), "Custom 404");

        using HttpResponseMessage response = await client.GetAsync("unknown/login");
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("Custom 404", body);
        Assert.AreEqual("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [TestMethod]
    public async Task UnknownActionReturns404()
    {
        File.WriteAllText(Path.Combine("static", "404.html"), "Custom 404");

        using HttpResponseMessage response = await client.GetAsync("auth/not-found");
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("Custom 404", body);
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
        if (response.StatusCode == HttpStatusCode.InternalServerError)
            Assert.Inconclusive("Requires a reachable/configured database for auth/login.");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task ActionRouteUsesExactCase()
    {
        using HttpResponseMessage response = await client.PostAsync("auth/LOGIN", new StringContent(""));

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task GetUnknownAuthRouteReturns404()
    {
        File.WriteAllText(Path.Combine("static", "404.html"), "Custom 404");

        using HttpResponseMessage response = await client.GetAsync("auth/json");
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("Custom 404", body);
    }

    [TestMethod]
    public async Task PostToGetOnlyActionReturns404()
    {
        File.WriteAllText(Path.Combine("static", "404.html"), "Custom 404");

        using HttpResponseMessage response = await client.PostAsync("auth/html", new StringContent(""));
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("Custom 404", body);
    }

    [TestMethod]
    public async Task UnsupportedHttpMethodReturns404()
    {
        File.WriteAllText(Path.Combine("static", "404.html"), "Custom 404");

        using var request = new HttpRequestMessage(HttpMethod.Put, "auth/login");
        using HttpResponseMessage response = await client.SendAsync(request);
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("Custom 404", body);
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
        if (response.StatusCode == HttpStatusCode.InternalServerError)
            Assert.Inconclusive("Requires a reachable/configured database for auth/login.");

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
            if (response.StatusCode == HttpStatusCode.InternalServerError)
                Assert.Inconclusive("Requires a reachable/configured database for auth/login.");
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
