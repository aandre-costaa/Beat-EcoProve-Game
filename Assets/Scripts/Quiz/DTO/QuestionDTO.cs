using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionDto
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("textoPergunta")]
    public string TextoPergunta { get; set; }

    [JsonProperty("tempoLimite")]
    public string TempoLimite { get; set; }

    [JsonProperty("categoria")]
    public string Categoria { get; set; }

    [JsonProperty("nivelDificuldade")]
    public int NivelDificuldade { get; set; }

    [JsonProperty("tipoPergunta")]
    public string TipoPergunta { get; set; }

    [JsonProperty("verdadeiroFalsos")]
    public List<VerdadeiroFalso> VerdadeiroFalsos { get; set; }

    [JsonProperty("escolhaMultiplas")]
    public List<EscolhaMultipla> EscolhaMultiplas { get; set; }

    [JsonProperty("ordemPalavras")]
    public List<OrdemPalavra> OrdemPalavras { get; set; }
}

public class VerdadeiroFalso
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("correta")]
    public bool Correta { get; set; }

    [JsonProperty("curiosidade")]
    public string Curiosidade { get; set; }
}

public class EscolhaMultipla
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("textoOpcao")]
    public string TextoOpcao { get; set; }

    [JsonProperty("correta")]
    public bool Correta { get; set; }
}

public class OrdemPalavra
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("palavra")]
    public string Palavra { get; set; }

    [JsonProperty("posicao")]
    public int Posicao { get; set; }
}
