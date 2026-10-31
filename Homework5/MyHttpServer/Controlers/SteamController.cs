using CustomHttpServer.Framework.Attributes;
using HtmlAgilityPack;
using MyHttpServer.frameworks.Аttributes;
using System;
using System.Net;
using System.Net.Mail;
using System.Xml.Linq;

namespace CustomHttpServer.Controllers
{

    [HttpController("steam")]
    public class SteamController
    {

        private List<string> cards;


        public List<string> HttpWeb(string hero)
        {
            var html = $"https://steamcommunity.com/market/search?category_Hero=npc_dota_hero_{hero}&appid=570";
            HtmlWeb web = new HtmlWeb();


            var htmlDoc = web.Load(html);

            var bigNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='CommunityTemplate']/div/div/div/div[1]/div[2]/div[2]/div[2]/div[2]/div/div[2]/div");
            Console.WriteLine($"Steam: {web.StatusCode}");
            if (bigNode != null)
            {
                cards = new List<string>();
                string str;

                foreach (var nodes in bigNode.ChildNodes)
                {
                    if (nodes.Name != "a") continue;

                    string itemUrl = nodes.GetAttributeValue("href", "");
                    var name = nodes.SelectSingleNode(".//div[1]/div[2]/span[1]")?.InnerText.Trim();
                    var count = nodes.SelectSingleNode(".//div[3]/span/span")?.InnerText.Trim();
                    var cost = nodes.SelectSingleNode(".//div[3]/div/span")?.InnerText.Trim();

                    str = $"- cсылка {itemUrl}, название карточки {name}, в наличии {count}, цена {cost}";
                    cards.Add(str);
                }
                return cards;
            }
            else return null;

        }

        // GET http://localhost:2323/steam/send?to=test@test.ru&hero=axe

        [GET("send")]
        public void Send(string to, string hero)
        {


            cards = HttpWeb(hero);
            if (cards == null)
            {
                cards = new List<string> { "по данному персонажу не нашлось товаров" };

            }


            
            MailAddress from = new MailAddress("Sveta22052006@yandex.ru", "Sveta");

            
            MailAddress toAddress = new MailAddress(to);

            
            MailMessage message = new MailMessage(from, toAddress);

            
            message.Subject = $"Сегодня в Steam с персонажем {hero}";

           
            string bodyItem = "";
            foreach (string card in cards)
            {
                bodyItem +=card;
                
            }
            message.Body = $"<h2>Список товаров для персонажа {hero}</h2>" +
                $"<div>{bodyItem}</div>";


            
            message.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 587);
            smtp.EnableSsl = true;
            
            smtp.Credentials = new NetworkCredential("Sveta22052006@yandex.ru", "gsekdnviqkrsfvcn");

            smtp.EnableSsl = true;

            smtp.Send(message);


            Console.WriteLine($"Отправлено на: {to}");
        }

    }
}