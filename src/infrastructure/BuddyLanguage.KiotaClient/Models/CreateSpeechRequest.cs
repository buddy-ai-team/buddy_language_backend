// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    internal class CreateSpeechRequest : IParsable {
        /// <summary>The text to generate audio for. The maximum length is 4096 characters.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Input { get; set; }
#nullable restore
#else
        public string Input { get; set; }
#endif
        /// <summary>One of the available [TTS models](/docs/models/tts): `tts-1` or `tts-1-hd`</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public CreateSpeechRequest_model? Model { get; set; }
#nullable restore
#else
        public CreateSpeechRequest_model Model { get; set; }
#endif
        /// <summary>The format to audio in. Supported formats are `mp3`, `opus`, `aac`, and `flac`.</summary>
        public CreateSpeechRequest_response_format? ResponseFormat { get; set; }
        /// <summary>The speed of the generated audio. Select a value from `0.25` to `4.0`. `1.0` is the default.</summary>
        public double? Speed { get; set; }
        /// <summary>The voice to use when generating the audio. Supported voices are `alloy`, `echo`, `fable`, `onyx`, `nova`, and `shimmer`.</summary>
        public CreateSpeechRequest_voice? Voice { get; set; }
        /// <summary>
        /// Instantiates a new CreateSpeechRequest and sets the default values.
        /// </summary>
        public CreateSpeechRequest() {
            ResponseFormat = CreateSpeechRequest_response_format.Mp3;
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static CreateSpeechRequest CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new CreateSpeechRequest();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"input", n => { Input = n.GetStringValue(); } },
                {"model", n => { Model = n.GetObjectValue<CreateSpeechRequest_model>(CreateSpeechRequest_model.CreateFromDiscriminatorValue); } },
                {"response_format", n => { ResponseFormat = n.GetEnumValue<CreateSpeechRequest_response_format>(); } },
                {"speed", n => { Speed = n.GetDoubleValue(); } },
                {"voice", n => { Voice = n.GetEnumValue<CreateSpeechRequest_voice>(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("input", Input);
            writer.WriteObjectValue<CreateSpeechRequest_model>("model", Model);
            writer.WriteEnumValue<CreateSpeechRequest_response_format>("response_format", ResponseFormat);
            writer.WriteDoubleValue("speed", Speed);
            writer.WriteEnumValue<CreateSpeechRequest_voice>("voice", Voice);
        }
        /// <summary>
        /// Composed type wrapper for classes string
        /// </summary>
        internal class CreateSpeechRequest_model : IComposedTypeWrapper, IParsable {
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
            public static CreateSpeechRequest_model CreateFromDiscriminatorValue(IParseNode parseNode) {
                _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
                var result = new CreateSpeechRequest_model();
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

