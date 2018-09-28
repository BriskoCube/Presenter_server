using System;
using System.Collections.Generic;
using System.Text;

namespace Persenter_server
{
    public static class ExtentionMethods
    {
        private const int MAX_LAST_LENGTH = 10;

        public static bool IsIn(this char chr, string list)
        {
            foreach(char c in list)
            {
                if (c == chr)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Line return on word
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lineLen"></param>
        /// <param name="eachLine"></param>
        /// <param name="breakable"></param>
        /// <returns></returns>
        public static string AutoReturn(this string str, int lineLen = -1, string eachLine = "", string breakable=" ,.")
        {
            if (lineLen == -1)
                lineLen = Console.WindowWidth - 16;

            Queue<int> linesLen = new Queue<int>();

            int last = 0;

            // Create line break list
            for (int i = 0; i < str.Length; i++)
            {
                // Keep line return in string
                if (str[i] == '\n')
                    last = i;

                // Once max length reached
                if ((i - last + 1) % lineLen == 0)
                {
                    int back = 0;

                    // Go back until allowed break char
                    while ((i - back >= 0) && !str[i - back].IsIn(breakable) && back < MAX_LAST_LENGTH) back++;

                    // Add line break to break list
                    if (back >= MAX_LAST_LENGTH)
                        linesLen.Enqueue(last = i);
                    else
                        linesLen.Enqueue(last = (i - back));

                    // Start seeking from last inserted char
                    i = last;
                }
            }

            string output = eachLine;
            for (int i = 0; i < str.Length; i++)
            {
                output += str[i];

                // New line in string, add start line char
                if (str[i] == '\n')
                    output += eachLine;

                int val;
                if (linesLen.TryPeek(out val) && val == i)
                {
                    output += "\n" + eachLine;
                    linesLen.Dequeue();
                }
            }

            return output;
        }
    }
}
