using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using UnityEngine;

namespace PolishedMachine.Bugfixes {
    public class BugfixManager {
        public bool enableLogging = true;

        public BugfixManager() {
            Application.RegisterLogCallback( new Application.LogCallback( this.HandleLog ) ); //Unity logging
        }

        public void EnableAllBugfixes() {
            enableLogging = true;
        }
        public void DisableAllBugfixes() {
            enableLogging = false;
        }

        public void HandleLog(string logString, string stackTrace, LogType type) {
            if( enableLogging ) {
                if( type == LogType.Error || type == LogType.Exception ) {
                    File.AppendAllText( "exceptionLog.txt", logString + Environment.NewLine );
                    File.AppendAllText( "exceptionLog.txt", stackTrace + Environment.NewLine );
                    return;
                }
                File.AppendAllText( "consoleLog.txt", logString + Environment.NewLine );
            }
        }
    }
}