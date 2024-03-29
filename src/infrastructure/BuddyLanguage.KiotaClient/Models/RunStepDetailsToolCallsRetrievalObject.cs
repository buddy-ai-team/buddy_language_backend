// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    internal class RunStepDetailsToolCallsRetrievalObject : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The ID of the tool call object.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Id { get; set; }
#nullable restore
#else
        public string Id { get; set; }
#endif
        /// <summary>For now, this is always going to be an empty object.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RunStepDetailsToolCallsRetrievalObject_retrieval? Retrieval { get; set; }
#nullable restore
#else
        public RunStepDetailsToolCallsRetrievalObject_retrieval Retrieval { get; set; }
#endif
        /// <summary>The type of tool call. This is always going to be `retrieval` for this type of tool call.</summary>
        public RunStepDetailsToolCallsRetrievalObject_type? Type { get; set; }
        /// <summary>
        /// Instantiates a new RunStepDetailsToolCallsRetrievalObject and sets the default values.
        /// </summary>
        public RunStepDetailsToolCallsRetrievalObject() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static RunStepDetailsToolCallsRetrievalObject CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new RunStepDetailsToolCallsRetrievalObject();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"id", n => { Id = n.GetStringValue(); } },
                {"retrieval", n => { Retrieval = n.GetObjectValue<RunStepDetailsToolCallsRetrievalObject_retrieval>(RunStepDetailsToolCallsRetrievalObject_retrieval.CreateFromDiscriminatorValue); } },
                {"type", n => { Type = n.GetEnumValue<RunStepDetailsToolCallsRetrievalObject_type>(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("id", Id);
            writer.WriteObjectValue<RunStepDetailsToolCallsRetrievalObject_retrieval>("retrieval", Retrieval);
            writer.WriteEnumValue<RunStepDetailsToolCallsRetrievalObject_type>("type", Type);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}

