using System;

[Serializable]
public class MainProfile
{
    public string id;
    public string username;
    public int level;
    public int levelPercentage;
    public int sustainabilityPoints;
    public int ecoScore;
    public int ecoCoins;
    public string avatarUrl;
}

[Serializable]
public class NestedProfile
{
}