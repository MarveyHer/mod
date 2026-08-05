using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;

namespace Magic
{
    class MagicStatsWindow : MonoBehaviour
    {
        private static GameObject contents;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        public static MagicStatsWindow instance;
        

         public static void init()
        {
            
            contents = WindowManager.windowContents["MagicStatsWindow"];
            instance = new GameObject("MagicStatsWindowInstance").AddComponent<MagicStatsWindow>();
            scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/MagicStatsWindow/Background/Scroll View");
            originalSize = contents.GetComponent<RectTransform>().sizeDelta;
            VerticalLayoutGroup layoutGroup = contents.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childScaleHeight = true;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = 50;
            loadSettingOptions();
        }

        private static void loadSettingOptions()
        {
            loadInputOptions();
            loadBoolOptions();
            
        }

        public static void openWindow()
        {
            Windows.ShowWindow("MagicStatsWindow");
        }

        private static void loadBoolOptions()
        {
            string language = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") as string;
            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, ((Main.savedSettings.boolOptions.Count))*180);
            foreach(KeyValuePair<string, bool> kv in Main.savedSettings.boolOptions)
            {
                UnityAction call = null;
                switch (kv.Key)
                {
                    case "NaturalBirth":
                        call = delegate{
                            Main.spawn_race = NCMS.Utils.PowerButtons.GetToggleValue("NaturalBirth");
                        };
                        if(language == "ru")
                        {
                            PowerButtons.CreateButton(
                                "NaturalBirth", 
                                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.earth_spirit.png"),
                                "Естесственное рождение", 
                                "Самостоятельное появление рас и существ в мире", 
                                new Vector2(0, 0), 
                                ButtonType.Toggle, 
                                contents.transform, 
                                delegate{
                                    Main.modifyBoolOption("NaturalBirth", NCMS.Utils.PowerButtons.GetToggleValue("NaturalBirth"), call);
                                }
                            );
                        }
                        else{
                            PowerButtons.CreateButton(
                                "NaturalBirth", 
                                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.earth_spirit.png"),
                                "Natural Birth", 
                                "The independent appearance of creatures in the world", 
                                new Vector2(0, 0), 
                                ButtonType.Toggle, 
                                contents.transform, 
                                delegate{
                                    Main.modifyBoolOption("NaturalBirth", NCMS.Utils.PowerButtons.GetToggleValue("NaturalBirth"), call);
                                }
                            );
                        }
                        
                        break;
                    case "SpiritBirth":
                        call = delegate{
                            Main.spirit_spawn = NCMS.Utils.PowerButtons.GetToggleValue("SpiritBirth");
                        };
                        if(language == "ru")
                        {
                            PowerButtons.CreateButton(
                                "SpiritBirth", 
                                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.air_spirit.png"), 
                                "Рождение духов", 
                                "Могут ли духи появляться в мире самостоятельно", 
                                new Vector2(0, 0), 
                                ButtonType.Toggle, 
                                contents.transform, 
                                delegate{
                                    Main.modifyBoolOption("SpiritBirth", NCMS.Utils.PowerButtons.GetToggleValue("SpiritBirth"), call);
                                }
                            );    
                        }
                        else{
                            PowerButtons.CreateButton(
                                "SpiritBirth", 
                                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.air_spirit.png"), 
                                "Spirit Birth", 
                                "The birth of spirits in the world", 
                                new Vector2(0, 0), 
                                ButtonType.Toggle, 
                                contents.transform, 
                                delegate{
                                    Main.modifyBoolOption("SpiritBirth", NCMS.Utils.PowerButtons.GetToggleValue("SpiritBirth"), call);
                                }
                            ); 
                        }
                        
                        break;
                    case "DeathMagic":
                        call = delegate{
                            Main.NewMagicOfDeath = NCMS.Utils.PowerButtons.GetToggleValue("DeathMagic");
                        };
                        if(language == "ru")
                        {
                            PowerButton DeathButton =PowerButtons.CreateButton(
                                "DeathMagic", 
                                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.magicOfDeath.png"), 
                                "Новая магия смерти", 
                                "Новая магия смерти (обнуляет количество убийств у владельца магии и его призывов)", 
                                new Vector2(0, 0), 
                                ButtonType.Toggle, 
                                contents.transform, 
                                delegate{
                                    Main.modifyBoolOption("DeathMagic", NCMS.Utils.PowerButtons.GetToggleValue("DeathMagic"), call);
                                }  
                            );     
                        }
                        else{
                            PowerButton DeathButton =PowerButtons.CreateButton(
                                "DeathMagic", 
                                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.magicOfDeath.png"), 
                                "New magic of Death", 
                                "New death magic (nullifies kills from the owner and his summons)", 
                                new Vector2(0, 0), 
                                ButtonType.Toggle, 
                                contents.transform, 
                                delegate{
                                    Main.modifyBoolOption("DeathMagic", NCMS.Utils.PowerButtons.GetToggleValue("DeathMagic"), call);
                                }  
                            );  
                        }
                        break;

                    case "DemonicBuild":
                        call = delegate{
                            Main.spawn_demon = NCMS.Utils.PowerButtons.GetToggleValue("DemonicBuild");
                        };
                        if(language == "ru")
                        {
                            PowerButtons.CreateButton(
                                "DemonicBuild", 
                                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Fire_spirit.png"),
                                "Осквернение инферно", 
                                "Самостоятельное появление демонических армий и зданий", 
                                new Vector2(0, 0), 
                                ButtonType.Toggle, 
                                contents.transform, 
                                delegate{
                                    Main.modifyBoolOption("DemonicBuild", NCMS.Utils.PowerButtons.GetToggleValue("DemonicBuild"), call);
                                }
                            );
                        }
                        else{
                            PowerButtons.CreateButton(
                                "DemonicBuild", 
                                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Fire_spirit.png"),
                                "Desecration of the inferno", 
                                "The independent appearance of creatures in the world", 
                                new Vector2(0, 0), 
                                ButtonType.Toggle, 
                                contents.transform, 
                                delegate{
                                    Main.modifyBoolOption("DemonicBuild", NCMS.Utils.PowerButtons.GetToggleValue("DemonicBuild"), call);
                                }
                            );
                        }
                        
                        break;
                    case "InvasionDemons":
                        call = delegate{
                            Main.InvasionDemons = NCMS.Utils.PowerButtons.GetToggleValue("InvasionDemons");
                        };
                        if(language == "ru")
                        {
                            PowerButtons.CreateButton(
                                "InvasionDemons", 
                                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Fire_spirit.png"),
                                "Нашествие демонов", 
                                "Высшие демоны ведут свои армии на разумные расы", 
                                new Vector2(0, 0), 
                                ButtonType.Toggle, 
                                contents.transform, 
                                delegate{
                                    Main.modifyBoolOption("InvasionDemons", NCMS.Utils.PowerButtons.GetToggleValue("InvasionDemons"), call);
                                }
                            );
                        }
                        else{
                            PowerButtons.CreateButton(
                                "InvasionDemons", 
                                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.Fire_spirit.png"),
                                "Invasion of demons", 
                                "The higher demons lead their armies against the intelligent races", 
                                new Vector2(0, 0), 
                                ButtonType.Toggle, 
                                contents.transform, 
                                delegate{
                                    Main.modifyBoolOption("InvasionDemons", NCMS.Utils.PowerButtons.GetToggleValue("InvasionDemons"), call);
                                }
                            );
                        }
                        
                        break;
                }
                /*if (kv.Key == "NaturalBirth")
                {
                    if(Main.savedSettings.boolOptions["NaturalBirth"])
                    {
                        PowerButtons.ToggleButton("NaturalBirths");
                        if (call != null)
                        {
                            call.Invoke();
                        }
                    }
                    
                }
                else */if (kv.Value)
                {
                    PowerButtons.ToggleButton(kv.Key);
                    if (call != null)
                    {
                        call.Invoke();
                    }
                }
            }
        }
        private static void loadInputOptions()
        {
            contents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, ((Main.savedSettings.magicOptions.Count))*250);
            foreach(KeyValuePair<string, InputOption> kv in Main.savedSettings.magicOptions)
            {

                UnityAction call = null;
                switch(kv.Key)
                {
                    case "MagicBirth%":
                        call = delegate{
                            if (Main.savedSettings.magicOptions["MagicBirth%"].active)
                            {
                                Main.magicBirth = int.Parse(Main.savedSettings.magicOptions["MagicBirth%"].value);
                            }
                            else
                            {
                                Main.magicBirth = 3f;
                                Debug.Log("Inactive");
                            }
                        };
                        break;
                    case "MagicInherit%":
                        call = delegate{
                            if (Main.savedSettings.magicOptions["MagicInherit%"].active)
                            {
                                Main.magicInherit = int.Parse(Main.savedSettings.magicOptions["MagicInherit%"].value);
                            }
                            else
                            {
                                Main.magicInherit = 30f;
                                Debug.Log("Inactive");
                            }
                        };
                        break;
                    case "AgeOfBloodMagic":
                        call = delegate{
                            if (Main.savedSettings.magicOptions["AgeOfBloodMagic"].active)
                            {
                                Main.BloodAge = int.Parse(Main.savedSettings.magicOptions["AgeOfBloodMagic"].value);
                            }
                            else
                            {
                                Main.BloodAge = 300f;
                                Debug.Log("Inactive");
                            }
                        };
                        break;
                    case "SpiritInitiation%":
                        call = delegate{
                            if (Main.savedSettings.magicOptions["SpiritInitiation%"].active)
                            {
                                Main.spiritInitiations = int.Parse(Main.savedSettings.magicOptions["SpiritInitiation%"].value);
                            }
                            else
                            {
                                Main.spiritInitiations = 5f;
                                Debug.Log("Inactive");
                            }
                        };
                        break;
                    case "DefilerBirth%":
                        call = delegate{
                            if (Main.savedSettings.magicOptions["DefilerBirth%"].active)
                            {
                                Main.DefilerBirth = int.Parse(Main.savedSettings.magicOptions["DefilerBirth%"].value);
                            }
                            else
                            {
                                Main.DefilerBirth = 0f;
                                Debug.Log("Inactive");
                            }
                        };
                        break;
                    case "DefilerInherit%":
                        call = delegate{
                            if (Main.savedSettings.magicOptions["DefilerInherit%"].active)
                            {
                                Main.DefilerInherit = int.Parse(Main.savedSettings.magicOptions["DefilerInherit%"].value);
                            }
                            else
                            {
                                Main.DefilerInherit = 0f;
                                Debug.Log("Inactive");
                            }
                        };
                        break;
                    case "DemonFighterBirth%":
                        call = delegate{
                            if (Main.savedSettings.magicOptions["DemonFighterBirth%"].active)
                            {
                                Main.DemonFighterBirth = int.Parse(Main.savedSettings.magicOptions["DemonFighterBirth%"].value);
                            }
                            else
                            {
                                Main.DemonFighterBirth = 0f;
                                Debug.Log("Inactive");
                            }
                        };
                        break;
                    case "DemonFighterInherit%":
                        call = delegate{
                            if (Main.savedSettings.magicOptions["DemonFighterInherit%"].active)
                            {
                                Main.DemonFighterInherit = int.Parse(Main.savedSettings.magicOptions["DemonFighterInherit%"].value);
                            }
                            else
                            {
                                Main.DemonFighterInherit = 10f;
                                Debug.Log("Inactive");
                            }
                        };
                        break;
                }
                if (call != null)
                {
                    call.Invoke();
                }

                NameInput input = NewUI.createInputOption(
                    $"{kv.Key}_setting", 
                    kv.Key, 
                    "Modify The Value Of This Setting", 
                    0, 
                    contents, 
                    kv.Value.value
                );
                input.inputField.characterValidation = InputField.CharacterValidation.Integer;
                input.inputField.onValueChanged.AddListener(delegate{
                    string pValue = NewUI.checkStatInput(input);
                    Main.modifyMagicOption(kv.Key, pValue, PowerButtons.GetToggleValue($"{kv.Key}Button"), call);
                    input.setText(pValue);
                });

                PowerButton activeButton = PowerButtons.CreateButton(
                    $"{kv.Key}Button", 
                    Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.units.icon.png"),
                    "Activate Setting", 
                    "", 
                    new Vector2(200, 0), 
                    ButtonType.Toggle, 
                    input.transform.parent.transform, 
                    delegate{
                        string pValue = NewUI.checkStatInput(input);
                        Main.modifyMagicOption(kv.Key, pValue, PowerButtons.GetToggleValue($"{kv.Key}Button"), call);
                        input.setText(pValue);
                    }
                );
                if (kv.Value.active)
                {
                    PowerButtons.ToggleButton($"{kv.Key}Button");
                }
                activeButton.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64);
            }
        }
    }
}