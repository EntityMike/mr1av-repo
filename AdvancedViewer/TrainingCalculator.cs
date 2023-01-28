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

        // training location data
        private int _altaVistaLevel1Percent = 0;
        private int _altaVistaLevel2Percent = 0;
        private int _salemSpecialPercent = 0;
        private int _renoSpecialPercent = 0;
        private int _tongaLevel1Percent = 0;
        private int _tongaLevel2Percent = 0;
        private int _hartvilleLevel1Percent = 0;
        private int _hartvilleLevel2Percent = 0;
        private int _bareesLevel1Percent = 0;
        private int _bareesLevel2Percent = 0;

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

            SetTextBoxes();

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
        }

        // Use this function for the 1 second update tick function
        private void CalculateTrainingTechPercentages()
        {
            /////// Tech Acquisition Requirements ///////
            // For Level 1:
            // Location Stat + Spoil + Fear + Random(1,99) >= 250
            // For Level 2 / Special:
            // Location Stat + Spoil + Fear + Random(1,99) >= 500
            //
            // The "minimum" values help us mathmatically get a percentage for the techs.
            // However, because the "base" numbers are supposed to also be adding in
            // a random number between 1 and 99 that means we need to subtract the 
            // minimum there, which is 1.
            //
            // We can see use the below numeric logic to help us determine if our math
            // formula output is correct
            // For Level 1:
            // 151 (1%) to 250 (100%) ; below 151 (0%) ; above 250 (100%)
            // For Level 2 / Special:
            // 401 (1%) to 500 (100%) ; below 401 (0%) ; above 500 (100%)
            //           

            const int level1Minimum = 150 - 1;
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
            _bareesLevel2Percent = Math.Clamp((intBase - level2Minimum), 0, 100);
        }

        private void SetTextBoxes()
        {
            tbAltaVista1.Text = _altaVistaLevel1Percent.ToString() + " %";
            tbAltaVista2.Text = _altaVistaLevel2Percent.ToString() + " %";
            tbSalem.Text = _salemSpecialPercent.ToString() + " %";
            tbReno.Text = _renoSpecialPercent.ToString() + " %";
            tbTonga1.Text = _tongaLevel1Percent.ToString() + " %";
            tbTonga2.Text = _tongaLevel2Percent.ToString() + " %";
            tbHartville1.Text = _hartvilleLevel1Percent.ToString() + " %";
            tbHartville2.Text = _hartvilleLevel2Percent.ToString() + " %";
            tbBarees1.Text = _bareesLevel1Percent.ToString() + " %";
            tbBarees2.Text = _bareesLevel2Percent.ToString() + " %";
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            CalculateTrainingTechPercentages();
            SetTextBoxes();
        }
    }
}
