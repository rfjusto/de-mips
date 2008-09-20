using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class UtilPreprocessor
    {
        /// <summary>
        /// Splits a lines into code and comment portions.
        /// </summary>
        /// <param name="code">Code to process.</param>
        /// <param name="commentMarker">Character(s) that mark regions of comments.</param>
        static public void PreprocessComments(string[] code, string commentMarker)
        {
            for(int i = 0; i < code.Length; i++)
            {
                if (code[i].Contains(commentMarker))
                    code[i] = code[i].Substring(0, code[i].IndexOf(commentMarker)).Trim();
            }
        }

        /// <summary>
        /// Reconstructs each line to include only one space between tokens.
        /// </summary>
        /// <param name="code">Code to process.</param>
        static public void PreprocessWhiteSpace(string[] code)
        {
            for(int i = 0; i < code.Length; i++)
            {
                string[] lineTokens = code[i].Split(' ');
                string processedLine = "";
                
                foreach (string token in lineTokens)
                    if (!token.Trim().Equals(""))
                        processedLine += token + " ";

                processedLine = processedLine.Trim(); //HACK: the above line always leaves a trailing space.
                code[i] = processedLine;

            }
        }
    }
}
