// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    internal class CreateEmbeddingResponse : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The list of embeddings generated by the model.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<Embedding>? Data { get; set; }
#nullable restore
#else
        public List<Embedding> Data { get; set; }
#endif
        /// <summary>The name of the model used to generate the embedding.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Model { get; set; }
#nullable restore
#else
        public string Model { get; set; }
#endif
        /// <summary>The object type, which is always &quot;list&quot;.</summary>
        public CreateEmbeddingResponse_object? Object { get; set; }
        /// <summary>The usage information for the request.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public CreateEmbeddingResponse_usage? Usage { get; set; }
#nullable restore
#else
        public CreateEmbeddingResponse_usage Usage { get; set; }
#endif
        /// <summary>
        /// Instantiates a new CreateEmbeddingResponse and sets the default values.
        /// </summary>
        public CreateEmbeddingResponse() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static CreateEmbeddingResponse CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new CreateEmbeddingResponse();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"data", n => { Data = n.GetCollectionOfObjectValues<Embedding>(Embedding.CreateFromDiscriminatorValue)?.ToList(); } },
                {"model", n => { Model = n.GetStringValue(); } },
                {"object", n => { Object = n.GetEnumValue<CreateEmbeddingResponse_object>(); } },
                {"usage", n => { Usage = n.GetObjectValue<CreateEmbeddingResponse_usage>(CreateEmbeddingResponse_usage.CreateFromDiscriminatorValue); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteCollectionOfObjectValues<Embedding>("data", Data);
            writer.WriteStringValue("model", Model);
            writer.WriteEnumValue<CreateEmbeddingResponse_object>("object", Object);
            writer.WriteObjectValue<CreateEmbeddingResponse_usage>("usage", Usage);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}

