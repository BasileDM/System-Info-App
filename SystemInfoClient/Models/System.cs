﻿using System.Text.Json.Serialization;

namespace SystemInfoClient.Models {
    public record class System(
        [property: JsonPropertyName("machineName")] string? MachineName,
        [property: JsonPropertyName("osDrive")] string? OsDrive,
        [property: JsonPropertyName("systemDirectory")] string? SystemDirectory,
        [property: JsonPropertyName("osArchitecture")] string? OsArchitecture,
        [property: JsonPropertyName("osVersion")] string? OsVersion,
        [property: JsonPropertyName("productName")] string? ProductName,
        [property: JsonPropertyName("releaseId")] string? ReleaseId,
        [property: JsonPropertyName("currentBuild")] string? CurrentBuild,
        [property: JsonPropertyName("ubr")] string? Ubr) {
    }
}
