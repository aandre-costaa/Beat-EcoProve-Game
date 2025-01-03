using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// API response classes
public class ApiResponse
{
    [JsonProperty("questions")]
    public QuestionWrapper Questions { get; set; }
}
