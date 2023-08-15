using System.IO;
using Elune.MPQ;
using NUnit.Framework;
using UnityEngine;
using static Elune.MPQ.MPQHashTable;

[TestFixture]
class TestHashTable {

    private Cryptography crypto;

    [SetUp]
    public void SetUp() {
        crypto = new Cryptography();   
    }

    [Test]
    public void InsertEntry_FindsUnoccupiedIndex() {
        const int LIST_SIZE = 8;
        MPQHashTable table = new(LIST_SIZE);

        int occupiedIndex = table.InsertEntry(crypto, "dir/path.txt", 0x0, 0x0, 0x0);

        int nextIndex = table.InsertEntry(crypto, "dir/path.txt", 0x0, 0x0, 0x0);

        int expected = (occupiedIndex+1)%LIST_SIZE;
        Assert.AreEqual(expected, nextIndex,
            "Inserting an entry at an occupied index should try the next index");

    }
    
    [Test]
    public void InsertEntry_ThrowsSearchWraparoundException() {
        const int LIST_SIZE = 8;
        MPQHashTable table = new(LIST_SIZE);
        // Fill the table
        for(int i = 0; i < LIST_SIZE; i++) {
            table.InsertEntry(crypto, "dir/path.txt", 0x0, 0x0, 0x0);
        }

        Assert.Throws<IndexWraparoundException>(
            () => table.InsertEntry(crypto, "dir/path.txt", 0x0, 0x0, 0x0)
        );
    }

    [Test]
    public void InsertEntry_ReturnsCorrectIndex() {
        const int LIST_SIZE = 8;
        MPQHashTable table = new(LIST_SIZE);

        ushort  locale = 0x409;
        ushort platform = 0x0;
        uint blockIndex = 0x5;
        
        int entryIndex = table.InsertEntry(crypto, "dir/path.txt", locale, platform, blockIndex);

        Assert.AreEqual(blockIndex, table.Get(entryIndex).BlockIndex);
    }

    [Test]
    public void FindEntry_ThrowsFileNotFoundException() {
        const int LIST_SIZE = 8;
        MPQHashTable table = new(LIST_SIZE);
        
        Assert.Throws<FileNotFoundException>(
            () => table.FindEntry(crypto, "This/Path/Does/Not/Exist"),
            "FindEntry should throw FileNotFoundException if no entry exists for the associated path.");
    }

    [Test]
    public void FindEntry_ThrowsIndexWraparoundException() {
        const int LIST_SIZE = 8;
        MPQHashTable table = new(LIST_SIZE);

        // Fill the table
        for(ushort i = 0; i < LIST_SIZE; i++) {
            table.InsertEntry(crypto, "dir/path.txt", 0x0, i, 0x0);
        }

        Assert.Throws<IndexWraparoundException>(
            () => table.FindEntry(crypto, "This/Path/Does/Not/Exist"),
            "FindEntry attempt that passes through the entire list should throw IndexWraparoundException");
    }
    
    [Test]
    public void FindEntry_FindsEntriesByName() {
        // Example hashtable:
        const int LIST_SIZE = 8;
        MPQHashTable table = new(LIST_SIZE);
        // Add an entry
        const string name = "foo";
        ushort locale = 0x409;
        ushort platform = 0x0;
        uint blockIndex = 0x3;
        table.InsertEntry(crypto, name, locale, platform, blockIndex);
        
        // Find the entry
        MPQHashEntry foundEntry = table.FindEntry(crypto, name);

        Assert.NotNull(foundEntry, 
            "FindEntry should not return null if used on a search string for an existing entry.");
    }
}