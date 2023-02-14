using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace IEnterBot
{
	internal static class Twitter
	{
		private static string _apiKey = "";
		private static string _apiKeySecret = "";
		private static string _accessToken = "";
		private static string _accessTokenSecret = "";
		private static Random random = new();
		private static bool _first = true;

		/// <summary>
		/// ツイートをする。
		/// </summary>
		/// <param name="text">ツイートする文章。</param>
		/// <returns>成功時には0。そうでなければステータスコードを返す。
		/// しかし、よきせぬエラーでは-1を返す。</returns>
		internal static int Tweet(string text)
		{
#if DEBUG
			return 0;
#endif
			if(_first) {
				_apiKey = Environment.GetEnvironmentVariable("TWITTER_API_KEY") ?? "";
				_apiKeySecret = Environment.GetEnvironmentVariable("TWITTER_API_KEY_SECRET") ?? "";
				_accessToken = Environment.GetEnvironmentVariable("TWITTER_ACCESS_TOKEN") ?? "";
				_accessTokenSecret = Environment.GetEnvironmentVariable("TWITTER_ACCESS_TOKEN_SECRET") ?? "";
			}
			var parameters = new Dictionary<string, string>();
			parameters.Add("text", text);
			return Post("https://api.twitter.com/2/tweets", parameters);
		}

		private static int Post(string url, IDictionary<string, string> parameters)
		{
			var parameters2 = GenerateParameters();
			var jsonParameters = JsonSerializer.Serialize(parameters);
			var signature = GenerateSignature("POST", url, parameters2);
			parameters2.Add("oauth_signature", UrlEncode(signature));
			return HttpPost(url, parameters2, jsonParameters);
		}

		private static int HttpPost(string url, IDictionary<string, string> parameters, string json)
		{
			var data = Encoding.ASCII.GetBytes(JoinParameters(parameters));
			var req = WebRequest.Create(url);
			req.Method = "POST";
			req.ContentType = "application/json";
			req.Headers.Add(GetHeader(parameters));
			using(var writer = new StreamWriter(req.GetRequestStream())) {
				writer.Write(json);
				writer.Flush();
			}
			var result = "";
			try {
				var res = req.GetResponse();
				using(var reader = new StreamReader(res.GetResponseStream())) {
					result = reader.ReadToEnd();
				}
			}catch(WebException we) {
				if(we.Response is not HttpWebResponse erres) {
					return -1;
				}
				return (int)erres.StatusCode;
			}

			return 0;
		}

		private static string GenerateSignature(string httpMethod, string url, SortedDictionary<string, string> parameters)
		{
			var signatureBase = GenerateSignatureBase(httpMethod, url, parameters);
			var hma = new HMACSHA1();
			hma.Key = Encoding.ASCII.GetBytes(UrlEncode(_apiKeySecret) + '&' + UrlEncode(_accessTokenSecret));
			var data = Encoding.ASCII.GetBytes(signatureBase);
			var hash = hma.ComputeHash(data);

			return Convert.ToBase64String(hash);
		}

		private static string GenerateSignatureBase(string httpMethod, string url, SortedDictionary<string, string> parameters)
		{
			var result = new StringBuilder();
			result.Append(httpMethod);
			result.Append('&');
			result.Append(UrlEncode(url));
			result.Append('&');
			result.Append(UrlEncode(JoinParameters(parameters)));

			return result.ToString();
		}

		private static string GenerateTimeStamp()
		{
			var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt64(ts.TotalSeconds).ToString();
		}

		private static string JoinParameters(IDictionary<string, string> parameters)
		{
			var result = new StringBuilder();
			var first = true;
			foreach(var parameter in parameters) {
				if(first) {
					first = false;
				} else {
					result.Append('&');
				}
				result.Append(parameter.Key);
				result.Append('=');
				result.Append(parameter.Value);
			}
			return result.ToString();
		}

		private static SortedDictionary<string, string> GenerateParameters()
		{
			var result = new SortedDictionary<string, string>();
			result.Add("oauth_consumer_key", _apiKey);
			result.Add("oauth_signature_method", "HMAC-SHA1");
			result.Add("oauth_timestamp", GenerateTimeStamp());
			result.Add("oauth_nonce", GenerateNonce());
			result.Add("oauth_version", "1.0");
			result.Add("oauth_token", _accessToken);

			return result;
		}

		private static string GenerateNonce()
		{
			var letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var result = new StringBuilder(8);
			for(var i = 0; i < 8; i++) {
				result.Append(letters[random.Next(letters.Length)]);
			}
			return result.ToString();
		}

		private static string GetHeader(IDictionary<string, string> parameters)
		{
			var result = "Authorization: OAuth ";
			var first = true;
			foreach(var parameter in parameters) {
				if(first) {
					first = false;
				} else {
					result += ", ";
				}
				result += string.Format("{0}=\"{1}\"", parameter.Key, parameter.Value);
			}
			return result;
		}

		private static string UrlEncode(string value)
		{
			var unreserved = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
			var result = new StringBuilder();
			var data = Encoding.UTF8.GetBytes(value);
			foreach(var b in data) {
				if(b < 0x80 && unreserved.IndexOf((char)b) != -1) {
					result.Append((char)b);
				} else {
					result.Append('%' + string.Format("{0:X2}", (int)b));
				}
			}
			return result.ToString();
		}
	}
}
