using App.Contracts;
using System.Net;

public record RecievResult(IPEndPoint IPEndPoint, Message? Message);