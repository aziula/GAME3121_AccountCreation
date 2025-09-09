/*
This RPG data streaming assignment was created by Fernando Restituto with 
pixel RPG characters created by Sean Browning.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;   // StreamWriter / StreamReader
using System.Text;
using System.Security.Cryptography;

#region Assignment Instructions
// (omitted for brevity)
#endregion

// ---------- PartyCharacter (shared model) ----------
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

#region Assignment Part 1
static public class AssignmentPart1
{
    // --- per-account save path (ties saves to LoginManager.CurrentUser) ---
    private static string Safe(string s)
    {
        if (string.IsNullOrEmpty(s)) return "guest";
        foreach (var bad in Path.GetInvalidFileNameChars())
            s = s.Replace(bad, '_');
        return s.Trim();
    }
    private static string CurrentUserOrGuest => Safe(LoginManager.CurrentUser);

    private static string UserRoot =>
        Path.Combine(Application.persistentDataPath, "users", CurrentUserOrGuest);

    private static string SavePath =>
        Path.Combine(UserRoot, "single_slot_party_save.txt");

    private static void EnsureUserFolder()
    {
        if (!Directory.Exists(UserRoot))
            Directory.CreateDirectory(UserRoot);
    }

    // Save the current party to disk (single slot per user)
    static public void SavePartyButtonPressed()
    {
        if (GameContent.partyCharacters == null || GameContent.partyCharacters.Count == 0)
        {
            Debug.Log("Nothing to save (party is empty).");
            return;
        }

        try
        {
            EnsureUserFolder();
            using (var sw = new StreamWriter(SavePath, false))
            {
                sw.WriteLine(GameContent.partyCharacters.Count);
                foreach (PartyCharacter pc in GameContent.partyCharacters)
                {
                    sw.Write($"{pc.classID} {pc.health} {pc.mana} {pc.strength} {pc.agility} {pc.wisdom}");

                    int eqCount = pc.equipment != null ? pc.equipment.Count : 0;
                    sw.Write($" {eqCount}");
                    if (pc.equipment != null)
                        foreach (int eq in pc.equipment) sw.Write($" {eq}");

                    sw.WriteLine();
                }
            }
            Debug.Log($"[Part1] Party saved for user '{CurrentUserOrGuest}' to {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Part1] Save failed: {e.Message}");
        }
    }

    // Load the party back from disk (if no file, start empty for new accounts)
    static public void LoadPartyButtonPressed()
    {
        try
        {
            EnsureUserFolder();
            if (!File.Exists(SavePath))
            {
                Debug.Log($"[Part1] No single-slot save for user '{CurrentUserOrGuest}'. Starting empty.");
                GameContent.partyCharacters = new LinkedList<PartyCharacter>();
                GameContent.RefreshUI();
                return;
            }

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
            Debug.Log($"[Part1] Party loaded for user '{CurrentUserOrGuest}'.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Part1] Load failed: {e.Message}");
        }
    }

    // --- delete the current user's single-slot saved party file (if any) ---
    public static void DeleteCurrentUserPartySave()
    {
        try
        {
            EnsureUserFolder();
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log($"[Part1] Deleted existing single-slot party save for '{CurrentUserOrGuest}'.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[Part1] Could not delete save for '{CurrentUserOrGuest}': {e.Message}");
        }
    }
}
#endregion

#region Assignment Part 2
//  Enable Part 2 UI
static public class AssignmentConfiguration
{
    public static int PartOfAssignmentThatIsInDevelopment = 2;  // dropdown + save/load/delete by name
}

/*
Part 2: per-user multi-save system (dropdown of named parties)
*/

static public class AssignmentPart2
{
    // ---- per-user storage layout ----
    private static string Safe(string s)
    {
        if (string.IsNullOrEmpty(s)) return "guest";
        foreach (var bad in Path.GetInvalidFileNameChars())
            s = s.Replace(bad, '_');
        return s.Trim();
    }
    private static string CurrentUserOrGuest => Safe(LoginManager.CurrentUser);

    private static string UserRoot =>
        Path.Combine(Application.persistentDataPath, "users", CurrentUserOrGuest);

    private static string IndexPath =>
        Path.Combine(UserRoot, "party_index.txt"); // list of party names for THIS user

    private static string FileFor(string name) =>
        Path.Combine(UserRoot, $"party_{Hash(name)}.txt"); // party files for THIS user

    private static void EnsureUserFolder()
    {
        if (!Directory.Exists(UserRoot))
            Directory.CreateDirectory(UserRoot);
    }

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

    // ---- index helpers (one name per line) ----
    private static void LoadIndex()
    {
        EnsureUserFolder();
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
        EnsureUserFolder();
        using var sw = new StreamWriter(IndexPath, false);
        foreach (var name in listOfPartyNames)
            sw.WriteLine(name);
    }

    // ---- party (de)serialization: same text format as Part 1 ----
    private static void WriteParty(string path)
    {
        EnsureUserFolder();
        using var sw = new StreamWriter(path, false);
        sw.WriteLine(GameContent.partyCharacters?.Count ?? 0);
        if (GameContent.partyCharacters == null) return;

        foreach (var pc in GameContent.partyCharacters)
        {
            sw.Write($"{pc.classID} {pc.health} {pc.mana} {pc.strength} {pc.agility} {pc.wisdom}");
            int eqCount = pc.equipment?.Count ?? 0;
            sw.Write($" {eqCount}");
            if (pc.equipment != null)
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
        LoadIndex();             // populate dropdown from *this user's* disk
        GameContent.RefreshUI();
    }

    static public List<string> GetListOfPartyNames() => listOfPartyNames ?? new List<string>();

    static public void LoadPartyDropDownChanged(string selectedName)
    {
        var path = FileFor(selectedName);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[Part2] Party file missing for '{selectedName}' (user '{CurrentUserOrGuest}').");
            return;
        }
        try
        {
            ReadParty(path);     // StreamReader path
            GameContent.RefreshUI();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Part2] Load failed: {e.Message}");
        }
    }

    static public void SavePartyButtonPressed()
    {
        string name = GameContent.GetPartyNameFromInput()?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("[Part2] Enter a party name before saving.");
            return;
        }

        try
        {
            // add to index if new
            if (!listOfPartyNames.Contains(name))
            {
                listOfPartyNames.Add(name);
                SaveIndex();     // update per-user index
            }

            WriteParty(FileFor(name)); // save the named party for this user
            GameContent.RefreshUI();
            Debug.Log($"[Part2] Saved party '{name}' for user '{CurrentUserOrGuest}'.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Part2] Save failed: {e.Message}");
        }
    }

    static public void DeletePartyButtonPressed()
    {
        string name = GameContent.GetPartyNameFromInput()?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("[Part2] Enter the party name you want to delete.");
            return;
        }

        try
        {
            // remove file
            var path = FileFor(name);
            if (File.Exists(path)) File.Delete(path);

            // remove from index
            if (listOfPartyNames.Remove(name)) SaveIndex();

            // optional: clear current party after delete
            GameContent.partyCharacters = new LinkedList<PartyCharacter>();
            GameContent.RefreshUI();
            Debug.Log($"[Part2] Deleted party '{name}' for user '{CurrentUserOrGuest}'.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Part2] Delete failed: {e.Message}");
        }
    }
}
#endregion
