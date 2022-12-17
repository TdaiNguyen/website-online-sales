using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FASTFOOD.Code
{
    [Serializable]
    public class UserSession
    {
        private string Email;

        public string get()
        {
            return Email;
        }
        private void set(string value)
        {
            Email = value;
        }
        public UserSession(string Email)
        {
            this.set(Email);
        }
    }
}