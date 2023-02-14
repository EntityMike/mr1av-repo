# Monster Rancher 1 Advanced Viewer README

## Author

Soken (EntityMike)

## Game Version

* Monster Rancher 1+2 DX 1.0.0.2 (not tested on previous versions) on [Steam](https://store.steampowered.com/app/1716120/Monster_Rancher_1__2_DX/)
   * This program targets Monster Rancher 1 DX __only__
   * If you would like an Advanced Viewer for Monster Rancher 2, see: [MR2AV](https://github.com/Lexichu/mr2av_repo)

## Usage

* Download the latest release of the MR1AdvancedViewer EXE, and put at desired location
   * The EXE is self-contained so it is bigger in size, but you don't need to download anything else (yay!)
* Start Monster Rancher 1 DX
* Start MR1AV
* __Enjoy!__

## Version History

### Version 1.2.0
* Various code cleanup
* Add embedded training calculator data (from v1.1.0) to the main form
   * A checkbox to enable or disable this functionality is next to the left of the "Training" button
   * Enabled (checked) by default
   * Disabling (un-checking) the checkbox will revert to the more simple layout from previous versions
   * Training Calculator (version 1) is still available if you want a popout window
   * Both can be used at the same time

### Version 1.1.0
* Various code cleanup
* Add Training Calculator (version 1)
   * The "Training" button is on the middle right of the main window
   * The new window always opens (initially) at the bottom left of the main window
   * Shows the following information:
      * Shows the different training locations
      * Shows percentage chance of acquiring a new tech at each training location
      * Shows percentage chance for both level 1 techs and level 2/special techs
      * Shows the monster stat which each location is based off of

### Version 1.0.0
* Advanced Viewer currently provides the following information:
   * Monster Breed
   * Monster Breed Name
   * Monster Age
   * Monster Potential
   * Monster Seriousness
   * Monster Guts Rate (Will Rate)
   * Monster Stats (LIF/POW/DEF/SKI/SPD/INT)
   * Monster Spoil
   * Monster Fear
   * Monster Loyalty (calculated)
   * Monster Stress
   * Monster Fatigue
   * Monster Life Index (calculated)
   * Monster Life Span (remaining)
   * Player Money
   * Game Date
* In the top right corner: there is a "?" button, which provides "Help/About" information
* Below the "?" button: If you see a blinking warning icon, that means there is a newer version available
   * If you click it, it will show provide a link to GitHub for the latest release
   * If you click it one time, it will stop blinking until the program is started again, but can still be clicked again for the info
* In the bottom right corner: there is a text box that tells you if the program detects your game or not
   * "Not detected": The program cannot see your game running, and is blanking out data fields
   * "Detected; Running": The program can see your game running and is attempting to display game data

## Bugs / Known Issues

* _INCOMPLETE_: The "Monster Name", "Monster Fame", "Monster Stat Gains", and "Player Name" text boxes are __not__ currently populated (I have a hunch the data exists but don't know where yet)

## Development Information

* Developed using C# (WinForms)
* Developed in Visual Studio 2022 (64-bit) - Version 17.2.6
* Under .NET 6.0 framework
* Uses Octokit Nuget package for easy ability to pull GitHub release versions

## Questions / Support

Have questions about Monster Rancher 1 or 2 DX (or other Monster Rancher games)? Or questions on this program? Feel free to join us in the [Monster Rancher discord](https://discord.gg/dfdvXxFHBz), and ask away!
 
