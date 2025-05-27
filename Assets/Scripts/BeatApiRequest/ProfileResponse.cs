using System;

[Serializable]
public class ProfileResponse
{
    public MainProfile mainProfile;
    public NestedProfile[] nestedProfiles;
}