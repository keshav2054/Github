using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Security.Cryptography;

namespace LinkdinApi
{
    public class AccessToken
    {
        internal string Token { get; private set; }
        public DateTime ExpireDate { get; private set; }

        public bool IsValid
        {
            get
            {
                return DateTime.Now <= this.ExpireDate;
            }
        }

        public TimeSpan TimeLeft
        {
            get
            {
                return this.ExpireDate - DateTime.Now;
            }
        }

        internal AccessToken(string token, int secondsRemaining)
        {
            this.Token = token;
            this.ExpireDate = DateTime.Now.AddSeconds(secondsRemaining);
        }

       

      

        private static byte[] GetEntropy(string salt)
        {
            return System.Text.Encoding.UTF8.GetBytes(salt);
        }

        private byte[] GetData()
        {
            return System.Text.Encoding.UTF8.GetBytes(String.Format("{0}%%%{1}", this.Token, this.ExpireDate.ToBinary()));
        }

     
    }
}