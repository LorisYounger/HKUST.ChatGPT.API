using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatUST.API.Convert.Controllers
{
    [ApiController]
    [Route("/")]
    public class IndexController : ControllerBase
    {

        [HttpGet("/")]
        public string Index()
        {
            return "ChatUST.API.Convert Powered By LorisYounger";
        }
        [HttpPost("raw/v1/chat/completions")]
        public string RAWCompletions([FromBody] ChatGPT.API.Framework.Completions completion)
        {
            Response.ContentType = "text/plain";

            var Authorization = HttpContext.Request.Headers["Authorization"].ToString();
            if (!Authorization.StartsWith("Bearer "))
            {
                HttpContext.Response.StatusCode = 401;
                return JsonConvert.SerializeObject(
                    new { Error = "Error Key: Authorization need Start With `Bearer `" }
                    );
            }
            string key = Authorization.Substring(7);

            if (completion.messages.Count == 0)
            {
                return (JsonConvert.SerializeObject(new { Fail = "Error Key" }));
            }

            //预处理
            ChatGPT.API.HKUST.Completions hkustcomp = new ChatGPT.API.HKUST.Completions()
            {
                frequency_penalty = completion.frequency_penalty,
                max_tokens = completion.max_tokens,
                n = completion.n,
                presence_penalty = completion.presence_penalty,
                temperature = completion.temperature,
                messages = completion.messages.Select(x => new ChatGPT.API.HKUST.Message()
                {
                    content = x.content,
                    role = (ChatGPT.API.HKUST.Message.RoleType)x.role,
                }).ToList()
            };

            ChatGPT.API.HKUST.Response? resp;
            try
            {
                resp = hkustcomp.GetResponse(
                    $"https://hkust.azure-api.net/openai/deployments/{completion.model.ToLower()}/chat/completions?api-version=2024-06-01",
                    key);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Fail = ex.ToString() });
            }
            if (resp == null)//生成失败
            {
                return (JsonConvert.SerializeObject(new { Fail = "EmptyResponse" }));
            }
            return resp.GetMessageContent() ?? "EmptyResponse";
        }

        [HttpPost("gpt/v1/chat/completions")]
        public async Task GPTCompletions([FromBody] ChatGPT.API.Framework.Completions completion)
        {
            var Authorization = HttpContext.Request.Headers["Authorization"].ToString();
            HttpContext.Response.ContentType = "application/json";
            if (!Authorization.StartsWith("Bearer "))
            {
                HttpContext.Response.StatusCode = 401;
                await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                    new { Error = "Error Key: Authorization need Start With `Bearer `" }
                    ));
                return;
            }
            string key = Authorization.Substring(7);

            if (completion.messages.Count == 0)
            {
                await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { Fail = "Error Key" }));
                return;
            }

            //预处理
            ChatGPT.API.HKUST.Completions hkustcomp = new ChatGPT.API.HKUST.Completions()
            {
                frequency_penalty = completion.frequency_penalty,
                max_tokens = completion.max_tokens,
                n = completion.n,
                presence_penalty = completion.presence_penalty,
                temperature = completion.temperature,
                messages = completion.messages.Select(x => new ChatGPT.API.HKUST.Message()
                {
                    content = x.content,
                    role = (ChatGPT.API.HKUST.Message.RoleType)x.role,
                }).ToList()
            };

            ChatGPT.API.HKUST.Response? resp;
            try
            {
                resp = hkustcomp.GetResponse(
                    $"https://hkust.azure-api.net/openai/deployments/{completion.model.ToLower()}/chat/completions?api-version=2024-06-01",
                    key);
            }
            catch (Exception ex)
            {
                await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { Fail = ex.ToString() }));
                return;
            }
            if (resp == null)//生成失败
            {
                await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { Fail = "EmptyResponse" }));
                return;
            }
            await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(resp));
            return;
        }
    }
}
