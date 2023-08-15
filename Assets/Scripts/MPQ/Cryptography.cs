using System;

namespace Elune.MPQ {

/// <summary>
/// Cryptographic functions for 
/// </summary>
public class Cryptography {

    const uint SEED1_INITIAL = 0x7FED_7FED;
    const uint SEED2_INITIAL = 0xEEEE_EEEE;

    const uint DEFAULT_TABLE_SIZE = 0x500;
    private readonly uint[] cryptTable;

    public enum HashType: uint {
        TableIndex = 0x000,
        NameA = 0x100,
        NameB = 0x200,
        FileKey = 0x300,
        SeedMix = 0x400
    }

    private bool isInitialized = false;

    public Cryptography() {
        this.cryptTable = new uint[0x500];
        InitializeCryptTable();
    }

    private void InitializeCryptTable() {
        // Only initialize once.
        if(isInitialized) {
            return;
        }

        uint seed = 0x0010_0001;

        for(uint index1 = 0x0; index1 < 0x100; index1++) {
            for( uint index2 = index1, i = 0; i < 0x5; i++, index2 += 0x100) {
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

        uint seed2 = SEED2_INITIAL;

        for(int i = 0; i < data.Length; i += sizeof(uint)) {
            // Modify Seed2
            seed2 += cryptTable[(int)(HashType.SeedMix + (seed1 & 0xFF))];

            // New Value
            uint value = BitConverter.ToUInt32(data, i);
            uint temp = value ^ (seed1 + seed2);
            byte[] bytes = BitConverter.GetBytes(temp);
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
            // Modify Seed2
            seed2 += cryptTable[(int)(HashType.SeedMix + (seed1 & 0xFF))];

            // New value
            uint value = BitConverter.ToUInt32(data, i);
            value ^= seed1 + seed2;
            var bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, data, i, bytes.Length);

            // Keys for next operation
            seed1 = ((~seed1 << 0x15) + 0x1111_1111) | (seed1 >> 0x0b);
            seed2 = value + seed2 + (seed2 << 5) + 3;
        }
    }

    public uint HashString(string str, HashType hashType) {
        uint seed1 = SEED1_INITIAL;
        uint seed2 = SEED2_INITIAL;

        str = str.ToUpper();

        for(int i = 0; i < str.Length; i++) {
            char ch = str[i]; 
            if(ch == '/') {
                ch = '\\';
            }

            seed1 = cryptTable[(int)hashType + ch] ^ (seed1 + seed2);
            seed2 = ch + seed1 + seed2 + (seed2 << 5) + 3;
        }

        return seed1;
    }
}

}// End Namespace