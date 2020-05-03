using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.IO;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;



namespace TelegramBot1
{
    class Program
    {
        public static class Bot
        {
            public static readonly TelegramBotClient Api = new TelegramBotClient("1217360139:AAEEvHlG5GFiWni4yfgksnta8uaZA4d2uJQ");
        }

       private static List<PersonModel> people = new List<PersonModel>();
        static void Main(string[] args)
        {




            using (WebApp.Start<Startup>("https://+:8443"))
            {



                Bot.Api.OnMessage += Bot_OnMessage;
                Bot.Api.OnMessageEdited += Bot_OnMessage;

                Bot.Api.StartReceiving();
                Console.ReadLine();
                Bot.Api.StopReceiving();


                // Register WebHook  
                // You should replace {YourHostname} with your Internet accessible hosname
                Bot.Api.SetWebhookAsync("https://telegrambotdeneme.herokuapp.com:8443/WebHook").Wait();

                Console.WriteLine("Server Started");


           


                // Stop Server after <Enter>
                Console.ReadLine();

                // Unregister WebHook
                Bot.Api.DeleteWebhookAsync().Wait();
            }

            //Bot.Api.OnMessage += Bot_OnMessage;
            //Bot.Api.OnMessageEdited += Bot_OnMessage;

            //Bot.Api.StartReceiving();
            //Console.ReadLine();
            //Bot.Api.StopReceiving();

        }

        private static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                if (e.Message.Text == "/How are you?")
                    await Bot.Api.SendTextMessageAsync(e.Message.Chat.Id, "Fine thank you,and you?");
                else if (e.Message.Text == "/Good Morning")
                    await Bot.Api.SendTextMessageAsync(e.Message.Chat.Id, "Good Morning");
              else if (e.Message.Text == "Listele")
                {
                    people = SqliteDataAccess.LoadPeople();
                    string list = "";

                    foreach (PersonModel p in people)
                    {
                        list = list + "\n" + p.FirstName + ":" + p.Text;
                    }
                    await Bot.Api.SendTextMessageAsync(e.Message.Chat.Id, list);
                }
                else
                {
                    Console.WriteLine(e.Message.From.FirstName + "" + e.Message.Text);

                    PersonModel p = new PersonModel();

                    p.FirstName = e.Message.From.FirstName;
                    p.Text = e.Message.Text;

                    SqliteDataAccess.SavePerson(p);
                }
            }
        }


        public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();

            configuration.Routes.MapHttpRoute("WebHook", "{controller}");

            app.UseWebApi(configuration);
        }
    }

    public class WebHookController : ApiController
    {
        public async Task<IHttpActionResult> Post(Update update)
        {
            var message = update.Message;

            Console.WriteLine("Received Message from {0}", message.Chat.Id);

            if (message.Type == MessageType.Text)
            {
                // Echo each Message
                await Bot.Api.SendTextMessageAsync(message.Chat.Id, message.Text);
            }
            else if (message.Type == MessageType.Photo)
            {
                // Download Photo
                var file = await Bot.Api.GetFileAsync(message.Photo.LastOrDefault()?.FileId);

                var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                using (var saveImageStream = File.Open(filename, FileMode.Create))
                {
                    await Bot.Api.DownloadFileAsync(file.FilePath, saveImageStream);
                }

                await Bot.Api.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
            }

            return Ok();
        }
    }
    }
}
