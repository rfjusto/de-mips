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
        static public void PreprocessComments(LinkedList<ProgramLine> code, string commentMarker) //TODO: move to proper file
        {
            foreach (ProgramLine line in code)
            {
                if (line.Assembly.Contains(commentMarker))
                    line.Assembly = line.Assembly.Substring(0, line.Assembly.IndexOf(commentMarker)).Trim();
            }
        }

        /// <summary>
        /// Reconstructs each line to include only one space between tokens.
        /// </summary>
        /// <param name="code">Code to process.</param>
        static public void PreprocessWhiteSpace(LinkedList<ProgramLine> code)
        {
            foreach (ProgramLine line in code)
            {
                string processedLine = "";
                string[] lineTokens = line.Assembly.Split(' ');

                foreach (string token in lineTokens)
                    if (!token.Trim().Equals(""))
                        processedLine += token + " ";

                processedLine = processedLine.Trim(); //HACK: the above line always leaves a trailing space.

                line.Assembly = processedLine;

            }

        }
    }
}
