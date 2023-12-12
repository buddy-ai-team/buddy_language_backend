// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    internal class CreateMessageRequest : IParsable {
        /// <summary>The content of the message.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Content { get; set; }
#nullable restore
#else
        public string Content { get; set; }
#endif
        /// <summary>A list of [File](/docs/api-reference/files) IDs that the message should use. There can be a maximum of 10 files attached to a message. Useful for tools like `retrieval` and `code_interpreter` that can access and use files.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<string>? FileIds { get; set; }
#nullable restore
#else
        public List<string> FileIds { get; set; }
#endif
        /// <summary>Set of 16 key-value pairs that can be attached to an object. This can be useful for storing additional information about the object in a structured format. Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public CreateMessageRequest_metadata? Metadata { get; set; }
#nullable restore
#else
        public CreateMessageRequest_metadata Metadata { get; set; }
#endif
        /// <summary>The role of the entity that is creating the message. Currently only `user` is supported.</summary>
        public CreateMessageRequest_role? Role { get; set; }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static CreateMessageRequest CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new CreateMessageRequest();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"content", n => { Content = n.GetStringValue(); } },
                {"file_ids", n => { FileIds = n.GetCollectionOfPrimitiveValues<string>()?.ToList(); } },
                {"metadata", n => { Metadata = n.GetObjectValue<CreateMessageRequest_metadata>(CreateMessageRequest_metadata.CreateFromDiscriminatorValue); } },
                {"role", n => { Role = n.GetEnumValue<CreateMessageRequest_role>(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("content", Content);
            writer.WriteCollectionOfPrimitiveValues<string>("file_ids", FileIds);
            writer.WriteObjectValue<CreateMessageRequest_metadata>("metadata", Metadata);
            writer.WriteEnumValue<CreateMessageRequest_role>("role", Role);
        }
    }
}

