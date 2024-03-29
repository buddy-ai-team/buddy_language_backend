// <auto-generated/>
using System.Runtime.Serialization;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    /// <summary>The current status of the fine-tuning job, which can be either `validating_files`, `queued`, `running`, `succeeded`, `failed`, or `cancelled`.</summary>
    internal enum FineTuningJob_status {
        [EnumMember(Value = "validating_files")]
        Validating_files,
        [EnumMember(Value = "queued")]
        Queued,
        [EnumMember(Value = "running")]
        Running,
        [EnumMember(Value = "succeeded")]
        Succeeded,
        [EnumMember(Value = "failed")]
        Failed,
        [EnumMember(Value = "cancelled")]
        Cancelled,
    }
}

