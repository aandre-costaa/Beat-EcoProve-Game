using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionWrapper
{
    [JsonProperty("result")]
    public List<QuestionDto> Result { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("exception")]
    public string Exception { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("isCanceled")]
    public bool IsCanceled { get; set; }

    [JsonProperty("isCompleted")]
    public bool IsCompleted { get; set; }

    [JsonProperty("isCompletedSuccessfully")]
    public bool IsCompletedSuccessfully { get; set; }
}
