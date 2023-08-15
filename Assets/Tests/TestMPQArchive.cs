using System.IO;
using System.Text;
using Elune.MPQ;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestMPQArchive {

    private Cryptography crypto;

    [SetUp]
    public void SetUp() {
        crypto = new Cryptography();
    }

    [Test]
    public void FindFile_ThrowNoHashTableException() {
        string path = Path.Combine(Application.dataPath, "Tests/Empty.MPQ");
        
        MPQArchive archive = MPQReader.LoadMPQ(path);
        Assert.Throws<NoHashTableException>(() => archive.FindFile(crypto, "literally/anything.txt"),
            "Searching in an archive without a HashTable should throw NoHashTableException");
    }

    [Test]
    public void FindFile_ThrowsFileNotFoundException() {
        string path = Path.Combine(Application.dataPath, "Tests/FileSearch.MPQ");
        
        MPQArchive archive = MPQReader.LoadMPQ(path);
        Assert.Throws<FileNotFoundException>(() => archive.FindFile(crypto, "nonexistent/file.path"),
            "Paths to files that do not exist should throw FileNotFoundException");
    }

    [Test]
    [TestCase("Tests/FileSearch.MPQ", "foo/example.txt", "Hello World!")]
    [TestCase("Tests/FileSearch.MPQ", "bar/example2.txt", "Goodnight Moon!")]
    public void FindFile_Success(string archivePath, string requestedFile, string expected) {
        string path = Path.Combine(Application.dataPath, archivePath);
        
        MPQArchive archive = MPQReader.LoadMPQ(path);
        byte[] actual = archive.FindFile(crypto, requestedFile);

        Assert.AreEqual(expected, actual, 
            "Must be able to read the contents of files within an archive");
    }
       
}