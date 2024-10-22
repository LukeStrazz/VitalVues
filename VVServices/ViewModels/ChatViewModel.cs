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
    public string? UserSID { get; set; }
    public DateTime ChatDate { get; set; }
    public string ChatTopic { get; set; }
    public List<MessageViewModel>? Messages { get; set; }
}

public class MessageViewModel
{
    public string role { get; set; }
    public string content { get; set; }
}