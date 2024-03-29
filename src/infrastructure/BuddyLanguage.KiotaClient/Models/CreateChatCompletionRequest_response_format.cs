// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    /// <summary>
    /// An object specifying the format that the model must output. Setting to `{ &quot;type&quot;: &quot;json_object&quot; }` enables JSON mode, which guarantees the message the model generates is valid JSON.**Important:** when using JSON mode, you **must** also instruct the model to produce JSON yourself via a system or user message. Without this, the model may generate an unending stream of whitespace until the generation reaches the token limit, resulting in increased latency and appearance of a &quot;stuck&quot; request. Also note that the message content may be partially cut off if `finish_reason=&quot;length&quot;`, which indicates the generation exceeded `max_tokens` or the conversation exceeded the max context length.
    /// </summary>
    internal class CreateChatCompletionRequest_response_format : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>Must be one of `text` or `json_object`.</summary>
        public CreateChatCompletionRequest_response_format_type? Type { get; set; }
        /// <summary>
        /// Instantiates a new CreateChatCompletionRequest_response_format and sets the default values.
        /// </summary>
        public CreateChatCompletionRequest_response_format() {
            AdditionalData = new Dictionary<string, object>();
            Type = CreateChatCompletionRequest_response_format_type.Text;
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static CreateChatCompletionRequest_response_format CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new CreateChatCompletionRequest_response_format();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"type", n => { Type = n.GetEnumValue<CreateChatCompletionRequest_response_format_type>(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteEnumValue<CreateChatCompletionRequest_response_format_type>("type", Type);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}

