// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    /// <summary>
    /// Represents an `assistant` that can call the model and use tools.
    /// </summary>
    internal class AssistantObject : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The Unix timestamp (in seconds) for when the assistant was created.</summary>
        public int? CreatedAt { get; set; }
        /// <summary>The description of the assistant. The maximum length is 512 characters.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Description { get; set; }
#nullable restore
#else
        public string Description { get; set; }
#endif
        /// <summary>A list of [file](/docs/api-reference/files) IDs attached to this assistant. There can be a maximum of 20 files attached to the assistant. Files are ordered by their creation date in ascending order.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<string>? FileIds { get; set; }
#nullable restore
#else
        public List<string> FileIds { get; set; }
#endif
        /// <summary>The identifier, which can be referenced in API endpoints.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Id { get; set; }
#nullable restore
#else
        public string Id { get; set; }
#endif
        /// <summary>The system instructions that the assistant uses. The maximum length is 32768 characters.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Instructions { get; set; }
#nullable restore
#else
        public string Instructions { get; set; }
#endif
        /// <summary>Set of 16 key-value pairs that can be attached to an object. This can be useful for storing additional information about the object in a structured format. Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public AssistantObject_metadata? Metadata { get; set; }
#nullable restore
#else
        public AssistantObject_metadata Metadata { get; set; }
#endif
        /// <summary>ID of the model to use. You can use the [List models](/docs/api-reference/models/list) API to see all of your available models, or see our [Model overview](/docs/models/overview) for descriptions of them.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Model { get; set; }
#nullable restore
#else
        public string Model { get; set; }
#endif
        /// <summary>The name of the assistant. The maximum length is 256 characters.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Name { get; set; }
#nullable restore
#else
        public string Name { get; set; }
#endif
        /// <summary>The object type, which is always `assistant`.</summary>
        public AssistantObject_object? Object { get; set; }
        /// <summary>A list of tool enabled on the assistant. There can be a maximum of 128 tools per assistant. Tools can be of types `code_interpreter`, `retrieval`, or `function`.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<AssistantObject_tools>? Tools { get; set; }
#nullable restore
#else
        public List<AssistantObject_tools> Tools { get; set; }
#endif
        /// <summary>
        /// Instantiates a new AssistantObject and sets the default values.
        /// </summary>
        public AssistantObject() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static AssistantObject CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new AssistantObject();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"created_at", n => { CreatedAt = n.GetIntValue(); } },
                {"description", n => { Description = n.GetStringValue(); } },
                {"file_ids", n => { FileIds = n.GetCollectionOfPrimitiveValues<string>()?.ToList(); } },
                {"id", n => { Id = n.GetStringValue(); } },
                {"instructions", n => { Instructions = n.GetStringValue(); } },
                {"metadata", n => { Metadata = n.GetObjectValue<AssistantObject_metadata>(AssistantObject_metadata.CreateFromDiscriminatorValue); } },
                {"model", n => { Model = n.GetStringValue(); } },
                {"name", n => { Name = n.GetStringValue(); } },
                {"object", n => { Object = n.GetEnumValue<AssistantObject_object>(); } },
                {"tools", n => { Tools = n.GetCollectionOfObjectValues<AssistantObject_tools>(AssistantObject_tools.CreateFromDiscriminatorValue)?.ToList(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteIntValue("created_at", CreatedAt);
            writer.WriteStringValue("description", Description);
            writer.WriteCollectionOfPrimitiveValues<string>("file_ids", FileIds);
            writer.WriteStringValue("id", Id);
            writer.WriteStringValue("instructions", Instructions);
            writer.WriteObjectValue<AssistantObject_metadata>("metadata", Metadata);
            writer.WriteStringValue("model", Model);
            writer.WriteStringValue("name", Name);
            writer.WriteEnumValue<AssistantObject_object>("object", Object);
            writer.WriteCollectionOfObjectValues<AssistantObject_tools>("tools", Tools);
            writer.WriteAdditionalData(AdditionalData);
        }
        /// <summary>
        /// Composed type wrapper for classes AssistantToolsCode, AssistantToolsFunction, AssistantToolsRetrieval
        /// </summary>
        internal class AssistantObject_tools : IComposedTypeWrapper, IParsable {
            /// <summary>Composed type representation for type AssistantToolsCode</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public OpenAI.GeneratedKiotaClient.Models.AssistantToolsCode? AssistantToolsCode { get; set; }
#nullable restore
#else
            public OpenAI.GeneratedKiotaClient.Models.AssistantToolsCode AssistantToolsCode { get; set; }
#endif
            /// <summary>Composed type representation for type AssistantToolsFunction</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public OpenAI.GeneratedKiotaClient.Models.AssistantToolsFunction? AssistantToolsFunction { get; set; }
#nullable restore
#else
            public OpenAI.GeneratedKiotaClient.Models.AssistantToolsFunction AssistantToolsFunction { get; set; }
#endif
            /// <summary>Composed type representation for type AssistantToolsRetrieval</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public OpenAI.GeneratedKiotaClient.Models.AssistantToolsRetrieval? AssistantToolsRetrieval { get; set; }
#nullable restore
#else
            public OpenAI.GeneratedKiotaClient.Models.AssistantToolsRetrieval AssistantToolsRetrieval { get; set; }
#endif
            /// <summary>
            /// Creates a new instance of the appropriate class based on discriminator value
            /// </summary>
            /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
            public static AssistantObject_tools CreateFromDiscriminatorValue(IParseNode parseNode) {
                _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
                var mappingValue = parseNode.GetChildNode("")?.GetStringValue();
                var result = new AssistantObject_tools();
                if("AssistantToolsCode".Equals(mappingValue, StringComparison.OrdinalIgnoreCase)) {
                    result.AssistantToolsCode = new OpenAI.GeneratedKiotaClient.Models.AssistantToolsCode();
                }
                else if("AssistantToolsFunction".Equals(mappingValue, StringComparison.OrdinalIgnoreCase)) {
                    result.AssistantToolsFunction = new OpenAI.GeneratedKiotaClient.Models.AssistantToolsFunction();
                }
                else if("AssistantToolsRetrieval".Equals(mappingValue, StringComparison.OrdinalIgnoreCase)) {
                    result.AssistantToolsRetrieval = new OpenAI.GeneratedKiotaClient.Models.AssistantToolsRetrieval();
                }
                return result;
            }
            /// <summary>
            /// The deserialization information for the current model
            /// </summary>
            public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
                if(AssistantToolsCode != null) {
                    return AssistantToolsCode.GetFieldDeserializers();
                }
                else if(AssistantToolsFunction != null) {
                    return AssistantToolsFunction.GetFieldDeserializers();
                }
                else if(AssistantToolsRetrieval != null) {
                    return AssistantToolsRetrieval.GetFieldDeserializers();
                }
                return new Dictionary<string, Action<IParseNode>>();
            }
            /// <summary>
            /// Serializes information the current object
            /// </summary>
            /// <param name="writer">Serialization writer to use to serialize this model</param>
            public virtual void Serialize(ISerializationWriter writer) {
                _ = writer ?? throw new ArgumentNullException(nameof(writer));
                if(AssistantToolsCode != null) {
                    writer.WriteObjectValue<OpenAI.GeneratedKiotaClient.Models.AssistantToolsCode>(null, AssistantToolsCode);
                }
                else if(AssistantToolsFunction != null) {
                    writer.WriteObjectValue<OpenAI.GeneratedKiotaClient.Models.AssistantToolsFunction>(null, AssistantToolsFunction);
                }
                else if(AssistantToolsRetrieval != null) {
                    writer.WriteObjectValue<OpenAI.GeneratedKiotaClient.Models.AssistantToolsRetrieval>(null, AssistantToolsRetrieval);
                }
            }
        }
    }
}

