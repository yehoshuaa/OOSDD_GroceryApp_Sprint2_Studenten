﻿namespace Grocery.Core.Models
{
    public partial class Client : Model
    {
        public string EmailAddress { get; private set; }
        public string Password { get; private set; }

        public Client(int id, string name, string emailAddress, string password)
            : base(id, name)
        {
            EmailAddress = emailAddress;
            Password = password;
        }
    }
}
