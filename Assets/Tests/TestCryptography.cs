using System;
using System.Text;
using Elune.MPQ;
using NUnit.Framework;

public class TestCryptography {

    private readonly Cryptography crypto;

    public TestCryptography() {
        crypto = new Cryptography();    
    }
    
    [Test]
    public void Encryption_TwoWay() {
        const string myStr = "alphab3t";
        const uint key = 0xA3E8_C975;

        byte[] target = Encoding.ASCII.GetBytes(myStr);
        crypto.EncryptBlock(target, key);
        crypto.DecryptBlock(target, key);
        string result = Encoding.ASCII.GetString(target);
        
        Assert.AreEqual(myStr, result,
            "Encrypting then Decrypting with the same key should get the original value back.");
    }

    [Test]
    public void EncryptBlock_DifferentResult() {
        const string myStr = "MyString";
        const uint key = 0xA3E8_C975; // This is just a random 32-bit prime I found online
        
        byte[] target = Encoding.ASCII.GetBytes(myStr);
        crypto.EncryptBlock(target, key);
        
        string result = Encoding.ASCII.GetString(target);
        Assert.AreNotEqual(myStr, result, 
            "Encryption should not result in the same input as output.");
    }

    [Test]
    public void EncryptBlock_LengthMultipleOf4() {
        const string myStr = "1234567";
        const uint key = 0xA3E8_C975;

        byte[] target = Encoding.ASCII.GetBytes(myStr);

        Assert.Throws<ArgumentException>(() => crypto.EncryptBlock(target, key),
            "Trying to encrypt blocks of (length%4 != 0) must throw an exception!");
    }

    [Test]
    public void DecryptBlock_DifferentResult() {
        const string myStr = "MyString";
        const uint key = 0xA3E8_C975; // This is just a random 32-bit prime I found online
        
        byte[] target = Encoding.ASCII.GetBytes(myStr);
        crypto.DecryptBlock(target, key);
        
        string result = Encoding.ASCII.GetString(target);
        Assert.AreNotEqual(myStr, result, 
            "Decryption should not result in the same input as output.");
    }

    [Test]
    public void DecryptBlock_LengthMultipleOf4() {
        const string myStr = "1234567";
        const uint key = 0xA3E8_C975;

        byte[] target = Encoding.ASCII.GetBytes(myStr);

        Assert.Throws<ArgumentException>(() => crypto.DecryptBlock(target, key),
            "Trying to decrypt blocks of (length%4 != 0) must throw an exception!");
    }

    [Test]
    public void HashString_DifferentStrings() {
        const string str1 = "interface/example/script.lua";
        const string str2 = "interface/foo/bar.xml"; 

        uint hash1 = crypto.HashString(str1, Cryptography.HashType.NameA);
        uint hash2 = crypto.HashString(str2, Cryptography.HashType.NameA);

        Assert.AreNotEqual(hash1, hash2, "Different strings should yield different hashes.");
    }

    [Test]
    public void HashString_DifferentTypes() {
        const string str = "hello/world.lua";

        uint hash1 = crypto.HashString(str, Cryptography.HashType.NameA);
        uint hash2 = crypto.HashString(str, Cryptography.HashType.NameB);

        Assert.AreNotEqual(hash1, hash2, "Different types should yield different hashes.");
    }

    [Test]
    public void HashString_SameResult() {
        const string str = "simple string";

        uint hash1 = crypto.HashString(str, Cryptography.HashType.SeedMix);
        uint hash2 = crypto.HashString(str, Cryptography.HashType.SeedMix);

        Assert.AreEqual(hash1, hash2, "Hashing the same string twice should give the same result.");
    }

    [Test]
    public void HashString_CaseInsensitive() {
        const string str1 = "HeLLo";
        const string str2 = "helLo";
        
        uint hash1 = crypto.HashString(str1, Cryptography.HashType.FileKey);
        uint hash2 = crypto.HashString(str2, Cryptography.HashType.FileKey);

        Assert.AreEqual(hash1, hash2, "Hashing should disregard case.");
    }

    [Test]
    public void HashString_ConvertSlashes() {
        const string str1 = "hello/world.lua";
        const string str2 = "hello\\world.lua";

        uint hash1 = crypto.HashString(str1, Cryptography.HashType.TableIndex);
        uint hash2 = crypto.HashString(str2, Cryptography.HashType.TableIndex);

        Assert.AreEqual(hash1, hash2, "Hashing should treat / and \\ as the same.");
    }

}