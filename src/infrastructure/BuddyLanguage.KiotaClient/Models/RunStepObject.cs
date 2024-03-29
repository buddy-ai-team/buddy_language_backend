// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    /// <summary>
    /// Represents a step in execution of a run.
    /// </summary>
    internal class RunStepObject : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The ID of the [assistant](/docs/api-reference/assistants) associated with the run step.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? AssistantId { get; set; }
#nullable restore
#else
        public string AssistantId { get; set; }
#endif
        /// <summary>The Unix timestamp (in seconds) for when the run step was cancelled.</summary>
        public int? CancelledAt { get; set; }
        /// <summary>The Unix timestamp (in seconds) for when the run step completed.</summary>
        public int? CompletedAt { get; set; }
        /// <summary>The Unix timestamp (in seconds) for when the run step was created.</summary>
        public int? CreatedAt { get; set; }
        /// <summary>The Unix timestamp (in seconds) for when the run step expired. A step is considered expired if the parent run is expired.</summary>
        public int? ExpiredAt { get; set; }
        /// <summary>The Unix timestamp (in seconds) for when the run step failed.</summary>
        public int? FailedAt { get; set; }
        /// <summary>The identifier of the run step, which can be referenced in API endpoints.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Id { get; set; }
#nullable restore
#else
        public string Id { get; set; }
#endif
        /// <summary>The last error associated with this run step. Will be `null` if there are no errors.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RunStepObject_last_error? LastError { get; set; }
#nullable restore
#else
        public RunStepObject_last_error LastError { get; set; }
#endif
        /// <summary>Set of 16 key-value pairs that can be attached to an object. This can be useful for storing additional information about the object in a structured format. Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RunStepObject_metadata? Metadata { get; set; }
#nullable restore
#else
        public RunStepObject_metadata Metadata { get; set; }
#endif
        /// <summary>The object type, which is always `thread.run.step``.</summary>
        public RunStepObject_object? Object { get; set; }
        /// <summary>The ID of the [run](/docs/api-reference/runs) that this run step is a part of.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? RunId { get; set; }
#nullable restore
#else
        public string RunId { get; set; }
#endif
        /// <summary>The status of the run step, which can be either `in_progress`, `cancelled`, `failed`, `completed`, or `expired`.</summary>
        public RunStepObject_status? Status { get; set; }
        /// <summary>The details of the run step.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RunStepObject_step_details? StepDetails { get; set; }
#nullable restore
#else
        public RunStepObject_step_details StepDetails { get; set; }
#endif
        /// <summary>The ID of the [thread](/docs/api-reference/threads) that was run.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ThreadId { get; set; }
#nullable restore
#else
        public string ThreadId { get; set; }
#endif
        /// <summary>The type of run step, which can be either `message_creation` or `tool_calls`.</summary>
        public RunStepObject_type? Type { get; set; }
        /// <summary>
        /// Instantiates a new RunStepObject and sets the default values.
        /// </summary>
        public RunStepObject() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static RunStepObject CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new RunStepObject();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"assistant_id", n => { AssistantId = n.GetStringValue(); } },
                {"cancelled_at", n => { CancelledAt = n.GetIntValue(); } },
                {"completed_at", n => { CompletedAt = n.GetIntValue(); } },
                {"created_at", n => { CreatedAt = n.GetIntValue(); } },
                {"expired_at", n => { ExpiredAt = n.GetIntValue(); } },
                {"failed_at", n => { FailedAt = n.GetIntValue(); } },
                {"id", n => { Id = n.GetStringValue(); } },
                {"last_error", n => { LastError = n.GetObjectValue<RunStepObject_last_error>(RunStepObject_last_error.CreateFromDiscriminatorValue); } },
                {"metadata", n => { Metadata = n.GetObjectValue<RunStepObject_metadata>(RunStepObject_metadata.CreateFromDiscriminatorValue); } },
                {"object", n => { Object = n.GetEnumValue<RunStepObject_object>(); } },
                {"run_id", n => { RunId = n.GetStringValue(); } },
                {"status", n => { Status = n.GetEnumValue<RunStepObject_status>(); } },
                {"step_details", n => { StepDetails = n.GetObjectValue<RunStepObject_step_details>(RunStepObject_step_details.CreateFromDiscriminatorValue); } },
                {"thread_id", n => { ThreadId = n.GetStringValue(); } },
                {"type", n => { Type = n.GetEnumValue<RunStepObject_type>(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("assistant_id", AssistantId);
            writer.WriteIntValue("cancelled_at", CancelledAt);
            writer.WriteIntValue("completed_at", CompletedAt);
            writer.WriteIntValue("created_at", CreatedAt);
            writer.WriteIntValue("expired_at", ExpiredAt);
            writer.WriteIntValue("failed_at", FailedAt);
            writer.WriteStringValue("id", Id);
            writer.WriteObjectValue<RunStepObject_last_error>("last_error", LastError);
            writer.WriteObjectValue<RunStepObject_metadata>("metadata", Metadata);
            writer.WriteEnumValue<RunStepObject_object>("object", Object);
            writer.WriteStringValue("run_id", RunId);
            writer.WriteEnumValue<RunStepObject_status>("status", Status);
            writer.WriteObjectValue<RunStepObject_step_details>("step_details", StepDetails);
            writer.WriteStringValue("thread_id", ThreadId);
            writer.WriteEnumValue<RunStepObject_type>("type", Type);
            writer.WriteAdditionalData(AdditionalData);
        }
        /// <summary>
        /// Composed type wrapper for classes RunStepDetailsMessageCreationObject, RunStepDetailsToolCallsObject
        /// </summary>
        internal class RunStepObject_step_details : IComposedTypeWrapper, IParsable {
            /// <summary>Composed type representation for type RunStepDetailsMessageCreationObject</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public OpenAI.GeneratedKiotaClient.Models.RunStepDetailsMessageCreationObject? RunStepDetailsMessageCreationObject { get; set; }
#nullable restore
#else
            public OpenAI.GeneratedKiotaClient.Models.RunStepDetailsMessageCreationObject RunStepDetailsMessageCreationObject { get; set; }
#endif
            /// <summary>Composed type representation for type RunStepDetailsToolCallsObject</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            public OpenAI.GeneratedKiotaClient.Models.RunStepDetailsToolCallsObject? RunStepDetailsToolCallsObject { get; set; }
#nullable restore
#else
            public OpenAI.GeneratedKiotaClient.Models.RunStepDetailsToolCallsObject RunStepDetailsToolCallsObject { get; set; }
#endif
            /// <summary>
            /// Creates a new instance of the appropriate class based on discriminator value
            /// </summary>
            /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
            public static RunStepObject_step_details CreateFromDiscriminatorValue(IParseNode parseNode) {
                _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
                var mappingValue = parseNode.GetChildNode("")?.GetStringValue();
                var result = new RunStepObject_step_details();
                if("RunStepDetailsMessageCreationObject".Equals(mappingValue, StringComparison.OrdinalIgnoreCase)) {
                    result.RunStepDetailsMessageCreationObject = new OpenAI.GeneratedKiotaClient.Models.RunStepDetailsMessageCreationObject();
                }
                else if("RunStepDetailsToolCallsObject".Equals(mappingValue, StringComparison.OrdinalIgnoreCase)) {
                    result.RunStepDetailsToolCallsObject = new OpenAI.GeneratedKiotaClient.Models.RunStepDetailsToolCallsObject();
                }
                return result;
            }
            /// <summary>
            /// The deserialization information for the current model
            /// </summary>
            public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
                if(RunStepDetailsMessageCreationObject != null) {
                    return RunStepDetailsMessageCreationObject.GetFieldDeserializers();
                }
                else if(RunStepDetailsToolCallsObject != null) {
                    return RunStepDetailsToolCallsObject.GetFieldDeserializers();
                }
                return new Dictionary<string, Action<IParseNode>>();
            }
            /// <summary>
            /// Serializes information the current object
            /// </summary>
            /// <param name="writer">Serialization writer to use to serialize this model</param>
            public virtual void Serialize(ISerializationWriter writer) {
                _ = writer ?? throw new ArgumentNullException(nameof(writer));
                if(RunStepDetailsMessageCreationObject != null) {
                    writer.WriteObjectValue<OpenAI.GeneratedKiotaClient.Models.RunStepDetailsMessageCreationObject>(null, RunStepDetailsMessageCreationObject);
                }
                else if(RunStepDetailsToolCallsObject != null) {
                    writer.WriteObjectValue<OpenAI.GeneratedKiotaClient.Models.RunStepDetailsToolCallsObject>(null, RunStepDetailsToolCallsObject);
                }
            }
        }
    }
}

