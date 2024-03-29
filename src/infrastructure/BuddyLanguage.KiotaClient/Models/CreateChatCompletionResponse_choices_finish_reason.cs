// <auto-generated/>
using System.Runtime.Serialization;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    /// <summary>The reason the model stopped generating tokens. This will be `stop` if the model hit a natural stop point or a provided stop sequence,`length` if the maximum number of tokens specified in the request was reached,`content_filter` if content was omitted due to a flag from our content filters,`tool_calls` if the model called a tool, or `function_call` (deprecated) if the model called a function.</summary>
    internal enum CreateChatCompletionResponse_choices_finish_reason {
        [EnumMember(Value = "stop")]
        Stop,
        [EnumMember(Value = "length")]
        Length,
        [EnumMember(Value = "tool_calls")]
        Tool_calls,
        [EnumMember(Value = "content_filter")]
        Content_filter,
        [EnumMember(Value = "function_call")]
        Function_call,
    }
}

