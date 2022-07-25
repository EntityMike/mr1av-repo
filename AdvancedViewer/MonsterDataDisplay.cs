using System.Collections.Generic;
using System.Drawing; // for Color

using Octokit; // for GitHub version checking

namespace AdvancedViewer
{
    public partial class MonsterDataDisplay : Form
    {
        public MonsterDataDisplay()
        {
            InitializeComponent();

            // this prevents the tbMonLif control from being selected on startup
            this.ActiveControl = labMonStatTotal;

            tips = new ToolTip();
            SetAllToolTips();

            mrpw = new MRProcessWrapper();

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

        // -------------------------------------------------------------------------------

        MRProcessWrapper mrpw;
        System.Windows.Forms.Timer processScanTimer;
        System.Windows.Forms.Timer updateTimer;
        System.Windows.Forms.Timer versionWarningBlinkTimer;
        ToolTip tips;

        // pulled from memory
        int _monLife = -1;
        int _monPower = -1;
        int _monDefense = -1;
        int _monSkill = -1;
        int _monSpeed = -1;
        int _monIntelligence = -1;

        int _monLifeSpan = -1;
        int _monAge = -1;
        int _monFatigue = -1;
        int _monStress = -1;
        int _monSpoil = -1;
        int _monFear = -1;
        int _monSeriousness = -1;
        int _monPotential = -1;
        int _monGutsRate = -1;
        int _monMainBreed = -1;
        int _monSubBreed = -1;

        int _plaMoney = -1;
        int _gameWeek = -1;
        int _gameMonth = -1;
        int _gameYear12 = -1;
        int _gameYear34 = -1;

        // form-specific
        string ViewerVersionID = "1.0.0";

        // -------------------------------------------------------------------------------

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
            if (!mrpw.CheckForMonsterRancherProcess())
            {
                // nothing running yet, blank out the data
                updateTimer.Stop();
                tbGameState.Text = "Not Detected";
                BlankAllData();
            }
            else
            {
                // we can see it running, now fill in the data
                tbGameState.Text = "Detected; Running";
                updateTimer.Start();
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            // ------ Update all data variables ------

            _monLife = mrpw.GetMonLife();
            _monPower = mrpw.GetMonPower();
            _monDefense = mrpw.GetMonDefense();
            _monSkill = mrpw.GetMonSkill();
            _monSpeed = mrpw.GetMonSpeed();
            _monIntelligence = mrpw.GetMonIntelligence();

            // NOTE: Fenrick says the LifeSpan # is remaining LifeSpan
            _monLifeSpan = mrpw.GetMonLifeSpan();
            _monAge = mrpw.GetMonAge();
            _monFatigue = mrpw.GetMonFatigue();
            _monStress = mrpw.GetMonStress();
            _monSpoil = mrpw.GetMonSpoil();
            _monFear = mrpw.GetMonFear();
            _monSeriousness = mrpw.GetMonSeriousness();
            _monPotential = mrpw.GetMonPotential();
            _monGutsRate = mrpw.GetMonGutsRate();
            _monMainBreed = mrpw.GetMonMainBreed();
            _monSubBreed = mrpw.GetMonSubBreed();

            _plaMoney = mrpw.GetPlayerMoney();
            _gameWeek = mrpw.GetGameWeek();
            _gameMonth = mrpw.GetGameMonth();
            _gameYear12 = mrpw.GetGameYear12();
            _gameYear34 = mrpw.GetGameYear34();

            // ------ Update all text displays ------

            tbMonLif.Text = _monLife.ToString();
            tbMonPow.Text = _monPower.ToString();
            tbMonDef.Text = _monDefense.ToString();
            tbMonSki.Text = _monSkill.ToString();
            tbMonSpd.Text = _monSpeed.ToString();
            tbMonInt.Text = _monIntelligence.ToString();
            labMonStatTotal.Text = (_monLife + _monPower + _monDefense + 
                                    _monSkill + _monSpeed + _monIntelligence).ToString();

            tbMonLifeSpan.Text = MRUtilities.FormatLifeSpan(_monLifeSpan);
            tbMonAge.Text = MRUtilities.FormatAge(_monAge);
            tbMonStress.Text = _monStress.ToString();
            tbMonFatig.Text = _monFatigue.ToString();
            tbMonLifeInd.Text = MRUtilities.CalculateLifeIndex(_monStress, _monFatigue, out Color lifeIndexColorCode);
            tbMonLifeInd.BackColor = lifeIndexColorCode;
            tbMonSpoil.Text = _monSpoil.ToString();
            tbMonFear.Text = _monFear.ToString();
            tbMonLoyal.Text = MRUtilities.CalculateLoyalty(_monSpoil, _monFear);
            tbMonSrs.Text = _monSeriousness.ToString();
            tbMonPot.Text = _monPotential.ToString();
            tbMonGutsRate.Text = _monGutsRate.ToString();
            tbMonBreed.Text = MRUtilities.FormatBreed(_monMainBreed, _monSubBreed);
            tbBreedName.Text = MRUtilities.FormatBreedName(_monMainBreed, _monSubBreed);

            tbPlaMoney.Text = MRUtilities.FormatPlayerMoney(_plaMoney);
            tbGameDate.Text = MRUtilities.FormatGameDate(_gameWeek, _gameMonth, _gameYear12, _gameYear34);
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
    }
}