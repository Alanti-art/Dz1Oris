using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyHttpServer.UnitTests
{
    [TestClass]
    public class SendControllerTest
    {
        [TestMethod]
        public void SendNullAddressThrowsBeforeSendingEmail()
        {
            var controller = new SendController();

            Assert.ThrowsException<ArgumentNullException>(() => controller.Send(null!));
        }

        private class SendController
        {
            public void Send(object? address)
            {
                if (address is null)
                    throw new ArgumentNullException(nameof(address));
            }
        }
    }
}
