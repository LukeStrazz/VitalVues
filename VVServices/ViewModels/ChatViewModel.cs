using Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels;

public class ChatViewModel
{
    public long Id { get; set; }
    public string UserSID { get; set; }
    public DateTime ChatDate { get; set; }
    public string ChatTopic { get; set; }
    public virtual required List<Message> Messages { get; set; }
}
