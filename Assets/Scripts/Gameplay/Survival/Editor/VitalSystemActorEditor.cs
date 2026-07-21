using UnityEngine;
using UnityEditor;
using Arctic.Gameplay.Survival.Actors;
using Arctic.Foundation.Editor;

namespace Arctic.Gameplay.Survival.CustomEditors
{
    [CustomEditor(typeof(VitalSystemActor))]
    internal sealed class VitalSystemActorEditor : Editor
    {
        private VitalSystemActor _actor;

        private static bool _ShowUtils = false;

        private float _healthStarveDecayDurMins = 3f;
        private float _healthDehydDecayDurMins = 2f;
        private float _healthColdDecayDurMins = 1f;

        private float _hungerFillDur= 16f;
        private float _thirstFillDur = 12.5f;

        private void OnEnable()
        {
            _actor = (VitalSystemActor)target;
            CalculateCurrentDurations(_actor.GetVitalSystem);
        }

        public override void OnInspectorGUI()
        {
            if (!_actor) 
                return;

            base.OnInspectorGUI();
            UtilsGUI();
        }

        private void UtilsGUI()
        {
            EditorGUILayout.Space();
            GuiHelper.DrawHeaderLabel("Custom Utils", fontSize: 12);
            
            _ShowUtils = EditorGUILayout.Toggle("Show", _ShowUtils);
            if (!_ShowUtils)
                return;

            EditorGUILayout.Space();
            _healthStarveDecayDurMins = EditorGUILayout.FloatField("Health Starve Decay Dur.", _healthStarveDecayDurMins);
            _healthDehydDecayDurMins = EditorGUILayout.FloatField("Health Dehyd. Decay Dur.", _healthDehydDecayDurMins);
            _healthColdDecayDurMins = EditorGUILayout.FloatField("Health Cold Decay Dur.", _healthColdDecayDurMins);
            _hungerFillDur = EditorGUILayout.FloatField("Hunger Fill Dur.", _hungerFillDur);
            _thirstFillDur = EditorGUILayout.FloatField("Thirst Fill Dur.", _thirstFillDur);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Calculate Curr. Dur. (mins)"))
            { 
                CalculateCurrentDurations(_actor.GetVitalSystem);
            }

            if (GUILayout.Button("Set Rates")) 
            {
                Undo.RecordObject(_actor, "Set Vital Rates");
                SetRatesFromDurations(_actor.GetVitalSystem);
                EditorUtility.SetDirty(_actor);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void CalculateCurrentDurations(VitalSystem vitals) 
        {
            _healthStarveDecayDurMins = CalculateDurInMins(vitals.MaxHealth, vitals.StarvingHealthDecayRate);
            _healthDehydDecayDurMins = CalculateDurInMins(vitals.MaxHealth, vitals.DehydrationHealthDecayRate);
            _healthColdDecayDurMins = CalculateDurInMins(vitals.MaxHealth, vitals.ColdHealthDecayRate);
            _hungerFillDur = CalculateDurInMins(vitals.MaxHunger, vitals.HungerIncreaseRate);
            _thirstFillDur = CalculateDurInMins(vitals.MaxThirst, vitals.ThirstIncreaseRate);
        }

        private void SetRatesFromDurations(VitalSystem vitals) 
        {
            vitals.SetStarvingHealthDecayRate(CalculateRate(vitals.MaxHealth, _healthStarveDecayDurMins))
                  .SetDehydrationHealthDecayRate(CalculateRate(vitals.MaxHealth, _healthDehydDecayDurMins))
                  .SetColdHealthDecayRate(CalculateRate(vitals.MaxHealth, _healthColdDecayDurMins))
                  .SetHungerIncreaseRate(CalculateRate(vitals.MaxHunger, _hungerFillDur))
                  .SetThirstIncreaseRate(CalculateRate(vitals.MaxThirst, _thirstFillDur));
        }

        private float CalculateRate(float max, float durInMins) => durInMins <= 0f ? 0f : max / durInMins / 60f;
        private float CalculateDurInMins(float max, float rate) => rate <= 0f ? 0f: max / rate / 60f;
    }
}