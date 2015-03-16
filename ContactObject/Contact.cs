using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactObject
{
    public sealed class Contact
    {
        private string Name;
        private string phoneNumber;

        public Contact(string name, string number)
        {
            Name = name;
            phoneNumber = number;
        }

        public string getName()
        {
            return Name;
        }

        public string getNumber()
        {
            return phoneNumber;
        }
    }
}
