namespace FlowScriptPrototype
{
    partial class NewCategoryForm
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
            this._catNameTextBox = new System.Windows.Forms.TextBox();
            this._catNameLabel = new System.Windows.Forms.Label();
            this._addCatBtn = new System.Windows.Forms.Button();
            this._cancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _catNameTextBox
            // 
            this._catNameTextBox.Location = new System.Drawing.Point(12, 35);
            this._catNameTextBox.Name = "_catNameTextBox";
            this._catNameTextBox.Size = new System.Drawing.Size(296, 20);
            this._catNameTextBox.TabIndex = 0;
            this._catNameTextBox.TextChanged += new System.EventHandler(this._catNameTextBox_TextChanged);
            // 
            // _catNameLabel
            // 
            this._catNameLabel.AutoSize = true;
            this._catNameLabel.Location = new System.Drawing.Point(12, 19);
            this._catNameLabel.Name = "_catNameLabel";
            this._catNameLabel.Size = new System.Drawing.Size(80, 13);
            this._catNameLabel.TabIndex = 1;
            this._catNameLabel.Text = "Category Name";
            // 
            // _addCatBtn
            // 
            this._addCatBtn.Location = new System.Drawing.Point(131, 61);
            this._addCatBtn.Name = "_addCatBtn";
            this._addCatBtn.Size = new System.Drawing.Size(96, 23);
            this._addCatBtn.TabIndex = 2;
            this._addCatBtn.Text = "Add Category";
            this._addCatBtn.UseVisualStyleBackColor = true;
            this._addCatBtn.Click += new System.EventHandler(this._addCatBtn_Click);
            // 
            // _cancelBtn
            // 
            this._cancelBtn.Location = new System.Drawing.Point(233, 61);
            this._cancelBtn.Name = "_cancelBtn";
            this._cancelBtn.Size = new System.Drawing.Size(75, 23);
            this._cancelBtn.TabIndex = 3;
            this._cancelBtn.Text = "Cancel";
            this._cancelBtn.UseVisualStyleBackColor = true;
            this._cancelBtn.Click += new System.EventHandler(this._cancelBtn_Click);
            // 
            // NewCategoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 96);
            this.Controls.Add(this._cancelBtn);
            this.Controls.Add(this._addCatBtn);
            this.Controls.Add(this._catNameLabel);
            this.Controls.Add(this._catNameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "NewCategoryForm";
            this.Text = "Add New Category";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _catNameTextBox;
        private System.Windows.Forms.Label _catNameLabel;
        private System.Windows.Forms.Button _addCatBtn;
        private System.Windows.Forms.Button _cancelBtn;
    }
}