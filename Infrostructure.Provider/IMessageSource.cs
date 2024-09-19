using App.Contracts;
using System.Net;

public interface IMessageSource
{
    Task<RecievResult> Recieve(CancellationToken token);
    Task Send(Message message, IPEndPoint endPoint, CancellationToken token);
    //IPEndPoint CreateEndpoint(string adress, int port);
    //IPEndPoint GetServerEndpoint();
}
