using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Elune.MPQ;
using System;

namespace Elune.Throwaway {
    
public class Foo : MonoBehaviour
{
    public SettingsManager settingsManager;

    // Start is called before the first frame update
    void Start()
    {
        // Only load MPQs if the path is valid
        string wowDir = settingsManager.settings.GameFilesPath;
        if(wowDir == null || wowDir == "") {
            return;
        }

        string myMPQ = "Data/base.MPQ";
        string path = Path.Combine(settingsManager.settings.GameFilesPath, myMPQ);

        path = Path.Combine(Application.dataPath, "Tests/FileSearch.MPQ");

        Cryptography crypto = new Cryptography();
        
        try {
            var serializer = new YamlDotNet.Serialization.SerializerBuilder().Build();
            MPQArchive archive = MPQReader.LoadMPQ(path);
            var yaml = serializer.Serialize(archive.Header);
            Debug.Log(yaml);
            yaml = serializer.Serialize(archive.HashTable);
            Debug.Log(yaml);

            yaml = serializer.Serialize(archive.HashTable.Get(0));
            Debug.Log(yaml);

            var file = archive.FindFile(crypto, "foo/example.txt");
        }
        catch (Exception ex) {
            Debug.LogException(ex);
        }
    }
    
}   

} // Namespace
