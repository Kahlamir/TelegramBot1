using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot1
{
   public class PersonModel
    {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string Text { get; set; }


        public string FullName
        {
            get
            {
                return $"{ FirstName } { Text }";
            }
        }
    }
}
