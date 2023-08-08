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
            string myMPQ = "Data/base.MPQ";
            string path = Path.Combine(settingsManager.settings.GameFilesPath, myMPQ);
            // string myMPQ = "Tests/Empty.MPQ";
            // string path = Path.Combine(Application.dataPath, myMPQ);
            
            try {
                MPQReader.LoadMPQ(path);
            }
            catch (Exception ex) {
                Debug.LogException(ex);
            }
        }
    }
    
}
