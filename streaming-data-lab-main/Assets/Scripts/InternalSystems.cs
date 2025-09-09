
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GameContent
{
    static GameObject systemManager;

    public static LinkedList<PartyCharacter> partyCharacters;

    static public class CharacterClassID
    {
        public const int Fighter = 1;
        public const int BlackMage = 2;
        public const int WhiteMage = 3;
        public const int RedMage = 4;
        public const int Thief = 5;
        public const int Monk = 6;

        static public Dictionary<int, string> lookUp;

        static public void Init()
        {
            lookUp = new Dictionary<int, string>();

            lookUp.Add(Fighter, "Fighter");
            lookUp.Add(BlackMage, "BlackMage");
            lookUp.Add(WhiteMage, "WhiteMage");
            lookUp.Add(RedMage, "RedMage");
            lookUp.Add(Thief, "Thief");
            lookUp.Add(Monk, "Monk");
        }

    }

    static public class EquipmentID
    {
        public const int BigSword = 1;
        public const int ShortSword = 2;
        public const int Dagger = 3;
        public const int Staff = 4;
        public const int Bow = 5;
        public const int Mace = 6;
        public const int Unarmed = 7;
        public const int MageMasher = 8;


        public const int Helmet = 101;
        public const int Cap = 102;
        public const int Hood = 103;
        public const int HeadBand = 104;

        public const int Plate = 201;
        public const int Leather = 202;
        public const int Robes = 203;

        static public Dictionary<int, string> lookUp;

        static public void Init()
        {
            lookUp = new Dictionary<int, string>();

            lookUp.Add(BigSword, "Big Sword");
            lookUp.Add(ShortSword, "Short Sword");
            lookUp.Add(Dagger, "Dagger");
            lookUp.Add(Staff, "Staff");
            lookUp.Add(Bow, "Bow");
            lookUp.Add(Mace, "Mace");
            lookUp.Add(Unarmed, "Unarmed");
            lookUp.Add(MageMasher, "Mage Masher");
            lookUp.Add(Helmet, "Helmet");
            lookUp.Add(Cap, "Cap");
            lookUp.Add(Hood, "Hood");
            lookUp.Add(HeadBand, "HeadBand");
            lookUp.Add(Plate, "Plate");
            lookUp.Add(Leather, "Leather");
            lookUp.Add(Robes, "Robes");
        }

    }

    static public void RerollParty()
    {
        GameContent.partyCharacters = new LinkedList<PartyCharacter>();

        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            PartyCharacter pc = new PartyCharacter();
            GameContent.partyCharacters.AddLast(pc);

            pc.classID = Random.Range(1, 6);

            if (pc.classID == GameContent.CharacterClassID.Fighter)
            {
                pc.health = 75 + Random.Range(1, 51);
                pc.mana = 25 + Random.Range(1, 26);

                pc.strength = Random.Range(1, 6) + 10;
                pc.agility = Random.Range(1, 6) + 8;
                pc.wisdom = Random.Range(1, 6) + 5;

                int helm = Random.Range(1, 4);
                int weapon = Random.Range(1, 4);

                if (helm == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.Helmet);
                else if (helm == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.HeadBand);

                pc.equipment.AddLast(GameContent.EquipmentID.Plate);

                if (weapon == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.BigSword);
                else if (weapon == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.ShortSword);
                else if (weapon == 3)
                    pc.equipment.AddLast(GameContent.EquipmentID.Bow);
                else if (weapon == 4)
                    pc.equipment.AddLast(GameContent.EquipmentID.Mace);
            }
            else if (pc.classID == GameContent.CharacterClassID.BlackMage)
            {
                pc.health = 25 + Random.Range(1, 26);
                pc.mana = 75 + Random.Range(1, 51);

                pc.strength = Random.Range(1, 6) + 5;
                pc.agility = Random.Range(1, 6) + 8;
                pc.wisdom = Random.Range(1, 6) + 10;

                int helm = Random.Range(1, 4);
                int weapon = Random.Range(1, 3);

                if (helm == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.Cap);
                else if (helm == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.Hood);

                pc.equipment.AddLast(GameContent.EquipmentID.Robes);

                if (weapon == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.Dagger);
                else if (weapon == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.Staff);
                else if (weapon == 3)
                    pc.equipment.AddLast(GameContent.EquipmentID.Bow);
            }
            else if (pc.classID == GameContent.CharacterClassID.WhiteMage)
            {
                pc.health = 25 + Random.Range(1, 26);
                pc.mana = 75 + Random.Range(1, 51);

                pc.strength = Random.Range(1, 6) + 5;
                pc.agility = Random.Range(1, 6) + 8;
                pc.wisdom = Random.Range(1, 6) + 10;

                int helm = Random.Range(1, 4);
                int weapon = Random.Range(1, 3);

                if (helm == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.Cap);
                else if (helm == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.Hood);

                pc.equipment.AddLast(GameContent.EquipmentID.Robes);

                if (weapon == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.Mace);
                else if (weapon == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.Staff);
                else if (weapon == 3)
                    pc.equipment.AddLast(GameContent.EquipmentID.Bow);
            }

            else if (pc.classID == GameContent.CharacterClassID.Thief)
            {
                pc.health = 50 + Random.Range(1, 51);
                pc.mana = 50 + Random.Range(1, 51);

                pc.strength = Random.Range(1, 6) + 8;
                pc.agility = Random.Range(1, 6) + 8;
                pc.wisdom = Random.Range(1, 6) + 8;

                int helm = Random.Range(1, 4);
                int weapon = Random.Range(1, 4);

                if (helm == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.Cap);
                else if (helm == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.Hood);

                pc.equipment.AddLast(GameContent.EquipmentID.Leather);

                if (weapon == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.ShortSword);
                else if (weapon == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.Dagger);
                else if (weapon == 3)
                    pc.equipment.AddLast(GameContent.EquipmentID.Bow);
                else if (weapon == 4)
                    pc.equipment.AddLast(GameContent.EquipmentID.MageMasher);
            }

            else if (pc.classID == GameContent.CharacterClassID.RedMage)
            {
                pc.health = 50 + Random.Range(1, 26);
                pc.mana = 50 + Random.Range(1, 51);

                pc.strength = Random.Range(1, 6) + 8;
                pc.agility = Random.Range(1, 6) + 8;
                pc.wisdom = Random.Range(1, 6) + 8;

                int helm = Random.Range(1, 4);
                int weapon = Random.Range(1, 4);

                if (helm == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.Cap);
                else if (helm == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.HeadBand);

                pc.equipment.AddLast(GameContent.EquipmentID.Leather);

                if (weapon == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.ShortSword);
                else if (weapon == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.Staff);
                else if (weapon == 3)
                    pc.equipment.AddLast(GameContent.EquipmentID.Bow);
                else if (weapon == 4)
                    pc.equipment.AddLast(GameContent.EquipmentID.Mace);
            }
            else if (pc.classID == GameContent.CharacterClassID.Monk)
            {
                pc.health = 75 + Random.Range(1, 51);
                pc.mana = 50 + Random.Range(1, 26);

                pc.strength = Random.Range(1, 6) + 10;
                pc.agility = Random.Range(1, 6) + 10;
                pc.wisdom = Random.Range(1, 6) + 5;

                int helm = Random.Range(1, 4);
                int weapon = Random.Range(1, 2);

                if (helm == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.Cap);
                else if (helm == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.HeadBand);

                pc.equipment.AddLast(GameContent.EquipmentID.Leather);

                if (weapon == 1)
                    pc.equipment.AddLast(GameContent.EquipmentID.Staff);
                else if (weapon == 2)
                    pc.equipment.AddLast(GameContent.EquipmentID.Unarmed);
            }
        }
    }

    static public void RefreshUI()
    {
        systemManager.GetComponent<SystemManager>().RefreshUI();
    }

    static public void SetSystemManager(GameObject SystemManager)
    {
        systemManager = SystemManager;
    }

    static public string GetPartyNameFromInput()
    {
        return systemManager.GetComponent<SystemManager>().GetPartyNameFromInput();
    }

}

public partial class PartyCharacter
{
    public PartyCharacter(int ClassID, int Health, int Mana, int Strength, int Agility, int Wisdom)
    {
        classID = ClassID;
        health = Health;
        mana = Mana;
        strength = Strength;
        agility = Agility;
        wisdom = Wisdom;
        equipment = new LinkedList<int>();
    }
    public PartyCharacter()
    {
        equipment = new LinkedList<int>();
    }

}
