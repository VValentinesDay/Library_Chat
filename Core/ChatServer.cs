
using App.Contracts;

public class ChatServer : ChatBase
{
    private readonly IMessageSource _source;
    private HashSet<User> _users = [];
    // когда кто-то будет использовать команду join
    // будет приходить оповещение с сервера всем участникам

    public ChatServer(IMessageSource source)
    {
        _source = source;
    }

    public override async Task Start()
    {
        await Task.CompletedTask;
        Task.Run(Listener);
    }

    protected override async Task Listener()
    {
        while (!CancellationToken.IsCancellationRequested)
        {
            try
            {
                // два вопроса - "если сообщение плохое"ы
                RecievResult message = await _source.Recieve(CancellationToken) ?? throw new Exception("Message is null");
                switch (message.Message!.Command)
                {
                    case Command.None:
                        await MessageHandler(message);
                        break;
                    case Command.Join:
                        await JoinHandler(message);
                        break;
                    case Command.Exit:
                        break;
                    case Command.Users:
                        break;
                    case Command.Confirm:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteAsync(ex.Message);
            }
        }
    }

    private async Task MessageHandler(RecievResult message)
    {
        if (message.Message!.RecepentID < 0) // если отправитель
        {
            await SendAllAsync(message.Message);
     
        }
        else // если получатель
        {
            await _source.Send(
                message.Message,
                // получение endpoint отправителя
                _users.First(u => u.Id == message.Message!.SenderID).EndPoint!,
            CancellationToken);

            var recipientEndPoint = _users.FirstOrDefault(u => u.Id == message.Message!.SenderID)?.EndPoint;
            if (recipientEndPoint != null)
            {
                await _source.Send(
               message.Message,
               recipientEndPoint,
               CancellationToken);
            }
        
        }
    }

    private async Task JoinHandler(RecievResult message)
    {// если получаем null, то такого пользователя нет, значит добавляем его


        User? user = _users.FirstOrDefault(u => u.Name == message.Message!.Text);
        if (user is null)
        {
            User userNew = new User() { Name = message.Message!.Text };
            _users.Add(userNew);
        }

        user.EndPoint = message.IPEndPoint;

        await _source.Send(
            new Message()
            {
                Command = Command.Join,
                RecepentID = user.Id
            },
            user.EndPoint,
            CancellationToken);


        await _source.Send(
        new Message()
        {
            Command = Command.Users,
            RecepentID = user.Id,
            Users = _users
        },
            user.EndPoint,
            CancellationToken);
        // отправление сообщения всем пользователям о том, что добавлен новый пользователь
        await SendAllAsync(new Message()
        {
            Command = Command.Confirm,
            Text = $"{user.Name} joined to chat"
        });


    }

    private async Task SendAllAsync(Message message)
    {
        foreach (var user in _users)
        {
            await _source.Send(message, user.EndPoint, CancellationToken);
        }
    }
}