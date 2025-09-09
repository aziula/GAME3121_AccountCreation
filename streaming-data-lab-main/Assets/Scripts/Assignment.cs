
/*
This RPG data streaming assignment was created by Fernando Restituto with 
pixel RPG characters created by Sean Browning.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;   // needed for StreamWriter and StreamReader

using System.Text;
using System.Security.Cryptography;


#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.  
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.  
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also, 
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */

public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;

}


/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/


#endregion


#region Assignment Part 1

static public class AssignmentPart1
{
    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "party_save.txt");

    // Save the current party to disk
    static public void SavePartyButtonPressed()
    {
        if (GameContent.partyCharacters == null ||
            GameContent.partyCharacters.Count == 0) return;

        try
        {
            // StreamWriter writes text to a file
            using (var sw = new StreamWriter(SavePath, false))
            {
                sw.WriteLine(GameContent.partyCharacters.Count);
                foreach (PartyCharacter pc in GameContent.partyCharacters)
                {
                    sw.Write($"{pc.classID} {pc.health} {pc.mana} {pc.strength} {pc.agility} {pc.wisdom}");

                    int eqCount = pc.equipment != null ? pc.equipment.Count : 0;
                    sw.Write($" {eqCount}");
                    foreach (int eq in pc.equipment)
                        sw.Write($" {eq}");

                    sw.WriteLine();
                }
            }
            Debug.Log($"Party saved to {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    // Load the party back from disk
    static public void LoadPartyButtonPressed()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        try
        {
            // StreamReader reads text back line by line
            using (var sr = new StreamReader(SavePath))
            {
                GameContent.partyCharacters = new LinkedList<PartyCharacter>();

                int count = int.Parse(sr.ReadLine().Trim());
                for (int i = 0; i < count; i++)
                {
                    string[] parts = sr.ReadLine().Split(' ');
                    int idx = 0;
                    var pc = new PartyCharacter(
                        int.Parse(parts[idx++]),
                        int.Parse(parts[idx++]),
                        int.Parse(parts[idx++]),
                        int.Parse(parts[idx++]),
                        int.Parse(parts[idx++]),
                        int.Parse(parts[idx++])
                    );

                    int eqCount = int.Parse(parts[idx++]);
                    for (int e = 0; e < eqCount; e++)
                        pc.equipment.AddLast(int.Parse(parts[idx++]));

                    GameContent.partyCharacters.AddLast(pc);
                }
            }
            GameContent.RefreshUI();
            Debug.Log("Party loaded.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
        }
    }
}



#endregion


#region Assignment Part 2

//  Before Proceeding!
//  To inform the internal systems that you are proceeding onto the second part of this assignment,
//  change the below value of AssignmentConfiguration.PartOfAssignmentInDevelopment from 1 to 2.
//  This will enable the needed UI/function calls for your to proceed with your assignment.
static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 2;  // enable Part 2 (dropdown + save/load/delete by name)
}

/*

In this part of the assignment you are challenged to expand on the functionality that you have already created.  
    You are being challenged to save, load and manage multiple parties.
    You are being challenged to identify each party via a string name (a member of the Party class).

To aid you in this challenge, the UI has been altered.  

    The load button has been replaced with a drop down list.  
    When this load party drop down list is changed, LoadPartyDropDownChanged(string selectedName) will be called.  
    When this drop down is created, it will be populated with the return value of GetListOfPartyNames().

    GameStart() is called when the program starts.

    For quality of life, a new SavePartyButtonPressed() has been provided to you below.

    An new/delete button has been added, you will also find below NewPartyButtonPressed() and DeletePartyButtonPressed()

Again, you are being challenged to develop the ability to save and load multiple parties.
    This challenge is different from the previous.
    In the above challenge, what you had to develop was much more directly named.
    With this challenge however, there is a much more predicate process required.
    Let me ask you,
        What do you need to program to produce the saving, loading and management of multiple parties?
        What are the variables that you will need to declare?
        What are the things that you will need to do?  
    So much of development is just breaking problems down into smaller parts.
    Take the time to name each part of what you will create and then, do it.

Good luck, journey well.

*/

static public class AssignmentPart2
{
    // ---- storage layout ----
    private static string Root => Application.persistentDataPath;
    private static string IndexPath => Path.Combine(Root, "party_index.txt"); // list of names
    private static string FileFor(string name) => Path.Combine(Root, $"party_{Hash(name)}.txt");

    // name -> file mapping via stable hash (safe for any characters, incl commas)
    private static string Hash(string s)
    {
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    static List<string> listOfPartyNames;

    // ---- index helpers (keeps it simple, one name per line) ----
    private static void LoadIndex()
    {
        listOfPartyNames = new List<string>();
        if (!File.Exists(IndexPath)) return;
        foreach (var line in File.ReadAllLines(IndexPath))
        {
            var name = line.Trim();
            if (!string.IsNullOrEmpty(name)) listOfPartyNames.Add(name);
        }
    }

    private static void SaveIndex()
    {
        using var sw = new StreamWriter(IndexPath, false);
        foreach (var name in listOfPartyNames)
            sw.WriteLine(name);
    }

    // ---- party (de)serialization: same text format as Part 1 ----
    private static void WriteParty(string path)
    {
        using var sw = new StreamWriter(path, false);
        sw.WriteLine(GameContent.partyCharacters?.Count ?? 0);
        if (GameContent.partyCharacters == null) return;

        foreach (var pc in GameContent.partyCharacters)
        {
            sw.Write($"{pc.classID} {pc.health} {pc.mana} {pc.strength} {pc.agility} {pc.wisdom}");
            int eqCount = pc.equipment?.Count ?? 0;
            sw.Write($" {eqCount}");
            foreach (int eq in pc.equipment) sw.Write($" {eq}");
            sw.WriteLine();
        }
    }

    private static void ReadParty(string path)
    {
        using var sr = new StreamReader(path);
        var list = new LinkedList<PartyCharacter>();
        int count = int.Parse(sr.ReadLine().Trim());
        for (int i = 0; i < count; i++)
        {
            string[] parts = sr.ReadLine().Split(' ');
            int idx = 0;
            var pc = new PartyCharacter(
                int.Parse(parts[idx++]),
                int.Parse(parts[idx++]),
                int.Parse(parts[idx++]),
                int.Parse(parts[idx++]),
                int.Parse(parts[idx++]),
                int.Parse(parts[idx++])
            );
            int eqCount = int.Parse(parts[idx++]);
            for (int e = 0; e < eqCount; e++) pc.equipment.AddLast(int.Parse(parts[idx++]));
            list.AddLast(pc);
        }
        GameContent.partyCharacters = list;
    }

    // ---- UI entry points ----
    static public void GameStart()
    {
        LoadIndex();             // populate dropdown from disk
        GameContent.RefreshUI();
    }

    static public List<string> GetListOfPartyNames() => listOfPartyNames ?? new List<string>();

    static public void LoadPartyDropDownChanged(string selectedName)
    {
        var path = FileFor(selectedName);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Party file missing for '{selectedName}'.");
            return;
        }
        try
        {
            ReadParty(path);     // StreamReader path
            GameContent.RefreshUI();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
        }
    }

    static public void SavePartyButtonPressed()
    {
        string name = GameContent.GetPartyNameFromInput()?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("Enter a party name before saving.");
            return;
        }

        try
        {
            // add to index if new
            if (!listOfPartyNames.Contains(name))
            {
                listOfPartyNames.Add(name);
                SaveIndex();     // StreamWriter index
            }

            WriteParty(FileFor(name)); // StreamWriter party
            GameContent.RefreshUI();
            Debug.Log($"Saved party '{name}'.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    static public void DeletePartyButtonPressed()
    {
        string name = GameContent.GetPartyNameFromInput()?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("Enter the party name you want to delete.");
            return;
        }

        try
        {
            // remove file
            var path = FileFor(name);
            if (File.Exists(path)) File.Delete(path);

            // remove from index
            if (listOfPartyNames.Remove(name)) SaveIndex();

            // optional: clear current party
            GameContent.partyCharacters = new LinkedList<PartyCharacter>();
            GameContent.RefreshUI();
            Debug.Log($"Deleted party '{name}'.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Delete failed: {e.Message}");
        }
    }
}

#endregion


