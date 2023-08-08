using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Elune.MPQ;

public class TestMPQReader
{

    [Test]
    public void FindHeaderPointer_NoSignature()
    {
        string path = Path.Combine(Application.dataPath, "Tests/InvalidHeader.MPQ");
        FileStream dataStream = new(path, FileMode.Open);
        Assert.Throws<SignatureNotFoundException>(() => MPQReader.FindHeaderPointer(dataStream),
            "InvalidHeader.MPQ -- Should not find a header signature.");
        dataStream.Close();
    }

    [Test]
    public void FindHeaderPointer_FindsHeaders()
    {
        string path = Path.Combine(Application.dataPath, "Tests/Empty.MPQ");
        FileStream dataStream = new(path, FileMode.Open);
        int headerPointer = MPQReader.FindHeaderPointer(dataStream);
        dataStream.Close();
        Assert.AreEqual(0x0, headerPointer, "Empty.MPQ starts at 0x0 -- Why does it appear to begin elsewhere?");
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
}
