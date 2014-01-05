namespace WordRead
{
    partial class BoardDisplay
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
            this.pBoard = new System.Windows.Forms.Panel();
            this.pLetters = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.WordBox = new System.Windows.Forms.ListBox();
            this.GenerateButton = new System.Windows.Forms.Button();
            this.PlaceButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.calcTime = new System.Windows.Forms.Label();
            this.calTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pBoard
            // 
            this.pBoard.Location = new System.Drawing.Point(1, 1);
            this.pBoard.Name = "pBoard";
            this.pBoard.Size = new System.Drawing.Size(450, 450);
            this.pBoard.TabIndex = 0;
            // 
            // pLetters
            // 
            this.pLetters.Location = new System.Drawing.Point(137, 466);
            this.pLetters.Name = "pLetters";
            this.pLetters.Size = new System.Drawing.Size(210, 30);
            this.pLetters.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(457, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Possible Words";
            // 
            // WordBox
            // 
            this.WordBox.Enabled = false;
            this.WordBox.FormattingEnabled = true;
            this.WordBox.Location = new System.Drawing.Point(461, 38);
            this.WordBox.Name = "WordBox";
            this.WordBox.Size = new System.Drawing.Size(233, 381);
            this.WordBox.TabIndex = 4;
            // 
            // GenerateButton
            // 
            this.GenerateButton.Location = new System.Drawing.Point(461, 425);
            this.GenerateButton.Name = "GenerateButton";
            this.GenerateButton.Size = new System.Drawing.Size(110, 26);
            this.GenerateButton.TabIndex = 5;
            this.GenerateButton.Text = "Generate Words";
            this.GenerateButton.UseVisualStyleBackColor = true;
            this.GenerateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // PlaceButton
            // 
            this.PlaceButton.Location = new System.Drawing.Point(582, 426);
            this.PlaceButton.Name = "PlaceButton";
            this.PlaceButton.Size = new System.Drawing.Size(112, 25);
            this.PlaceButton.TabIndex = 6;
            this.PlaceButton.Text = "Place Word";
            this.PlaceButton.UseVisualStyleBackColor = true;
            this.PlaceButton.Click += new System.EventHandler(this.PlaceButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Purple;
            this.label2.Location = new System.Drawing.Point(101, 520);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(523, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "Welcome to Deep Purple! May I take your order?";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(80, 470);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Tiles:";
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLabel.ForeColor = System.Drawing.Color.Purple;
            this.timeLabel.Location = new System.Drawing.Point(410, 469);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(168, 24);
            this.timeLabel.TabIndex = 9;
            this.timeLabel.Text = "Time To Calculate:";
            // 
            // calcTime
            // 
            this.calcTime.AutoSize = true;
            this.calcTime.Location = new System.Drawing.Point(579, 470);
            this.calcTime.Name = "calcTime";
            this.calcTime.Size = new System.Drawing.Size(0, 13);
            this.calcTime.TabIndex = 10;
            // 
            // calTime
            // 
            this.calTime.AutoSize = true;
            this.calTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calTime.ForeColor = System.Drawing.Color.Red;
            this.calTime.Location = new System.Drawing.Point(578, 470);
            this.calTime.Name = "calTime";
            this.calTime.Size = new System.Drawing.Size(60, 24);
            this.calTime.TabIndex = 11;
            this.calTime.Text = "\"blah\"";
            // 
            // BoardDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 567);
            this.Controls.Add(this.calTime);
            this.Controls.Add(this.calcTime);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PlaceButton);
            this.Controls.Add(this.GenerateButton);
            this.Controls.Add(this.WordBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pLetters);
            this.Controls.Add(this.pBoard);
            this.KeyPreview = true;
            this.Name = "BoardDisplay";
            this.Text = "Deep Purple - A Scrabble Solver By Ethan Benjamin";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pBoard;
        private System.Windows.Forms.Panel pLetters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox WordBox;
        private System.Windows.Forms.Button GenerateButton;
        private System.Windows.Forms.Button PlaceButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Label calcTime;
        private System.Windows.Forms.Label calTime;

    }
}