using UnityEngine;
using System.IO;
using System.Text;

namespace Elune.MPQ {
    /// <summary>
    /// Reads MPQ Files
    /// </summary>
    public class MPQReader {
        const string ID_MPQ = "MPQ\x1A";
        const string ID_MPQ_USERDATA = "MPQ\x1B";
        const int SEARCH_HEADER_PERIOD = 0x200;
        const int HEADER_SIZE_V1 = 0x20; // Used up to WoW:BC

        public static void LoadMPQ(string filePath) {
            if(!File.Exists(filePath)) {
                throw new FileNotFoundException(filePath);
            }

            FileStream dataStream = new(filePath, FileMode.Open);

            int? headerPointer = FindHeaderPointer(dataStream);
            if(headerPointer == null) {
                Debug.Log("Header pointer not found.");
            }
            Debug.Log($"Header pointer: {headerPointer}");

            dataStream.Close();
        }

        public static int? FindHeaderPointer(FileStream dataStream) {
            int searchIndex = 0;
            while(dataStream.Length >= searchIndex*SEARCH_HEADER_PERIOD + HEADER_SIZE_V1) {
                var headerPointer = searchIndex*SEARCH_HEADER_PERIOD;
                // Try a position
                byte[] bytes = new byte[ID_MPQ.Length];
                dataStream.Seek(headerPointer, SeekOrigin.Begin);
                dataStream.Read(bytes);
                var section = Encoding.UTF8.GetString(bytes);

                if(ID_MPQ.Equals(section)) {
                    // Found it!
                    return headerPointer;
                }
                searchIndex++;
            }
            return null; // I don't like returning -1 =( 
        } 
    }
}