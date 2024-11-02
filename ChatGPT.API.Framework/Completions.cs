using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using static ChatGPT.API.HKUST.Response_Stream.Choices;

namespace ChatGPT.API.HKUST
{
    /// <summary>
    /// a completion for the chat message
    /// </summary>
    public class Completions
    {
        /// <summary>
        /// What sampling temperature to use, between 0 and 2. 
        /// Higher values like 0.8 will make the output more random, 
        /// while lower values like 0.2 will make it more focused and deterministic.
        /// </summary>
        public double temperature { get; set; } = 1;
        /// <summary>
        /// The maximum number of tokens allowed for the generated answer. 
        /// By default, the number of tokens the model can return will be (4096 - prompt tokens).
        /// </summary>
        public int max_tokens { get; set; } = 2048;
        /// <summary>
        /// Number between -2.0 and 2.0. 
        /// Positive values penalize new tokens based on whether they appear in the text so far
        /// increasing the model's likelihood to talk about new topics.
        /// </summary>
        public double presence_penalty { get; set; } = 0;
        /// <summary>
        /// Number between -2.0 and 2.0. 
        /// Positive values penalize new tokens based on their existing frequency in the text so far
        /// decreasing the model's likelihood to repeat the same line verbatim
        /// </summary>
        public double frequency_penalty { get; set; } = 0;
        /// <summary>
        /// The messages to generate chat completions for
        /// </summary>
        public List<Message> messages { get; set; } = new List<Message>();
        /// <summary>
        /// How many chat completion choices to generate for each input message.
        /// </summary>
        public int n { get; set; } = 1;
        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse
        /// </summary>
        public string user { get; set; } = "default";
#nullable enable
        /// <summary>
        /// Get Response
        /// </summary>
        public Response? GetResponse(string APIUrl, string APIKey, HttpMessageHandler? Proxy = null) => GetResponse_async(APIUrl, APIKey, Proxy).Result;
        /// <summary>
        /// Get Response
        /// </summary>
        public async Task<Response?> GetResponse_async(string APIUrl, string APIKey, HttpMessageHandler? Proxy = null)
        {
            using (var httpClient = Proxy == null ? new HttpClient() : new HttpClient(Proxy))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + APIKey);
                httpClient.DefaultRequestHeaders.Add("api-key", APIKey);
                var content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(APIUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();
                var rs = JsonConvert.DeserializeObject<Response>(responseString);
                var msg = rs?.GetMessage();
                if (msg != null)
                    messages.Add(msg);
                return rs;
            }
        }
#nullable enable
        /// <summary>
        /// Ask and Get Response
        /// </summary>
        public Response? Ask(string usermessage, string APIUrl, string APIKey) => Ask_async(usermessage, APIUrl, APIKey).Result;

        /// <summary>
        /// Ask and Get Response
        /// </summary>
        public async Task<Response?> Ask_async(string usermessage, string APIUrl, string APIKey, HttpMessageHandler? Proxy = null)
        {
            messages.Add(new Message() { role = Message.RoleType.user, content = usermessage });
            return await GetResponse_async(APIUrl, APIKey, Proxy);
        }
    }
}
