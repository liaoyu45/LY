using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KeyboardSynchronizer {
    static class Tips {
        public static readonly Dictionary<string, string> All = new Dictionary<string, string>(new[] {
            new[] {
                "quit",
                 "Click 'quit' button twice quickly or use any other ways to quit. Press 'F1' to see some tips."
            },
            new[] {
                "f1",
                 "Press 'F1' to show next tip, Press 'Shift' + 'F1' to show previous tip."
            },
            new[] {
                "pick",
                 "Move cursor to a target window, then press enter | space."
            },
            new[] {
                "specify0",
                 "Specify a window, press and hold '`', press 0-9 once or more to specify a window, then..."
            },
            new[] {
                "specify1",
                 "press a NaN key to stop specifying, after '`' is released, then..."
            },
            new[] {
                "specify2",
                 "the NaN key will be sent to the window."
            },
            new[] {
                "maxmin",
                "Let one windows be foreground, then press and hold '`', and press an NaN key 3 times, see what would happen."
            },
            new[] {
                "already",
                 "Each window can be included for once."
            },
            new[] {
                "whatIsMainWindow",
                 "The synchronization will be sent when one of main windows is foreground."
            },
            new[] {
                "hello",
                 "Hello, this is a free software."
            },
            new[] {
                "retry",
                 "This software self will not be included for synchronization."
            },
            new[] {
                "yeller",
                 "Free to be alive."
            },
            new[] {
                "toggle",
                 "Whenever want to start/pause, press and hold '`' + any NaN key once."
            },
            new[] {
                "title",
                 "Keyboard Synchronizer"
            },
            new[] {
                "mainWindow",
                 "Select one or more windows as main windows."
            },
            new[] {
                "free",
                 $"Press and hold '{Keys.LControlKey}' and press '{string.Join(string.Empty, Yeller.FREE)}' to set this at top."
            },
            new[] {
                "excludedKeys",
                 "Excluded keys will not be sychronized, focus the textbox and press 'ESC' to clear them."
            },
            new[] {
                "keyup",
                 "To toggle modes(text/game), press '`' + an NaN key twice."
            },
            new[] {
                "mode",
                 "Key up events will not been sent in text mode, which are required in game mode."
            },
            new[] {
                "additional0",
                 "Key '`' is on left side of '1' on main keyboard, NaN means 'not a number'."
            },
            new[] {
                "additional1",
                 "Index starts from '0'."
            },
            new[] {
                "me",
                "Me: liaoyu45@163.com, the auther of this software, anywhere."
            }
        }.ToDictionary(arr => arr[0], arr => arr[1]));

        public static readonly InfiniteEnumerator<KeyValuePair<string, string>> Enumerator = new InfiniteEnumerator<KeyValuePair<string, string>>(All.ToList());
    }
}