
namespace MView.Forms
{
    partial class CryptographyForm
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
            this.decryptButton = new System.Windows.Forms.Button();
            this.encryptButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.codeCheckBox = new System.Windows.Forms.CheckBox();
            this.verifyCheckBox = new System.Windows.Forms.CheckBox();
            this.saveDirectoryButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.saveDirectoryBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.codeButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.codeBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // decryptButton
            // 
            this.decryptButton.Location = new System.Drawing.Point(497, 182);
            this.decryptButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.decryptButton.Name = "decryptButton";
            this.decryptButton.Size = new System.Drawing.Size(75, 24);
            this.decryptButton.TabIndex = 11;
            this.decryptButton.Text = "Decrypt";
            this.decryptButton.UseVisualStyleBackColor = true;
            this.decryptButton.Click += new System.EventHandler(this.decryptButton_Click);
            // 
            // encryptButton
            // 
            this.encryptButton.Location = new System.Drawing.Point(414, 182);
            this.encryptButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.encryptButton.Name = "encryptButton";
            this.encryptButton.Size = new System.Drawing.Size(75, 24);
            this.encryptButton.TabIndex = 10;
            this.encryptButton.Text = "Encrypt";
            this.encryptButton.UseVisualStyleBackColor = true;
            this.encryptButton.Click += new System.EventHandler(this.encryptButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(410, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "※ A \'data/System.json\' file is required to obtain the encryption code.\r\n";
            // 
            // codeCheckBox
            // 
            this.codeCheckBox.AutoSize = true;
            this.codeCheckBox.Checked = true;
            this.codeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.codeCheckBox.Location = new System.Drawing.Point(13, 130);
            this.codeCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.codeCheckBox.Name = "codeCheckBox";
            this.codeCheckBox.Size = new System.Drawing.Size(266, 19);
            this.codeCheckBox.TabIndex = 5;
            this.codeCheckBox.Text = "Use encryption code for decryption operation";
            this.codeCheckBox.UseVisualStyleBackColor = true;
            this.codeCheckBox.CheckedChanged += new System.EventHandler(this.codeCheckBox_CheckedChanged);
            // 
            // verifyCheckBox
            // 
            this.verifyCheckBox.AutoSize = true;
            this.verifyCheckBox.Checked = true;
            this.verifyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.verifyCheckBox.Location = new System.Drawing.Point(13, 106);
            this.verifyCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.verifyCheckBox.Name = "verifyCheckBox";
            this.verifyCheckBox.Size = new System.Drawing.Size(451, 19);
            this.verifyCheckBox.TabIndex = 4;
            this.verifyCheckBox.Text = "Verify fake header (To check for correct RPG Maker MV encryption resource files.)" +
    "\r\n";
            this.verifyCheckBox.UseVisualStyleBackColor = true;
            // 
            // saveDirectoryButton
            // 
            this.saveDirectoryButton.Location = new System.Drawing.Point(499, 22);
            this.saveDirectoryButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.saveDirectoryButton.Name = "saveDirectoryButton";
            this.saveDirectoryButton.Size = new System.Drawing.Size(50, 25);
            this.saveDirectoryButton.TabIndex = 1;
            this.saveDirectoryButton.Text = "...";
            this.saveDirectoryButton.UseVisualStyleBackColor = true;
            this.saveDirectoryButton.Click += new System.EventHandler(this.saveDirectoryButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Save directory";
            // 
            // saveDirectoryBox
            // 
            this.saveDirectoryBox.Location = new System.Drawing.Point(114, 23);
            this.saveDirectoryBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.saveDirectoryBox.Name = "saveDirectoryBox";
            this.saveDirectoryBox.Size = new System.Drawing.Size(379, 23);
            this.saveDirectoryBox.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.codeButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.codeBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.codeCheckBox);
            this.groupBox1.Controls.Add(this.verifyCheckBox);
            this.groupBox1.Controls.Add(this.saveDirectoryButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.saveDirectoryBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 14);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(560, 160);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Encrypt/Decrypt";
            // 
            // codeButton
            // 
            this.codeButton.Location = new System.Drawing.Point(499, 53);
            this.codeButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.codeButton.Name = "codeButton";
            this.codeButton.Size = new System.Drawing.Size(50, 25);
            this.codeButton.TabIndex = 3;
            this.codeButton.Text = "...";
            this.codeButton.UseVisualStyleBackColor = true;
            this.codeButton.Click += new System.EventHandler(this.codeButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "Encryption code";
            // 
            // codeBox
            // 
            this.codeBox.Location = new System.Drawing.Point(114, 54);
            this.codeBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.codeBox.Name = "codeBox";
            this.codeBox.Size = new System.Drawing.Size(379, 23);
            this.codeBox.TabIndex = 2;
            // 
            // CryptographyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 221);
            this.Controls.Add(this.decryptButton);
            this.Controls.Add(this.encryptButton);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CryptographyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MView Cryptography";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button decryptButton;
        private System.Windows.Forms.Button encryptButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox codeCheckBox;
        private System.Windows.Forms.CheckBox verifyCheckBox;
        private System.Windows.Forms.Button saveDirectoryButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox saveDirectoryBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button codeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox codeBox;
    }
}