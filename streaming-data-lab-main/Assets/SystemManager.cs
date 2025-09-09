using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour
{

    GameObject saveButton;
    GameObject loadButton;
    GameObject rerollButton;

    GameObject partyNameInputField;

    GameObject loadPartyDropDown;

    GameObject characterStatsTemplate;

    LinkedList<GameObject> partyCharacters;

    Dictionary<int, Sprite> spriteLookUp;

    GameObject saveButton2;
    GameObject newButton;
    GameObject deleteButton;

    void Start()
    {
        GameContent.CharacterClassID.Init();
        GameContent.EquipmentID.Init();

        spriteLookUp = new Dictionary<int, Sprite>();
        spriteLookUp.Add(GameContent.CharacterClassID.Fighter, Resources.Load<Sprite>("Fighter"));
        spriteLookUp.Add(GameContent.CharacterClassID.BlackMage, Resources.Load<Sprite>("BlackMage"));
        spriteLookUp.Add(GameContent.CharacterClassID.WhiteMage, Resources.Load<Sprite>("WhiteMage"));
        spriteLookUp.Add(GameContent.CharacterClassID.RedMage, Resources.Load<Sprite>("RedMage"));
        spriteLookUp.Add(GameContent.CharacterClassID.Thief, Resources.Load<Sprite>("Thief"));
        spriteLookUp.Add(GameContent.CharacterClassID.Monk, Resources.Load<Sprite>("Monk"));

        Object[] GameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (Object go in GameObjects)
        {
            if (go.name == "SaveButton")
                saveButton = (GameObject)go;
            else if (go.name == "LoadButton")
                loadButton = (GameObject)go;
            else if (go.name == "RerollButton")
                rerollButton = (GameObject)go;
            else if (go.name == "PartyNameInputField")
                partyNameInputField = (GameObject)go;
            else if (go.name == "CharacterStatsDisplayContainer")
                characterStatsTemplate = (GameObject)go;
            else if (go.name == "LoadPartyDropdown")
                loadPartyDropDown = (GameObject)go;
            else if (go.name == "SaveButton2")
                saveButton2 = (GameObject)go;
            else if (go.name == "NewButton")
                newButton = (GameObject)go;
            else if (go.name == "DeleteButton")
                deleteButton = (GameObject)go;
        }

        characterStatsTemplate.SetActive(false);

        saveButton.GetComponent<Button>().onClick.AddListener(SaveButtonPressed);
        loadButton.GetComponent<Button>().onClick.AddListener(LoadButtonPressed);
        rerollButton.GetComponent<Button>().onClick.AddListener(RerollButtonPressed);
        loadPartyDropDown.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { LoadDropDownChanged(); });

        saveButton2.GetComponent<Button>().onClick.AddListener(SaveButton2Pressed);
        newButton.GetComponent<Button>().onClick.AddListener(NewButtonPressed);
        deleteButton.GetComponent<Button>().onClick.AddListener(DeleteButtonPressed);

        GameContent.SetSystemManager(gameObject);

        AssignmentPart2.GameStart();
        GameContent.RerollParty();
        RefreshUI();

    }

    void Update()
    {

    }

    public void RefreshUI()
    {
        saveButton.SetActive(false);
        loadButton.SetActive(false);
        rerollButton.SetActive(false);
        partyNameInputField.SetActive(false);
        loadPartyDropDown.SetActive(false);
        newButton.SetActive(false);
        deleteButton.SetActive(false);
        saveButton2.SetActive(false);

        if (AssignmentConfiguration.PartOfAssignmentThatIsInDevelopment == 1)
        {
            saveButton.SetActive(true);
            loadButton.SetActive(true);
            rerollButton.SetActive(true);
        }
        else if (AssignmentConfiguration.PartOfAssignmentThatIsInDevelopment == 2)
        {
            saveButton2.SetActive(true);
            //newButton.SetActive(true);
            deleteButton.SetActive(true);
            partyNameInputField.SetActive(true);
            loadPartyDropDown.SetActive(true);
            rerollButton.SetActive(true);


            //loadPartyDropDown.GetComponent<Dropdown>().setl

            //             int menuIndex = loadPartyDropDown.GetComponent<Dropdown>().value;
            // List<Dropdown.OptionData> menuOptions = loadPartyDropDown.GetComponent<Dropdown>().options;
            // string value = menuOptions[menuIndex].text;
            // AssignmentPart2.LoadPartyDropDownChanged(value);


            Dropdown dropdown = loadPartyDropDown.GetComponent<Dropdown>();
            dropdown.options.Clear();
            foreach (string option in AssignmentPart2.GetListOfPartyNames())
            {
                dropdown.options.Add(new Dropdown.OptionData(option));
            }

        }


        if (partyCharacters != null)
        {
            while (partyCharacters.Count > 0)
            {
                GameObject d = partyCharacters.First.Value;
                partyCharacters.RemoveFirst();
                Destroy(d);
            }
        }
        else
            partyCharacters = new LinkedList<GameObject>();


        if (GameContent.partyCharacters != null)
        {
            if (GameContent.partyCharacters.Count > 0)
            {
                float pos = -((float)GameContent.partyCharacters.Count / 2f - 0.5f);
                foreach (PartyCharacter pc in GameContent.partyCharacters)
                {
                    GameObject go = Instantiate(characterStatsTemplate, characterStatsTemplate.transform.parent);
                    go.SetActive(true);
                    go.GetComponent<RectTransform>().position = go.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width / 2 + (pos * ((float)Screen.width / (float)GameContent.partyCharacters.Count)), Screen.height / 2 + 30, 0);
                    pos++;
                    partyCharacters.AddLast(go);

                    string info = GameContent.CharacterClassID.lookUp[pc.classID] + "\n" +
                        "HP: " + pc.health + "\n" +
                        "MP: " + pc.mana + "\n" +
                        "Str: " + pc.strength + "\n" +
                        "Agi: " + pc.agility + "\n" +
                        "Wis: " + pc.wisdom;

                    foreach (int equipID in pc.equipment)
                    {
                        info = info + "\n" + GameContent.EquipmentID.lookUp[equipID];
                    }

                    go.GetComponentInChildren<Text>().text = info;

                    go.GetComponentInChildren<Image>().sprite = spriteLookUp[pc.classID];
                }
            }
        }

    }

    public void RerollButtonPressed()
    {
        GameContent.RerollParty();
        RefreshUI();
    }

    public void SaveButtonPressed()
    {
        AssignmentPart1.SavePartyButtonPressed();

    }

    public void LoadButtonPressed()
    {
        AssignmentPart1.LoadPartyButtonPressed();
    }

    public void LoadDropDownChanged()
    {
        int menuIndex = loadPartyDropDown.GetComponent<Dropdown>().value;
        List<Dropdown.OptionData> menuOptions = loadPartyDropDown.GetComponent<Dropdown>().options;
        string value = menuOptions[menuIndex].text;
        AssignmentPart2.LoadPartyDropDownChanged(value);
    }


    public void SaveButton2Pressed()
    {
        AssignmentPart2.SavePartyButtonPressed();
    }

    public void NewButtonPressed()
    {
        //AssignmentPart2.NewPartyButtonPressed();
    }

    public void DeleteButtonPressed()
    {
        AssignmentPart2.DeletePartyButtonPressed();
    }

    public string GetPartyNameFromInput()
    {

        return partyNameInputField.GetComponentsInChildren<Text>()[1].text;

    }

}

