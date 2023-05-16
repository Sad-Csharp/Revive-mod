using BepInEx;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Revive
{
    [BepInPlugin(GUID: "plugin.revive.mod", Name: "Revive", Version: "2.0.0")]
    public class Revive : BaseUnityPlugin
    {
        [Header("Ui Variables")]
        Vector2 scrollLights = Vector3.zero;
        Vector2 scrollKeyBinds = Vector3.zero;
        public bool showUi = true;
        public bool cameraUi = true;
        public bool carUi;
        public bool enviromentUi;
        public bool goProUi;
        public bool animationUi;
        public bool mapUi;
        public bool keyBindsUi;
        public bool settingsUi;
        public float buttonXPosCamera;
        public float buttonXPosCar;
        public float buttonXPosEnviroment;
        public float buttonXPosGoPro;
        public float buttonXPosAnimation;
        public float buttonXPosMap;
        public float buttonXPosKeyBinds;
        public float buttonXPosSettings;
        public float buttonXPosOn = 15f;
        public float buttonXPosOff = 35f;
        public float buttonAnimationSpeed = 15;

        public bool ctEnabled;
        public Camera mainCamera;
        public GameObject ctCam;
        public Vector3 cameraPosition;
        public Quaternion cameraRotation;

        public bool noClip;
        public bool hideScore;

        public bool movementResistance = true;
        public bool controlCamera;
        public float movementSpeed = 100f;
        public float xVal;
        public float yVal;
        public float zVal;
        public float mouseX;
        public float mouseY;
        public float mouseSensitivity = 1f;
        public bool smoothCamera;
        public float smoothValue = 3f;
        public float tiltAngle;
        public float tiltFactor = 10f;
        public float fov = 75;
        public float fovFactor = 10f;

        [Header("Wobble")]
        public bool wobble;
        public float wobbleAmount;
        public float wobbleSpeed;
        public Quaternion wobbleRot;
        public float randomizeEvery;
        public float randomizeTimer;

        [Header("General")]
        public RaceCar[] cars;

        [Header("Car Settings")]
        public RaceCar playersCar;
        public float brakeValue;
        public bool freezeCar;
        public Vector3 freezePos;
        public Quaternion freezeRot;

        [Header("Enviroment")]
        public bool nightMode;
        public GameObject sun;
        public GameObject lightPrefab;
        public bool symmetry;
        public int maxLights = 40;
        public GameObject[] lights;
        public Light[] lightsInScene;
        public Vector3 mirrorPos;
        public Quaternion mirrorRot;
        public string lightName;
        public float lightIntensity = 1f;
        public float lightRange = 1f;
        public Color lightColor = Color.white;
        public float lightColorR = 1f;
        public float lightColorG = 1f;
        public float lightColorB = 1f;
        public bool[] strobe;
        public bool strobeTemp;
        public bool[] bindLight;
        public GameObject[] brakeLight;
        public bool lightsOn;
        public GameObject[] strobeLights;
        public float strobeSpeedTemp;
        public float strobeForTemp;
        public float[] strobeSpeed;
        public float[] strobeFor;
        public float[] strobeForTimer;
        public float[] strobeTimer;
        public Component[] comps;

        [Header("Custom Camera")]
        public Vector3 customCameraPosition;
        public bool customCamera = true;
        public bool dynamicFollow = true;
        public float followSpeed = 10f;
        public float distance = 5f;
        public float height = 2f;

        [Header("Mount/Drone Mode")]
        public bool goProMode;
        public RaceCar mountCar;
        public RaceCar lookingCar;
        public Vector3 goProOffset;
        public bool mount;
        public bool focusOnCar;

        [Header("Animation")]
        public bool animate;
        public float moveSpeed;
        public float rotationSpeed;
        public float fovSpeed;
        public int maxPoints = 16;
        public Transform[] points;
        public GameObject[] currentPoints;
        public float[] fovs;
        public int currentPosition;

        [Header("KeyBinds")]
        public KeyCode enableCTKey = KeyCode.Minus;
        public KeyCode enableUIKey = KeyCode.Equals;
        public int keyBinds = 21;
        public string[] keyBindName;
        public bool[] editingKey;
        public KeyCode[] keyBindKey;

        public GUIR skin = new GUIR();

        void Start()
        {
            lights = new GameObject[maxLights];
            strobe = new bool[maxLights];
            bindLight = new bool[maxLights];
            strobeLights = new GameObject[maxLights];
            strobeSpeed = new float[maxLights];
            strobeFor = new float[maxLights];
            strobeForTimer = new float[maxLights];
            strobeTimer = new float[maxLights];
            brakeLight = new GameObject[maxLights];

            for (int i = 0; i < strobe.Length; i++)
            {
                strobe[i] = false;
                strobeSpeed[i] = 0.05f;
                strobeFor[i] = 0.05f;
            }

            points = new Transform[maxPoints];
            fovs = new float[maxPoints];

            keyBindName = new string[keyBinds];
            editingKey = new bool[keyBinds];
            keyBindKey = new KeyCode[keyBinds];

            NameKeybinds();
            DefaultKeyBinds();

        }

        void LateUpdate()
        {
            QualitySettings.shadows = ShadowQuality.All;

            if (Input.GetKeyDown(enableCTKey))
            {
                ctEnabled = !ctEnabled;
                CTCameraEnable();
            }

            if (Input.GetKeyDown(enableUIKey))
            {
                if (ctEnabled)
                {
                    showUi = !showUi;
                }
            }

            StrobeAnimator();
            ClampValues();
            LightOptimizations();

            if (ctEnabled)
            {
                FovChanger();
                FreezeCar();
                GUIAnimations();
                SearchForCars();
                CameraMovement();
                CameraAnimation();

                KeyBindsActions();

                if (sun == null)
                {
                    sun = GameObject.Find("sunlight");
                }
                if (lightPrefab == null)
                {
                    lightPrefab = sun;
                }
            }
            else
            {
                showUi = false;
            }

            if (mountCar == null)
            {
                mount = false;
            }
            if (lookingCar == null)
            {
                focusOnCar = false;
            }

            if (playersCar != null)
            {
                brakeValue = playersCar.GetComponent<CARXCar>().brake;
                Taillight();
            }
        }

        void NightMode()
        {
            GameObject ebisu = GameObject.Find("ebisu");
            if (ebisu != null)
            {
                Component[] comps = ebisu.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject redrock = GameObject.Find("redrock");
            if (redrock != null)
            {
                Component[] comps = redrock.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject parking = GameObject.Find("parking");
            if (parking != null)
            {
                Component[] comps = parking.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject japan = GameObject.Find("japan");
            if (japan != null)
            {
                Component[] comps = japan.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject redring = GameObject.Find("redring");
            if (redring != null)
            {
                Component[] comps = redring.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject silverstone = GameObject.Find("silverstone");
            if (silverstone != null)
            {
                Component[] comps = silverstone.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject airfield = GameObject.Find("airfield");
            if (airfield != null)
            {
                Component[] comps = airfield.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject winterfell = GameObject.Find("winterfell");
            if (winterfell != null)
            {
                Component[] comps = winterfell.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject bathurst = GameObject.Find("bathurst");
            if (bathurst != null)
            {
                Component[] comps = bathurst.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject losAngeles = GameObject.Find("losAngeles");
            if (losAngeles != null)
            {
                Component[] comps = losAngeles.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject petersburg = GameObject.Find("petersburg");
            if (petersburg != null)
            {
                Component[] comps = petersburg.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject irwindale = GameObject.Find("irwindale");
            if (irwindale != null)
            {
                Component[] comps = irwindale.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }

            GameObject fiorano = GameObject.Find("fiorano");
            if (fiorano != null)
            {
                Component[] comps = fiorano.GetComponents(typeof(Component));
                Destroy(comps[2]);
            }
        }

        void NameKeybinds()
        {
            keyBindName[0] = "Move Forward";
            keyBindName[1] = "Move Backwards";
            keyBindName[2] = "Move Left";
            keyBindName[3] = "Move Right";
            keyBindName[4] = "Move Up";
            keyBindName[5] = "Move Down";
            keyBindName[6] = "Zoom In";
            keyBindName[7] = "Zoom Out";
            keyBindName[8] = "Til Left";
            keyBindName[9] = "Tilt Right";
            keyBindName[10] = "Control Camera";
            keyBindName[11] = "Teleport Camera To Car";
            keyBindName[12] = "Custom Camera";
            keyBindName[13] = "Freeze Car";
            keyBindName[14] = "Teleport Car To Camera";
            keyBindName[15] = "Eanble/Disable Symmetry";
            keyBindName[16] = "Add Headlight";
            keyBindName[17] = "Add Spotlight";
            keyBindName[18] = "Add Spotlight (Follow Car)";
            keyBindName[19] = "Add Pointligh";
            keyBindName[20] = "Add Pointlight (Follow Car)";
        }

        void DefaultKeyBinds()
        {
            keyBindKey[0] = KeyCode.W;
            keyBindKey[1] = KeyCode.S;
            keyBindKey[2] = KeyCode.A;
            keyBindKey[3] = KeyCode.D;
            keyBindKey[4] = KeyCode.Q;
            keyBindKey[5] = KeyCode.E;
            keyBindKey[6] = KeyCode.PageUp;
            keyBindKey[7] = KeyCode.PageDown;
            keyBindKey[8] = KeyCode.Z;
            keyBindKey[9] = KeyCode.X;
            keyBindKey[10] = KeyCode.Keypad1;
            keyBindKey[11] = KeyCode.Keypad4;
            keyBindKey[12] = KeyCode.Keypad2;
            keyBindKey[13] = KeyCode.Keypad3;
            keyBindKey[14] = KeyCode.Keypad5;
            keyBindKey[15] = KeyCode.Keypad6;
            keyBindKey[15] = KeyCode.Keypad7;
            keyBindKey[16] = KeyCode.Keypad8;
            keyBindKey[17] = KeyCode.Keypad9;
            keyBindKey[18] = KeyCode.KeypadDivide;
            keyBindKey[19] = KeyCode.KeypadEnter;
            keyBindKey[20] = KeyCode.KeypadMultiply;
        }

        void KeyBindsActions()
        {
            if (Input.GetKeyDown(keyBindKey[10]))
            {
                controlCamera = !controlCamera;
            }
            if (Input.GetKeyDown(keyBindKey[11]))
            {
                TeleportCameraToCar();
            }
            if (Input.GetKeyDown(keyBindKey[12]))
            {
                customCamera = !customCamera;
            }
            if (Input.GetKeyDown(keyBindKey[13]))
            {
                SetFreezeTransform();
                freezeCar = !freezeCar;
            }
            if (Input.GetKeyDown(keyBindKey[14]))
            {
                TeleportCarToCamera();
            }
            if (Input.GetKeyDown(keyBindKey[15]))
            {
                symmetry = !symmetry;
            }
            if (Input.GetKeyDown(keyBindKey[16]))
            {
                AddHeadlight(lightPrefab.GetOrAddComponent<Light>(), playersCar.gameObject, lightName);
            }
            if (Input.GetKeyDown(keyBindKey[17]))
            {
                AddPointLight(lightPrefab.GetOrAddComponent<Light>(), lightName);
            }
            if (Input.GetKeyDown(keyBindKey[18]))
            {
                AddPointLightOnCar(lightPrefab.GetOrAddComponent<Light>(), playersCar.gameObject, lightName);
            }
            if (Input.GetKeyDown(keyBindKey[18]))
            {
                AddPointLight(lightPrefab.GetOrAddComponent<Light>(), lightName);
            }
            if (Input.GetKeyDown(keyBindKey[19]))
            {
                AddPointLightOnCar(lightPrefab.GetOrAddComponent<Light>(), playersCar.gameObject, lightName);
            }
        }

        void ClampValues()
        {
            fov = Mathf.Clamp(fov, 10f, 170f);
            tiltAngle = Mathf.Clamp(tiltAngle, -180f, 180f);
        }

        void SearchForCars()
        {
            cars = FindObjectsOfType<RaceCar>();
        }

        void SearchForPlayersCar()
        {
            cars = FindObjectsOfType<RaceCar>();
            for (int i = 0; i < cars.Length; i++)
            {
                if (cars[i].isNetworkCar == false)
                {
                    playersCar = cars[i];
                    break;
                }
            }
        }

        void FreezeCar()
        {
            if (freezeCar)
            {
                playersCar.transform.position = freezePos;
                playersCar.transform.rotation = freezeRot;
                playersCar.lockState = true;
            }
            else
            {
                playersCar.lockState = false;
            }
        }

        void SetFreezeTransform()
        {
            freezePos = playersCar.transform.position;
            freezeRot = playersCar.transform.rotation;
        }

        void Taillight()
        {
            for (int i = 0; i < brakeLight.Length; i++)
            {
                if (brakeLight[i] != null)
                {
                    brakeLight[i].GetComponent<Light>().intensity = 10f * brakeValue;
                    brakeLight[i].transform.GetChild(0).GetComponent<Light>().intensity = .2f * brakeValue;
                }
            }
        }

        void AddHeadlight(Light light, GameObject car, string name)
        {
            if (symmetry)
            {
                lightColor = new Color(lightColorR, lightColorG, lightColorB);

                light.type = LightType.Spot;
                light.spotAngle = 70f;
                light.innerSpotAngle = 50f;
                light.range = lightRange;
                light.intensity = lightIntensity;
                light.color = lightColor;

                for (int i = 0; i < lights.Length; i++)
                {
                    if (lights[i] == null && lights[i + 1] == null)
                    {
                        GameObject headlight = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        headlight.transform.parent = car.transform;

                        if (lightName != null)
                        {
                            headlight.gameObject.name = name;
                        }
                        else
                        {
                            headlight.gameObject.name = "Light";
                        }

                        GameObject headlightM = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        headlightM.transform.parent = car.transform;

                        mirrorPos = new Vector3(-headlight.transform.localPosition.x, headlight.transform.localPosition.y, headlight.transform.localPosition.z);
                        mirrorRot = Quaternion.Inverse(headlight.transform.localRotation);
                        Quaternion fixedRot = new Quaternion(headlight.transform.localRotation.x, mirrorRot.y, mirrorRot.z, mirrorRot.w);

                        headlightM.transform.localPosition = mirrorPos;
                        headlightM.transform.localRotation = fixedRot;

                        if (lightName != null)
                        {
                            headlightM.transform.name = lightName + " M";
                        }
                        else
                        {
                            headlightM.transform.name = "Light M";
                        }

                        light.type = LightType.Point;
                        light.range = 2f;
                        light.intensity = 4f;
                        light.color = lightColor;
                        GameObject headlightChild = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        headlightChild.transform.parent = headlight.transform;
                        headlightChild.gameObject.name = "HeadlightChild";

                        GameObject headlightChildM = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        headlightChildM.transform.parent = headlightM.transform;
                        headlightChildM.transform.localPosition = Vector3.zero;
                        headlightChildM.transform.rotation = mirrorRot;
                        headlightChildM.gameObject.name = "HeadlightChildM";

                        for (int x = 0; x < lights.Length; x++)
                        {
                            if (lights[x] == null && lights[x + 1] == null)
                            {
                                lights[x] = headlight;
                                strobe[x] = false;
                                lights[x + 1] = headlightM;
                                strobe[x + 1] = false;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                lightColor = new Color(lightColorR, lightColorG, lightColorB);

                light.type = LightType.Spot;
                light.spotAngle = 90f;
                light.innerSpotAngle = 50f;
                light.range = lightRange;
                light.intensity = lightIntensity;
                light.color = lightColor;

                for (int i = 0; i < lights.Length; i++)
                {
                    if (lights[i] == null)
                    {
                        GameObject headlight = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        headlight.transform.parent = car.transform;

                        if (lightName != null)
                        {
                            headlight.gameObject.name = name;
                        }
                        else
                        {
                            headlight.gameObject.name = "Light";
                        }

                        light.type = LightType.Point;
                        light.range = 5f;
                        light.intensity = 10f;
                        light.color = lightColor;
                        GameObject headlightChild = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        headlightChild.transform.parent = headlight.transform;
                        headlightChild.gameObject.name = "HeadlightChild";

                        for (int x = 0; x < lights.Length; x++)
                        {
                            if (lights[x] == null)
                            {
                                lights[x] = headlight;
                                strobe[x] = false;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        void AddTaillight(Light light, GameObject car, string name)
        {
            if (symmetry)
            {
                lightColor = new Color(lightColorR, lightColorG, lightColorB);

                light.type = LightType.Spot;
                light.spotAngle = 90f;
                light.innerSpotAngle = 50f;
                light.range = 10f;
                light.intensity = 5f;
                light.color = lightColor;

                for (int i = 0; i < lights.Length; i++)
                {
                    if (lights[i] == null && lights[i + 1] == null)
                    {
                        GameObject taillight = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        taillight.transform.parent = car.transform;

                        if (lightName != null)
                        {
                            taillight.gameObject.name = name;
                        }
                        else
                        {
                            taillight.gameObject.name = "Light";
                        }

                        GameObject taillightM = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        taillightM.transform.parent = car.transform;

                        mirrorPos = new Vector3(-taillight.transform.localPosition.x, taillight.transform.localPosition.y, taillight.transform.localPosition.z);
                        mirrorRot = Quaternion.Inverse(taillight.transform.localRotation);
                        Quaternion fixedRot = new Quaternion(taillight.transform.localRotation.x, mirrorRot.y, mirrorRot.z, mirrorRot.w);

                        taillightM.transform.localPosition = mirrorPos;
                        taillightM.transform.localRotation = fixedRot;

                        if (lightName != null)
                        {
                            taillightM.transform.name = name + " M";
                        }
                        else
                        {
                            taillightM.transform.name = "Light M";
                        }

                        light.type = LightType.Point;
                        light.range = 1f;
                        light.intensity = .2f;
                        light.color = lightColor;
                        GameObject taillightChild = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        taillightChild.transform.parent = taillight.transform;
                        taillightChild.gameObject.name = "TaillightChild";

                        GameObject taillightChildM = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        taillightChildM.transform.parent = taillightM.transform;
                        taillightChildM.transform.localPosition = Vector3.zero;
                        taillightChildM.transform.rotation = mirrorRot;
                        taillightChildM.gameObject.name = "TaillightChildM";

                        for (int x = 0; x < lights.Length; x++)
                        {
                            if (lights[x] == null && lights[x + 1] == null)
                            {
                                lights[x] = taillight;
                                strobe[x] = false;
                                lights[x + 1] = taillightM;
                                strobe[x + 1] = false;
                                break;
                            }
                        }
                        for (int z = 0; z < brakeLight.Length; z++)
                        {
                            if (brakeLight[z] == null && brakeLight[z + 1] == null)
                            {
                                brakeLight[z] = taillight;
                                brakeLight[z + 1] = taillightM;
                            }
                        }
                        break;
                    }
                }
            }
        }

        void AddSpotLight(Light light, string name)
        {
            lightColor = new Color(lightColorR, lightColorG, lightColorB);

            light.type = LightType.Spot;
            light.spotAngle = 90f;
            light.innerSpotAngle = 30f;
            light.range = lightRange;
            light.intensity = lightIntensity;
            light.color = lightColor;

            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i] == null)
                {
                    GameObject spotlight = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                    if (lightName != null)
                    {
                        spotlight.gameObject.name = name;
                    }
                    else
                    {
                        spotlight.gameObject.name = "Light";
                    }

                    light.type = LightType.Point;
                    light.range = 3f;
                    light.intensity = 2f;
                    light.color = lightColor;
                    GameObject spotlightChild = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                    spotlightChild.transform.parent = spotlight.transform;
                    spotlightChild.gameObject.name = "SpotlightChild";

                    for (int x = 0; x < lights.Length; x++)
                    {
                        if (lights[x] == null)
                        {
                            lights[x] = spotlight;
                            strobe[x] = false;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        void AddSpotLightOnCar(Light light, GameObject car, string name)
        {
            if (symmetry)
            {
                lightColor = new Color(lightColorR, lightColorG, lightColorB);

                light.type = LightType.Spot;
                light.spotAngle = 90f;
                light.innerSpotAngle = 30f;
                light.range = lightRange;
                light.intensity = lightIntensity;
                light.color = lightColor;

                for (int i = 0; i < lights.Length; i++)
                {
                    if (lights[i] == null && lights[i + 1] == null)
                    {
                        GameObject spotlight = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        spotlight.transform.parent = car.transform;
                        if (lightName != null)
                        {
                            spotlight.gameObject.name = name;
                        }
                        else
                        {
                            spotlight.gameObject.name = "Light";
                        }


                        GameObject spotlightM = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        spotlightM.transform.parent = car.transform;

                        mirrorPos = new Vector3(-spotlight.transform.localPosition.x, spotlight.transform.localPosition.y, spotlight.transform.localPosition.z);
                        mirrorRot = Quaternion.Inverse(spotlight.transform.localRotation);
                        Quaternion fixedRot = new Quaternion(spotlight.transform.localRotation.x, mirrorRot.y, mirrorRot.z, mirrorRot.w);

                        spotlightM.transform.localPosition = mirrorPos;
                        spotlightM.transform.localRotation = fixedRot;

                        if (lightName != null)
                        {
                            spotlightM.transform.name = name + " M";
                        }
                        else
                        {
                            spotlightM.transform.name = "Light M";
                        }

                        for (int x = 0; x < lights.Length; x++)
                        {
                            if (lights[x] == null && lights[x + 1] == null)
                            {
                                lights[x] = spotlight;
                                strobe[x] = false;
                                lights[x + 1] = spotlightM;
                                strobe[x + 1] = false;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                lightColor = new Color(lightColorR, lightColorG, lightColorB);

                light.type = LightType.Spot;
                light.spotAngle = 90f;
                light.innerSpotAngle = 30f;
                light.range = lightRange;
                light.intensity = lightIntensity;
                light.color = lightColor;

                for (int i = 0; i < lights.Length; i++)
                {
                    if (lights[i] == null)
                    {
                        GameObject spotlight = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        spotlight.transform.parent = car.transform;
                        if (lightName != null)
                        {
                            spotlight.gameObject.name = name;
                        }
                        else
                        {
                            spotlight.gameObject.name = "Light";
                        }

                        for (int x = 0; i < lights.Length; x++)
                        {
                            if (lights[x] == null)
                            {
                                lights[x] = spotlight;
                                strobe[x] = false;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        void AddPointLight(Light light, string name)
        {
            lightColor = new Color(lightColorR, lightColorG, lightColorB);
            light.type = LightType.Point;
            light.range = lightRange;
            light.intensity = lightIntensity;
            light.color = lightColor;

            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i] == null)
                {
                    GameObject pointlight = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                    if (lightName != null)
                    {
                        pointlight.gameObject.name = name;
                    }
                    else
                    {
                        pointlight.gameObject.name = "Light";
                    }

                    for (int x = 0; x < lights.Length; x++)
                    {
                        if (lights[x] == null)
                        {
                            lights[x] = pointlight;
                            strobe[x] = false;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        void AddPointLightOnCar(Light light, GameObject car, string name)
        {
            if (symmetry)
            {
                lightColor = new Color(lightColorR, lightColorG, lightColorB);
                light.type = LightType.Point;
                light.range = lightRange;
                light.intensity = lightIntensity;
                light.color = lightColor;

                for (int i = 0; i < lights.Length; i++)
                {
                    if (lights[i] == null && lights[i + 1] == null)
                    {
                        GameObject pointlight = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        pointlight.transform.parent = car.transform;
                        if (lightName != null)
                        {
                            pointlight.gameObject.name = name;
                        }
                        else
                        {
                            pointlight.gameObject.name = "Light";
                        }

                        GameObject pointlightM = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        pointlightM.transform.parent = car.transform;

                        mirrorPos = new Vector3(-pointlight.transform.localPosition.x, pointlight.transform.localPosition.y, pointlight.transform.localPosition.z);
                        mirrorRot = Quaternion.Inverse(pointlight.transform.localRotation);
                        Quaternion fixedRot = new Quaternion(pointlight.transform.localRotation.x, mirrorRot.y, mirrorRot.z, mirrorRot.w);

                        pointlightM.transform.localPosition = mirrorPos;
                        pointlightM.transform.localRotation = fixedRot;

                        if (lightName != null)
                        {
                            pointlightM.transform.name = name + " M";
                        }
                        else
                        {
                            pointlightM.transform.name = "Light M";
                        }

                        for (int x = 0; x < lights.Length; x++)
                        {
                            if (lights[x] == null && lights[x + 1] == null)
                            {
                                lights[x] = pointlight;
                                strobe[x] = false;
                                lights[x + 1] = pointlightM;
                                strobe[x + 1] = false;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                lightColor = new Color(lightColorR, lightColorG, lightColorB);
                light.type = LightType.Point;
                light.range = lightRange;
                light.intensity = lightIntensity;
                light.color = lightColor;

                for (int i = 0; i < lights.Length; i++)
                {
                    if (lights[i] == null)
                    {
                        GameObject pointlight = Instantiate(lightPrefab.gameObject, ctCam.transform.position, ctCam.transform.rotation);
                        pointlight.transform.parent = car.transform;
                        if (lightName != null)
                        {
                            pointlight.gameObject.name = name;
                        }
                        else
                        {
                            pointlight.gameObject.name = "Light";
                        }

                        for (int x = 0; x < lights.Length; x++)
                        {
                            if (lights[x] == null)
                            {
                                lights[x] = pointlight;
                                strobe[x] = false;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        void LightOptimizations()
        {
            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i] != null)
                {
                    if (ctEnabled)
                    {
                        if (ctCam != null)
                        {
                            if (Vector3.Distance(ctCam.transform.position, lights[i].transform.position) > 350f)
                            {
                                lights[i].GetComponent<Light>().enabled = false;
                            }
                            else
                            {
                                lights[i].GetComponent<Light>().enabled = true;
                            }
                        }
                    }
                    else
                    {
                        if (playersCar != null)
                        {
                            if (Vector3.Distance(playersCar.transform.position, lights[i].transform.position) > 150f)
                            {
                                lights[i].GetComponent<Light>().enabled = false;
                            }
                            else
                            {
                                lights[i].GetComponent<Light>().enabled = true;
                            }
                        }
                    }
                }
            }
        }

        void SyncStrobes()
        {
            for (int i = 0; i < strobeLights.Length; i++)
            {
                if (strobeLights[i] != null)
                {
                    if (strobe[i])
                    {
                        strobeSpeed[i] = strobeSpeedTemp;
                        strobeFor[i] = strobeForTemp;
                        strobeTimer[i] = 0f;
                    }
                }
            }
        }

        void StrobeAnimator()
        {
            for (int i = 0; i < strobeLights.Length; i++)
            {
                if (strobeLights[i] != null)
                {
                    if (strobe[i])
                    {
                        strobeTimer[i] += Time.deltaTime;
                        if (strobeTimer[i] > strobeSpeed[i])
                        {
                            strobeLights[i].SetActive(true);
                            strobeForTimer[i] += Time.deltaTime;
                            if (strobeForTimer[i] > strobeFor[i])
                            {
                                strobeLights[i].SetActive(false);
                                strobeTimer[i] = 0f;
                                strobeForTimer[i] = 0f;
                            }
                        }
                    }
                }
            }
        }

        void TeleportCameraToCar()
        {
            if (playersCar != null)
            {
                cameraPosition = playersCar.transform.position + Vector3.up * 2f;
                cameraRotation = playersCar.transform.rotation;
            }
            else
            {
                SearchForPlayersCar();
                cameraPosition = playersCar.transform.position + Vector3.up * 2f;
                cameraRotation = playersCar.transform.rotation;
            }
        }

        void TeleportCarToCamera()
        {
            if (playersCar != null)
            {
                playersCar.transform.position = ctCam.transform.position;
                playersCar.transform.rotation = ctCam.transform.rotation;
            }
            else
            {
                SearchForPlayersCar();
                playersCar.transform.position = ctCam.transform.position;
                playersCar.transform.rotation = ctCam.transform.rotation;
            }
        }

        void FovChanger()
        {
            if (ctCam.GetComponent<Camera>().fieldOfView != fov)
            {
                ctCam.GetComponent<Camera>().fieldOfView = fov;
            }
            if (Input.GetKey(keyBindKey[6]))
            {
                fov += fovFactor * Time.deltaTime;
            }
            else if (Input.GetKey(keyBindKey[7]))
            {
                fov -= fovFactor * Time.deltaTime;
            }
        }

        void CameraMovement()
        {
            if (animate != true)
            {
                if (customCamera == false)
                {
                    if (controlCamera)
                    {
                        if (movementResistance)
                        {
                            if (Input.GetKey(keyBindKey[0]))
                            {
                                zVal += 1f * Time.deltaTime;
                            }
                            else if (Input.GetKey(keyBindKey[1]))
                            {
                                zVal -= 1f * Time.deltaTime;
                            }
                            else
                            {
                                zVal = Mathf.Lerp(zVal, 0f, movementSpeed / 2f * Time.deltaTime);
                            }

                            zVal = Mathf.Clamp(zVal, -1f, 1f);

                            if (Input.GetKey(keyBindKey[2]))
                            {
                                xVal -= 1f * Time.deltaTime;
                            }
                            else if (Input.GetKey(keyBindKey[3]))
                            {
                                xVal += 1f * Time.deltaTime;
                            }
                            else
                            {
                                xVal = Mathf.Lerp(xVal, 0f, movementSpeed / 2f * Time.deltaTime);
                            }

                            xVal = Mathf.Clamp(xVal, -1f, 1f);

                            if (Input.GetKey(keyBindKey[4]))
                            {
                                yVal += 1f * Time.deltaTime;
                            }
                            else if (Input.GetKey(keyBindKey[5]))
                            {
                                yVal -= 1f * Time.deltaTime;
                            }
                            else
                            {
                                yVal = Mathf.Lerp(yVal, 0f, movementSpeed / 2f * Time.deltaTime);
                            }

                            yVal = Mathf.Clamp(yVal, -1f, 1f);

                            if (Input.GetKey(keyBindKey[8]))
                            {
                                tiltAngle += tiltFactor * Time.deltaTime;
                            }
                            else if (Input.GetKey(keyBindKey[9]))
                            {
                                tiltAngle -= tiltFactor * Time.deltaTime;
                            }

                        }
                        else //resistance false
                        {
                            if (Input.GetKey(keyBindKey[0]))
                            {
                                zVal += 1f * Time.deltaTime;
                            }
                            if (Input.GetKey(keyBindKey[1]))
                            {
                                zVal -= 1f * Time.deltaTime;
                            }
                            if (Input.GetKey(keyBindKey[2]))
                            {
                                xVal -= 1f * Time.deltaTime;
                            }
                            if (Input.GetKey(keyBindKey[3]))
                            {
                                xVal += 1f * Time.deltaTime;
                            }
                            if (Input.GetKey(keyBindKey[4]))
                            {
                                yVal += 1f * Time.deltaTime;
                            }
                            if (Input.GetKey(keyBindKey[5]))
                            {
                                yVal -= 1f * Time.deltaTime;
                            }
                            if (Input.GetKey(keyBindKey[8]))
                            {
                                tiltAngle += tiltFactor * Time.deltaTime;
                            }
                            else if (Input.GetKey(keyBindKey[9]))
                            {
                                tiltAngle -= tiltFactor * Time.deltaTime;
                            }
                        }

                    }

                    if (goProMode == false)
                    {
                        mount = false;
                        focusOnCar = false;

                        cameraPosition += ctCam.transform.forward * movementSpeed * zVal * Time.deltaTime + ctCam.transform.right * movementSpeed * xVal * Time.deltaTime + ctCam.transform.up * movementSpeed * yVal * Time.deltaTime;
                        ctCam.transform.position = cameraPosition;

                        if (controlCamera && showUi == false)
                        {
                            mouseX += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
                            mouseY += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
                        }

                        if (focusOnCar == true)
                        {
                            ctCam.transform.LookAt(lookingCar.transform);
                        }
                        else
                        {
                            if (smoothCamera)
                            {
                                ctCam.transform.rotation = Quaternion.Slerp(ctCam.transform.rotation, cameraRotation, smoothValue * Time.deltaTime);
                            }
                            else if (smoothCamera != true)
                            {
                                ctCam.transform.rotation = cameraRotation;
                            }
                        }

                        if (wobble)
                        {
                            smoothCamera = true;
                            randomizeTimer += Time.deltaTime;
                            if (randomizeTimer > randomizeEvery)
                            {
                                wobbleRot = Quaternion.Euler(Random.Range(-wobbleAmount, wobbleAmount), Random.Range(-wobbleAmount, wobbleAmount), Random.Range(-wobbleAmount, wobbleAmount));
                                randomizeTimer = 0f;
                            }
                            cameraRotation = Quaternion.Euler(new Vector3(-mouseY + wobbleRot.x, mouseX + wobbleRot.y, ctCam.transform.rotation.z + tiltAngle + wobbleRot.z));
                        }
                        else
                        {
                            cameraRotation = Quaternion.Euler(new Vector3(-mouseY, mouseX, ctCam.transform.rotation.z + tiltAngle));
                        }

                    }
                    else if (goProMode == true)
                    {

                        if (mount != true)
                        {
                            cameraPosition += ctCam.transform.forward * movementSpeed * zVal * Time.deltaTime + ctCam.transform.right * movementSpeed * xVal * Time.deltaTime + ctCam.transform.up * movementSpeed * yVal * Time.deltaTime;
                            ctCam.transform.position = cameraPosition;
                        }
                        if (focusOnCar == false)
                        {
                            if (controlCamera)
                            {
                                if (showUi != true)
                                {
                                    mouseX += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
                                    mouseY += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
                                }
                                if (smoothCamera)
                                {
                                    ctCam.transform.rotation = Quaternion.Slerp(ctCam.transform.rotation, cameraRotation, smoothValue * Time.deltaTime);
                                }
                                else if (smoothCamera != true)
                                {
                                    ctCam.transform.rotation = cameraRotation;
                                }
                                cameraRotation = Quaternion.Euler(new Vector3(-mouseY + ctCam.transform.parent.eulerAngles.x, mouseX + ctCam.transform.parent.eulerAngles.y, ctCam.transform.parent.eulerAngles.z + tiltAngle));
                            }
                        }
                        else if (focusOnCar)
                        {
                            if (lookingCar != null)
                            {
                                ctCam.transform.LookAt(lookingCar.transform);
                            }
                        }
                    }
                }
                else
                {
                    if (dynamicFollow)
                    {
                        customCameraPosition = playersCar.transform.position + Vector3.up * height + playersCar.transform.TransformDirection(Vector3.back * distance);
                        cameraPosition = Vector3.Lerp(cameraPosition, customCameraPosition, followSpeed * Time.deltaTime);
                        ctCam.transform.position = cameraPosition;
                        ctCam.transform.LookAt(playersCar.transform);
                    }
                    else
                    {
                        customCameraPosition = playersCar.transform.position + Vector3.up * height + playersCar.transform.TransformDirection(Vector3.back * distance);
                        cameraPosition = customCameraPosition;
                        ctCam.transform.position = cameraPosition;
                        ctCam.transform.LookAt(playersCar.transform);
                    }
                }
            }
            else
            {
                ctCam.transform.position = cameraPosition;
                ctCam.transform.rotation = cameraRotation;
            }
        }

        void CTCameraEnable()
        {
            if (ctEnabled)
            {
                mainCamera = Camera.main;
                Camera.main.gameObject.tag = "MainCamera";
                Camera.main.GetComponent<Camera>().enabled = false;

                AudioListener[] listeners = GameObject.FindObjectsOfType<AudioListener>();
                for (int i = 0; i < listeners.Length; i++)
                {
                    if (listeners[i] != null)
                    {
                        listeners[i].enabled = false;
                    }
                }

                if (ctCam == null)
                {
                    ctCam = new GameObject();
                    ctCam.AddComponent<Camera>();
                    ctCam.GetComponent<Camera>().nearClipPlane = 0.001f;
                    ctCam.AddComponent<AudioListener>();
                    ctCam.GetComponent<AudioListener>().enabled = true;
                    TeleportCameraToCar();
                }
                else
                {
                    ctCam.GetComponent<Camera>().enabled = true;
                    ctCam.AddComponent<AudioListener>();
                }

                if (playersCar == null)
                {
                    SearchForPlayersCar();
                }
            }
            else
            {
                try
                {
                    if (ctCam != null)
                    {
                        ctCam.GetComponent<Camera>().enabled = false;
                        Destroy(ctCam.GetComponent<AudioListener>());
                    }
                    GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
                    mainCamera.GetComponent<Camera>().enabled = true;
                    mainCamera.GetComponent<AudioListener>().enabled = true;
                }
                catch (Exception)
                {

                }
            }
        }

        void CameraAnimation()
        {
            if (animate)
            {
                Console.WriteLine(currentPosition);
                if (points[currentPosition] != null)
                {
                    cameraPosition = Vector3.MoveTowards(cameraPosition, points[currentPosition].transform.position, moveSpeed * Time.deltaTime);
                    Console.Write(cameraPosition);
                    cameraRotation = Quaternion.Slerp(cameraRotation, points[currentPosition].transform.rotation, rotationSpeed * Time.deltaTime);
                    Console.Write(cameraRotation);
                    if (fov != fovs[currentPosition])
                    {
                        fov = Mathf.Lerp(fov, fovs[currentPosition], fovSpeed * Time.deltaTime);
                    }
                    if (Vector3.Distance(ctCam.transform.position, points[currentPosition].transform.position) < 0.2f)
                    {
                        if (currentPosition == points.Length - 1)
                        {
                            currentPosition = 0;
                        }
                        else if (currentPosition < points.Length)
                        {
                            currentPosition++;
                        }
                    }
                }
                else
                {
                    if (currentPosition == points.Length - 1)
                    {
                        currentPosition = 0;
                    }
                    else if (currentPosition < points.Length)
                    {
                        currentPosition++;
                    }
                }
            }
            else
            {
                currentPosition = 0;
            }
        }

        void AddPoint()
        {
            Transform point = new GameObject().transform;
            Transform newPoint = Instantiate(point, ctCam.transform.position, ctCam.transform.rotation);
            Console.Write(newPoint.position);

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] == null)
                {
                    points[i] = newPoint.transform;
                    fovs[i] = fov;
                    break;
                }
            }
        }

        void GUIAnimations()
        {
            if (cameraUi)
            {
                buttonXPosCamera = Mathf.Lerp(buttonXPosCamera, buttonXPosOn, buttonAnimationSpeed * Time.deltaTime);
            }
            else
            {
                buttonXPosCamera = Mathf.Lerp(buttonXPosCamera, buttonXPosOff, buttonAnimationSpeed * Time.deltaTime);
            }

            if (carUi)
            {
                buttonXPosCar = Mathf.Lerp(buttonXPosCar, buttonXPosOn, buttonAnimationSpeed * Time.deltaTime);
            }
            else
            {
                buttonXPosCar = Mathf.Lerp(buttonXPosCar, buttonXPosOff, buttonAnimationSpeed * Time.deltaTime);
            }

            if (enviromentUi)
            {
                buttonXPosEnviroment = Mathf.Lerp(buttonXPosEnviroment, buttonXPosOn, buttonAnimationSpeed * Time.deltaTime);
            }
            else
            {
                buttonXPosEnviroment = Mathf.Lerp(buttonXPosEnviroment, buttonXPosOff, buttonAnimationSpeed * Time.deltaTime);
            }

            if (goProUi)
            {
                buttonXPosGoPro = Mathf.Lerp(buttonXPosGoPro, buttonXPosOn, buttonAnimationSpeed * Time.deltaTime);
            }
            else
            {
                buttonXPosGoPro = Mathf.Lerp(buttonXPosGoPro, buttonXPosOff, buttonAnimationSpeed * Time.deltaTime);
            }

            if (animationUi)
            {
                buttonXPosAnimation = Mathf.Lerp(buttonXPosAnimation, buttonXPosOn, buttonAnimationSpeed * Time.deltaTime);
            }
            else
            {
                buttonXPosAnimation = Mathf.Lerp(buttonXPosAnimation, buttonXPosOff, buttonAnimationSpeed * Time.deltaTime);
            }

            if (keyBindsUi)
            {
                buttonXPosKeyBinds = Mathf.Lerp(buttonXPosKeyBinds, buttonXPosOn, buttonAnimationSpeed * Time.deltaTime);
            }
            else
            {
                buttonXPosKeyBinds = Mathf.Lerp(buttonXPosKeyBinds, buttonXPosOff, buttonAnimationSpeed * Time.deltaTime);
            }

            if (settingsUi)
            {
                buttonXPosSettings = Mathf.Lerp(buttonXPosSettings, buttonXPosOn, buttonAnimationSpeed * Time.deltaTime);
            }
            else
            {
                buttonXPosSettings = Mathf.Lerp(buttonXPosSettings, buttonXPosOff, buttonAnimationSpeed * Time.deltaTime);
            }
        }

        private void OnGUI()
        {
            if (ctEnabled)
            {
                if (showUi)
                {
                    GUI.skin = skin.NavBarSkin;
                    GUI.Box(new Rect(20, 20, 40, 700), "");
                    GUI.skin = skin.NavBarShadowSkin;
                    GUI.Box(new Rect(30, 20, 30, 700), "");

                    //naav bar buttons
                    if (cameraUi)
                    {
                        GUI.skin = skin.CameraOnSkin;
                    }
                    else
                    {
                        GUI.skin = skin.CameraOffSkin;
                    }
                    if (GUI.Button(new Rect(buttonXPosCamera, 30, 60, 60), ""))
                    {
                        cameraUi = true;
                        carUi = false;
                        enviromentUi = false;
                        goProUi = false;
                        animationUi = false;
                        mapUi = false;
                        keyBindsUi = false;
                        settingsUi = false;
                    }

                    if (enviromentUi)
                    {
                        GUI.skin = skin.EnviromentOnSkin;
                    }
                    else
                    {
                        GUI.skin = skin.EnviromentOffSkin;
                    }
                    if (GUI.Button(new Rect(buttonXPosEnviroment, 80, 60, 60), ""))
                    {
                        cameraUi = false;
                        carUi = false;
                        enviromentUi = true;
                        goProUi = false;
                        animationUi = false;
                        mapUi = false;
                        keyBindsUi = false;
                        settingsUi = false;
                    }

                    if (goProUi)
                    {
                        GUI.skin = skin.GoProOnSkin;
                    }
                    else
                    {
                        GUI.skin = skin.GoProOffSkin;
                    }
                    if (GUI.Button(new Rect(buttonXPosGoPro, 140, 60, 60), ""))
                    {
                        cameraUi = false;
                        carUi = false;
                        enviromentUi = false;
                        goProUi = true;
                        animationUi = false;
                        mapUi = false;
                        keyBindsUi = false;
                        settingsUi = false;
                    }

                    if (animationUi)
                    {
                        GUI.skin = skin.AnimationOnSkin;
                    }
                    else
                    {
                        GUI.skin = skin.AnimationOffSkin;
                    }
                    if (GUI.Button(new Rect(buttonXPosAnimation, 200, 60, 60), ""))
                    {
                        cameraUi = false;
                        carUi = false;
                        enviromentUi = false;
                        goProUi = false;
                        animationUi = true;
                        mapUi = false;
                        keyBindsUi = false;
                        settingsUi = false;
                    }

                    if (keyBindsUi)
                    {
                        GUI.skin = skin.KeyBindsOnSkin;
                    }
                    else
                    {
                        GUI.skin = skin.KeyBindsOffSkin;
                    }
                    if (GUI.Button(new Rect(buttonXPosKeyBinds, 590, 60, 60), ""))
                    {
                        cameraUi = false;
                        carUi = false;
                        enviromentUi = false;
                        goProUi = false;
                        animationUi = false;
                        mapUi = false;
                        keyBindsUi = true;
                        settingsUi = false;
                    }

                    if (settingsUi)
                    {
                        GUI.skin = skin.SettingsOnSkin;
                    }
                    else
                    {
                        GUI.skin = skin.SettingsOffSkin;
                    }
                    if (GUI.Button(new Rect(buttonXPosSettings, 650, 60, 60), ""))
                    {
                        cameraUi = false;
                        carUi = false;
                        enviromentUi = false;
                        goProUi = false;
                        animationUi = false;
                        mapUi = false;
                        keyBindsUi = false;
                        settingsUi = true;
                    }

                    #region cameraui
                    if (cameraUi)
                    {
                        GUI.skin = skin.WindowSkin;
                        GUI.Box(new Rect(60, 20, 950, 700), "");

                        GUI.skin = skin.TextHeaderSkin;
                        GUI.Label(new Rect(490, 40, 500, 100), "CAMERA", skin.TextStyle);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 70, 300, 20), "Control The Camera", skin.TextStyle);
                        if (controlCamera)
                        {
                            GUI.skin = skin.CheckBoxCheckSkin;
                            if (GUI.Button(new Rect(300, 70, 20, 20), ""))
                            {
                                controlCamera = !controlCamera;
                            }
                        }
                        else
                        {
                            yVal = 0f;
                            xVal = 0f;
                            zVal = 0f;
                            GUI.skin = skin.CheckBoxUncheckSkin;
                            if (GUI.Button(new Rect(300, 70, 20, 20), ""))
                            {
                                customCamera = false;
                                controlCamera = !controlCamera;
                            }
                        }
                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 100, 300, 20), "Movement Resistance", skin.TextStyle);
                        if (movementResistance)
                        {
                            GUI.skin = skin.CheckBoxCheckSkin;
                            if (GUI.Button(new Rect(300, 100, 20, 20), ""))
                            {
                                movementResistance = !movementResistance;
                            }
                        }
                        else
                        {
                            GUI.skin = skin.CheckBoxUncheckSkin;
                            if (GUI.Button(new Rect(300, 100, 20, 20), ""))
                            {
                                movementResistance = !movementResistance;
                            }
                        }

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 130, 300, 20), "Camera Smoothing", skin.TextStyle);
                        if (smoothCamera)
                        {
                            GUI.skin = skin.CheckBoxCheckSkin;
                            if (GUI.Button(new Rect(300, 130, 20, 20), ""))
                            {
                                smoothCamera = !smoothCamera;
                            }
                        }
                        else
                        {
                            GUI.skin = skin.CheckBoxUncheckSkin;
                            if (GUI.Button(new Rect(300, 130, 20, 20), ""))
                            {
                                smoothCamera = !smoothCamera;
                            }
                        }

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 160, 300, 20), "Teleport To Car", skin.TextStyle);
                        GUI.skin = skin.CheckBoxUncheckSkin;
                        if (GUI.Button(new Rect(300, 160, 20, 20), ""))
                        {
                            TeleportCameraToCar();
                        }

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 190, 300, 20), "Custom Camera", skin.TextStyle);
                        if (customCamera)
                        {
                            GUI.skin = skin.CheckBoxCheckSkin;
                            if (GUI.Button(new Rect(300, 190, 20, 20), ""))
                            {
                                customCamera = !customCamera;
                            }
                        }
                        else
                        {
                            GUI.skin = skin.CheckBoxUncheckSkin;
                            if (GUI.Button(new Rect(300, 190, 20, 20), ""))
                            {
                                customCamera = !customCamera;
                            }
                        }

                        if (customCamera)
                        {
                            controlCamera = false;
                            GUI.skin = skin.TextSkin;
                            GUI.Label(new Rect(100, 220, 300, 20), "Dynamic Follow", skin.TextStyle);
                            if (dynamicFollow)
                            {
                                GUI.skin = skin.CheckBoxCheckSkin;
                                if (GUI.Button(new Rect(300, 220, 20, 20), ""))
                                {
                                    dynamicFollow = !dynamicFollow;
                                }
                            }
                            else
                            {
                                GUI.skin = skin.CheckBoxUncheckSkin;
                                if (GUI.Button(new Rect(300, 220, 20, 20), ""))
                                {
                                    dynamicFollow = !dynamicFollow;
                                }
                            }
                        }

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 250, 300, 20), "Camera Wobble", skin.TextStyle);
                        if (wobble)
                        {
                            GUI.skin = skin.CheckBoxCheckSkin;
                            if (GUI.Button(new Rect(300, 250, 20, 20), ""))
                            {
                                wobble = !wobble;
                            }
                        }
                        else
                        {
                            GUI.skin = skin.CheckBoxUncheckSkin;
                            if (GUI.Button(new Rect(300, 250, 20, 20), ""))
                            {
                                wobble = !wobble;
                            }
                        }

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 280, 300, 20), "Freeze Car", skin.TextStyle);
                        if (freezeCar)
                        {
                            GUI.skin = skin.CheckBoxCheckSkin;
                            if (GUI.Button(new Rect(300, 280, 20, 20), ""))
                            {
                                SetFreezeTransform();
                                freezeCar = !freezeCar;
                            }
                        }
                        else
                        {
                            GUI.skin = skin.CheckBoxUncheckSkin;
                            if (GUI.Button(new Rect(300, 280, 20, 20), ""))
                            {
                                SetFreezeTransform();
                                freezeCar = !freezeCar;
                            }
                        }

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 310, 300, 20), "Teleport To Camera", skin.TextStyle);
                        GUI.skin = skin.CheckBoxUncheckSkin;
                        if (GUI.Button(new Rect(300, 310, 20, 20), ""))
                        {
                            TeleportCarToCamera();
                        }

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(390, 70, 300, 20), "Movement Speed", skin.TextStyle);
                        GUI.Box(new Rect(570, 70, 40, 20), "");
                        GUI.Label(new Rect(570, 70, 40, 20), movementSpeed.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        movementSpeed = GUI.HorizontalSlider(new Rect(620, 70, 380, 13), movementSpeed, 0f, 100f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(390, 100, 300, 20), "Mouse Sensitivity", skin.TextStyle);
                        GUI.Box(new Rect(570, 100, 40, 20), "");
                        GUI.Label(new Rect(570, 100, 40, 20), mouseSensitivity.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        mouseSensitivity = GUI.HorizontalSlider(new Rect(620, 100, 380, 13), mouseSensitivity, 0f, 10f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(390, 130, 300, 20), "Camera Smoothness", skin.TextStyle);
                        GUI.Box(new Rect(570, 130, 40, 20), "");
                        GUI.Label(new Rect(570, 130, 40, 20), smoothValue.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        smoothValue = GUI.HorizontalSlider(new Rect(620, 130, 380, 13), smoothValue, 0f, 10f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(390, 160, 300, 20), "Camera FOV", skin.TextStyle);
                        GUI.Box(new Rect(570, 160, 40, 20), "");
                        GUI.Label(new Rect(570, 160, 40, 20), fov.ToString("0.0"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        fov = GUI.HorizontalSlider(new Rect(620, 160, 380, 13), fov, 10f, 150f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(390, 190, 300, 20), "FOV Change Speed", skin.TextStyle);
                        GUI.Box(new Rect(570, 190, 40, 20), "");
                        GUI.Label(new Rect(576, 190, 40, 20), fovFactor.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        fovFactor = GUI.HorizontalSlider(new Rect(620, 190, 380, 13), fovFactor, 0f, 100f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(390, 220, 300, 20), "Tilt Angle", skin.TextStyle);
                        GUI.Box(new Rect(565, 220, 45, 20), "");
                        GUI.Label(new Rect(570, 220, 40, 20), tiltAngle.ToString("0"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        tiltAngle = GUI.HorizontalSlider(new Rect(620, 220, 380, 13), tiltAngle, -180f, 180f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(390, 250, 300, 20), "Tilt Factor", skin.TextStyle);
                        GUI.Box(new Rect(570, 250, 40, 20), "");
                        GUI.Label(new Rect(570, 250, 40, 20), tiltFactor.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        tiltFactor = GUI.HorizontalSlider(new Rect(620, 250, 380, 13), tiltFactor, 0, 100);

                        if (customCamera)
                        {
                            GUI.skin = skin.TextSkin;
                            GUI.Label(new Rect(390, 280, 300, 20), "Follow Distance", skin.TextStyle);
                            GUI.Box(new Rect(570, 280, 40, 20), "");
                            GUI.Label(new Rect(570, 280, 40, 20), distance.ToString("0.00"), skin.TextStyle);
                            GUI.skin = skin.SliderSkin;
                            distance = GUI.HorizontalSlider(new Rect(620, 280, 380, 13), distance, 1f, 10f);

                            GUI.skin = skin.TextSkin;
                            GUI.Label(new Rect(390, 310, 300, 20), "Follow Height", skin.TextStyle);
                            GUI.Box(new Rect(570, 310, 40, 20), "");
                            GUI.Label(new Rect(570, 310, 40, 20), height.ToString("0.00"), skin.TextStyle);
                            GUI.skin = skin.SliderSkin;
                            height = GUI.HorizontalSlider(new Rect(620, 310, 380, 13), height, 0.5f, 15f);

                            if (dynamicFollow)
                            {
                                GUI.skin = skin.TextSkin;
                                GUI.Label(new Rect(390, 340, 300, 20), "Follow Speed", skin.TextStyle);
                                GUI.Box(new Rect(570, 340, 40, 20), "");
                                GUI.Label(new Rect(570, 340, 40, 20), followSpeed.ToString("0.00"), skin.TextStyle);
                                GUI.skin = skin.SliderSkin;
                                followSpeed = GUI.HorizontalSlider(new Rect(620, 340, 380, 13), followSpeed, 0.5f, 20f);
                            }
                        }

                        if (wobble)
                        {
                            GUI.skin = skin.TextSkin;
                            GUI.Label(new Rect(390, 370, 300, 20), "Wobble Amount", skin.TextStyle);
                            GUI.Box(new Rect(570, 370, 40, 20), "");
                            GUI.Label(new Rect(570, 370, 40, 20), wobbleAmount.ToString("0.00"), skin.TextStyle);
                            GUI.skin = skin.SliderSkin;
                            wobbleAmount = GUI.HorizontalSlider(new Rect(620, 370, 380, 13), wobbleAmount, 0f, 100f);

                            GUI.skin = skin.TextSkin;
                            GUI.Label(new Rect(390, 400, 300, 20), "Wobble Generate Speed", skin.TextStyle);
                            GUI.Box(new Rect(570, 400, 40, 20), "");
                            GUI.Label(new Rect(570, 400, 40, 20), randomizeEvery.ToString("0.00"), skin.TextStyle);
                            GUI.skin = skin.SliderSkin;
                            randomizeEvery = GUI.HorizontalSlider(new Rect(620, 400, 380, 13), randomizeEvery, 0f, 1f);
                        }
                    }
                    #endregion

                    #region enviromentui
                    if (enviromentUi)
                    {
                        GUI.skin = skin.WindowSkin;
                        GUI.Box(new Rect(60, 20, 950, 700), "");

                        GUI.skin = skin.TextHeaderSkin;
                        GUI.Label(new Rect(490, 40, 500, 100), "LIGHTS", skin.TextStyle);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(850, 50, 500, 20), "Name", skin.TextStyle);
                        GUI.skin = skin.TextFieldSkin;
                        lightName = GUI.TextField(new Rect(780, 70, 190, 20), lightName);

                        GUI.skin = skin.ButtonSkin;
                        if (symmetry)
                        {
                            if (GUI.Button(new Rect(780, 100, 90, 20), "Headlights"))
                            {
                                AddHeadlight(lightPrefab.GetComponent<Light>(), playersCar.gameObject, lightName);
                            }
                            if (GUI.Button(new Rect(780, 130, 90, 20), "Taillights"))
                            {
                                AddTaillight(lightPrefab.GetComponent<Light>(), playersCar.gameObject, lightName);
                            }
                            if (GUI.Button(new Rect(780, 160, 90, 20), "Spotlights Car"))
                            {
                                AddSpotLightOnCar(lightPrefab.GetComponent<Light>(), playersCar.gameObject, lightName);
                            }
                            if (GUI.Button(new Rect(780, 190, 90, 20), "Pointlights Car"))
                            {
                                AddPointLightOnCar(lightPrefab.GetComponent<Light>(), playersCar.gameObject, lightName);
                            }
                        }
                        else
                        {
                            if (GUI.Button(new Rect(780, 100, 90, 20), "Headlight"))
                            {
                                AddHeadlight(lightPrefab.GetComponent<Light>(), playersCar.gameObject, lightName);
                            }
                            if (GUI.Button(new Rect(780, 130, 90, 20), "Spotlight"))
                            {
                                AddSpotLight(lightPrefab.GetComponent<Light>(), lightName);
                            }
                            if (GUI.Button(new Rect(780, 160, 90, 20), "Spotlight Car"))
                            {
                                AddSpotLightOnCar(lightPrefab.GetComponent<Light>(), playersCar.gameObject, lightName);
                            }
                            if (GUI.Button(new Rect(780, 190, 90, 20), "Pointlight"))
                            {
                                AddPointLight(lightPrefab.GetComponent<Light>(), lightName);
                            }
                            if (GUI.Button(new Rect(780, 220, 90, 20), "Pointlight Car"))
                            {
                                AddPointLightOnCar(lightPrefab.GetComponent<Light>(), playersCar.gameObject, lightName);
                            }
                        }

                        if (symmetry)
                        {
                            if (GUI.Button(new Rect(880, 100, 90, 20), "Symmetry Off"))
                            {
                                symmetry = !symmetry;
                            }
                        }
                        else
                        {
                            if (GUI.Button(new Rect(880, 100, 90, 20), "Symmetry"))
                            {
                                symmetry = !symmetry;
                            }
                        }

                        if (GUI.Button(new Rect(880, 130, 90, 20), "Darker Mode"))
                        {
                            NightMode();
                        }

                        if (GUI.Button(new Rect(880, 160, 90, 20), "Sync Strobes"))
                        {
                            SyncStrobes();
                        }

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 70, 300, 20), "Intensity: " + lightIntensity.ToString("0"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        lightIntensity = GUI.HorizontalSlider(new Rect(250, 70, 500, 13), lightIntensity, 1f, 15000f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 100, 300, 20), "Range: " + lightRange.ToString("0"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        lightRange = GUI.HorizontalSlider(new Rect(250, 100, 500, 13), lightRange, 1f, 500f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 130, 40, 20), "Red Value: " + lightColorR.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        lightColorR = GUI.HorizontalSlider(new Rect(250, 130, 500, 13), lightColorR, 0f, 1f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 160, 40, 20), "Green Value: " + lightColorG.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        lightColorG = GUI.HorizontalSlider(new Rect(250, 160, 500, 13), lightColorG, 0f, 1f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 190, 40, 20), "Blue Value: " + lightColorB.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        lightColorB = GUI.HorizontalSlider(new Rect(250, 190, 500, 13), lightColorB, 0f, 1f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 220, 40, 20), "Strobe Speed: " + strobeSpeedTemp.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        strobeSpeedTemp = GUI.HorizontalSlider(new Rect(250, 220, 500, 13), strobeSpeedTemp, 0f, 5f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 250, 40, 20), "Strobe For: " + strobeForTemp.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        strobeForTemp = GUI.HorizontalSlider(new Rect(250, 250, 500, 13), strobeForTemp, 0f, 5f);

                        lightsInScene = GameObject.FindObjectsOfType<Light>();

                        GUI.skin = skin.ScrollBarSkin;
                        scrollLights = GUI.BeginScrollView(new Rect(100, 280, 900, 430), scrollLights, new Rect(0, 0, 0, 30 * lightsInScene.Length));

                        for (int i = 0; i < lights.Length; i++)
                        {
                            if (lights[i] != null)
                            {
                                float temp = 30 * i;
                                GUI.skin = skin.TextSkin;
                                GUI.Label(new Rect(0, temp, 300, 20), lights[i].name, skin.TextStyle);
                                GUI.skin = skin.ButtonSkin;
                                if (GUI.Button(new Rect(150, temp, 90, 20), "Delete"))
                                {
                                    Destroy(lights[i].gameObject);
                                }

                                if (GUI.Button(new Rect(250, temp, 90, 20), "Reposition"))
                                {
                                    lights[i].transform.position = ctCam.transform.position;
                                    lights[i].transform.rotation = ctCam.transform.rotation;
                                    Console.WriteLine(lights[i].transform.position);
                                    Console.WriteLine(lights[i].transform.localPosition);
                                }

                                if (GUI.Button(new Rect(350, temp, 90, 20), "Edit Light"))
                                {
                                    lightColor = new Color(lightColorR, lightColorG, lightColorB);
                                    lights[i].GetComponent<Light>().color = lightColor;
                                    lights[i].GetComponent<Light>().intensity = lightIntensity;
                                    lights[i].GetComponent<Light>().range = lightRange;
                                }

                                if (strobe[i])
                                {
                                    if (GUI.Button(new Rect(450, temp, 90, 20), "Don't Strobe"))
                                    {
                                        strobe[i] = false;
                                        strobeLights[i] = null;
                                    }
                                }
                                else
                                {
                                    if (GUI.Button(new Rect(450, temp, 90, 20), "Strobe"))
                                    {
                                        strobe[i] = true;
                                        strobeLights[i] = lights[i];
                                        strobeSpeed[i] = strobeSpeedTemp;
                                        strobeFor[i] = strobeForTemp;
                                        strobeTimer[i] = 0f;
                                    }
                                }

                                if (GUI.Button(new Rect(550, temp, 90, 20), "Rename"))
                                {
                                    lights[i].name = lightName;
                                }

                                if (GUI.Button(new Rect(650, temp, 90, 20), "Load Settings"))
                                {
                                    lightIntensity = lights[i].GetComponent<Light>().intensity;
                                    lightRange = lights[i].GetComponent<Light>().range;
                                    lightColorR = lights[i].GetComponent<Light>().color.r;
                                    lightColorG = lights[i].GetComponent<Light>().color.g;
                                    lightColorB = lights[i].GetComponent<Light>().color.b;
                                }
                            }
                        }

                        GUI.EndScrollView();
                    }
                    #endregion

                    #region goProUi
                    if (goProUi)
                    {
                        GUI.skin = skin.WindowSkin;
                        GUI.Box(new Rect(60, 20, 950, 700), "");

                        GUI.skin = skin.TextHeaderSkin;
                        GUI.Label(new Rect(490, 40, 500, 100), "GOPRO", skin.TextStyle);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 70, 300, 20), "GoPro Mode", skin.TextStyle);
                        if (goProMode)
                        {
                            GUI.skin = skin.CheckBoxCheckSkin;
                            if (GUI.Button(new Rect(300, 70, 20, 20), ""))
                            {
                                goProMode = !goProMode;
                            }
                        }
                        else
                        {
                            GUI.skin = skin.CheckBoxUncheckSkin;
                            if (GUI.Button(new Rect(300, 70, 20, 20), ""))
                            {
                                goProMode = !goProMode;
                            }
                        }

                        //Car list
                        if (cars.Length > 0)
                        {
                            for (int i = 0; i < cars.Length; i++)
                            {
                                float temp = 30 * i;
                                GUI.skin = skin.TextSkin;
                                GUI.Label(new Rect(390, 70 + temp, 300, 20), cars[i].name, skin.TextStyle);

                                if (goProMode)
                                {
                                    GUI.skin = skin.ButtonSkin;
                                    if (mountCar == cars[i])
                                    {
                                        if (GUI.Button(new Rect(620, 70 + temp, 70, 20), "Mounted"))
                                        {
                                            mountCar = null;
                                            mount = false;
                                            ctCam.transform.parent = transform;
                                        }
                                    }
                                    else
                                    {
                                        if (GUI.Button(new Rect(620, 70 + temp, 70, 20), "Mount"))
                                        {
                                            mountCar = cars[i];
                                            mount = true;
                                            ctCam.transform.parent = mountCar.transform;
                                        }
                                    }

                                    if (lookingCar == cars[i])
                                    {
                                        if (GUI.Button(new Rect(710, 70 + temp, 70, 20), "Focused"))
                                        {
                                            lookingCar = null;
                                            focusOnCar = false;
                                        }
                                    }
                                    else
                                    {
                                        if (GUI.Button(new Rect(710, 70 + temp, 70, 20), "Focus"))
                                        {
                                            lookingCar = cars[i];
                                            focusOnCar = true;
                                        }
                                    }
                                }

                                GUI.skin = skin.ButtonSkin;
                                if (GUI.Button(new Rect(800, 70 + temp, 70, 20), "Teleport"))
                                {
                                    cameraPosition = cars[i].transform.position;
                                    ctCam.transform.position = cars[i].transform.position + new Vector3( 15, 15, 0);
                                }

                                GUI.skin = skin.ButtonSkin;
                                if (GUI.Button(new Rect(890, 70 + temp, 70, 20), "Headlight"))
                                {
                                    AddHeadlight(lightPrefab.GetComponent<Light>(), cars[i].gameObject, lightName);
                                }
                            }
                        }
                    }
                    #endregion

                    #region animationUi
                    if (animationUi)
                    {
                        GUI.skin = skin.WindowSkin;
                        GUI.Box(new Rect(60, 20, 950, 700), "");

                        GUI.skin = skin.TextHeaderSkin;
                        GUI.Label(new Rect(490, 40, 500, 100), "ANIMATION", skin.TextStyle);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 70, 300, 20), "Animate", skin.TextStyle);
                        if (animate)
                        {
                            controlCamera = false;
                            customCamera = false;
                            GUI.skin = skin.CheckBoxCheckSkin;
                            if (GUI.Button(new Rect(300, 70, 20, 20), ""))
                            {
                                animate = !animate;
                            }
                        }
                        else
                        {
                            GUI.skin = skin.CheckBoxUncheckSkin;
                            if (GUI.Button(new Rect(300, 70, 20, 20), ""))
                            {
                                animate = !animate;
                            }
                        }

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 100, 300, 20), "Add Point", skin.TextStyle);
                        GUI.skin = skin.ButtonSkin;
                        if (GUI.Button(new Rect(250, 100, 120, 20), "Add"))
                        {
                            AddPoint();
                        }

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(390, 70, 300, 20), "Movement Speed", skin.TextStyle);
                        GUI.Box(new Rect(570, 70, 40, 20), "");
                        GUI.Label(new Rect(570, 70, 40, 20), moveSpeed.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        moveSpeed = GUI.HorizontalSlider(new Rect(620, 70, 380, 13), moveSpeed, 0f, 20f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(390, 100, 300, 20), "Rotation Speed", skin.TextStyle);
                        GUI.Box(new Rect(570, 100, 40, 20), "");
                        GUI.Label(new Rect(570, 100, 40, 20), rotationSpeed.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        rotationSpeed = GUI.HorizontalSlider(new Rect(620, 100, 380, 13), rotationSpeed, 0f, 20f);

                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(390, 130, 300, 20), "Fov Speed", skin.TextStyle);
                        GUI.Box(new Rect(570, 130, 40, 20), "");
                        GUI.Label(new Rect(570, 130, 40, 20), fovSpeed.ToString("0.00"), skin.TextStyle);
                        GUI.skin = skin.SliderSkin;
                        fovSpeed = GUI.HorizontalSlider(new Rect(620, 130, 380, 13), fovSpeed, 0f, 20f);

                        for (int i = 0; i < points.Length; i++)
                        {
                            if (points[i] != null)
                            {
                                float temp = 30 * i;
                                GUI.skin = skin.TextSkin;
                                GUI.Label(new Rect(390, 160 + temp, 300, 20), points[i].name, skin.TextStyle);

                                GUI.skin = skin.ButtonSkin;
                                if (GUI.Button(new Rect(619, 160 + temp, 80, 20), "Delete"))
                                {
                                    Destroy(points[i].gameObject);
                                }

                                GUI.skin = skin.ButtonSkin;
                                if (GUI.Button(new Rect(719, 160 + temp, 80, 20), "Preview"))
                                {
                                    cameraPosition = points[i].transform.position;
                                    cameraRotation = points[i].transform.rotation;

                                    ctCam.transform.position = cameraPosition;
                                    ctCam.transform.rotation = cameraRotation;
                                }

                                GUI.skin = skin.ButtonSkin;
                                if (GUI.Button(new Rect(819, 160 + temp, 80, 20), "Reposition"))
                                {
                                    points[i] = ctCam.transform;
                                }
                            }
                        }
                    }
                    #endregion

                    #region keyBindsUi
                    if (keyBindsUi)
                    {
                        Event inputKey = Event.current;
                        GUI.skin = skin.WindowSkin;
                        GUI.Box(new Rect(60, 20, 950, 700), "");

                        GUI.skin = skin.TextHeaderSkin;
                        GUI.Label(new Rect(490, 40, 500, 100), "KEY BINDS", skin.TextStyle);

                        GUI.skin = skin.ScrollBarSkin;
                        scrollKeyBinds = GUI.BeginScrollView(new Rect(100, 60, 900, 650), scrollKeyBinds, new Rect(0, 0, 0, 30 * keyBinds));

                        for (int i = 0; i < keyBinds; i++)
                        {
                            float temp = 30 * i;
                            GUI.skin = skin.TextSkin;
                            GUI.Label(new Rect(0, temp, 300, 20), keyBindName[i], skin.TextStyle);
                            GUI.skin = skin.ButtonSkin;
                            GUI.Label(new Rect(300, temp, 300, 20), "Current Key: " + keyBindKey[i].ToString(), skin.TextStyle);

                            if (editingKey[i] != true)
                            {
                                if (GUI.Button(new Rect(600, temp, 100, 20), "Bind"))
                                {
                                    editingKey[i] = true;
                                }
                            }
                            else
                            {
                                if (GUI.Button(new Rect(600, temp, 100, 20), "Save"))
                                {
                                    editingKey[i] = false;
                                }
                            }

                            if (editingKey[i])
                            {
                                if (inputKey.keyCode != enableCTKey && inputKey.keyCode != enableUIKey)
                                {
                                    keyBindKey[i] = inputKey.keyCode;
                                    if (Input.GetKeyUp(keyBindKey[i]))
                                    {
                                        editingKey[i] = false;
                                    }
                                }
                            }

                            if (GUI.Button(new Rect(710, temp, 100, 20), "Unbind"))
                            {
                                keyBindKey[i] = KeyCode.None;
                            }
                        }

                        GUI.EndScrollView();
                    }
                    #endregion

                    #region settingsUi
                    if (settingsUi)
                    {
                        GUI.skin = skin.WindowSkin;
                        GUI.Box(new Rect(60, 20, 950, 700), "");
                        GUI.skin = skin.TextHeaderSkin;
                        GUI.Label(new Rect(100, 40, 800, 40), "Revive Mod", skin.TextStyle);
                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 60, 800, 40), "Developer: Karasoulas / Zi9", skin.TextStyle);
                        GUI.skin = skin.TextHeaderSkin;
                        GUI.Label(new Rect(100, 90, 800, 40), "Credits", skin.TextStyle);
                        GUI.skin = skin.TextSkin;
                        GUI.Label(new Rect(100, 110, 800, 40), "dre raphael: Supported the project.", skin.TextStyle);
                        GUI.Label(new Rect(100, 150, 800, 40), "Thanks everyone who bought the V2.", skin.TextStyle);
                        GUI.Label(new Rect(100, 170, 800, 40), "Thanks everyone who bought the V3.", skin.TextStyle);
                        GUI.Label(new Rect(100, 190, 800, 40), "Big thanks to Bubu and diip.", skin.TextStyle);
                        GUI.Label(new Rect(100, 210, 800, 40), "Big thanks to Zi9 for allowing me (Sad) to update this mod!", skin.TextStyle);
                        GUI.Label(new Rect(100, 230, 800, 40), "Big thanks to Cynical for the updated textures/dark mode!", skin.TextStyle);
                    }
                    #endregion
                }
            }
        }
    }
}
