using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class FrontendMIPS : IFrontend
    {
        public void TranslateLine(ProgramLine line, IProgramChunk lineChunk)
        {
            //detect labels
            if (line.Assembly.Contains(":"))
                lineChunk = new ProgramChunkJumpTarget(line.Assembly.Substring(line.Assembly.IndexOf(":")));
            else
                throw new Exception("FrontendMIPS: Unidentified assembly encountered during translation.");
        }

        /// <summary>
        /// TODO: full preprocessor. checks for: malformed input, labels existing on same line as keyword, etc.
        /// </summary>
        /// <param name="code">Code to process.</param>
        static public void Preprocess(LinkedList<ProgramLine> code)
        {
            //TODO: split labels from code
            UtilPreprocessor.PreprocessComments(code, "#");
            UtilPreprocessor.PreprocessWhiteSpace(code);
        }
    }

    //FUTURE: Setting up for supporting more than MIPS.
    interface IFrontend
    {
        void TranslateLine(ProgramLine line, IProgramChunk lineChunk);
        //void Preprocess(LinkedList<ProgramLine> code);
    }
}
