using System.IO;
using UnityEngine;
using BepInEx;

namespace Revive
{
    public class GUIR
    {
        public GUISkin TextHeaderSkin;
        public GUISkin TextSkin;
        public GUISkin NavBarSkin;
        public GUISkin NavBarShadowSkin;
        public GUISkin WindowSkin;
        public GUISkin WindowShadowSkin;
        public GUISkin CheckBoxCheckSkin;
        public GUISkin CheckBoxUncheckSkin;
        public GUISkin CameraOnSkin;
        public GUISkin CameraOffSkin;
        public GUISkin CarOnSkin;
        public GUISkin CarOffSkin;
        public GUISkin EnviromentOnSkin;
        public GUISkin EnviromentOffSkin;
        public GUISkin GoProOnSkin;
        public GUISkin GoProOffSkin;
        public GUISkin AnimationOnSkin;
        public GUISkin AnimationOffSkin;
        public GUISkin MapOnSkin;
        public GUISkin MapOffSkin;
        public GUISkin KeyBindsOnSkin;
        public GUISkin KeyBindsOffSkin;
        public GUISkin SettingsOnSkin;
        public GUISkin SettingsOffSkin;
        public GUISkin TextFieldSkin;
        public GUISkin SliderSkin;
        public GUISkin SliderThumbSkin;
        public GUISkin ButtonSkin;
        public GUISkin ScrollBarSkin;
        public GUIStyle TextStyle;

        Font robotoLight;
        Texture2D NAVBAR_BG = LoadTexture("NavBar_BG.png");
        Texture2D NAVBARSHADOW = LoadTexture("NavBar_Shadow.png");
        Texture2D WINDOW_BG = LoadTexture("Window_BG.png");
        Texture2D WINDOW_BGSHADOW = LoadTexture("Window_BGShadow.png");
        Texture2D CHECK = LoadTexture("CheckBox_Check.png");
        Texture2D UNCHECK = LoadTexture("CheckBox_Uncheck.png");
        Texture2D CAMERA_BUTTON_ON = LoadTexture("Button_Camera_On.png");
        Texture2D CAMERA_BUTTON_OFF = LoadTexture("Button_Camera_Off.png");
        Texture2D CAR_BUTTON_ON = LoadTexture("Button_Car_On.png");
        Texture2D CAR_BUTTON_OFF = LoadTexture("Button_Car_Off.png");
        Texture2D ENVIROMENT_BUTTON_ON = LoadTexture("Button_Enviroment_On.png");
        Texture2D ENVIROMENT_BUTTON_OFF = LoadTexture("Button_Enviroment_Off.png");
        Texture2D GOBRO_BUTTON_ON = LoadTexture("Button_GoPro_On.png");
        Texture2D GOBRO_BUTTON_OFF = LoadTexture("Button_GoPro_Off.png");
        Texture2D ANIMATION_BUTTON_ON = LoadTexture("Button_Animation_On.png");
        Texture2D ANIMATION_BUTTON_OFF = LoadTexture("Button_Animation_Off.png");
        Texture2D KEYBINDS_BUTTON_ON = LoadTexture("Button_KeyBinds_On.png");
        Texture2D KEYBINDS_BUTTON_OFF = LoadTexture("Button_KeyBinds_Off.png");
        Texture2D SETTINGS_BUTTON_ON = LoadTexture("Button_Settings_On.png");
        Texture2D SETTINGS_BUTTON_OFF = LoadTexture("Button_Settings_Off.png");
        Texture2D TEXTFIELD = LoadTexture("TextField.png");
        Texture2D SLIDER = LoadTexture("Slider.png");
        Texture2D SLIDER_THUMB = LoadTexture("Slider_Thumb.png");
        Texture2D BUTTON = LoadTexture("Button.png");
        Texture2D BUTTON_PRESSED = LoadTexture("Button_Pressed.png");
        Texture2D SCROLLBAR= LoadTexture("ScrollBar.png");

        public static Texture2D LoadTexture(string name)
        {
            Texture2D texture = new Texture2D(4, 4);
            FileStream fs = new FileStream(Paths.PluginPath + "/Revive/Resources/Textures/" + name, FileMode.Open, FileAccess.Read);
            byte[] imageData = new byte[fs.Length];
            fs.Read(imageData, 0, (int)fs.Length);
            texture.LoadImage(imageData);
            return texture;
        }

        public GUIR()
        {
            robotoLight = Font.CreateDynamicFontFromOSFont("Roboto Light", 12);

            TextHeaderSkin = new GUISkin();
            TextSkin = new GUISkin();
            NavBarSkin = new GUISkin();
            NavBarShadowSkin = new GUISkin();
            WindowSkin = new GUISkin();
            WindowShadowSkin = new GUISkin();
            CheckBoxCheckSkin = new GUISkin();
            CheckBoxUncheckSkin = new GUISkin();
            CameraOnSkin = new GUISkin();
            CameraOffSkin = new GUISkin();
            CarOnSkin = new GUISkin();
            CarOffSkin = new GUISkin();
            EnviromentOnSkin = new GUISkin();
            EnviromentOffSkin = new GUISkin();
            GoProOnSkin = new GUISkin();
            GoProOffSkin = new GUISkin();
            AnimationOnSkin = new GUISkin();
            AnimationOffSkin = new GUISkin();
            KeyBindsOnSkin = new GUISkin();
            KeyBindsOffSkin = new GUISkin();
            SettingsOnSkin = new GUISkin();
            SettingsOffSkin = new GUISkin();
            TextFieldSkin = new GUISkin();
            SliderSkin = new GUISkin();
            ButtonSkin = new GUISkin();
            ScrollBarSkin = new GUISkin();
            TextStyle = new GUIStyle();

            TextHeaderSkin.label.font = robotoLight;
            TextHeaderSkin.label.fontSize = 18;
            TextSkin.label.font = robotoLight;
            TextSkin.label.fontSize = 13;
            TextFieldSkin.textField.normal.background = TEXTFIELD;
            TextFieldSkin.textField.font = robotoLight;
            TextFieldSkin.textField.alignment = TextAnchor.MiddleCenter;
            TextFieldSkin.textField.fontSize = 13;
            TextFieldSkin.textField.stretchHeight = false;
            TextFieldSkin.textField.stretchWidth = false;
            NavBarSkin.box.normal.background = NAVBAR_BG;
            NavBarShadowSkin.box.normal.background = NAVBARSHADOW;
            WindowSkin.box.normal.background = WINDOW_BG;
            WindowShadowSkin.box.normal.background = WINDOW_BGSHADOW;
            CheckBoxCheckSkin.button.normal.background = CHECK;
            CheckBoxCheckSkin.button.stretchHeight = false;
            CheckBoxCheckSkin.button.stretchWidth = false;
            CheckBoxCheckSkin.button.fixedHeight = 17;
            CheckBoxCheckSkin.button.fixedWidth = 17;
            CheckBoxUncheckSkin.button.normal.background = UNCHECK;
            CheckBoxUncheckSkin.button.stretchHeight = false;
            CheckBoxUncheckSkin.button.stretchWidth = false;
            CheckBoxUncheckSkin.button.fixedHeight = 17;
            CheckBoxUncheckSkin.button.fixedWidth = 17;
            CameraOnSkin.button.normal.background = CAMERA_BUTTON_ON;
            CameraOffSkin.button.normal.background = CAMERA_BUTTON_OFF;
            CarOnSkin.button.normal.background = CAR_BUTTON_ON;
            CarOffSkin.button.normal.background = CAR_BUTTON_OFF;
            EnviromentOnSkin.button.normal.background = ENVIROMENT_BUTTON_ON;
            EnviromentOffSkin.button.normal.background = ENVIROMENT_BUTTON_OFF;
            GoProOnSkin.button.normal.background = GOBRO_BUTTON_ON;
            GoProOffSkin.button.normal.background = GOBRO_BUTTON_OFF;
            AnimationOnSkin.button.normal.background = ANIMATION_BUTTON_ON;
            AnimationOffSkin.button.normal.background = ANIMATION_BUTTON_OFF;
            KeyBindsOnSkin.button.normal.background = KEYBINDS_BUTTON_ON;
            KeyBindsOffSkin.button.normal.background = KEYBINDS_BUTTON_OFF;
            SettingsOnSkin.button.normal.background = SETTINGS_BUTTON_ON;
            SettingsOffSkin.button.normal.background = SETTINGS_BUTTON_OFF;
            SliderSkin.horizontalSlider.normal.background = SLIDER;
            SliderSkin.horizontalSlider.stretchHeight = false;
            SliderSkin.horizontalSlider.stretchWidth = false;
            SliderSkin.horizontalSlider.fixedHeight = 16;
            SliderSkin.horizontalSliderThumb.normal.background = CHECK;
            SliderSkin.horizontalSliderThumb.stretchHeight = false;
            SliderSkin.horizontalSliderThumb.stretchWidth = false;
            SliderSkin.horizontalSliderThumb.fixedHeight = 16;
            SliderSkin.horizontalSliderThumb.fixedWidth = 16;
            ButtonSkin.button.normal.background = BUTTON;
            ButtonSkin.button.active.background = BUTTON_PRESSED;
            ButtonSkin.button.font = robotoLight;
            ButtonSkin.button.normal.textColor = Color.white;
            ButtonSkin.button.alignment = TextAnchor.MiddleCenter;
            ScrollBarSkin.verticalScrollbar.normal.background = SCROLLBAR;
            ScrollBarSkin.verticalScrollbar.stretchHeight = false;
            ScrollBarSkin.verticalScrollbar.stretchWidth = false;
            ScrollBarSkin.verticalScrollbar.fixedWidth = 16;
            ScrollBarSkin.verticalScrollbarThumb.normal.background = SLIDER_THUMB;
            ScrollBarSkin.verticalScrollbarThumb.stretchHeight = false;
            ScrollBarSkin.verticalScrollbarThumb.stretchWidth = false;
            TextStyle.normal.textColor = Color.white;
        }
    }
}
