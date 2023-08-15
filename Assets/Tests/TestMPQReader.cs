using System.IO;
using NUnit.Framework;
using UnityEngine;
using Elune.MPQ;
using System;

[TestFixture]
public class TestMPQReader
{

    [Test]
    public void FindHeaderPointer_NoSignature()
    {
        string path = Path.Combine(Application.dataPath, "Tests/MissingSignature.MPQ");
        FileStream dataStream = new(path, FileMode.Open);
        Assert.Throws<SignatureNotFoundException>(() => MPQReader.FindHeaderPointer(dataStream),
            "'InvalidHeader.MPQ' should not find a header signature.");
        dataStream.Close();
    }

    [Test]
    public void FindHeaderPointer_FindsHeaders()
    {
        string path = Path.Combine(Application.dataPath, "Tests/Empty.MPQ");
        FileStream dataStream = new(path, FileMode.Open);
        int headerPointer = MPQReader.FindHeaderPointer(dataStream);
        dataStream.Close();
        Assert.AreEqual(0x0, headerPointer,
            "Empty.MPQ starts at 0x0 -- Why does it appear to begin elsewhere?");
    }
    
    [Test]
    public void FindHeaderPointer_HeaderOffsets()
    {
        string path = Path.Combine(Application.dataPath, "Tests/Offset.MPQ");
        FileStream dataStream = new(path, FileMode.Open);
        int headerPointer = MPQReader.FindHeaderPointer(dataStream);
        dataStream.Close();
        Assert.AreEqual(0x200, headerPointer, "Offset.MPQ starts at 0x200");
    }

    [Test]
    public void LoadMPQ_ThrowsFileNotFound() {
        string path = Path.Combine(Application.dataPath, "Tests/MISSING_MPQ_PATH.MPQ");
        
        Assert.Throws<FileNotFoundException>(() => MPQReader.LoadMPQ(path),
            "Paths to nonexistent files should throw FileNotFoundException.");
    }

    [Test]
    public void LoadMPQ_ThrowsSignatureNotFoundException() {
        string path = Path.Combine(Application.dataPath, "Tests/MissingSignature.MPQ");
        
        Assert.Throws<SignatureNotFoundException>(() => MPQReader.LoadMPQ(path),
            "Archive with malformed headers should throw a SignatureNotFoundException");
    }
    
    [Test]
    public void LoadMPQ_EmptyHashTable() {
        string path = Path.Combine(Application.dataPath, "Tests/Empty.MPQ");
        
        MPQArchive archive = MPQReader.LoadMPQ(path);
        Assert.Zero(archive.Header.HashTableSize,
            "Empty.MPQ should have a HashTableSize of 0");
        Assert.IsNull(archive.HashTable, 
            "Empty.MPQ should have null HashTable");
    }

    // [Test]
    // public void LoadMPQ_EmptyBlockTable() {
    //     string path = Path.Combine(Application.dataPath, "Tests/Empty.MPQ");
    //     MPQArchive archive = MPQReader.LoadMPQ(path);

    //     Assert.Zero(archive.Header.BlockTableSize,
    //         "Empty.MPQ should have a BlockTableSize of 0");
    //     Assert.IsNull(archive.BlockTable,
    //         "Empty.MPQ should have null HashTable.");
    // }
    
}
