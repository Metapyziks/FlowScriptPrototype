namespace FlowScriptPrototype
{
    partial class NewNodeForm
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
            this._addNodeBtn = new System.Windows.Forms.Button();
            this._nodeNameLabel = new System.Windows.Forms.Label();
            this._nodeNameTextBox = new System.Windows.Forms.TextBox();
            this._inputCountNUD = new System.Windows.Forms.NumericUpDown();
            this._inputCountLabel = new System.Windows.Forms.Label();
            this._outputCountLabel = new System.Windows.Forms.Label();
            this._outputCountNUD = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this._inputCountNUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._outputCountNUD)).BeginInit();
            this.SuspendLayout();
            // 
            // _cancelBtn
            // 
            this._cancelBtn.Location = new System.Drawing.Point(233, 102);
            this._cancelBtn.Name = "_cancelBtn";
            this._cancelBtn.Size = new System.Drawing.Size(75, 23);
            this._cancelBtn.TabIndex = 7;
            this._cancelBtn.Text = "Cancel";
            this._cancelBtn.UseVisualStyleBackColor = true;
            this._cancelBtn.Click += new System.EventHandler(this._cancelBtn_Click);
            // 
            // _addNodeBtn
            // 
            this._addNodeBtn.Location = new System.Drawing.Point(131, 102);
            this._addNodeBtn.Name = "_addNodeBtn";
            this._addNodeBtn.Size = new System.Drawing.Size(96, 23);
            this._addNodeBtn.TabIndex = 6;
            this._addNodeBtn.Text = "Add Node";
            this._addNodeBtn.UseVisualStyleBackColor = true;
            this._addNodeBtn.Click += new System.EventHandler(this._addNodeBtn_Click);
            // 
            // _nodeNameLabel
            // 
            this._nodeNameLabel.AutoSize = true;
            this._nodeNameLabel.Location = new System.Drawing.Point(12, 9);
            this._nodeNameLabel.Name = "_nodeNameLabel";
            this._nodeNameLabel.Size = new System.Drawing.Size(64, 13);
            this._nodeNameLabel.TabIndex = 5;
            this._nodeNameLabel.Text = "Node Name";
            // 
            // _nodeNameTextBox
            // 
            this._nodeNameTextBox.Location = new System.Drawing.Point(12, 25);
            this._nodeNameTextBox.Name = "_nodeNameTextBox";
            this._nodeNameTextBox.Size = new System.Drawing.Size(296, 20);
            this._nodeNameTextBox.TabIndex = 4;
            this._nodeNameTextBox.TextChanged += new System.EventHandler(this._nodeNameTextBox_TextChanged);
            // 
            // _inputCountNUD
            // 
            this._inputCountNUD.Location = new System.Drawing.Point(12, 70);
            this._inputCountNUD.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this._inputCountNUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._inputCountNUD.Name = "_inputCountNUD";
            this._inputCountNUD.Size = new System.Drawing.Size(145, 20);
            this._inputCountNUD.TabIndex = 8;
            this._inputCountNUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _inputCountLabel
            // 
            this._inputCountLabel.AutoSize = true;
            this._inputCountLabel.Location = new System.Drawing.Point(12, 54);
            this._inputCountLabel.Name = "_inputCountLabel";
            this._inputCountLabel.Size = new System.Drawing.Size(62, 13);
            this._inputCountLabel.TabIndex = 10;
            this._inputCountLabel.Text = "Input Count";
            // 
            // _outputCountLabel
            // 
            this._outputCountLabel.AutoSize = true;
            this._outputCountLabel.Location = new System.Drawing.Point(163, 54);
            this._outputCountLabel.Name = "_outputCountLabel";
            this._outputCountLabel.Size = new System.Drawing.Size(70, 13);
            this._outputCountLabel.TabIndex = 12;
            this._outputCountLabel.Text = "Output Count";
            // 
            // _outputCountNUD
            // 
            this._outputCountNUD.Location = new System.Drawing.Point(163, 70);
            this._outputCountNUD.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this._outputCountNUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._outputCountNUD.Name = "_outputCountNUD";
            this._outputCountNUD.Size = new System.Drawing.Size(145, 20);
            this._outputCountNUD.TabIndex = 11;
            this._outputCountNUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NewNodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 137);
            this.Controls.Add(this._outputCountLabel);
            this.Controls.Add(this._outputCountNUD);
            this.Controls.Add(this._inputCountLabel);
            this.Controls.Add(this._inputCountNUD);
            this.Controls.Add(this._cancelBtn);
            this.Controls.Add(this._addNodeBtn);
            this.Controls.Add(this._nodeNameLabel);
            this.Controls.Add(this._nodeNameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "NewNodeForm";
            this.Text = "Add New Node";
            ((System.ComponentModel.ISupportInitialize)(this._inputCountNUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._outputCountNUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _cancelBtn;
        private System.Windows.Forms.Button _addNodeBtn;
        private System.Windows.Forms.Label _nodeNameLabel;
        private System.Windows.Forms.TextBox _nodeNameTextBox;
        private System.Windows.Forms.NumericUpDown _inputCountNUD;
        private System.Windows.Forms.Label _inputCountLabel;
        private System.Windows.Forms.Label _outputCountLabel;
        private System.Windows.Forms.NumericUpDown _outputCountNUD;
    }
}