// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    /// <summary>
    /// Specifies a tool the model should use. Use to force the model to call a specific function.
    /// </summary>
    internal class ChatCompletionNamedToolChoice : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The function property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public ChatCompletionNamedToolChoice_function? Function { get; set; }
#nullable restore
#else
        public ChatCompletionNamedToolChoice_function Function { get; set; }
#endif
        /// <summary>The type of the tool. Currently, only `function` is supported.</summary>
        public ChatCompletionNamedToolChoice_type? Type { get; set; }
        /// <summary>
        /// Instantiates a new ChatCompletionNamedToolChoice and sets the default values.
        /// </summary>
        public ChatCompletionNamedToolChoice() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static ChatCompletionNamedToolChoice CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new ChatCompletionNamedToolChoice();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"function", n => { Function = n.GetObjectValue<ChatCompletionNamedToolChoice_function>(ChatCompletionNamedToolChoice_function.CreateFromDiscriminatorValue); } },
                {"type", n => { Type = n.GetEnumValue<ChatCompletionNamedToolChoice_type>(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<ChatCompletionNamedToolChoice_function>("function", Function);
            writer.WriteEnumValue<ChatCompletionNamedToolChoice_type>("type", Type);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}

