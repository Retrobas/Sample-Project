using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SystemOption
{
    public SystemOption(float bgm, float se)
    {
        this.bgm = bgm;
        this.se = se;
    }

    public float bgm;
    public float se;
}

public class SaveManager : Singleton<SaveManager>
{
    public void SaveSystemOption(SystemOption option)
    {
        var textdata = SaveLoad.ObjectToJson(option);
        var AEStextdata = AES256.Encrypt256(textdata, "aes256=32CharA49AScdg5135=48Fk63");
        SaveLoad.CreateJsonFile(Application.streamingAssetsPath, "SystemOptionData", AEStextdata);
    }

    public SystemOption LoadSystemOption()
    {
        try
        {
            var data = SaveLoad.LoadJsonFileAES<SystemOption>(Application.streamingAssetsPath, "SystemOptionData", "aes256=32CharA49AScdg5135=48Fk63");
            return data;
        }
        catch (IOException e)
        {
            return null;
        }
    }
}
