using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// app-specific
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AdvancedViewer
{
    internal class MRProcessWrapper
    {
        const int PROCESS_ALLACCESS = 0x1F0FFF;
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        // Not writing anything just yet
        private static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        Process? _gameProcess;             // PSXProcess
        IntPtr _gameMemPtr;                // psxPTR
        int _gameMemBase;                  // PSXBase
        string _gameProcessName;           // EmuFileName
        byte[] _scratchData;               // ScratchData
        int _hasRead;                      // so ReadProcessMemory stops complaining

        #region MEMORY ADDRESS OFFSETS
        // monster stat address offsets
        const int MON_LIF_ADDR_OFFSET = 0x0032A1E6;
        const int MON_POW_ADDR_OFFSET = 0x0032A1EA;
        const int MON_DEF_ADDR_OFFSET = 0x0032A1EC;
        const int MON_SKI_ADDR_OFFSET = 0x0032A1EE;
        const int MON_SPD_ADDR_OFFSET = 0x0032A1F2;
        const int MON_INT_ADDR_OFFSET = 0x0032A1F4;

        // monster misc data address offsets
        // TODO: monster stat gains?
        // TODO: monster name?
        const int MON_LIFESPAN_ADDR_OFFSET    = 0x0032C37C;
        const int MON_AGE_ADDR_OFFSET         = 0x0032C370;
        const int MON_FATIGUE_ADDR_OFFSET     = 0x0032A1D3;
        const int MON_STRESS_ADDR_OFFSET      = 0x0032A1D9;
        const int MON_SPOIL_ADDR_OFFSET       = 0x0032A1D7;
        const int MON_FEAR_ADDR_OFFSET        = 0x0032A1D8;
        //const int MON_SERIOUSNESS_ADDR_OFFSET = 0x0032C468; // determined not to be seriousness
        const int MON_SERIOUSNESS_ADDR_OFFSET = 0x0032C357;
        const int MON_POTENTIAL_ADDR_OFFSET   = 0x0032C46A;
        // TODO: fame?
        const int MON_GUTS_ADDR_OFFSET        = 0x0032A1F0;   // guts rate
        const int MON_MAIN_ADDR_OFFSET        = 0x0032A1FC;   // main breed
        const int MON_SUB_ADDR_OFFSET         = 0x0032A1FE;   // sub breed
        // TODO: gold peach (found)?

        // player/game data address offsets
        // TODO: player name?
        const int PLA_MONEY_ADDR_OFFSET   = 0x0032C528;
        const int GAME_WEEK_ADDR_OFFSET   = 0x0032C508;
        const int GAME_MONTH_ADDR_OFFSET  = 0x0032C50C;
        const int GAME_YEAR34_ADDR_OFFSET = 0x0032C510;       // year: last 2 digits
        const int GAME_YEAR12_ADDR_OFFSET = 0x0032C514;       // year: first 2 digits
        #endregion // MEMORY ADDRESS OFFSETS

        #region PROCESS DEFINES
        const string MONSTER_RANCHER_PROCESS_NAME = "MF";
        #endregion


        public MRProcessWrapper()
        {
            _gameProcess = null;
            //_gameMemPtr;
            _gameMemBase = 0x00000000;
            _gameProcessName = MONSTER_RANCHER_PROCESS_NAME; // only doing MR1DX (Steam)
            _scratchData = new byte[4];
            _hasRead = 0;
        }
        
        public bool CheckForMonsterRancherProcess()
        {
            bool running = false;

            try
            {
                Process[] possibleMfProcesses = Process.GetProcessesByName(_gameProcessName);
                if (possibleMfProcesses.Length > 0) // Monster Rancher process is running
                {
                    running = true;
                    // only attach to the process if we haven't already
                    if (_gameMemBase == 0)
                    {
                        // default to the first process in the list, in the event that multiple are running
                        _gameProcess = possibleMfProcesses[0];
                        _gameMemPtr = OpenProcess(PROCESS_ALLACCESS, false, _gameProcess.Id);
                        _gameMemBase = (int)_gameProcess.MainModule.BaseAddress;
                    }
                }
                else
                {
                    running = false;
                    _gameProcess = null;
                    _gameMemBase = 0;
                }
            }
            catch // catch everything within here in case we stop abruptly
            {
                running = false;
                _gameProcess = null;
                _gameMemBase = 0;
            }

            return running;
        }


        #region MEMORY ACCESS FUNCTIONS
        /******************** BASE FUNCTIONS ********************/
        private bool MemReadBool(int offset)
        {
            return Convert.ToBoolean(MemReadSingle(offset));
        }

        private int MemReadSingle(int offset)
        {
            ReadProcessMemory(_gameMemPtr, _gameMemBase + offset, _scratchData, 1, ref _hasRead);
            return Convert.ToInt16(_scratchData[0]);
        }

        private int MemReadDouble(int offset)
        {
            ReadProcessMemory(_gameMemPtr, _gameMemBase + offset, _scratchData, 2, ref _hasRead);
            return BitConverter.ToInt16(_scratchData, 0);
        }

        private int MemReadQuad(int offset)
        {
            ReadProcessMemory(_gameMemPtr, _gameMemBase + offset, _scratchData, 4, ref _hasRead);
            return BitConverter.ToInt32(_scratchData, 0);
        }

        /******************** ACCESSORS ********************/
        /////////////////// MONSTER STATS ///////////////////
        public int GetMonLife()
        {
            return MemReadDouble(MON_LIF_ADDR_OFFSET);
        }

        public int GetMonPower()
        {
            return MemReadDouble(MON_POW_ADDR_OFFSET);
        }

        public int GetMonDefense()
        {
            return MemReadDouble(MON_DEF_ADDR_OFFSET);
        }

        public int GetMonSkill()
        {
            return MemReadDouble(MON_SKI_ADDR_OFFSET);
        }

        public int GetMonSpeed()
        {
            return MemReadDouble(MON_SPD_ADDR_OFFSET);
        }

        public int GetMonIntelligence()
        {
            return MemReadDouble(MON_INT_ADDR_OFFSET);
        }

        /////////////////// MONSTER MISC ///////////////////
        public int GetMonLifeSpan()
        {
            return MemReadDouble(MON_LIFESPAN_ADDR_OFFSET);
        }

        public int GetMonAge()
        {
            return MemReadDouble(MON_AGE_ADDR_OFFSET);
        }

        public int GetMonFatigue()
        {
            return MemReadSingle(MON_FATIGUE_ADDR_OFFSET);
        }

        public int GetMonStress()
        {
            return MemReadSingle(MON_STRESS_ADDR_OFFSET);
        }

        public int GetMonSpoil()
        {
            return MemReadSingle(MON_SPOIL_ADDR_OFFSET);
        }

        public int GetMonFear()
        {
            return MemReadSingle(MON_FEAR_ADDR_OFFSET);
        }

        public int GetMonSeriousness()
        {
            return MemReadSingle(MON_SERIOUSNESS_ADDR_OFFSET);
        }

        public int GetMonPotential()
        {
            return MemReadDouble(MON_POTENTIAL_ADDR_OFFSET);
        }

        public int GetMonGutsRate()
        {
            return MemReadSingle(MON_GUTS_ADDR_OFFSET);
        }

        public int GetMonMainBreed()
        {
            return MemReadSingle(MON_MAIN_ADDR_OFFSET);
        }

        public int GetMonSubBreed()
        {
            return MemReadSingle(MON_SUB_ADDR_OFFSET);
        }

        /////////////////// PLAYER/GAME DATA ///////////////////

        public int GetGameWeek()
        {
            return MemReadSingle(GAME_WEEK_ADDR_OFFSET);
        }

        public int GetGameMonth()
        {
            return MemReadSingle(GAME_MONTH_ADDR_OFFSET);
        }

        public int GetGameYear12()
        {
            return MemReadSingle(GAME_YEAR12_ADDR_OFFSET);
        }

        public int GetGameYear34()
        {
            return MemReadSingle(GAME_YEAR34_ADDR_OFFSET);
        }

        public int GetPlayerMoney()
        {
            return MemReadQuad(PLA_MONEY_ADDR_OFFSET);
        }

        // TODO: Add accessors here (if any)
        #endregion
    }
}
