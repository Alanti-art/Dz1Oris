using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyHttpServer.frameworks.core;

namespace MyHttpServer.Tests;

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
        File.WriteAllText(Path.Combine("static", "404.html"), "Not found");

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
    public async Task ExistingCssFileReturnsFileAndContentType()
    {
        string expected = "body { color: red; }";
        File.WriteAllText(Path.Combine("static", "style.css"), expected);

        using HttpResponseMessage response = await client.GetAsync("style.css");
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("text/css", response.Content.Headers.ContentType!.MediaType);
        Assert.AreEqual(expected, body);
        Assert.AreEqual(expected.Length, response.Content.Headers.ContentLength);
    }

    [TestMethod]
    public async Task EmptyFileReturnsEmptyBody()
    {
        File.WriteAllBytes(Path.Combine("static", "empty.txt"), Array.Empty<byte>());

        using HttpResponseMessage response = await client.GetAsync("empty.txt");
        byte[] body = await response.Content.ReadAsByteArrayAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(0, body.Length);
    }

    [TestMethod]
    public async Task BinaryFileReturnsSameBytes()
    {
        byte[] expected = { 0, 1, 127, 128, 255 };
        File.WriteAllBytes(Path.Combine("static", "image.bin"), expected);

        using HttpResponseMessage response = await client.GetAsync("image.bin");
        byte[] actual = await response.Content.ReadAsByteArrayAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task LargeFileReturnsSameBytes()
    {
        byte[] expected = new byte[1024 * 1024];
        new Random(1).NextBytes(expected);
        File.WriteAllBytes(Path.Combine("static", "large.bin"), expected);

        using HttpResponseMessage response = await client.GetAsync("large.bin");
        byte[] actual = await response.Content.ReadAsByteArrayAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task QueryStringDoesNotChangeStaticFilePath()
    {
        File.WriteAllText(Path.Combine("static", "style.css"), "css content");

        using HttpResponseMessage response = await client.GetAsync("style.css?version=1");
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("css content", body);
    }

    [TestMethod]
    public async Task DotInDirectoryNameStillServesFile()
    {
        Directory.CreateDirectory(Path.Combine("static", "assets.v1"));
        File.WriteAllText(Path.Combine("static", "assets.v1", "file.txt"), "nested file");

        using HttpResponseMessage response = await client.GetAsync("assets.v1/file.txt");
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("nested file", body);
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

    [DataTestMethod]
    [DataRow(".html", "text/html")]
    [DataRow(".htm", "text/html")]
    [DataRow(".css", "text/css")]
    [DataRow(".js", "text/javascript")]
    [DataRow(".mjs", "text/javascript")]
    [DataRow(".json", "application/json")]
    [DataRow(".map", "application/json")]
    [DataRow(".xml", "application/xml")]
    [DataRow(".txt", "text/plain")]
    [DataRow(".csv", "text/csv")]
    [DataRow(".md", "text/markdown")]
    [DataRow(".png", "image/png")]
    [DataRow(".jpg", "image/jpeg")]
    [DataRow(".jpeg", "image/jpeg")]
    [DataRow(".jpe", "image/jpeg")]
    [DataRow(".jfif", "image/jpeg")]
    [DataRow(".gif", "image/gif")]
    [DataRow(".svg", "image/svg+xml")]
    [DataRow(".ico", "image/x-icon")]
    [DataRow(".bmp", "image/bmp")]
    [DataRow(".webp", "image/webp")]
    [DataRow(".avif", "image/avif")]
    [DataRow(".tif", "image/tiff")]
    [DataRow(".tiff", "image/tiff")]
    [DataRow(".woff", "font/woff")]
    [DataRow(".woff2", "font/woff2")]
    [DataRow(".ttf", "font/ttf")]
    [DataRow(".otf", "font/otf")]
    [DataRow(".eot", "application/vnd.ms-fontobject")]
    [DataRow(".mp3", "audio/mpeg")]
    [DataRow(".wav", "audio/wav")]
    [DataRow(".ogg", "audio/ogg")]
    [DataRow(".oga", "audio/ogg")]
    [DataRow(".m4a", "audio/mp4")]
    [DataRow(".flac", "audio/flac")]
    [DataRow(".mp4", "video/mp4")]
    [DataRow(".webm", "video/webm")]
    [DataRow(".ogv", "video/ogg")]
    [DataRow(".avi", "video/x-msvideo")]
    [DataRow(".mov", "video/quicktime")]
    [DataRow(".mkv", "video/x-matroska")]
    [DataRow(".pdf", "application/pdf")]
    [DataRow(".zip", "application/zip")]
    [DataRow(".gz", "application/gzip")]
    [DataRow(".7z", "application/x-7z-compressed")]
    [DataRow(".rar", "application/vnd.rar")]
    [DataRow(".wasm", "application/wasm")]
    [DataRow(".webmanifest", "application/manifest+json")]
    [DataRow(".doc", "application/msword")]
    [DataRow(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [DataRow(".xls", "application/vnd.ms-excel")]
    [DataRow(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [DataRow(".ppt", "application/vnd.ms-powerpoint")]
    [DataRow(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation")]
    public async Task ExtensionSetsExpectedContentType(string extension, string expectedType)
    {
        string fileName = "sample" + extension;
        File.WriteAllText(Path.Combine("static", fileName), "test content");

        using HttpResponseMessage response = await client.GetAsync(fileName);
        string body = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(expectedType, response.Content.Headers.ContentType!.MediaType);
        Assert.AreEqual("test content", body);
    }

    [TestMethod]
    public async Task MissingFileWithout404PageStillReturns404()
    {
        File.Delete(Path.Combine("static", "404.html"));

        using HttpResponseMessage response = await client.GetAsync("missing.html");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task UnknownExtensionUsesBinaryContentType()
    {
        File.WriteAllText(Path.Combine("static", "file.unknown"), "content");

        using HttpResponseMessage response = await client.GetAsync("file.unknown");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("application/octet-stream", response.Content.Headers.ContentType?.MediaType);
    }

    [TestMethod]
    public async Task UppercaseExtensionUsesHtmlContentType()
    {
        File.WriteAllText(Path.Combine("static", "page.HTML"), "content");

        using HttpResponseMessage response = await client.GetAsync("page.HTML");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}
