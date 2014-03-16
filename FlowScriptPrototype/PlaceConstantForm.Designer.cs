namespace FlowScriptPrototype
{
    partial class PlaceConstantForm
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
            if (disposing && (components != null)) {
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
            this._cancelBtn = new System.Windows.Forms.Button();
            this._createBtn = new System.Windows.Forms.Button();
            this._constValLabel = new System.Windows.Forms.Label();
            this._constValTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _cancelBtn
            // 
            this._cancelBtn.Location = new System.Drawing.Point(233, 67);
            this._cancelBtn.Name = "_cancelBtn";
            this._cancelBtn.Size = new System.Drawing.Size(75, 23);
            this._cancelBtn.TabIndex = 7;
            this._cancelBtn.Text = "Cancel";
            this._cancelBtn.UseVisualStyleBackColor = true;
            this._cancelBtn.Click += new System.EventHandler(this._cancelBtn_Click);
            // 
            // _createBtn
            // 
            this._createBtn.Location = new System.Drawing.Point(131, 67);
            this._createBtn.Name = "_createBtn";
            this._createBtn.Size = new System.Drawing.Size(96, 23);
            this._createBtn.TabIndex = 6;
            this._createBtn.Text = "Create";
            this._createBtn.UseVisualStyleBackColor = true;
            this._createBtn.Click += new System.EventHandler(this._createBtn_Click);
            // 
            // _constValLabel
            // 
            this._constValLabel.AutoSize = true;
            this._constValLabel.Location = new System.Drawing.Point(12, 19);
            this._constValLabel.Name = "_constValLabel";
            this._constValLabel.Size = new System.Drawing.Size(79, 13);
            this._constValLabel.TabIndex = 5;
            this._constValLabel.Text = "Constant Value";
            // 
            // _constValTextBox
            // 
            this._constValTextBox.Location = new System.Drawing.Point(12, 35);
            this._constValTextBox.Name = "_constValTextBox";
            this._constValTextBox.Size = new System.Drawing.Size(296, 20);
            this._constValTextBox.TabIndex = 4;
            this._constValTextBox.TextChanged += new System.EventHandler(this._constValTextBox_TextChanged);
            // 
            // PlaceConstantForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 102);
            this.Controls.Add(this._cancelBtn);
            this.Controls.Add(this._createBtn);
            this.Controls.Add(this._constValLabel);
            this.Controls.Add(this._constValTextBox);
            this.Name = "PlaceConstantForm";
            this.Text = "Place Constant";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _cancelBtn;
        private System.Windows.Forms.Button _createBtn;
        private System.Windows.Forms.Label _constValLabel;
        private System.Windows.Forms.TextBox _constValTextBox;
    }
}