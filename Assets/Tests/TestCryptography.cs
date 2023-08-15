using System;
using System.Text;
using Elune.MPQ;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestCryptography {

    private Cryptography crypto;

    [SetUp]
    public void SetUp() {
        crypto = new Cryptography();    
    }

    [Test]
    public void Encryption_TwoWay() {
        const string myStr = "MyString";
        const uint key = 0xA3E8_C975; // This is just a random 32-bit prime I found online
        
        byte[] target = Encoding.ASCII.GetBytes(myStr);
        string result1 = Encoding.ASCII.GetString(target);
        crypto.EncryptBlock(target, key);
        string result2 = Encoding.ASCII.GetString(target);
        Assert.AreNotEqual(result1, result2, 
            "Encrypting a target should change the contents");
        crypto.DecryptBlock(target, key);
        string result3 = Encoding.ASCII.GetString(target);

        Assert.AreEqual(result1, result3, 
            "Encrypting then Decrypting with the same key should get the original value back");
    }

    [Test]
    public void EncryptBlock_DifferentResult() {
        const string myStr = "MyString";
        const uint key = 0xA3E8_C975; // This is just a random 32-bit prime I found online
        
        byte[] target = Encoding.ASCII.GetBytes(myStr);
        crypto.EncryptBlock(target, key);
        
        string result = Encoding.ASCII.GetString(target);
        Assert.AreNotEqual(myStr, result, 
            "Encryption should not result in the same input as output");
    }

    [Test]
    public void EncryptBlock_SameKey() {
        const string myStr = "MyString";
        const uint key = 0xA3E8_C975;
        
        byte[] target1 = Encoding.ASCII.GetBytes(myStr);
        byte[] target2 = Encoding.ASCII.GetBytes(myStr);

        crypto.EncryptBlock(target1, key);
        string result1 = Encoding.ASCII.GetString(target1);

        crypto.EncryptBlock(target2, key);
        string result2 = Encoding.ASCII.GetString(target2);

        Assert.AreEqual(result1, result2, 
            "Encrypting a string twice with the same key must always return the same result");
    }
    [Test]
    public void EncryptBlock_DifferentKey() {
        const string myStr = "MyString";
        const uint key1 = 0xA3E8_C975;
        const uint key2 = 0xC3AF_3770;
        
        byte[] target1 = Encoding.ASCII.GetBytes(myStr);
        byte[] target2 = Encoding.ASCII.GetBytes(myStr);

        crypto.EncryptBlock(target1, key1);
        string result1 = Encoding.ASCII.GetString(target1);

        crypto.EncryptBlock(target2, key2);
        string result2 = Encoding.ASCII.GetString(target2);

        Assert.AreNotEqual(result1, result2, 
            "Encrypting a string with different keys must return different results");
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
            "Decryption should not result in the same input as output");
    }

    [Test]
    public void DecryptBlock_LengthMultipleOf4() {
        const string myStr = "1234567";
        const uint key = 0xA3E8_C975;

        byte[] target = Encoding.ASCII.GetBytes(myStr);

        Assert.Throws<ArgumentException>(() => crypto.DecryptBlock(target, key),
            "Trying to decrypt blocks of (length%4 != 0) must throw an exception");
    }

    [Test]
    public void DecryptBlock_SameKey() {
        const string myStr = "MyString";
        const uint key = 0xA3E8_C975;
        
        byte[] target1 = Encoding.ASCII.GetBytes(myStr);
        byte[] target2 = Encoding.ASCII.GetBytes(myStr);

        crypto.EncryptBlock(target1, key);
        string result1 = Encoding.ASCII.GetString(target1);

        crypto.EncryptBlock(target2, key);
        string result2 = Encoding.ASCII.GetString(target2);

        Assert.AreEqual(result1, result2, 
            "Decrypting a string twice with the same key must always return the same result");
    }

    [Test]
    public void DecryptBlock_DifferentKey() {
        const string myStr = "MyString";
        const uint key1 = 0xA3E8_C975;
        const uint key2 = 0xC3AF_3770;
        
        byte[] target1 = Encoding.ASCII.GetBytes(myStr);
        byte[] target2 = Encoding.ASCII.GetBytes(myStr);

        crypto.DecryptBlock(target1, key1);
        string result1 = Encoding.ASCII.GetString(target1);

        crypto.DecryptBlock(target2, key2);
        string result2 = Encoding.ASCII.GetString(target2);

        Assert.AreNotEqual(result1, result2, 
            "Decrypting a string with different keys must return different results");
    }

    [TestCase("Hello World!",     new byte[]{0x84, 0xAA, 0x50, 0xEA, 0x4F, 0xAC, 0x6E, 0xBB, 0x6E, 0xD7, 0x84, 0xE8})]
    [TestCase("example Strings~", new byte[]{0xA9, 0xB7, 0x5D, 0xEB, 0x4D, 0xF3, 0x4B, 0xF5, 0x89, 0x09, 0xF4, 0xF3, 0x76, 0x77, 0xAB, 0x86})]
    [TestCase("12345678",         new byte[]{0xFD, 0xFD, 0x0F, 0xB2, 0x3C, 0x6F, 0x37, 0xA4})]
    [TestCase("spare change",     new byte[]{0xBF, 0xBF, 0x5D, 0xF4, 0x2E, 0xB7, 0x4D, 0xB2, 0xFC, 0x45, 0xE4, 0xE2})]
    public void EncryptBlock_StormParity(string start, byte[] expected) {
        uint key = crypto.HashString("(hash table)", Cryptography.HashType.FileKey);
        byte[] target = Encoding.ASCII.GetBytes(start);
        
        crypto.EncryptBlock(target, key);
        
        Assert.AreEqual(expected, target,
            $"Encrypting '{start}' with key {key:X8} should return '{expected}' ");
    }

    [TestCase("Hello World!",     new byte[]{0x84, 0xAA, 0x50, 0xEA, 0x4F, 0xAC, 0x6E, 0xBB, 0x6E, 0xD7, 0x84, 0xE8})]
    [TestCase("example Strings~", new byte[]{0xA9, 0xB7, 0x5D, 0xEB, 0x4D, 0xF3, 0x4B, 0xF5, 0x89, 0x09, 0xF4, 0xF3, 0x76, 0x77, 0xAB, 0x86})]
    [TestCase("12345678",         new byte[]{0xFD, 0xFD, 0x0F, 0xB2, 0x3C, 0x6F, 0x37, 0xA4})]
    [TestCase("spare change",     new byte[]{0xBF, 0xBF, 0x5D, 0xF4, 0x2E, 0xB7, 0x4D, 0xB2, 0xFC, 0x45, 0xE4, 0xE2})]
    public void DecryptBlock_StormParity(string expected, byte[] start) {
        uint key = crypto.HashString("(hash table)", Cryptography.HashType.FileKey);

        byte[] target = new byte[start.Length];
        Array.Copy(start, target, start.Length);
        
        crypto.DecryptBlock(target, key);
        
        Assert.AreEqual(expected, target,
            $"Decrypting '{start}' with key {key:X8} should return '{expected}' ");
    }

    // TODO: Take HashString tests out of the unit for encrypt/decrypt

    [Test]
    public void HashString_DifferentStrings() {
        const string str1 = "interface/example/script.lua";
        const string str2 = "interface/foo/bar.xml"; 

        uint hash1 = crypto.HashString(str1, Cryptography.HashType.NameA);
        uint hash2 = crypto.HashString(str2, Cryptography.HashType.NameA);

        Assert.AreNotEqual(hash1, hash2, "Different strings should yield different hashes");
    }

    [Test]
    public void HashString_DifferentTypes() {
        const string str = "hello/world.lua";

        uint hash1 = crypto.HashString(str, Cryptography.HashType.NameA);
        uint hash2 = crypto.HashString(str, Cryptography.HashType.NameB);

        Assert.AreNotEqual(hash1, hash2, "Different types should yield different hashes");
    }

    [Test]
    public void HashString_SameResult() {
        const string str = "simple string";

        uint hash1 = crypto.HashString(str, Cryptography.HashType.SeedMix);
        uint hash2 = crypto.HashString(str, Cryptography.HashType.SeedMix);

        Assert.AreEqual(hash1, hash2, "Hashing the same string twice should give the same result");
    }

    [Test]
    public void HashString_CaseInsensitive() {
        const string str1 = "HeLLo";
        const string str2 = "helLo";
        
        uint hash1 = crypto.HashString(str1, Cryptography.HashType.FileKey);
        uint hash2 = crypto.HashString(str2, Cryptography.HashType.FileKey);

        Assert.AreEqual(hash1, hash2, "Hashing should disregard case");
    }

    [Test]
    public void HashString_ConvertSlashes() {
        const string str1 = "hello/world.lua";
        const string str2 = "hello\\world.lua";

        uint hash1 = crypto.HashString(str1, Cryptography.HashType.TableIndex);
        uint hash2 = crypto.HashString(str2, Cryptography.HashType.TableIndex);

        Assert.AreEqual(hash1, hash2, "Hashing should treat / and \\ as the same");
    }

    [Test]
    public void HashString_Parity1() {
        const string str = "(hash table)";

        uint hash1 = crypto.HashString(str, Cryptography.HashType.FileKey);

        uint expected = 0xC3AF3770;
        Assert.AreEqual(expected, hash1, $"{str} should hash as a file key to {expected:x}");
    }

    [Test]
    public void HashString_Parity2() {
        const string str = "(block table)";

        uint hash1 = crypto.HashString(str, Cryptography.HashType.FileKey);

        uint expected = 0xEC83B3A3;
        Assert.AreEqual(expected, hash1, $"{str} should hash as a file key to {expected:x}");
    }

}