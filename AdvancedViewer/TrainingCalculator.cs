using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvancedViewer
{
    public partial class TrainingCalculator : Form
    {
        // -------------------------------------------------------------------------------

        // monster data
        private int _localMonLife = 0;
        private int _localMonPower = 0;
        private int _localMonDefense = 0;
        private int _localMonSkill = 0;
        private int _localMonSpeed = 0;
        private int _localMonIntelligence = 0;

        private int _localMonSpoil = 0;
        private int _localMonFear = 0;

        private int _localMonMainBreed = 0;

        // controls
        private System.Windows.Forms.Timer updateTimer;

        // -------------------------------------------------------------------------------

        public TrainingCalculator(Point startHere)
        {
            InitializeComponent();

            // this prevents the tbAltaVista1 control from being selected on startup
            this.ActiveControl = label1;

            // Start this window at the bottom of the original viewer
            this.StartPosition = FormStartPosition.Manual;
            this.Location = startHere;

            // TODO: maybe integrate tool tips at some point later?

            CalculateTrainingTechPercentages();

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 1000; // 1s ; limit re-calculations to save CPU
            updateTimer.Tick += new EventHandler(updateTimer_Tick);

            updateTimer.Start();
        }

        // Used by MonsterDataDisplay 250ms update tick function
        public void UpdateMonsterStats(int[] update)
        {
            _localMonLife = update[0];
            _localMonPower = update[1];
            _localMonDefense = update[2];
            _localMonSkill = update[3];
            _localMonSpeed = update[4];
            _localMonIntelligence = update[5];
            _localMonSpoil = update[6];
            _localMonFear = update[7];
            _localMonMainBreed = update[8];
        }

        // Use this function for the 1 second update tick function
        private void CalculateTrainingTechPercentages()
        {
            // old calculation code for posterity
            /*const int level1Minimum = 150 - 1;
            const int level2Minimum = 400 - 1;
            const int specialMinimum = 400 - 1;

            int lifBase = _localMonSpoil + _localMonFear + _localMonLife;
            int powBase = _localMonSpoil + _localMonFear + _localMonPower;
            int defBase = _localMonSpoil + _localMonFear + _localMonDefense;
            int skiBase = _localMonSpoil + _localMonFear + _localMonSkill;
            int spdBase = _localMonSpoil + _localMonFear + _localMonSpeed;
            int intBase = _localMonSpoil + _localMonFear + _localMonIntelligence;

            _altaVistaLevel1Percent = Math.Clamp((lifBase - level1Minimum), 0, 100);
            _altaVistaLevel2Percent = Math.Clamp((lifBase - level2Minimum), 0, 100);

            _salemSpecialPercent = Math.Clamp((powBase - specialMinimum), 0, 100);

            _renoSpecialPercent = Math.Clamp((defBase - specialMinimum), 0, 100);

            _tongaLevel1Percent = Math.Clamp((skiBase - level1Minimum), 0, 100);
            _tongaLevel2Percent = Math.Clamp((skiBase - level2Minimum), 0, 100);

            _hartvilleLevel1Percent = Math.Clamp((spdBase - level1Minimum), 0, 100);
            _hartvilleLevel2Percent = Math.Clamp((spdBase - level2Minimum), 0, 100);

            _bareesLevel1Percent = Math.Clamp((intBase - level1Minimum), 0, 100);
            _bareesLevel2Percent = Math.Clamp((intBase - level2Minimum), 0, 100);*/
            
            tbAltaVista1.Text = MRUtilities.CalculateLevel1Percentage(_localMonSpoil, _localMonFear, _localMonLife) + " %";
            tbAltaVista2.Text = MRUtilities.CalculateLevel2Percentage(_localMonSpoil, _localMonFear, _localMonLife) + " %";
            tbSalem.Text = MRUtilities.CalculateSpecialPercentage(_localMonSpoil, _localMonFear, _localMonPower) + " %";
            tbReno.Text = MRUtilities.CalculateSpecialPercentage(_localMonSpoil, _localMonFear, _localMonDefense) + " %";
            tbTonga1.Text = MRUtilities.CalculateLevel1Percentage(_localMonSpoil, _localMonFear, _localMonSkill) + " %";
            tbTonga2.Text = MRUtilities.CalculateLevel2Percentage(_localMonSpoil, _localMonFear, _localMonSkill) + " %";
            tbHartville1.Text = MRUtilities.CalculateLevel1Percentage(_localMonSpoil, _localMonFear, _localMonSpeed) + " %";
            tbHartville2.Text = MRUtilities.CalculateLevel2Percentage(_localMonSpoil, _localMonFear, _localMonSpeed) + " %";
            tbBarees1.Text = MRUtilities.CalculateLevel1Percentage(_localMonSpoil, _localMonFear, _localMonIntelligence) + " %";
            tbBarees2.Text = MRUtilities.CalculateLevel2Percentage(_localMonSpoil, _localMonFear, _localMonIntelligence) + " %";

            label18.Text = MRUtilities.GetHeavy1TechName(_localMonMainBreed);
            label21.Text = MRUtilities.GetHit1TechName(_localMonMainBreed);
            label22.Text = MRUtilities.GetRanged1TechName(_localMonMainBreed);
            label23.Text = MRUtilities.GetWither1TechName(_localMonMainBreed);

            label25.Text = MRUtilities.GetHeavy2TechName(_localMonMainBreed);
            label26.Text = MRUtilities.GetSpecial1TechName(_localMonMainBreed);
            label27.Text = MRUtilities.GetSpecial2TechName(_localMonMainBreed);
            label28.Text = MRUtilities.GetHit2TechName(_localMonMainBreed);
            label29.Text = MRUtilities.GetRanged2TechName(_localMonMainBreed);
            label30.Text = MRUtilities.GetWither2TechName(_localMonMainBreed);
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            CalculateTrainingTechPercentages();
        }
    }
}
