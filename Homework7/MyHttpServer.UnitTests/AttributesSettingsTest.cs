using CustomHttpServer.Framework.Attributes;
using MyHttpServer.frameworks.Аttributes;

namespace MyHttpServer.UnitTests
{
    [TestClass]
    public sealed class AttributesTest
    {
        [TestMethod]
        public void GetAttribute_StoresRoute()
        {
            var attribute = new GETAttribute("login");

            Assert.AreEqual("login", attribute.Route);
        }
        [TestMethod]
        public void GetAttribute_StoresRouteEmty()
        {
            var attribute = new GETAttribute("");

            Assert.AreEqual("", attribute.Route);
        }


        [TestMethod]
        public void PostAttribute_StoresRoute()
        {
            var attribute = new POSTAttribute("submit");

            Assert.AreEqual("submit", attribute.Route);
        }

        [TestMethod]
        public void HttpControllerAttribute_StoresRoute()
        {
            var attribute = new HttpController("auth");

            Assert.AreEqual("auth", attribute.Route);
        }

        [TestMethod]
        public void FormDataAttribute_WithoutName_HasNullName()
        {
            var attribute = new FormDataAttribute();

            Assert.IsNull(attribute.Name);
        }

        [TestMethod]
        public void Settings_PropertiesAreNullByDefault_CurrentBehavior()
        {
            var settings = new Settings();

            Assert.IsNull(settings.Host);
            Assert.IsNull(settings.Port);
            Assert.IsNull(settings.Path);
        }

        [TestMethod]
        public void Settings_PropertiesCanBeSetAndRead()
        {
            var settings = new Settings
            {
                Host = "127.0.0.1",
                Port = "8888",
                Path = "connection"
            };

            Assert.AreEqual("127.0.0.1", settings.Host);
            Assert.AreEqual("8888", settings.Port);
            Assert.AreEqual("connection", settings.Path);
        }

        [TestMethod]
        public void Settings_AcceptsEmptyStrings_CurrentBehavior()
        {
            var settings = new Settings
            {
                Host = "",
                Port = "",
                Path = ""
            };

            Assert.AreEqual("", settings.Host);
            Assert.AreEqual("", settings.Port);
            Assert.AreEqual("", settings.Path);
        }
        [TestMethod]
        public void Attributes_CanBeDiscoveredByReflection()
        {
         
            var expectedController = new HttpController("probe");
            var expectedGet = new GETAttribute("read");
            var expectedPost = new POSTAttribute("write");

            var controllerType = typeof(AttributeProbeController);
            var controllerAttribute = Attribute.GetCustomAttribute(controllerType,typeof(HttpController)) as HttpController;

            var getMethod = controllerType.GetMethod(nameof(AttributeProbeController.Read));
            var getAttribute = getMethod?.GetCustomAttributes(typeof(GETAttribute), inherit: false).SingleOrDefault() as GETAttribute;

            var postMethod = controllerType.GetMethod(nameof(AttributeProbeController.Write));
            var postAttribute = postMethod?.GetCustomAttributes(typeof(POSTAttribute), inherit: false).SingleOrDefault() as POSTAttribute;

            Assert.IsNotNull(controllerAttribute);
            Assert.IsNotNull(getAttribute);
            Assert.IsNotNull(postAttribute);

            Assert.AreEqual(expectedController.Route, controllerAttribute.Route);
            Assert.AreEqual(expectedGet.Route, getAttribute.Route);
            Assert.AreEqual(expectedPost.Route, postAttribute.Route);
        }
        [HttpController("probe")]
        private sealed class AttributeProbeController
        {
            [GET("read")]
            public void Read() { }

            [POST("write")]
            public void Write() { }
        }
    }
}
