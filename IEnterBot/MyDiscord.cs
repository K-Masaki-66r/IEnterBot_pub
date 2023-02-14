using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;

namespace IEnterBot
{
	internal class MyDiscord
	{
		internal string StateText { get; set; } = "";
		internal ulong MessageId { get; set; } = 0;

		private string _token = "";
		private ulong _channelId = 0;

		private DiscordSocketClient _client;
		private ITextChannel? _channel;
		private bool _enablePosting = true;

		private bool _postingNew = false;
		private object _lock = new();

		internal MyDiscord()
		{
#if DEBUG
			_channelId = 0;
#else
			if(!ulong.TryParse(Environment.GetEnvironmentVariable("DISCORD_IENTERBOT_CHANNEL_ID"), out _channelId)) {
				_enablePosting = false;
				MessageBox.Show("チャンネルIDが読み込めませんでした。");
			}
#endif
			_token = Environment.GetEnvironmentVariable("DISCORD_IENTERBOT_TOKEN") ?? "";
			_client = new(new DiscordSocketConfig {
				GatewayIntents = GatewayIntents.All,
			});

			_client.LoginAsync(TokenType.Bot, _token).Wait();
			_client.StartAsync().Wait();

			try {
				_channel = _client.GetChannelAsync(_channelId).Result as ITextChannel;
				if(_channel == null) {
					MessageBox.Show("指定されたチャンネルはテキストチャンネルではありません。");
					_enablePosting = false;
				}
			} catch(Discord.Net.HttpException) {
				MessageBox.Show("指定されたチャンネルにアクセスできませんでした。");
				_enablePosting = false;
				return;
			}
		}

		internal async Task PostState(string text, bool reset = false)
		{
			while(!await PostStateWithoutOthers(text, reset)) {
			}
		}

		internal async Task Post(string text)
		{
			if(!_enablePosting) return;

			await _channel.SendMessageAsync(text);
		}

		internal async Task ChangeState(bool isOpen)
		{
			if(isOpen && StateText != "") {
				await _client.SetGameAsync(StateText);
				return;
			}

			StateText = "";
			await _client.SetGameAsync(isOpen ? "open" : "close");
		}

		/// <summary>
		/// 状態を投稿するときに、他からの呼び出しと被って投稿しないようにした関数
		/// </summary>
		/// <param name="text">投稿文</param>
		/// <param name="reset">新しく投稿するか、既存の投稿を編集するか</param>
		/// <returns>他の呼び出し元より先に投稿できたかどうか</returns>
		private async Task<bool> PostStateWithoutOthers(string text, bool reset = false)
		{
			if(!_enablePosting) return true;

			lock(_lock) {
				if(_postingNew) return false;
				_postingNew = true;
			}

			if(reset) {
				MessageId = 0;
			}

			if(MessageId == 0) {
				await PostNew(text);
			} else {
				try {
					await _channel.ModifyMessageAsync(MessageId, message => {
						message.Content = text;
					});
				} catch(Discord.Net.HttpException) {
					await PostNew(text);
				}
			}

			lock(_lock) {
				_postingNew = false;
				return true;
			}
		}


		private async Task PostNew(string text)
		{
			if(!_enablePosting) return;

			var message = _channel.SendMessageAsync(text);
			await message;
			MessageId = message.Result.Id;
		}

	}
}
