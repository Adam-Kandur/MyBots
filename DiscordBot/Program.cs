using System;
using Discord;
using Discord.WebSocket;

class Program
{
    public static Task Main(string[] args) =>
        new Program().MainAsync();

    private DiscordSocketClient? _client;
    
    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();

        _client.Log += Log;
        _client.MessageReceived += ClientOnMessageReceived;
        _client.UserVoiceStateUpdated += ClientOnVoiceStateUpdate;
        _client.UserJoined += AnnounceJoinedUser;

        var token = File.ReadAllText("token.txt");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private static Task ClientOnMessageReceived(SocketMessage arg)
    {
        if (arg.Content.StartsWith("!ping"))
        {
            arg.Channel.SendMessageAsync(
                $"Hi, '{arg.Author.Username}'! Pong."
            );
        }

        return Task.CompletedTask;
    }

    private async Task ClientOnVoiceStateUpdate(SocketUser user,
                                                SocketVoiceState oldState,
                                                SocketVoiceState newState)
    {
        const ulong createVcRoomChannelId = 937710723851747358;
        const ulong parentId = 937710683452239885;

        if (newState.VoiceChannel != null &&
            newState.VoiceChannel.Id == createVcRoomChannelId)
        {
            const string newVoiceChannelName = user.Username;
            _client.Guilds.CreateVoiceChannelAsync(newVoiceChannelName);
            await Console.WriteLine("hi");
        }
    }

    public async Task AnnounceJoinedUser(SocketGuildUser user)
    {
        const ulong logRoomId = 937748347085799454;
        // ulong moderatorRoleId = 938181506633961492;
        var channel = _client.GetChannel(logRoomId) as SocketTextChannel;
        await channel.SendMessageAsync(user.Mention + "прибыл");
    } 
}
