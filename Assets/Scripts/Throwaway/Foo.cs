using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Elune.MPQ;

namespace Elune.Throwaway {
    
    public class Foo : MonoBehaviour
    {
        public SettingsManager settingsManager;

        // Start is called before the first frame update
        void Start()
        {
            string myMPQ = "Data/base.MPQ";
            string path = Path.Combine(settingsManager.settings.GameFilesPath, myMPQ);
            MPQReader.LoadMPQ(path);
        }
    }
    
}