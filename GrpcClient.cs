using System;
using System.Runtime.InteropServices;
using Grpc.Core;
using Services;

namespace GrpcWrapper
{
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class GrpcClient
{
private Channel _channel;
private DatabaseService.DatabaseServiceClient _client;
private string _serverAddress = "localhost";
private int _serverPort = 50051;


    public string ServerAddress
    {
        get { return _serverAddress; }
        set { _serverAddress = value; }
    }

    public int ServerPort
    {
        get { return _serverPort; }
        set { _serverPort = value; }
    }

    public string LastError { get; private set; } = "";

    public bool Initialize()
    {
        try
        {
            _channel = new Channel(_serverAddress, _serverPort, ChannelCredentials.Insecure);
            _client = new DatabaseService.DatabaseServiceClient(_channel);
            LastError = "";
            return true;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return false;
        }
    }

    public string SayHello(string name)
    {
        try
        {
            if (_client == null && !Initialize())
                return "ERROR: Failed to connect - " + LastError;

            var request = new HelloRequest { Name = name };
            var response = _client.SayHello(request);
            return response.Message;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return "ERROR: " + ex.Message;
        }
    }

    public string SayGoodbye(string name)
    {
        try
        {
            if (_client == null && !Initialize())
                return "ERROR: Failed to connect - " + LastError;

            var request = new HelloRequest { Name = name };
            var response = _client.SayGoodbye(request);
            return response.Message;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return "ERROR: " + ex.Message;
        }
    }

    public string AddNumbers(int a, int b)
    {
        try
        {
            if (_client == null && !Initialize())
                return "ERROR: Failed to connect - " + LastError;

            var request = new AddRequest { A = a, B = b };
            var response = _client.AddNumbers(request);
            return response.Message + " (Result: " + response.Result + ")";
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return "ERROR: " + ex.Message;
        }
    }

    public string GetStatus()
    {
        try
        {
            if (_client == null && !Initialize())
                return "ERROR: Failed to connect - " + LastError;

            var request = new StatusRequest();
            var response = _client.GetStatus(request);
            return "Status: " + response.Status + 
                   ", Uptime: " + response.Uptime + 
                   ", Requests: " + response.RequestCount;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return "ERROR: " + ex.Message;
        }
    }

    public string Echo(string text, bool uppercase)
    {
        try
        {
            if (_client == null && !Initialize())
                return "ERROR: Failed to connect - " + LastError;

            var request = new EchoRequest { Text = text, Uppercase = uppercase };
            var response = _client.Echo(request);
            return response.EchoedText;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            return "ERROR: " + ex.Message;
        }
    }

    public bool TestConnection()
    {
        try
        {
            var result = SayHello("ConnectionTest");
            return !result.StartsWith("ERROR:");
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        try
        {
            if (_channel != null)
            {
                _channel.ShutdownAsync().Wait(5000);
                _channel = null;
                _client = null;
            }
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
        }
    }
}


}
