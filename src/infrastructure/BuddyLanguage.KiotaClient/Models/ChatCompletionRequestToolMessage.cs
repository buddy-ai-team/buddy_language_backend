// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    internal class ChatCompletionRequestToolMessage : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The contents of the tool message.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Content { get; set; }
#nullable restore
#else
        public string Content { get; set; }
#endif
        /// <summary>The role of the messages author, in this case `tool`.</summary>
        public ChatCompletionRequestToolMessage_role? Role { get; set; }
        /// <summary>Tool call that this message is responding to.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ToolCallId { get; set; }
#nullable restore
#else
        public string ToolCallId { get; set; }
#endif
        /// <summary>
        /// Instantiates a new ChatCompletionRequestToolMessage and sets the default values.
        /// </summary>
        public ChatCompletionRequestToolMessage() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static ChatCompletionRequestToolMessage CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new ChatCompletionRequestToolMessage();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"content", n => { Content = n.GetStringValue(); } },
                {"role", n => { Role = n.GetEnumValue<ChatCompletionRequestToolMessage_role>(); } },
                {"tool_call_id", n => { ToolCallId = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("content", Content);
            writer.WriteEnumValue<ChatCompletionRequestToolMessage_role>("role", Role);
            writer.WriteStringValue("tool_call_id", ToolCallId);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}

