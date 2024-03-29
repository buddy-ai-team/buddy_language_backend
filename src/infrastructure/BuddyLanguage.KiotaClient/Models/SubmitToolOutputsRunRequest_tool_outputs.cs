// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    internal class SubmitToolOutputsRunRequest_tool_outputs : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The output of the tool call to be submitted to continue the run.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Output { get; set; }
#nullable restore
#else
        public string Output { get; set; }
#endif
        /// <summary>The ID of the tool call in the `required_action` object within the run object the output is being submitted for.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ToolCallId { get; set; }
#nullable restore
#else
        public string ToolCallId { get; set; }
#endif
        /// <summary>
        /// Instantiates a new SubmitToolOutputsRunRequest_tool_outputs and sets the default values.
        /// </summary>
        public SubmitToolOutputsRunRequest_tool_outputs() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static SubmitToolOutputsRunRequest_tool_outputs CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new SubmitToolOutputsRunRequest_tool_outputs();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"output", n => { Output = n.GetStringValue(); } },
                {"tool_call_id", n => { ToolCallId = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("output", Output);
            writer.WriteStringValue("tool_call_id", ToolCallId);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}

