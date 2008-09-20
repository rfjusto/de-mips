using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class FrontendMIPS : IFrontend
    {
        public IProgramChunk TranslateLine(string assembly, ProgramBlock parentBlock)
        {
            IProgramChunk translatedChunk = null;

            //***LABELS***
            if (assembly.Contains(":"))
            {
                string labelName = assembly.Substring(0, assembly.IndexOf(":"));

                //label does not exist as an orphan
                if(parentBlock.GetOrphanJumpTargetByLabel(labelName) == null)
                    translatedChunk = new ProgramChunkJumpTarget(labelName);
                //label does exist as an orphan.
                else
                {
                    translatedChunk = parentBlock.GetOrphanJumpTargetByLabel(labelName);
                    parentBlock.RemoveOrphanJumpTarget(parentBlock.GetOrphanJumpTargetByLabel(labelName));
                }
            }
            //**KEYWORDS**
            else
            {
                string lineKeyword = assembly.Split(' ')[0];
                string[] lineParameters = assembly.Substring(assembly.IndexOf(' ') + 1).Split(' ');

                switch (lineKeyword)
                {
                    #region J - Unconditional Jump
                    case "j":
                    {
                        ProgramChunkJumpTarget target = parentBlock.GetJumpTargetByLabel(lineParameters[0]);

                        //label does not exist but should be defined later. create an orphan placeholder.
                        if (target == null)
                        {
                            target = new ProgramChunkJumpTarget(lineParameters[0]);
                            parentBlock.AddOrphanJumpTarget(target);
                        }

                        translatedChunk = new ProgramChunkJumpUnconditional(target);
                    }
                    break;
                    #endregion
                }
            }

            if (translatedChunk == null)
            {
                translatedChunk = new ProgramChunkNoOperation();
                UtilDebugConsole.AddException(new Exception("FrontendMIPS - Unidentified assembly encountered during translation."));
            }

            return translatedChunk;
        }

        /// <summary>
        /// TODO: full preprocessor. checks for: malformed input, labels existing on same line as keyword, etc.
        /// </summary>
        /// <param name="code">Code to process.</param>
        public void Preprocess(string[] file)
        {
            //TODO: split labels from code
            UtilPreprocessor.PreprocessComments(file, "#");
            UtilPreprocessor.PreprocessWhiteSpace(file);
        }
    }

    //FUTURE: Setting up for supporting more than MIPS.
    interface IFrontend
    {
        IProgramChunk TranslateLine(string assembly, ProgramBlock parentBlock);
        void Preprocess(string[] file);
    }
}
