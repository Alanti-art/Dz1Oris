using MimeKit;
using MyHttpServer.Controlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.UnitTests
{

    [TestClass]
    public class SendControllerTest
    {
        [TestClass]
        public class SendControllerTests
        {
            [TestMethod]
            public void SendNullAddressThrowsBeforeSendingEmail()
            {
                var controller = new SendController();

                Assert.ThrowsException<ArgumentNullException>(() => controller.Send(null!));
            }
        }
    }

}
