using ConsoleApp;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace MiniTemplateEngine
{
    [TestClass]
    public sealed class HtmlTemplateRenderTests
    {
        //[TestMethod]
        //public void RenderFromStringTest()
        //{
        //    string template = "<p>Имя: ${name}</p>" +
        //                      "<p>группа: ${group}</p>" +
        //                      "<p>возраст: ${age}</p>" +
        //                      "<p>пол: ${sex}</p>";
        //    var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
        //    string expectString = "<p>Имя: Sanya</p>" +
        //        "<p>группа: 11-541</p>" +
        //        "<p>возраст: 20</p>" +
        //        "<p>пол: True</p>";

        //    var serv = new HtmlTemplateRenderer();

        //    string res = serv.RenderFromString(template, user);

        //    Assert.AreEqual(expectString, res);
        //}

        //[TestMethod]
        //public void RenderFromStringTestWhenNullRetrnException()
        //{
        //    string template = "<p>Имя: ${name}</p>" +
        //                    "<p>группа: ${group}</p>" +
        //                    "<p>возраст: ${age}</p>" +
        //                    "<p>пол: ${sex}</p>";
        //    var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
        //    string expectString = "<p>Имя: Sanya</p>" +
        //        "<p>группа: 11-541</p>" +
        //        "<p>возраст: 20</p>" +
        //        "<p>пол: rue</p>";

        //    var serv = new HtmlTemplateRenderer();

        //    Assert.ThrowsException<ArgumentNullException>(() => serv.RenderFromString(null!, user));

        //}

        [TestMethod]
        public void RenderVarsUnitTest1()
        {
            string template = "<p>Имя: ${name}</p>" +
                              "<p>группа: ${group}</p>" +
                              "<p>возраст: ${age}</p>" +
                              "<p>пол: ${sex}</p>";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Имя: Sanya</p>" +
                "<p>группа: 11-541</p>" +
                "<p>возраст: 20</p>" +
                "<p>пол: True</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }


        [TestMethod]
        public void RenderVarsUnitTest2()
        {
            string template =
                   "<p>Размеры: ${size.Width}, ${size.Height}, ${size.Depth}</p>" +
                    "<p>Цвет: ${color.ColorName}</p>" +
                    "<p>Страна производства: ${madein.Name}</p>" +
                    "<p>Город: ${madein.City}</p>";

            Bag bag = new ConsoleApp.Bag("Дорожная сумка", 100, new Size(55, 40, 30), new Color("black"), new Madein(15, "Russia", "Ivanovo"));

            string expectString =
                       "<p>Размеры: 55, 40, 30</p>" +
                       "<p>Цвет: black</p>" +
                       "<p>Страна производства: Russia</p>" +
                       "<p>Город: Ivanovo</p>";


            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);

        }


        [TestMethod]
        public void RenderVarsUnitTest3()
        {
            string template = "$foreach (tag in tags)\n[${tag}]$endforeach";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 100, new Size(55, 40, 30), new Color("black"), new Madein(15, "Russia", "Ivanovo"));
            bag.Tags = new List<string> { "яблоко", "груша" };

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual("[яблоко][груша]", res);

        }
        [TestMethod]
        public void RenderVarsUnitTest4()
        {
            string template = "<p>Имя: ${name}</p>" +
                "<p>Имя: ${Name}</p>" +
                "<p>Имя: ${NAME}</p>";

            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Имя: Sanya</p>" +
                "<p>Имя: Sanya</p>" +
                "<p>Имя: Sanya</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderVarsUnitTest5()
        {
            string template = "<p>Имя: ${name}</p>" +
                             "<p>группа: ${group}</p>" +
                             "<p>возраст: ${age}</p>" +
                             "<p>пол: ${sex}</p>" +
                             "<p>как дела: ${business}</p>" +
                             "<p>как мама: ${good}</p>";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Имя: Sanya</p>" +
                "<p>группа: 11-541</p>" +
                "<p>возраст: 20</p>" +
                "<p>пол: True</p>" +
                "<p>как дела: ${business}</p>" +
                "<p>как мама: ${good}</p>";


            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }
        [TestMethod]
        public void RenderVarsUnitTest6()
        {
            string template = "<p>Имя: ${name}</p>" +
                              "<p>группа: ${group}</p>" +
                              "<p>возраст: ${age}</p>" +
                              "<p>пол: ${sex}</p>";
            var user = new ConsoleApp.User(null, "11-541", 20, true);
            string expectString = "<p>Имя: </p>" +
                "<p>группа: 11-541</p>" +
                "<p>возраст: 20</p>" +
                "<p>пол: True</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }
        [TestMethod]
        public void RenderVarsUnitTest7()
        {
            string template = "<p>Имя: ${aaaaaaaaa}</p>" +
                              "<p>группа: ${aaaaaaaaa}</p>" +
                              "<p>возраст: ${aaaaaaaaa}</p>" +
                              "<p>пол: ${aaaaaaaaa}</p>";
            var user = new ConsoleApp.User(null, "11-541", 20, true);
            string expectString = "<p>Имя: ${aaaaaaaaa}</p>" +
                              "<p>группа: ${aaaaaaaaa}</p>" +
                              "<p>возраст: ${aaaaaaaaa}</p>" +
                              "<p>пол: ${aaaaaaaaa}</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }


        [TestMethod]
        public void RenderIfUnitTest1()
        {
            string template = "$if(${age} > 18)\n" +
                              "<p>Пиво можно</p>" +
                              "$else\n" +
                              "<p>Попей лучше сок</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Пиво можно</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest2()
        {
            string template = "$if(${age} > 18)\n" +
                              "<p>Пиво можно</p>" +
                              "$else\n" +
                              "<p>Не, Саня, покей лучше сок</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 17, true);
            string expectString = "<p>Не, Саня, покей лучше сок</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest3()
        {
            string template = "$if(${age} > 18) || (${name} == Misha)\n" +
                              "<p>Тебе налеваем</p>" +
                              "$else\n" +
                              "<p>А тебе нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Тебе налеваем</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest4()
        {
            string template = "$if(${age} > 18) && (${name} == Misha)\n" +
                              "<p>Тебе налеваем</p>" +
                              "$else\n" +
                              "<p>А тебе нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>А тебе нет</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest5()
        {
            string template = "$if(${sex})\n" +
                              "<p>Саня, ты женщина???</p>" +
                              "$else\n" +
                              "<p>Саня, ты не женщина?( </p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Саня, ты женщина???</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest6()
        {
            string template = "$if(${age} == 20)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest7()
        {
            string template = "$if(${age} == 18)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Нет</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest8()
        {
            string template = "$if(${age} != 18)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest9()
        {
            string template = "$if(${age} != 20)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Нет</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest10()
        {
            string template = "$if(${age} > 18)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest11()
        {
            string template = "$if(${age} > 20)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Нет</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest12()
        {
            string template = "$if(${age} < 21)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest13()
        {
            string template = "$if(${age} < 20)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Нет</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest14()
        {
            string template = "$if(${age} >= 20)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest15()
        {
            string template = "$if(${age} >= 21)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Нет</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest16()
        {
            string template = "$if(${age} <= 20)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest17()
        {
            string template = "$if(${age} <= 19)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Нет</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest18()
        {
            string template = "$if(${age} * 19)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);

            var serv = new HtmlTemplateRenderer();

            Assert.ThrowsException<FormatException>(() => serv.RenderFromString(template, user));
        }

        [TestMethod]
        public void RenderIfUnitTest19()
        {
            string template = "$if(${name} == ${group})\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Нет</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest20()
        {
            string template = "$if(${unknown})\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);

            var serv = new HtmlTemplateRenderer();

            Assert.ThrowsException<FormatException>(() => serv.RenderFromString(template, user));
        }

        [TestMethod]
        public void RenderIfUnitTest21()
        {
            string template = "$if(${age} > 18 && ${name} == Sanya && ${group} == 11-541)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }
        
        [TestMethod]
        public void RenderIfUnitTest22()
        {
            string template = "$if(${age} > 18 && ${name} == Misha && ${group} == 11-541)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Нет</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest23()
        {
            string template = "$if(${age} < 18 || ${name} == Misha || ${group} == 11-541)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest24()
        {
            string template = "$if(${age} < 18 || ${name} == Misha || ${group} == 10-000)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Нет</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest25()
        {
            string template = "$if(${age} > 18 && ${name} == Sanya || ${group} == 10-000)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest26()
        {
            string template = "$if(${age} > 18 && ${name} == Misha || ${group} == 11-541)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest27()
        {
            string template = "$if(${age} < 18 && ${name} == Sanya || ${group} == 10-000)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Нет</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest28()
        {
            string template = "$if(${age} > 18 || ${name} == Misha && ${group} == 10-000)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest29()
        {
            string template = "$if(${age} > 18)\n" +
                              "$if(${name} == Sanya)\n" +
                              "<p>Привет, Саня</p>" +
                              "$else\n" +
                              "<p>Привет, незнакомец</p>" +
                              "$endif" +
                              "$else\n" +
                              "<p>Малявка</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Привет, Саня</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest30()
        {
            string template = "$if(${age} > 18)\n" +
                              "$if(${name} == Misha)\n" +
                              "<p>Привет, Миша</p>" +
                              "$else\n" +
                              "<p>Привет, незнакомец</p>" +
                              "$endif" +
                              "$else\n" +
                              "<p>Малявка</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 20, true);
            string expectString = "<p>Привет, незнакомец</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest31()
        {
            string template = "$if(${age} > 18)\n" +
                              "$if(${name} == Sanya)\n" +
                              "<p>Привет, Саня</p>" +
                              "$else\n" +
                              "<p>Привет, незнакомец</p>" +
                              "$endif" +
                              "$else\n" +
                              "<p>Малявка</p>" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 17, true);
            string expectString = "<p>Малявка</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest32()
        {
            string template = "$if(${age} > 18)\n" +
                              "<p>Взрослый</p>" +
                              "$else\n" +
                              "$if(${age} > 14)\n" +
                              "<p>Подросток</p>" +
                              "$else\n" +
                              "<p>Ребенок</p>" +
                              "$endif" +
                              "$endif";
            var user = new ConsoleApp.User("Sanya", "11-541", 16, true);
            string expectString = "<p>Подросток</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, user);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest33()
        {
            string template = "$if(${size.Width} > 50)\n" +
                              "<p>Широкая</p>" +
                              "$else\n" +
                              "<p>Узкая</p>" +
                              "$endif";
            Bag bag = new ConsoleApp.Bag("Дорожная сумка", 100, new Size(55, 40, 30), new Color("black"), new Madein(15, "Russia", "Ivanovo"));
            string expectString = "<p>Широкая</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest34()
        {
            string template = "$if(${size.Width} > 60)\n" +
                              "<p>Широкая</p>" +
                              "$else\n" +
                              "<p>Узкая</p>" +
                              "$endif";
            Bag bag = new ConsoleApp.Bag("Дорожная сумка", 100, new Size(55, 40, 30), new Color("black"), new Madein(15, "Russia", "Ivanovo"));
            string expectString = "<p>Узкая</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest35()
        {
            string template = "$if(${madein.City} == Ivanovo)\n" +
                              "<p>Из Иваново</p>" +
                              "$else\n" +
                              "<p>Не из Иваново</p>" +
                              "$endif";
            Bag bag = new ConsoleApp.Bag("Дорожная сумка", 100, new Size(55, 40, 30), new Color("black"), new Madein(15, "Russia", "Ivanovo"));
            string expectString = "<p>Из Иваново</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderIfUnitTest36()
        {
            string template = "$if(${size.Width} > 50 && ${color.ColorName} == black)\n" +
                              "<p>Да</p>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            Bag bag = new ConsoleApp.Bag("Дорожная сумка", 100, new Size(55, 40, 30), new Color("black"), new Madein(15, "Russia", "Ivanovo"));
            string expectString = "<p>Да</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }
        [TestMethod]
        public void RenderForeachUnitTest1()
        {
            string template = "<h3>Теги</h3>" +
                              "<ul>" +
                              "$foreach (tag in tags)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>" +
                              "<p>Конец</p>";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag.Tags = new List<string> { "город", "учеба" };
            string expectString = "<h3>Теги</h3><ul><li>город</li><li>учеба</li></ul><p>Конец</p>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest2()
        {
            string template = "<ul>" +
                              "$foreach (bag in bags)\n" +
                              "<li>${bag.name}: ${bag.price}, ${bag.size.Width}x${bag.size.Height}, ${bag.madein.City}</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            Bag bag2 = new ConsoleApp.Bag("Клатч", 900, new Size(20, 10, 5), new Color("Красный"), new Madein(77, "Элита", "Москва"));
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1, bag2 } };
            string expectString = "<ul><li>Рюкзак: 300, 30x45, Тверь</li><li>Клатч: 900, 20x10, Москва</li></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest3()
        {
            string template = "<ul>" +
                              "$foreach (bag in bags)\n" +
                              "<li>${bag.name}</li>" +
                              "$endforeach" +
                              "</ul>";
            var category = new Category { Name = "Пустая", Bags = new List<Bag>() };
            string expectString = "<ul></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest4()
        {
            string template = "<ul>" +
                              "$foreach (tag in tags)\n" +
                              "<li>${name}: ${tag}</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag.Tags = new List<string> { "город", "учеба" };
            string expectString = "<ul><li>Рюкзак: город</li><li>Рюкзак: учеба</li></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest5()
        {
            string template = "<ul>" +
                              "$foreach (bag in bags)\n" +
                              "<li>${bag.wrong}</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            Bag bag2 = new ConsoleApp.Bag("Клатч", 900, new Size(20, 10, 5), new Color("Красный"), new Madein(77, "Элита", "Москва"));
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1, bag2 } };
            string expectString = "<ul><li>${bag.wrong}</li><li>${bag.wrong}</li></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest6()
        {
            string template = "<ul>" +
                              "$foreach (bag in bags)\n" +
                              "<li>${bag.name}" +
                              "<ul>" +
                              "$foreach (tag in bag.tags)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>" +
                              "</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            Bag bag2 = new ConsoleApp.Bag("Клатч", 900, new Size(20, 10, 5), new Color("Красный"), new Madein(77, "Элита", "Москва"));
            bag1.Tags = new List<string> { "город", "учеба" };
            bag2.Tags = new List<string> { "вечер" };
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1, bag2 } };
            string expectString = "<ul>" +
                                  "<li>Рюкзак<ul><li>город</li><li>учеба</li></ul></li>" +
                                  "<li>Клатч<ul><li>вечер</li></ul></li>" +
                                  "</ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest7()
        {
            string template = "<ul>" +
                              "$foreach (bag in bags)\n" +
                              "<li>${bag.name}" +
                              "<ul>" +
                              "$foreach (tag in bag.tags)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>" +
                              "</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            Bag bag2 = new ConsoleApp.Bag("Клатч", 900, new Size(20, 10, 5), new Color("Красный"), new Madein(77, "Элита", "Москва"));
            bag1.Tags = new List<string> { "город" };
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1, bag2 } };
            string expectString = "<ul>" +
                                  "<li>Рюкзак<ul><li>город</li></ul></li>" +
                                  "<li>Клатч<ul></ul></li>" +
                                  "</ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest8()
        {
            string template = "<ul>" +
                              "$foreach (bag in bags)\n" +
                              "<li>${bag.name}" +
                              "<ul>" +
                              "$foreach (tag in tags)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>" +
                              "</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            Bag bag2 = new ConsoleApp.Bag("Клатч", 900, new Size(20, 10, 5), new Color("Красный"), new Madein(77, "Элита", "Москва"));
            bag1.Tags = new List<string> { "город", "учеба" };
            bag2.Tags = new List<string> { "вечер" };
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1, bag2 } };
            string expectString = "<ul>" +
                                  "<li>Рюкзак<ul><li>город</li><li>учеба</li></ul></li>" +
                                  "<li>Клатч<ul><li>вечер</li></ul></li>" +
                                  "</ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest9()
        {
            string template = "<ul>" +
                              "$foreach (x in bags)\n" +
                              "<li>${x.name}" +
                              "<ul>" +
                              "$foreach (x in x.tags)\n" +
                              "<li>${x}</li>" +
                              "$endforeach" +
                              "</ul>" +
                              "</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            Bag bag2 = new ConsoleApp.Bag("Клатч", 900, new Size(20, 10, 5), new Color("Красный"), new Madein(77, "Элита", "Москва"));
            bag1.Tags = new List<string> { "город", "учеба" };
            bag2.Tags = new List<string> { "вечер" };
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1, bag2 } };
            string expectString = "<ul>" +
                                  "<li>Рюкзак<ul><li>город</li><li>учеба</li></ul></li>" +
                                  "<li>Клатч<ul><li>вечер</li></ul></li>" +
                                  "</ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest10()
        {
            string template = "<ul>" +
                              "$foreach (tag in tags)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>" +
                              "<ol>" +
                              "$foreach (tag in tags)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ol>";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag.Tags = new List<string> { "город", "учеба" };
            string expectString = "<ul><li>город</li><li>учеба</li></ul>" +
                                  "<ol><li>город</li><li>учеба</li></ol>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest11()
        {
            string template = "<ul>" +
                              "$foreach (bag in bags)\n" +
                              "$if(${bag.price} > 500)\n" +
                              "<li>Дорого</li>" +
                              "$else\n" +
                              "<li>Дешево</li>" +
                              "$endif" +
                              "$endforeach" +
                              "</ul>";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            Bag bag2 = new ConsoleApp.Bag("Клатч", 900, new Size(20, 10, 5), new Color("Красный"), new Madein(77, "Элита", "Москва"));
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1, bag2 } };
            string expectString = "<ul><li>Дешево</li><li>Дорого</li></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest12()
        {
            string template = "$if(${price} > 50)\n" +
                              "<ul>" +
                              "$foreach (tag in tags)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>" +
                              "$else\n" +
                              "<p>Нет</p>" +
                              "$endif";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 100, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag.Tags = new List<string> { "город", "учеба" };
            string expectString = "<ul><li>город</li><li>учеба</li></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest13()
        {
            string template = "<ul>" +
                              "$foreach (c in name)\n" +
                              "<li>${c}</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag = new ConsoleApp.Bag("Сумка", 100, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            string expectString = "<ul><li>С</li><li>у</li><li>м</li><li>к</li><li>а</li></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest14()
        {
            string template = "<ul>" +
                              "$foreach (tag in tags)\r\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag.Tags = new List<string> { "город", "учеба" };
            string expectString = "<ul><li>город</li><li>учеба</li></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest15()
        {
            string template = "<ul>" +
                              "$foreach(tag   in   tags)  \n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag.Tags = new List<string> { "город", "учеба" };
            string expectString = "<ul><li>город</li><li>учеба</li></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest16()
        {
            string template = "<ul>" +
                              "$foreach (tag in tags)\n" +
                              "<li>\n" +
                              "${tag}\n" +
                              "</li>\n" +
                              "$endforeach" +
                              "</ul>";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag.Tags = new List<string> { "город", "учеба" };
            string expectString = "<ul><li>\nгород\n</li>\n<li>\nучеба\n</li>\n</ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest17()
        {
            string template = "<ul>" +
                              "$foreach (bag in unknown)\n" +
                              "<li>${bag.name}</li>" +
                              "$endforeach" +
                              "</ul>";
            var category = new Category { Name = "Все", Bags = new List<Bag>() };

            var serv = new HtmlTemplateRenderer();

            Assert.ThrowsException<Exception>(() => serv.RenderFromString(template, category));
        }

        [TestMethod]
        public void RenderForeachUnitTest18()
        {
            string template = "<ul>" +
                              "$foreach (x in price)\n" +
                              "<li>${x}</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));

            var serv = new HtmlTemplateRenderer();

            Assert.ThrowsException<Exception>(() => serv.RenderFromString(template, bag));
        }

        [TestMethod]
        public void RenderForeachUnitTest19()
        {
            string template = "<ul>" +
                              "$foreach (bag in bags)\n" +
                              "<li>${bag.name}</li>" +
                              "$endforeach" +
                              "</ul>";
            var category = new Category { Name = "Пустая", Bags = null };

            var serv = new HtmlTemplateRenderer();

            Assert.ThrowsException<Exception>(() => serv.RenderFromString(template, category));
        }

        [TestMethod]
        public void RenderForeachUnitTest20()
        {
            string template = "<ul>" +
                              "$foreach (bag in bags)\n" +
                              "<li>${bag.name}" +
                              "<ul>" +
                              "$foreach (tag in bag.unknown)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>" +
                              "</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1 } };

            var serv = new HtmlTemplateRenderer();

            Assert.ThrowsException<Exception>(() => serv.RenderFromString(template, category));
        }

        [TestMethod]
        public void RenderForeachUnitTest21()
        {
            string template = "<ul>" +
                              "$foreach (bag in bags)\n" +
                              "<li>${bag.name}" +
                              "<ul>" +
                              "$foreach (tag in BAG.tags)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>" +
                              "</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag1.Tags = new List<string> { "город", "учеба" };
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1 } };
            string expectString = "<ul>" +
                                  "<li>Рюкзак<ul><li>город</li><li>учеба</li></ul></li>" +
                                  "</ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest22()
        {
            string template = "<ul>" +
                              "$foreach (tag in tags)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag.Tags = new List<string> { "один" };
            string expectString = "<ul><li>один</li></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest23()
        {
            string template = "$foreach (bag in bags)\n" +
                              "[${bag.name}]" +
                              "$endforeach";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1, null! } };
            string expectString = "[Рюкзак][${bag.name}]";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest24()
        {
            string template = "<ul>" +
                              "$foreach (bag in bags)\n" +
                              "$foreach (tag in bag.tags)\n" +
                              "<li>${tag} (${bag.name})</li>" +
                              "$endforeach" +
                              "$endforeach" +
                              "</ul>";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            Bag bag2 = new ConsoleApp.Bag("Клатч", 900, new Size(20, 10, 5), new Color("Красный"), new Madein(77, "Элита", "Москва"));
            bag1.Tags = new List<string> { "город", "учеба" };
            bag2.Tags = new List<string> { "вечер" };
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1, bag2 } };
            string expectString = "<ul><li>город (Рюкзак)</li><li>учеба (Рюкзак)</li><li>вечер (Клатч)</li></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest25()
        {
            string template = "$foreach (bag in bags)\n" +
                              "<p>${bag.name}</p>" +
                              "<ul>" +
                              "$foreach (tag in bag.tags)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ul>" +
                              "<ol>" +
                              "$foreach (tag in bag.tags)\n" +
                              "<li>${tag}</li>" +
                              "$endforeach" +
                              "</ol>" +
                              "$endforeach";
            Bag bag1 = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag1.Tags = new List<string> { "город", "учеба" };
            var category = new Category { Name = "Все", Bags = new List<Bag> { bag1 } };
            string expectString = "<p>Рюкзак</p>" +
                                  "<ul><li>город</li><li>учеба</li></ul>" +
                                  "<ol><li>город</li><li>учеба</li></ol>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, category);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest26()
        {
            string template = "$foreach (tag in tags)[${tag}]$endforeach";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag.Tags = new List<string> { "город", "учеба" };
            string expectString = "$foreach (tag in tags)[${tag}]$endforeach";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest27()
        {
            string template = "$foreach (tag in tags)\n" +
                              "[${tag}]";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag.Tags = new List<string> { "город", "учеба" };
            string expectString = "$foreach (tag in tags)\n" +
                                  "[${tag}]";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest28()
        {
            string template = "<ul>" +
                              "$foreach (tag in tags)\n" +
                              "$endforeach" +
                              "</ul>";
            Bag bag = new ConsoleApp.Bag("Рюкзак", 300, new Size(30, 45, 15), new Color("Черный"), new Madein(101, "Рюкзакофф", "Тверь"));
            bag.Tags = new List<string> { "город", "учеба" };
            string expectString = "<ul></ul>";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, bag);

            Assert.AreEqual(expectString, res);
        }

        [TestMethod]
        public void RenderForeachUnitTest29()
        {
            string template = "$foreach (n in numbers)\n" +
                              "${n};" +
                              "$endforeach";
            var model = new { Numbers = new List<int> { 1, 2, 3 } };
            string expectString = "1;2;3;";

            var serv = new HtmlTemplateRenderer();

            string res = serv.RenderFromString(template, model);

            Assert.AreEqual(expectString, res);
        }






    }
}






