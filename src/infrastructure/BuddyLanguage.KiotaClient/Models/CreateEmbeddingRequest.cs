// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    internal class CreateEmbeddingRequest : IParsable {
        /// <summary>The format to return the embeddings in. Can be either `float` or [`base64`](https://pypi.org/project/pybase64/).</summary>
        public CreateEmbeddingRequest_encoding_format? EncodingFormat { get; set; }
        /// <summary>Input text to embed, encoded as a string or array of tokens. To embed multiple inputs in a single request, pass an array of strings or array of token arrays. The input must not exceed the max input tokens for the model (8192 tokens for `text-embedding-ada-002`) and cannot be an empty string. [Example Python code](https://cookbook.openai.com/examples/how_to_count_tokens_with_tiktoken) for counting tokens.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public CreateEmbeddingRequest_input? Input { get; set; }
#nullable restore
#else
        public CreateEmbeddingRequest_input Input { get; set; }
#endif
        /// <summary>ID of the model to use. You can use the [List models](/docs/api-reference/models/list) API to see all of your available models, or see our [Model overview](/docs/models/overview) for descriptions of them.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public CreateEmbeddingRequest_model? Model { get; set; }
#nullable restore
#else
        public CreateEmbeddingRequest_model Model { get; set; }
#endif
        /// <summary>A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse. [Learn more](/docs/guides/safety-best-practices/end-user-ids).</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? User { get; set; }
#nullable restore
#else
        public string User { get; set; }
#endif
        /// <summary>
        /// Instantiates a new CreateEmbeddingRequest and sets the default values.
        /// </summary>
        public CreateEmbeddingRequest() {
            EncodingFormat = CreateEmbeddingRequest_encoding_format.Float;
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static CreateEmbeddingRequest CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new CreateEmbeddingRequest();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"encoding_format", n => { EncodingFormat = n.GetEnumValue<CreateEmbeddingRequest_encoding_format>(); } },
                {"input", n => { Input = n.GetObjectValue<CreateEmbeddingRequest_input>(CreateEmbeddingRequest_input.CreateFromDiscriminatorValue); } },
                {"model", n => { Model = n.GetObjectValue<CreateEmbeddingRequest_model>(CreateEmbeddingRequest_model.CreateFromDiscriminatorValue); } },
                {"user", n => { User = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteEnumValue<CreateEmbeddingRequest_encoding_format>("encoding_format", EncodingFormat);
            writer.WriteObjectValue<CreateEmbeddingRequest_input>("input", Input);
            writer.WriteObjectValue<CreateEmbeddingRequest_model>("model", Model);
            writer.WriteStringValue("user", User);
        }
        /// <summary>
        /// Composed type wrapper for classes CreateEmbeddingRequest_inputMember1, integer, string
        /// </summary>
        internal class CreateEmbeddingRequest_input : IComposedTypeWrapper, IParsable {
            /// <summary>Composed type representation for type CreateEmbeddingRequest_inputMember1</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public CreateEmbeddingRequest_inputMember1? CreateEmbeddingRequestInputMember1 { get; set; }
#nullable restore
#else
            public CreateEmbeddingRequest_inputMember1 CreateEmbeddingRequestInputMember1 { get; set; }
#endif
            /// <summary>Composed type representation for type integer</summary>
            public int? Integer { get; set; }
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
            public static CreateEmbeddingRequest_input CreateFromDiscriminatorValue(IParseNode parseNode) {
                _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
                var mappingValue = parseNode.GetChildNode("")?.GetStringValue();
                var result = new CreateEmbeddingRequest_input();
                if("".Equals(mappingValue, StringComparison.OrdinalIgnoreCase)) {
                    result.CreateEmbeddingRequestInputMember1 = new CreateEmbeddingRequest_inputMember1();
                }
                else if(parseNode.GetIntValue() is int integerValue) {
                    result.Integer = integerValue;
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
                if(CreateEmbeddingRequestInputMember1 != null) {
                    return CreateEmbeddingRequestInputMember1.GetFieldDeserializers();
                }
                return new Dictionary<string, Action<IParseNode>>();
            }
            /// <summary>
            /// Serializes information the current object
            /// </summary>
            /// <param name="writer">Serialization writer to use to serialize this model</param>
            public virtual void Serialize(ISerializationWriter writer) {
                _ = writer ?? throw new ArgumentNullException(nameof(writer));
                if(CreateEmbeddingRequestInputMember1 != null) {
                    writer.WriteObjectValue<CreateEmbeddingRequest_inputMember1>(null, CreateEmbeddingRequestInputMember1);
                }
                else if(Integer != null) {
                    writer.WriteIntValue(null, Integer);
                }
                else if(String != null) {
                    writer.WriteStringValue(null, String);
                }
            }
        }
        /// <summary>
        /// Composed type wrapper for classes string
        /// </summary>
        internal class CreateEmbeddingRequest_model : IComposedTypeWrapper, IParsable {
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
            public static CreateEmbeddingRequest_model CreateFromDiscriminatorValue(IParseNode parseNode) {
                _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
                var result = new CreateEmbeddingRequest_model();
                if(parseNode.GetStringValue() is string stringValue) {
                    result.String = stringValue;
                }
                return result;
            }
            /// <summary>
            /// The deserialization information for the current model
            /// </summary>
            public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
                return new Dictionary<string, Action<IParseNode>>();
            }
            /// <summary>
            /// Serializes information the current object
            /// </summary>
            /// <param name="writer">Serialization writer to use to serialize this model</param>
            public virtual void Serialize(ISerializationWriter writer) {
                _ = writer ?? throw new ArgumentNullException(nameof(writer));
                if(String != null) {
                    writer.WriteStringValue(null, String);
                }
            }
        }
    }
}

