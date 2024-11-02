using ChatGPT.API.Framework;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Text;

namespace ChatUST.API.Convert
{
    public class NewtonsoftJsonModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // 从请求体中读取原始JSON字符串
            using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body, Encoding.UTF8))
            {
                var json = await reader.ReadToEndAsync();

                if (string.IsNullOrEmpty(json))
                {
                    return;
                }

                try
                {
                    // 使用Newtonsoft.Json进行反序列化
                    var model = JsonConvert.DeserializeObject(json, bindingContext.ModelType);
                    bindingContext.Result = ModelBindingResult.Success(model);
                }
                catch (JsonException ex)
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"Error occurred while parsing JSON: {ex.Message}");
                }
            }
        }
    }
    public class NewtonsoftJsonModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                return null;
            }

            if (context.Metadata.ModelType == typeof(Completions))
            {
                return new NewtonsoftJsonModelBinder();
            }

            return null;
        }
    }
}
