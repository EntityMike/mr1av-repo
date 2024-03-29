using System.Collections.Generic;
using System.Drawing; // for Color

using Octokit; // for GitHub version checking

namespace AdvancedViewer
{
    public struct MonsterStats
    {
        public int monLife;
        public int monPower;
        public int monDefense;
        public int monSkill;
        public int monSpeed;
        public int monIntelligence;

        public int monLifeSpan;
        public int monAge;
        public int monFatigue;
        public int monStress;
        public int monSpoil;
        public int monFear;
        public int monSeriousness;
        public int monPotential;
        public int monGutsRate;
        public int monMainBreed;
        public int monSubBreed;

        public MonsterStats()
        {
            monLife = -1;
            monPower = -1;
            monDefense = -1;
            monSkill = -1;
            monSpeed = -1;
            monIntelligence = -1;

            monLifeSpan = -1;
            monAge = -1;
            monFatigue = -1;
            monStress = -1;
            monSpoil = -1;
            monFear = -1;
            monSeriousness = -1;
            monPotential = -1;
            monGutsRate = -1;
            monMainBreed = -1;
            monSubBreed = -1;
        }
    }

    public struct GameStats
    {
        public int plaMoney;
        public int gameWeek;
        public int gameMonth;
        public int gameYear12;
        public int gameYear34;

        public GameStats()
        {
            plaMoney = -1;
            gameWeek = -1;
            gameMonth = -1;
            gameYear12 = -1;
            gameYear34 = -1;
        }
    }

    public partial class MonsterDataDisplay : Form
    {
        // -------------------------------------------------------------------------------

        // form controls
        System.Windows.Forms.Timer processScanTimer;
        System.Windows.Forms.Timer updateTimer;
        System.Windows.Forms.Timer versionWarningBlinkTimer;
        ToolTip tips;
        List<Control> moveTheseControls;

        // a counter used to limit CPU of embedded training calculations.
        // setting this to 3 allows an immediate data update on tick #1.
        int tickCounter = 3;

        // our classes
        static MRProcessWrapper _mrpw;
        // pulled from memory
        public static MonsterStats _monStats;
        public static GameStats _gameStats;

        public TrainingCalculator? _tCalc;

        // versioning
        string ViewerVersionID = "1.3.0";

        // -------------------------------------------------------------------------------

        public MonsterDataDisplay()
        {
            InitializeComponent();

            // this prevents the tbMonLif control from being selected on startup
            this.ActiveControl = labMonStatTotal;

            // has to be called after InitializeComponent()
            InitializeMyControlCollection();

            tips = new ToolTip();
            SetAllToolTips();

            _mrpw = new MRProcessWrapper();
            _monStats = new MonsterStats();
            _gameStats = new GameStats();

            processScanTimer = new System.Windows.Forms.Timer();
            processScanTimer.Interval = 50; ; // 50ms ; lets do this really often
            processScanTimer.Tick += new EventHandler(processScanTimer_Tick);

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 250; // 250ms ; not as often but fast enough to show "instantly"
            updateTimer.Tick += new EventHandler(updateTimer_Tick);

            versionWarningBlinkTimer = new System.Windows.Forms.Timer();
            versionWarningBlinkTimer.Interval = 1000; // 1s
            versionWarningBlinkTimer.Tick += new EventHandler(versionWarningBlinkTimer_Tick);

            processScanTimer.Start();
        }

        // keep track of a subset of controls that can move around later if the user wants to
        private void InitializeMyControlCollection()
        {
            // this list helps us move around these controls a lot easier
            moveTheseControls = new List<Control>();
            moveTheseControls.Add(this.label7);
            moveTheseControls.Add(this.tbMonSpoil);
            moveTheseControls.Add(this.label8);
            moveTheseControls.Add(this.tbMonFear);
            moveTheseControls.Add(this.label9);
            moveTheseControls.Add(this.tbMonLoyal);
            moveTheseControls.Add(this.label10);
            moveTheseControls.Add(this.tbMonStress);
            moveTheseControls.Add(this.label11);
            moveTheseControls.Add(this.tbMonFatig);
            moveTheseControls.Add(this.label12);
            moveTheseControls.Add(this.tbMonLifeInd);
            moveTheseControls.Add(this.label20);
            moveTheseControls.Add(this.tbMonLifeSpan);
        }

        // Pillaged from MR2AV (Thanks Lexichu)
        private async Task CheckGitHubNewerVersion()
        {
            // Get all releases from GitHub
            // Source: https://octokitnet.readthedocs.io/en/latest/getting-started/
            GitHubClient client = new GitHubClient(new ProductHeaderValue("mr1av-repo"));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("EntityMike", "mr1av-repo");

            // Setup the versions
            Version latestGitHubVersion = new Version(releases[0].TagName);
            Version localVersion = new Version(ViewerVersionID);

            // Compare the Versions
            // Source: https://stackoverflow.com/questions/7568147/compare-version-numbers-without-using-split-function
            int versionComparison = localVersion.CompareTo(latestGitHubVersion);
            
            if (versionComparison < 0) // verison outdated; new version available
            {
                pbWarningIcon.Visible = true;
                versionWarningBlinkTimer.Start();
            }
            else if (versionComparison > 0) // how are you on a newer version? (maybe a dev?)
            {}
            else // version is current
            {
                pbWarningIcon.Visible = false;
            }
        }

        private void BlankAllData()
        {
            // just using tbMonLife as an indicator of if we've already blanked out the data.
            // this way we avoid running the loop every time we call this.
            if (tbMonLif.Text != "---")
            {
                // deal with all the text boxes
                foreach (Control c in this.Controls)
                {
                    // leave the process state text box alone
                    if (c is TextBox && c.Name != "tbProcessState")
                    {
                        c.Text = "---";
                    }
                }

                // deal with other controls
                labMonStatTotal.Text = "---";
                tbMonLifeInd.BackColor = SystemColors.Control;
            }
        }

        private void SetAllToolTips()
        {
            string tooltip;

            // info tooltips
            tooltip = "= Will Rate";
            tips.SetToolTip(label25, tooltip);
            tips.SetToolTip(tbMonGutsRate, tooltip);
            tooltip = "LI chart: " + "\n" +     // change this if CalculateLifeIndex() changes
                      "LI = 0 - 3:    0 weeks" + "\n" +
                      "LI = 4 - 5:   -1 week" + "\n" +
                      "LI = 6 - 7:   -2 weeks" + "\n" +
                      "LI = 8 - 9:   -3 weeks" + "\n" +
                      "LI = 10 - 11: -4 weeks" + "\n" +
                      "LI = 12 - 13: -5 weeks" + "\n" +
                      "LI = 14 - 15: -6 weeks";
            tips.SetToolTip(label12, tooltip);
            tips.SetToolTip(tbMonLifeInd, tooltip);

            // todo tooltips
            tooltip = "TODO";
            tips.SetToolTip(label14, tooltip);
            tips.SetToolTip(tbMonLifGain, tooltip);
            tips.SetToolTip(tbMonPowGain, tooltip);
            tips.SetToolTip(tbMonDefGain, tooltip);
            tips.SetToolTip(tbMonSkiGain, tooltip);
            tips.SetToolTip(tbMonSpdGain, tooltip);
            tips.SetToolTip(tbMonIntGain, tooltip);
            tips.SetToolTip(label17, tooltip);
            tips.SetToolTip(tbMonName, tooltip);
            tips.SetToolTip(label22, tooltip);
            tips.SetToolTip(tbPlaName, tooltip);
            tips.SetToolTip(label26, tooltip);
            tips.SetToolTip(tbMonFame, tooltip);

            // version tooltip
            tooltip = "New version available! Click here!";
            tips.SetToolTip(pbWarningIcon, tooltip);
            tips.ShowAlways = true;

            // TODO: add tips for other terms as needed
        }
        
        private async void MonsterDataDisplay_Load(object sender, EventArgs e)
        {
            // add version to the form title
            this.Text += ViewerVersionID.ToString();

            Task verisonCheck = CheckGitHubNewerVersion();

            // TODO: perform any other boot checks here

            try
            {
                await verisonCheck;
            }
            catch // catch everything
            {
                // since we can't verify the version, ignore the icon
                pbWarningIcon.Visible = false;
            }
        }

        private void processScanTimer_Tick(object sender, EventArgs e)
        {
            if (!_mrpw.CheckForMonsterRancherProcess())
            {
                // nothing running yet, blank out the data
                updateTimer.Stop();
                tbGameState.Text = "Not Detected";
                BlankAllData();
                if (_tCalc != null)
                {
                    _tCalc.Close();
                    _tCalc = null;
                }
                butTraining.Enabled = false;
            }
            else
            {
                // we can see it running, now fill in the data
                tbGameState.Text = "Detected; Running";
                updateTimer.Start();
                butTraining.Enabled = true;
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            // ------ Update all data variables ------

            _monStats.monLife = _mrpw.GetMonLife();
            _monStats.monPower = _mrpw.GetMonPower();
            _monStats.monDefense = _mrpw.GetMonDefense();
            _monStats.monSkill = _mrpw.GetMonSkill();
            _monStats.monSpeed = _mrpw.GetMonSpeed();
            _monStats.monIntelligence = _mrpw.GetMonIntelligence();

            // NOTE: Fenrick says the LifeSpan # is remaining LifeSpan
            _monStats.monLifeSpan = _mrpw.GetMonLifeSpan();
            _monStats.monAge = _mrpw.GetMonAge();
            _monStats.monFatigue = _mrpw.GetMonFatigue();
            _monStats.monStress = _mrpw.GetMonStress();
            _monStats.monSpoil = _mrpw.GetMonSpoil();
            _monStats.monFear = _mrpw.GetMonFear();
            _monStats.monSeriousness = _mrpw.GetMonSeriousness();
            _monStats.monPotential = _mrpw.GetMonPotential();
            _monStats.monGutsRate = _mrpw.GetMonGutsRate();
            _monStats.monMainBreed = _mrpw.GetMonMainBreed();
            _monStats.monSubBreed = _mrpw.GetMonSubBreed();

            _gameStats.plaMoney = _mrpw.GetPlayerMoney();
            _gameStats.gameWeek = _mrpw.GetGameWeek();
            _gameStats.gameMonth = _mrpw.GetGameMonth();
            _gameStats.gameYear12 = _mrpw.GetGameYear12();
            _gameStats.gameYear34 = _mrpw.GetGameYear34();

            // ------ Update all text displays ------

            tbMonLif.Text = _monStats.monLife.ToString();
            tbMonPow.Text = _monStats.monPower.ToString();
            tbMonDef.Text = _monStats.monDefense.ToString();
            tbMonSki.Text = _monStats.monSkill.ToString();
            tbMonSpd.Text = _monStats.monSpeed.ToString();
            tbMonInt.Text = _monStats.monIntelligence.ToString();
            labMonStatTotal.Text = (_monStats.monLife + _monStats.monPower + _monStats.monDefense + 
                                    _monStats.monSkill + _monStats.monSpeed + _monStats.monIntelligence).ToString();

            tbMonLifeSpan.Text = MRUtilities.FormatLifeSpan(_monStats.monLifeSpan);
            tbMonAge.Text = MRUtilities.FormatAge(_monStats.monAge);
            tbMonStress.Text = _monStats.monStress.ToString();
            tbMonFatig.Text = _monStats.monFatigue.ToString();
            tbMonLifeInd.Text = MRUtilities.CalculateLifeIndex(_monStats.monStress, _monStats.monFatigue, out Color lifeIndexColorCode);
            tbMonLifeInd.BackColor = lifeIndexColorCode;
            tbMonSpoil.Text = _monStats.monSpoil.ToString();
            tbMonFear.Text = _monStats.monFear.ToString();
            tbMonLoyal.Text = MRUtilities.CalculateLoyalty(_monStats.monSpoil, _monStats.monFear);
            tbMonSrs.Text = _monStats.monSeriousness.ToString();
            tbMonPot.Text = _monStats.monPotential.ToString();
            tbMonGutsRate.Text = _monStats.monGutsRate.ToString();
            tbMonBreed.Text = MRUtilities.FormatBreed(_monStats.monMainBreed, _monStats.monSubBreed);
            tbBreedName.Text = MRUtilities.FormatBreedName(_monStats.monMainBreed, _monStats.monSubBreed);

            tbPlaMoney.Text = MRUtilities.FormatPlayerMoney(_gameStats.plaMoney);
            tbGameDate.Text = MRUtilities.FormatGameDate(_gameStats.gameWeek, _gameStats.gameMonth, _gameStats.gameYear12, _gameStats.gameYear34);

            if (_tCalc != null)
            {
                _tCalc.UpdateMonsterStats(new int[] { _monStats.monLife, _monStats.monPower, _monStats.monDefense, _monStats.monSkill, 
                                                      _monStats.monSpeed, _monStats.monIntelligence, _monStats.monSpoil, _monStats.monFear,
                                                      _monStats.monMainBreed});
            }

            // attempt to lower CPU load to 1Hz for these calculations, like the _tCalc form does
            tickCounter++;
            if (gbEmbeddedTraining.Visible && tickCounter == 4)
            {
                tbAltaVista1.Text = MRUtilities.CalculateLevel1Percentage(_monStats.monSpoil, _monStats.monFear, _monStats.monLife) + " %";
                tbAltaVista2.Text = MRUtilities.CalculateLevel2Percentage(_monStats.monSpoil, _monStats.monFear, _monStats.monLife) + " %";
                tbSalem.Text = MRUtilities.CalculateSpecialPercentage(_monStats.monSpoil, _monStats.monFear, _monStats.monPower) + " %";
                tbReno.Text = MRUtilities.CalculateSpecialPercentage(_monStats.monSpoil, _monStats.monFear, _monStats.monDefense) + " %";
                tbTonga1.Text = MRUtilities.CalculateLevel1Percentage(_monStats.monSpoil, _monStats.monFear, _monStats.monSkill) + " %";
                tbTonga2.Text = MRUtilities.CalculateLevel2Percentage(_monStats.monSpoil, _monStats.monFear, _monStats.monSkill) + " %";
                tbHartville1.Text = MRUtilities.CalculateLevel1Percentage(_monStats.monSpoil, _monStats.monFear, _monStats.monSpeed) + " %";
                tbHartville2.Text = MRUtilities.CalculateLevel2Percentage(_monStats.monSpoil, _monStats.monFear, _monStats.monSpeed) + " %";
                tbBarees1.Text = MRUtilities.CalculateLevel1Percentage(_monStats.monSpoil, _monStats.monFear, _monStats.monIntelligence) + " %";
                tbBarees2.Text = MRUtilities.CalculateLevel2Percentage(_monStats.monSpoil, _monStats.monFear, _monStats.monIntelligence) + " %";
                tickCounter = 0;
            }
        }

        private void butTraining_Click(object sender, EventArgs e)
        {
            if (_tCalc == null)
            {
                // TODO: We CAN make this more complicated if we want, and take into account the display size
                // to make sure it does not appear off-screen if this window is too far down ... but meh?
                _tCalc = new TrainingCalculator(new Point(this.Location.X, (this.Location.Y + this.Height)));
                _tCalc.FormClosing += trainingCalcFormClose;
                _tCalc.Show();
            }
        }

        private void versionWarningBlinkTimer_Tick(object sender, EventArgs e)
        {
            if (pbWarningIcon.BackColor == SystemColors.Control)
            {
                pbWarningIcon.BackColor = Color.OrangeRed;
            }
            else // == Color.OrangeRed
            {
                pbWarningIcon.BackColor = SystemColors.Control;
            }
        }

        private void pbWarningIcon_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"You are currently on an outdated build of MR1AV.
                              Please visit https://github.com/EntityMike/mr1av-repo/releases/",
                             "MR1AV Version Notice", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);

            versionWarningBlinkTimer.Stop();
            pbWarningIcon.BackColor = SystemColors.Control;
        }

        private void butAbout_Click(object sender, EventArgs e)
        {
            string displayText = "Author:  Soken (https://github.com/EntityMike/mr1av-repo)" + "\n\n" +
                                  "Game:    Monster Rancher 1 DX" + "\n\n" +
                                  "Support: https://discord.gg/dfdvXxFHBz (MR discord)" + "\n\n" +
                                  "Thanks:  Koei Tecmo for releasing MR1+2 DX!" + "\n" +
                                  "         Fenrick for help with alpha testing" + "\n" +
                                  "         (https://LegendCup.com)" + "\n" +
                                  "         Lexichu for making MR2AV (which inspired this)" + "\n" +
                                  "         The MR community for being awesome" + "\n" +
                                  "         My wife and kids: I love you!" + "\n\n" +
                                  "Disclaimers: This is a fan project. This project is not funded," + "\n" +
                                  "         supported, or produced by Koei Tecmo Games Co. LTD or" + "\n" +
                                  "         related companies. All images and sounds related to" + "\n" +
                                  "         Monster Rancher are property of Tecmo, inc. I, nor" + "\n" +
                                  "         anyone else related to this project, do not claim to" + "\n" +
                                  "         own any of these properties, images, graphics, or" + "\n" +
                                  "         sounds." + "\n\n" +
                                  "Please buy Monster Rancher 1 + 2 DX on Steam:" + "\n" +
                                  "https://store.steampowered.com/app/1716120/Monster_Rancher_1__2_DX/" + "\n\n" +
                                  "Enjoy!";

            MessageBox.Show(displayText, "About/Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // form close callbacks
        public void trainingCalcFormClose(object sender, FormClosingEventArgs e)
        {
            _tCalc = null;
        }

        private void cbEmbedTraining_CheckedChanged(object sender, EventArgs e)
        {
            // This value was determined by experimentation
            int offset = 170;

            // logic based on the state of the check box
            if (!cbEmbedTraining.Checked)
            {
                // move stuff to the left
                offset *= -1;
                // and make embedded training invisible
                gbEmbeddedTraining.Visible = false;
            }
            else
            {
                // move stuff to the right, which is the default
                // and make embedded training visible
                gbEmbeddedTraining.Visible = true;
            }

            // move the subset of controls as needed
            foreach (Control c in moveTheseControls)
            {
                c.Location = new System.Drawing.Point((c.Location.X + offset), c.Location.Y);
            }
        }
    }
}