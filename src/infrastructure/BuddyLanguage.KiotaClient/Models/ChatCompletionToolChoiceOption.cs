// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    /// <summary>
    /// Composed type wrapper for classes ChatCompletionNamedToolChoice, string
    /// </summary>
    internal class ChatCompletionToolChoiceOption : IComposedTypeWrapper, IParsable {
        /// <summary>Composed type representation for type ChatCompletionNamedToolChoice</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public OpenAI.GeneratedKiotaClient.Models.ChatCompletionNamedToolChoice? ChatCompletionNamedToolChoice { get; set; }
#nullable restore
#else
        public OpenAI.GeneratedKiotaClient.Models.ChatCompletionNamedToolChoice ChatCompletionNamedToolChoice { get; set; }
#endif
        /// <summary>Composed type representation for type string</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? String { get; set; }
#nullable restore
#else
        public string String { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static ChatCompletionToolChoiceOption CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            var mappingValue = parseNode.GetChildNode("")?.GetStringValue();
            var result = new ChatCompletionToolChoiceOption();
            if("ChatCompletionNamedToolChoice".Equals(mappingValue, StringComparison.OrdinalIgnoreCase)) {
                result.ChatCompletionNamedToolChoice = new OpenAI.GeneratedKiotaClient.Models.ChatCompletionNamedToolChoice();
            }
            else if(parseNode.GetStringValue() is string stringValue) {
                result.String = stringValue;
            }
            return result;
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            if(ChatCompletionNamedToolChoice != null) {
                return ChatCompletionNamedToolChoice.GetFieldDeserializers();
            }
            return new Dictionary<string, Action<IParseNode>>();
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            if(ChatCompletionNamedToolChoice != null) {
                writer.WriteObjectValue<OpenAI.GeneratedKiotaClient.Models.ChatCompletionNamedToolChoice>(null, ChatCompletionNamedToolChoice);
            }
            else if(String != null) {
                writer.WriteStringValue(null, String);
            }
        }
    }
}

