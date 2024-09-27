using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMailService
    {
        void SendEmail(string toEmail, string API_Key, string Domain, string subject, string content);
       

    }
}
