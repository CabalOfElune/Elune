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
            
            try {
                MPQReader.LoadMPQ(path);
            }
            catch (Exception ex) {
                Debug.LogException(ex);
            }
        }
    }
    
}
