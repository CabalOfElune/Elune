using System;
using System.Text;
using Elune.MPQ;
using NUnit.Framework;

public class TestCryptography {
    
    [Test]
    public void Encryption_TwoWay() {
        const string myStr = "alphab3t";
        const uint key = 0xA3E8_C975;

        byte[] target = Encoding.ASCII.GetBytes(myStr);
        var crypto = new Cryptography();
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
        var crypto = new Cryptography();
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
        var crypto = new Cryptography();

        Assert.Throws<ArgumentException>(() => crypto.EncryptBlock(target, key),
            "Trying to encrypt blocks of (length%4 != 0) must throw an exception!");
    }

    [Test]
    public void DecryptBlock_DifferentResult() {
        const string myStr = "MyString";
        const uint key = 0xA3E8_C975; // This is just a random 32-bit prime I found online
        
        byte[] target = Encoding.ASCII.GetBytes(myStr);
        var crypto = new Cryptography();
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
        var crypto = new Cryptography();

        Assert.Throws<ArgumentException>(() => crypto.DecryptBlock(target, key),
            "Trying to decrypt blocks of (length%4 != 0) must throw an exception!");
    }

}