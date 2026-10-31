using MyHttpServer.Applications;

namespace MyHttpServer.UnitTests
{
    [TestClass]
    public class ApplicationStoreTests
    {
        [TestMethod]
        public void Create_SetsStatusPending()
        {
            ApplicationStore store = new ApplicationStore();

            Application app = store.Create("Ivan", "backend");

            Assert.AreEqual("pending", app.Status);
        }

        [TestMethod]
        public void Create_GivesDifferentIds()
        {
            ApplicationStore store = new ApplicationStore();

            Application a = store.Create("Ivan", "backend");
            Application b = store.Create("Petr", "design");

            Assert.AreNotEqual(a.Id, b.Id);
        }

        [TestMethod]
        public void Create_ShortName_ReturnsNull()
        {
            ApplicationStore store = new ApplicationStore();

            Assert.IsNull(store.Create("I", "backend"));
        }

        [TestMethod]
        public void Create_LongName_ReturnsNull()
        {
            ApplicationStore store = new ApplicationStore();

            Assert.IsNull(store.Create(new string('a', 61), "backend"));
        }

        [TestMethod]
        public void Create_UnknownDirection_ReturnsNull()
        {
            ApplicationStore store = new ApplicationStore();

            Assert.IsNull(store.Create("Ivan", "cooking"));
        }

        [TestMethod]
        public void Update_ChangesAllFields()
        {
            ApplicationStore store = new ApplicationStore();
            Application app = store.Create("Ivan", "backend");

            bool ok = store.Update(app.Id, "Petr", "design", "approved");

            Assert.IsTrue(ok);
            Assert.AreEqual("Petr", app.Applicant);
            Assert.AreEqual("design", app.Direction);
            Assert.AreEqual("approved", app.Status);
        }

        [TestMethod]
        public void Update_WrongStatus_ReturnsFalse()
        {
            ApplicationStore store = new ApplicationStore();
            Application app = store.Create("Ivan", "backend");

            Assert.IsFalse(store.Update(app.Id, "Ivan", "backend", "rejected"));
        }

        [TestMethod]
        public void Find_FiltersByDirection()
        {
            ApplicationStore store = new ApplicationStore();
            store.Create("Ivan", "backend");
            store.Create("Petr", "design");

            Assert.AreEqual(1, store.Find("backend").Count);
            Assert.AreEqual(2, store.Find("").Count);
        }
    }
}
