using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace National4HSatrusLive.Models
{
    public class ContactModel
    {
        public Guid Id { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address1Street1 { get; set; }
        public string Address1Street2 { get; set; }
        public string Address1Street3 { get; set; }
        public string Address1City1 { get; set; }
        public string Address1StateProvince { get; set; }
        public string Address1ZipPostalCode { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public bool SendBulkEmail { get; set; }
        public string EntityName { get; set; }
        public int StateValue { get; set; }
        public int StatusValue { get; set; }
        public List<string> Interests { get; set; }
    }
}