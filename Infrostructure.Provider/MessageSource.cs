using App.Contracts;
using App.Contracts.Extensions;
using System.Net;
using System.Net.Sockets;

public class MessageSource : IMessageSource
{
    private readonly UdpClient _udpClient;

    public MessageSource (UdpClient udpClient)
    {
        _udpClient = udpClient;
    }

    public IPEndPoint CreateEndpoint(string adress, int port)
    {
        throw new NotImplementedException();
    }

    public async Task<RecievResult?> Recieve(CancellationToken token)
    {
        var data =  await _udpClient.ReceiveAsync(token);
        return new(data.RemoteEndPoint, data.Buffer.ToMessage());
    }

    public async Task Send(Message message, IPEndPoint endPoint, CancellationToken token)
    {
        await _udpClient.SendAsync(message.ToBytes(), endPoint, token);
    }
}
