using System;
using System.Collections;
using System.IO;
using System.Linq;
using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xft;

namespace RainbowMod
{
    public class Plugin : IPlugin
    {
        public static readonly Ini Ini = new Ini(Path.Combine(Environment.CurrentDirectory, "rainbow.cfg"));
        public string Name
        {
            get
            {
                return "Rainbow Mod";
            }
        }

 
        public string Version
        {
            get
            {
                return "2.0";
            }
        }

        public bool bg;
        public bool saber;
        public bool trail;
        public bool miss;

        public void OnApplicationStart()
        {
            this._randomColor = ScriptableObject.CreateInstance<ColorRandom>();
            SceneManager.activeSceneChanged += this.SceneManagerOnActiveSceneChanged;
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "rainbow.cfg")))
            {
                Ini.WriteValue("rainbow-background", "true");
                Ini.WriteValue("rainbow-saber", "true");
                Ini.WriteValue("rainbow-trail", "true");
                Ini.WriteValue("black-on-miss", "true");
                Ini.Save();
            }
            Ini.Load();
            bg = Ini.GetValue("rainbow-background", "", "true") == "true";
            saber = Ini.GetValue("rainbow-saber", "", "true") == "true";
            trail = Ini.GetValue("rainbow-trail", "", "true") == "true";
            miss = Ini.GetValue("black-on-miss", "", "true") == "true";
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= this.SceneManagerOnActiveSceneChanged;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            this.colors = Resources.FindObjectsOfTypeAll<SimpleColorSO>();
            this._lightSwitches = UnityEngine.Object.FindObjectsOfType<LightSwitchEventEffect>();
            this.sabers = UnityEngine.Object.FindObjectsOfType<Saber>();
            if (bg)
            {
                foreach (LightSwitchEventEffect obj in this._lightSwitches)
                {
                    ReflectionUtil.SetPrivateField(obj, "_lightColor0", this._randomColor);
                    ReflectionUtil.SetPrivateField(obj, "_lightColor1", this._randomColor);
                    ReflectionUtil.SetPrivateField(obj, "_highlightColor0", this._randomColor);
                    ReflectionUtil.SetPrivateField(obj, "_highlightColor1", this._randomColor);
                }
            }
            Material[] source = Resources.FindObjectsOfTypeAll<Material>();
            this._blueSaberMat = source.FirstOrDefault((Material x) => x.name == "BlueSaber");
            this.blueColor = this._blueSaberMat.color;
            this._redSaberMat = source.FirstOrDefault((Material x) => x.name == "RedSaber");
            this.redColor = this._redSaberMat.color;
            this.weapontrails = UnityEngine.Object.FindObjectsOfType<XWeaponTrail>();
            this.score = UnityEngine.Object.FindObjectOfType<ScoreController>();
            bool flag = this.score != null;
            if (flag)
            {
                this.score.noteWasCutEvent += this.Notehit;
                if(miss)
                    this.score.noteWasMissedEvent += this.notemiss;
            }
        }

        private IEnumerator waittilltime(float sabertype)
        {
            yield return new WaitUntil(() => this.notedat.afterCutSwingRatingCounter.didFinish);
            yield break;
        }

        public void Notehit(NoteData data, NoteCutInfo notecut, int h)
        {
            Color a = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            bool flag = data.noteType == NoteData.NoteType.NoteA;
            if (flag)
            {
                if (saber)
                {
                    this._redSaberMat.SetColor("_Color", a * (notecut.swingRating * 4f));
                    this._redSaberMat.SetColor("_EmissionColor", a * (notecut.swingRating * 4f));
                } else
                {
                    this._redSaberMat.SetColor("_Color", this.redColor);
                    this._redSaberMat.SetColor("_EmissionColor", this.redColor);
                }
                bool flag2 = !notecut.allIsOK;
                if (flag2 && miss)
                {
                    this._redSaberMat.SetColor("_Color", new Color(0f, 0f, 0f, 0f));
                    this._redSaberMat.SetColor("_EmissionColor", new Color(0f, 0f, 0f, 0f));
                }
            }
            else
            {
                bool flag3 = data.noteType == NoteData.NoteType.NoteB;
                if (flag3)
                {
                    if (saber)
                    {
                        this._blueSaberMat.SetColor("_Color", a * (notecut.swingRating * 4f));
                        this._blueSaberMat.SetColor("_EmissionColor", a * (notecut.swingRating * 4f));
                    }
                    else
                    {
                        this._blueSaberMat.SetColor("_Color", this.blueColor);
                        this._blueSaberMat.SetColor("_EmissionColor", this.blueColor);
                    }
                    bool flag4 = !notecut.allIsOK;
                    if (flag4 && miss)
                    {
                        this._blueSaberMat.SetColor("_Color", new Color(0f, 0f, 0f, 0f));
                        this._blueSaberMat.SetColor("_EmissionColor", new Color(0f, 0f, 0f, 0f));
                    }
                }
            }
        }

        public void notemiss(NoteData data, int h)
        {
            bool flag = data.noteType == NoteData.NoteType.NoteA;
            if (flag)
            {
                this._redSaberMat.SetColor("_Color", new Color(0f, 0f, 0f, 0f));
                this._redSaberMat.SetColor("_EmissionColor", new Color(0f, 0f, 0f, 0f));
            }
            else
            {
                bool flag2 = data.noteType == NoteData.NoteType.NoteB;
                if (flag2)
                {
                    this._blueSaberMat.SetColor("_Color", new Color(0f, 0f, 0f, 0f));
                    this._blueSaberMat.SetColor("_EmissionColor", new Color(0f, 0f, 0f, 0f));
                }
            }
        }

        public void OnFixedUpdate()
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnUpdate()
        {
            if (trail)
            {
                foreach (XWeaponTrail obj in this.weapontrails)
                {
                    ReflectionUtil.SetPrivateField(obj, "MyColor", new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
                }
            }
        }

        private LightSwitchEventEffect[] _lightSwitches;

        private ColorRandom _randomColor;

        private SimpleColorSO[] colors;

        private Saber[] sabers;

        private Material _blueSaberMat;

        private Color blueColor;

        private Material _redSaberMat;

        private Color redColor;

        private XWeaponTrail[] weapontrails;

        private ScoreController score;

        private Saber.SaberType currentsab;

        private NoteCutInfo notedat;
    }
}
