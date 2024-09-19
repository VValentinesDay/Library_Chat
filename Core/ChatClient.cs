
using App.Contracts;
using System.Net;

public class ChatClient : ChatBase
{
    private readonly IMessageSource _source;
    private readonly User _user;
    private readonly IPEndPoint _serverEndPoint;
    private IEnumerable<User> _users = [];

    public ChatClient(string username ,IPEndPoint serverEndPoint ,IMessageSource source)
    {
       
        _source = source;
        _user = new User { Name = username };
        _serverEndPoint = serverEndPoint;

    }

    public override async Task Start()
    {
        var join = new Message { Text = _user.Name, Command = Command.Join };
        await _source.Send(join, _serverEndPoint, CancellationToken);

        Task.Run(Listener);

        while (!CancellationToken.IsCancellationRequested) { 
            string input = (await Console.In.ReadLineAsync()) ?? string.Empty;
            // создание обычного сообщения
            Message msg = new() { Text = input, SenderID = _user.Id, Command = Command.None };
            await _source.Send(msg, _serverEndPoint, CancellationToken);
        }
    }

    protected override async Task Listener()
    {
        while (!CancellationToken.IsCancellationRequested)
        {
            try
            {
                RecievResult? message = await _source.Recieve(CancellationToken);
                if (message == null)
                {
                    throw new Exception("Message is null");
                }
                //  Обработчик команд
                if(message.Message.Command == Command.Join)
                {
                    JoinHandler(message.Message);
                }   
                else if(message.Message.Command == Command.Users)
                {
                    UserHandler(message.Message);

                }   
                else if(message.Message.Command == Command.None)
                {
                    MessageHandler(message.Message);

                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteAsync(ex.Message);
            }
        }
    }


    // Методы для обработки команд
    private void MessageHandler(Message message)
    {
        Console.WriteLine($"{_users.First(u => u.Id == message.SenderID )} : {message.Text}");
    }

    private void UserHandler(Message message)
    {
        _users = message.Users;   }

    private void JoinHandler(Message message)
    {
        _user.Id = message.RecepentID;
        Console.WriteLine("Join sucsess");
    }
}
