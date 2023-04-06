using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Util;

public class ChatGptTranslate
{
    private HttpClient Client { get; } = new();

    /// <summary>
    /// 通过ChatGpt翻译
    /// </summary>
    /// <param name="text">要翻译的文本</param>
    /// <returns>翻译的文本</returns>
    public async Task<string> Chat(string text)
    {
        var url = @"https://api.openai.com/v1/chat/completions";
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "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");
        var data = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = $"请把大括号里面的英文部分翻译成中文：{{{text}}}"
                }
            }
        };

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        });

        var response = await Client.PostAsync(url, new StringContent(json, Encoding.UTF8 ,"application/json"));
        var responseJson = await response.Content.ReadAsStringAsync();
        var resModel = JsonSerializer.Deserialize<ChatGptResponse>(responseJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return resModel.Choices.First().Message.Content;
    }
}