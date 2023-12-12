// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    /// <summary>
    /// Details on the action required to continue the run. Will be `null` if no action is required.
    /// </summary>
    internal class RunObject_required_action : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>Details on the tool outputs needed for this run to continue.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RunObject_required_action_submit_tool_outputs? SubmitToolOutputs { get; set; }
#nullable restore
#else
        public RunObject_required_action_submit_tool_outputs SubmitToolOutputs { get; set; }
#endif
        /// <summary>For now, this is always `submit_tool_outputs`.</summary>
        public RunObject_required_action_type? Type { get; set; }
        /// <summary>
        /// Instantiates a new RunObject_required_action and sets the default values.
        /// </summary>
        public RunObject_required_action() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static RunObject_required_action CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new RunObject_required_action();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"submit_tool_outputs", n => { SubmitToolOutputs = n.GetObjectValue<RunObject_required_action_submit_tool_outputs>(RunObject_required_action_submit_tool_outputs.CreateFromDiscriminatorValue); } },
                {"type", n => { Type = n.GetEnumValue<RunObject_required_action_type>(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<RunObject_required_action_submit_tool_outputs>("submit_tool_outputs", SubmitToolOutputs);
            writer.WriteEnumValue<RunObject_required_action_type>("type", Type);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
