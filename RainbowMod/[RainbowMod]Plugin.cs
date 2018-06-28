using System;
using System.Collections;
using System.Linq;
using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xft;

namespace RainbowMod
{
    public class RainbowMod : IPlugin
    {
 
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
                return "1.0";
            }
        }

        public void OnApplicationStart()
        {
            this._randomColor = ScriptableObject.CreateInstance<ColorRandom>();
            SceneManager.activeSceneChanged += this.SceneManagerOnActiveSceneChanged;
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
            foreach (LightSwitchEventEffect obj in this._lightSwitches)
            {
                ReflectionUtil.SetPrivateField(obj, "_lightColor0", this._randomColor);
                ReflectionUtil.SetPrivateField(obj, "_lightColor1", this._randomColor);
                ReflectionUtil.SetPrivateField(obj, "_highlightColor0", this._randomColor);
                ReflectionUtil.SetPrivateField(obj, "_highlightColor1", this._randomColor);
            }
            Material[] source = Resources.FindObjectsOfTypeAll<Material>();
            this._blueSaberMat = source.FirstOrDefault((Material x) => x.name == "BlueSaber");
            this._redSaberMat = source.FirstOrDefault((Material x) => x.name == "RedSaber");
            this.weapontrails = UnityEngine.Object.FindObjectsOfType<XWeaponTrail>();
            this.score = UnityEngine.Object.FindObjectOfType<ScoreController>();
            bool flag = this.score != null;
            if (flag)
            {
                this.score.noteWasCutEvent += this.Notehit;
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
                this._redSaberMat.SetColor("_Color", a * (notecut.swingRating * 4f));
                this._redSaberMat.SetColor("_EmissionColor", a * (notecut.swingRating * 4f));
                bool flag2 = !notecut.allIsOK;
                if (flag2)
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
                    this._blueSaberMat.SetColor("_Color", a * (notecut.swingRating * 4f));
                    this._blueSaberMat.SetColor("_EmissionColor", a * (notecut.swingRating * 4f));
                    bool flag4 = !notecut.allIsOK;
                    if (flag4)
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
            foreach (Saber saber in this.sabers)
            {
                bool flag = saber.saberType == Saber.SaberType.SaberA;
                if (flag)
                {
                }
                bool flag2 = saber.saberType == Saber.SaberType.SaberB;
                if (flag2)
                {
                }
            }
            foreach (XWeaponTrail obj in this.weapontrails)
            {
                ReflectionUtil.SetPrivateField(obj, "MyColor", new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
            }
        }

        private LightSwitchEventEffect[] _lightSwitches;

        private ColorRandom _randomColor;

        private SimpleColorSO[] colors;

        private Saber[] sabers;

        private Material _blueSaberMat;

        private Material _redSaberMat;

        private XWeaponTrail[] weapontrails;

        private ScoreController score;

        private Saber.SaberType currentsab;

        private NoteCutInfo notedat;
    }
}
