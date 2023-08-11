using System;

namespace Elune.MPQ {

/// <summary>
/// Cryptographic functions for 
/// </summary>
public class Cryptography {

    const uint DEFAULT_TABLE_SIZE = 0x500;

    const uint HASH_TABLE_INDEX = 0x000;
    const uint HASH_NAME_A = 0x100;
    const uint HASH_NAME_B = 0x200;
    const uint HASH_FILE_KEY = 0x300;
    const uint HASH_SEED2_MIX = 0x400;
    const uint SEED2_INITIAL = 0xEEEE_EEEE;

    private uint[] cryptTable;

    private bool isInitialized = false;

    public Cryptography() : this(DEFAULT_TABLE_SIZE) {}

    public Cryptography(uint tableSize) {
        cryptTable = new uint[tableSize];
        Initialize();
    }

    private void Initialize() {
        // Only initialize once.
        if(isInitialized) {
            return;
        }

        uint seed = 0x0010_0001;

        for(uint index1 = 0x0; index1 < 0x100; index1++) {
            for( uint index2 = 0, i = 0; i < 0x5; i++, index2 += 0x100) {
                seed = (seed * 125 + 3) % 0x002A_AAAB;
                uint temp1 = (seed & 0xFFFF) << 0x10;

                seed = (seed * 125 + 3) % 0x002A_AAAB;
                uint temp2 =  seed & 0xFFFF ;

                cryptTable[index2] = temp1 | temp2;
            }
        }

        isInitialized = true;
    }

    public void EncryptBlock(byte[] data, uint seed1) {
        if(data == null) {
            throw new ArgumentNullException("data");
        }
        if(data.Length % 4 != 0) {
            throw new ArgumentException("Tried to encrypt a block with length not divisible by 4 bytes.", "data");
        }

        var seed2 = SEED2_INITIAL;
        
        for(int i = 0; i < data.Length; i += sizeof(uint)) {
            uint value = BitConverter.ToUInt32(data, i);
            
            // Modify Seed2
            seed2 += cryptTable[HASH_SEED2_MIX + (seed1 & 0xFF)];

            // New Value
            value ^= seed1 + seed2;
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, data, i, bytes.Length);

            // Keys for next operation
            seed1 = ((~seed1 << 0x15) + 0x1111_1111) | (seed1 >> 0x0b);
            seed2 = value + seed2 + (seed2 << 5) + 3;

        }
    }

    public void DecryptBlock(byte[] data, uint seed1) {
        if(data == null) {
            throw new ArgumentNullException("data");
        }
        if(data.Length % 4 != 0) {
            throw new ArgumentException("Tried to decrypt a block with length not divisible by 4 bytes.", "data");
        }

        var seed2 = SEED2_INITIAL;

        for(int i = 0; i < data.Length; i += sizeof(uint)) {
            uint value = BitConverter.ToUInt32(data, i);
            uint valueTemp = value; // Store for updating seeds later
            
            // Modify Seed2
            seed2 += cryptTable[HASH_SEED2_MIX + (seed1 & 0xFF)];

            // New value
            value ^= seed1 + seed2;
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, data, i, bytes.Length);

            // Keys for next operation
            seed1 = ((~seed1 << 0x15) + 0x1111_1111) | (seed1 >> 0x0b);
            seed2 = valueTemp + seed2 + (seed2 << 5) + 3;
        }
    }
}

}// End Namespace