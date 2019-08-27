using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerManagement.Base.Models
{
    public class Customer : IdentifiableBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
