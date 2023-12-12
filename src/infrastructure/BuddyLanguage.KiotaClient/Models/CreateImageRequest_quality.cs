// <auto-generated/>
using System.Runtime.Serialization;
using System;
namespace OpenAI.GeneratedKiotaClient.Models {
    /// <summary>The quality of the image that will be generated. `hd` creates images with finer details and greater consistency across the image. This param is only supported for `dall-e-3`.</summary>
    internal enum CreateImageRequest_quality {
        [EnumMember(Value = "standard")]
        Standard,
        [EnumMember(Value = "hd")]
        Hd,
    }
}

