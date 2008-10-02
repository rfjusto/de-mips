namespace DeMIPS
{
    partial class DeAssemblyForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TextBoxOutput = new System.Windows.Forms.TextBox();
            this.TextBoxInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBoxFileName = new System.Windows.Forms.TextBox();
            this.ButtonSelectFile = new System.Windows.Forms.Button();
            this.ButtonDecompile = new System.Windows.Forms.Button();
            this.ButtonQuit = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LabelSynced = new System.Windows.Forms.Label();
            this.TextBoxConsole = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TextBoxOutput
            // 
            this.TextBoxOutput.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxOutput.Location = new System.Drawing.Point(459, 58);
            this.TextBoxOutput.Multiline = true;
            this.TextBoxOutput.Name = "TextBoxOutput";
            this.TextBoxOutput.ReadOnly = true;
            this.TextBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBoxOutput.Size = new System.Drawing.Size(425, 486);
            this.TextBoxOutput.TabIndex = 2;
            this.TextBoxOutput.WordWrap = false;
            // 
            // TextBoxInput
            // 
            this.TextBoxInput.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxInput.Location = new System.Drawing.Point(15, 58);
            this.TextBoxInput.Multiline = true;
            this.TextBoxInput.Name = "TextBoxInput";
            this.TextBoxInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBoxInput.Size = new System.Drawing.Size(425, 486);
            this.TextBoxInput.TabIndex = 3;
            this.TextBoxInput.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "MIPS Assembly Input:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(456, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Generic C Output:";
            // 
            // TextBoxFileName
            // 
            this.TextBoxFileName.Location = new System.Drawing.Point(77, 13);
            this.TextBoxFileName.Name = "TextBoxFileName";
            this.TextBoxFileName.Size = new System.Drawing.Size(363, 20);
            this.TextBoxFileName.TabIndex = 6;
            // 
            // ButtonSelectFile
            // 
            this.ButtonSelectFile.Location = new System.Drawing.Point(484, 10);
            this.ButtonSelectFile.Name = "ButtonSelectFile";
            this.ButtonSelectFile.Size = new System.Drawing.Size(100, 25);
            this.ButtonSelectFile.TabIndex = 7;
            this.ButtonSelectFile.Text = "Select File";
            this.ButtonSelectFile.UseVisualStyleBackColor = true;
            this.ButtonSelectFile.Click += new System.EventHandler(this.ButtonSelectFile_Click);
            // 
            // ButtonDecompile
            // 
            this.ButtonDecompile.Location = new System.Drawing.Point(619, 10);
            this.ButtonDecompile.Name = "ButtonDecompile";
            this.ButtonDecompile.Size = new System.Drawing.Size(125, 25);
            this.ButtonDecompile.TabIndex = 8;
            this.ButtonDecompile.Text = "Load and Decompile";
            this.ButtonDecompile.UseVisualStyleBackColor = true;
            this.ButtonDecompile.Click += new System.EventHandler(this.ButtonDecompile_Click);
            // 
            // ButtonQuit
            // 
            this.ButtonQuit.Location = new System.Drawing.Point(777, 10);
            this.ButtonQuit.Name = "ButtonQuit";
            this.ButtonQuit.Size = new System.Drawing.Size(100, 25);
            this.ButtonQuit.TabIndex = 9;
            this.ButtonQuit.Text = "Quit";
            this.ButtonQuit.UseVisualStyleBackColor = true;
            this.ButtonQuit.Click += new System.EventHandler(this.ButtonQuit_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Active File:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(625, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Status:";
            // 
            // LabelSynced
            // 
            this.LabelSynced.AutoSize = true;
            this.LabelSynced.Location = new System.Drawing.Point(671, 41);
            this.LabelSynced.Name = "LabelSynced";
            this.LabelSynced.Size = new System.Drawing.Size(53, 13);
            this.LabelSynced.TabIndex = 12;
            this.LabelSynced.Text = "Unknown";
            // 
            // TextBoxConsole
            // 
            this.TextBoxConsole.Location = new System.Drawing.Point(15, 573);
            this.TextBoxConsole.Multiline = true;
            this.TextBoxConsole.Name = "TextBoxConsole";
            this.TextBoxConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBoxConsole.Size = new System.Drawing.Size(869, 142);
            this.TextBoxConsole.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 551);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Decompilation Results:";
            // 
            // DeAssemblyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 727);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TextBoxConsole);
            this.Controls.Add(this.LabelSynced);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ButtonQuit);
            this.Controls.Add(this.ButtonDecompile);
            this.Controls.Add(this.ButtonSelectFile);
            this.Controls.Add(this.TextBoxFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxInput);
            this.Controls.Add(this.TextBoxOutput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "DeAssemblyForm";
            this.Text = "DeMIPS - v0.1 alpha";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxOutput;
        private System.Windows.Forms.TextBox TextBoxInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBoxFileName;
        private System.Windows.Forms.Button ButtonSelectFile;
        private System.Windows.Forms.Button ButtonDecompile;
        private System.Windows.Forms.Button ButtonQuit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label LabelSynced;
        private System.Windows.Forms.TextBox TextBoxConsole;
        private System.Windows.Forms.Label label6;
    }
}

