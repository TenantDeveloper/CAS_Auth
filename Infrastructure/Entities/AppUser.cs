using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Entities
{
   public class AppUser
    {
        public int Id { get; set; }
        public string NameIdentifier { get; set; }
        public string UserName { get; set; }
        public int UserType { get; set; }
        public int UserStatus { get; set; }
        public int Department { get; set; }
        public string Name { get; set; }
    }
}
