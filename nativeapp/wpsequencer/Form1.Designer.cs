
namespace wpsequencer
{
    partial class Form1
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
            this.TextBoxTxCmd = new System.Windows.Forms.TextBox();
            this.BLUE = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gold = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBoxTxParam = new System.Windows.Forms.TextBox();
            this.TextBoxRxParam = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TextBoxRxCmd = new System.Windows.Forms.TextBox();
            this.TextBoxNameLocation = new System.Windows.Forms.TextBox();
            this.TextBoxPhones = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.none = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextBoxTxCmd
            // 
            this.TextBoxTxCmd.Location = new System.Drawing.Point(77, -3);
            this.TextBoxTxCmd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TextBoxTxCmd.Name = "TextBoxTxCmd";
            this.TextBoxTxCmd.Size = new System.Drawing.Size(119, 20);
            this.TextBoxTxCmd.TabIndex = 0;
            // 
            // BLUE
            // 
            this.BLUE.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BLUE.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BLUE.Location = new System.Drawing.Point(660, 4);
            this.BLUE.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.BLUE.Name = "BLUE";
            this.BLUE.Size = new System.Drawing.Size(113, 26);
            this.BLUE.TabIndex = 1;
            this.BLUE.Text = "1. GO BLUE !";
            this.BLUE.UseVisualStyleBackColor = false;
            this.BLUE.Click += new System.EventHandler(this.BLUE_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, -1);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Last Tx Cmd";
            // 
            // gold
            // 
            this.gold.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.gold.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gold.Location = new System.Drawing.Point(777, 4);
            this.gold.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gold.Name = "gold";
            this.gold.Size = new System.Drawing.Size(119, 26);
            this.gold.TabIndex = 5;
            this.gold.Text = "2. GET GOLD !";
            this.gold.UseVisualStyleBackColor = false;
            this.gold.Click += new System.EventHandler(this.GOLD_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(226, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Last Tx Param";
            // 
            // TextBoxTxParam
            // 
            this.TextBoxTxParam.Location = new System.Drawing.Point(304, 0);
            this.TextBoxTxParam.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TextBoxTxParam.Multiline = true;
            this.TextBoxTxParam.Name = "TextBoxTxParam";
            this.TextBoxTxParam.Size = new System.Drawing.Size(353, 18);
            this.TextBoxTxParam.TabIndex = 8;
            // 
            // TextBoxRxParam
            // 
            this.TextBoxRxParam.Location = new System.Drawing.Point(304, 18);
            this.TextBoxRxParam.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TextBoxRxParam.Multiline = true;
            this.TextBoxRxParam.Name = "TextBoxRxParam";
            this.TextBoxRxParam.Size = new System.Drawing.Size(353, 18);
            this.TextBoxRxParam.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(224, 18);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Last Rx Param";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 17);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Last Rx Cmd";
            // 
            // TextBoxRxCmd
            // 
            this.TextBoxRxCmd.Location = new System.Drawing.Point(77, 15);
            this.TextBoxRxCmd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TextBoxRxCmd.Name = "TextBoxRxCmd";
            this.TextBoxRxCmd.Size = new System.Drawing.Size(119, 20);
            this.TextBoxRxCmd.TabIndex = 9;
            // 
            // TextBoxNameLocation
            // 
            this.TextBoxNameLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxNameLocation.Location = new System.Drawing.Point(23, 35);
            this.TextBoxNameLocation.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TextBoxNameLocation.Name = "TextBoxNameLocation";
            this.TextBoxNameLocation.Size = new System.Drawing.Size(668, 27);
            this.TextBoxNameLocation.TabIndex = 13;
            // 
            // TextBoxPhones
            // 
            this.TextBoxPhones.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxPhones.Location = new System.Drawing.Point(747, 34);
            this.TextBoxPhones.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TextBoxPhones.Name = "TextBoxPhones";
            this.TextBoxPhones.Size = new System.Drawing.Size(251, 27);
            this.TextBoxPhones.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(-1, 37);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Ask";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(701, 37);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Phones";
            // 
            // none
            // 
            this.none.Location = new System.Drawing.Point(913, 4);
            this.none.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.none.Name = "none";
            this.none.Size = new System.Drawing.Size(102, 22);
            this.none.TabIndex = 17;
            this.none.Text = "Or, set ? if no gold";
            this.none.UseVisualStyleBackColor = true;
            this.none.Click += new System.EventHandler(this.none_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1382, 69);
            this.Controls.Add(this.none);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TextBoxPhones);
            this.Controls.Add(this.TextBoxNameLocation);
            this.Controls.Add(this.TextBoxRxParam);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TextBoxRxCmd);
            this.Controls.Add(this.TextBoxTxParam);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.gold);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BLUE);
            this.Controls.Add(this.TextBoxTxCmd);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "wpsequencer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxTxCmd;
        private System.Windows.Forms.Button BLUE;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button gold;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBoxTxParam;
        private System.Windows.Forms.TextBox TextBoxRxParam;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TextBoxRxCmd;
        private System.Windows.Forms.TextBox TextBoxNameLocation;
        private System.Windows.Forms.TextBox TextBoxPhones;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button none;
    }
}

