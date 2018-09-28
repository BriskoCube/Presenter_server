using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Persenter_server
{
    public delegate void OnInput(string inputText);

    public static class AdvancedConsole
    {
        private const char COMMAND_FIRST = '/';
        const string NEW_CMD_LINE = " > ";


        public static event OnInput OnInputEvent;
        public static event OnInput OnCommandInputEvent;
        public static event OnInput OnTextInputEvent;

        private static Thread _inputThread;

        private static ConsoleColor _errorTextColor = ConsoleColor.Red;
        private static ConsoleColor _warningTextColor = ConsoleColor.Yellow;

        public static ConsoleColor ErrorTextColor { get => _errorTextColor; set => _errorTextColor = value; }
        public static ConsoleColor WarningTextColor { get => _warningTextColor; set => _warningTextColor = value; }


        /// <summary>
        /// Mandatory to use Events
        /// Start threads
        /// </summary>
        public static void Start()
        {
            Logger.Init();
            InputLoop();
        }

        /// <summary>
        /// Loop for console input. Used to trigger input event
        /// </summary>
        private static void InputLoop()
        {
            _inputThread = new Thread(() => {

                try
                {
                    while (true)
                    {
                        string input = "";

                        try
                        {
                            // Allow to stop thread
                            input = Reader.ReadLine(1000);
                        }
                        catch(TimeoutException ex)
                        {
                            input = null;
                        }

                        // Needed for thread to interrupt 
                        Thread.Sleep(1);

                        if(input != null)
                        {
                            if(input.Length > 0)
                                if (input[0] == COMMAND_FIRST)
                                    OnCommandInputEvent?.Invoke(input);
                                else
                                    OnTextInputEvent?.Invoke(input);

                            OnInputEvent?.Invoke(input);
                        }

                    }
                }
                // On thread stopped
                catch (ThreadInterruptedException ex)
                {
                }
            
            });
            _inputThread.Start();
            Logger.Log().Info("InputLoop Thread Started");
        }                                        

        /// <summary>
        /// Write and log an error
        /// </summary>
        /// <param name="text"></param>
        public static void WriteError(string text)
        {
            WriteAlternativeColor("Error: {0}", text, _errorTextColor);
            Logger.Log().Error(text);
        }

        /// <summary>
        /// Wrinte and log an warning
        /// </summary>
        /// <param name="text"></param>
        public static void WriteWarning(string text)
        {
            WriteAlternativeColor("Warning: {0}", text, _warningTextColor);
            Logger.Log().Warn(text);
        }

        public static void Write(string text)
        {
            WriteAlternativeColor(text);
        }

        /// <summary>
        /// Display a string with specific format and color.
        /// Reset colors after display
        /// </summary>
        /// <param name="format">String.Format string, only a single string si allowed</param>
        /// <param name="text">String to insert in format</param>
        /// <param name="foreground">Foreground color</param>
        /// <param name="background">Background color</param>
        public static void WriteAlternativeColor(string format, object text, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            WriteAlternativeColor(string.Format(format, text), foreground, background);
        }

        /// <summary>
        /// Display a string with specific format and color.
        /// Reset colors after display
        /// </summary>
        /// <param name="format">String.Format string, multiple strings are allowed</param>
        /// <param name="foreground">Foreground color</param>
        /// <param name="background">Background color</param>
        /// <param name="text">Strings to insert in format</param>
        public static void WriteAlternativeColor(string format, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black, params object[] text)
        {
            WriteAlternativeColor(string.Format(format, text), foreground, background);
        }

        /// <summary>
        /// Display a string with a specific color
        /// Reset colors after display
        /// </summary>
        /// <param name="text">String to write in console</param>
        /// <param name="foreground">Foreground color</param>
        /// <param name="background">Background color</param>
        public static void WriteAlternativeColor(string text, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black, bool newLine = true)
        {
            ConsoleColor lastForColor = Console.ForegroundColor;
            ConsoleColor lastBackColor = Console.BackgroundColor;

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            if (newLine)
                Console.WriteLine(text);
            else
                Console.Write(text);

            Console.ForegroundColor = lastForColor;
            Console.BackgroundColor = lastBackColor;

        }

        public static void NewCMDLine()
        {
            WriteAlternativeColor(NEW_CMD_LINE, ConsoleColor.DarkYellow, ConsoleColor.Black, false);
        }

        /// <summary>
        /// Stop threads
        /// </summary>
        public static void Stop()
        {
            _inputThread?.Interrupt();
            Logger.Log().Info("InputLoop Thread Stopped");
        }

        /// <summary>
        /// Class to manage a timeout ReadLine
        /// https://stackoverflow.com/questions/57615/how-to-add-a-timeout-to-console-readline by JSQuareD
        /// </summary>
        private class Reader
        {
            private static Thread inputThread;
            private static AutoResetEvent getInput, gotInput;
            private static string input;

            static Reader()
            {
                getInput = new AutoResetEvent(false);
                gotInput = new AutoResetEvent(false);
                inputThread = new Thread(reader);
                inputThread.IsBackground = true;
                inputThread.Start();
            }

            private static void reader()
            {
                while (true)
                {
                    getInput.WaitOne();
                    input = Console.ReadLine();
                    gotInput.Set();
                }
            }

            /// <summary>
            /// ReadLine with timeout
            /// </summary>
            /// <param name="timeOutMillisecs">Timeout in ms</param>
            /// <returns></returns>
            public static string ReadLine(int timeOutMillisecs = Timeout.Infinite)
            {
                getInput.Set();
                bool success = gotInput.WaitOne(timeOutMillisecs);
                if (success)
                    return input;
                else
                    throw new TimeoutException("User did not provide input within the timelimit.");
            }
        }

    }


}
