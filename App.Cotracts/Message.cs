using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Contracts
{


    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int SenderID { get; set; }
        public int RecepentID { get; set; } = -1; // чтобы не разбиратся с null able
        public Command Command { get; set; } = Command.None;

        public IEnumerable<User> Users { get; set; } = [];
    }
}

public enum Command
{
    None,
    Join,
    Exit,
    Users,
    Confirm
}
