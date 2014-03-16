namespace FlowScriptPrototype
{
    partial class EditorForm
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
            this._nodeMenu = new System.Windows.Forms.MenuStrip();
            this._viewPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // _nodeMenu
            // 
            this._nodeMenu.Location = new System.Drawing.Point(0, 0);
            this._nodeMenu.Name = "_nodeMenu";
            this._nodeMenu.Size = new System.Drawing.Size(800, 24);
            this._nodeMenu.TabIndex = 0;
            this._nodeMenu.Text = "menuStrip1";
            // 
            // _viewPanel
            // 
            this._viewPanel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this._viewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._viewPanel.Location = new System.Drawing.Point(0, 24);
            this._viewPanel.Margin = new System.Windows.Forms.Padding(0);
            this._viewPanel.Name = "_viewPanel";
            this._viewPanel.Size = new System.Drawing.Size(800, 576);
            this._viewPanel.TabIndex = 1;
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this._viewPanel);
            this.Controls.Add(this._nodeMenu);
            this.MainMenuStrip = this._nodeMenu;
            this.Name = "EditorForm";
            this.Text = "EditorForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip _nodeMenu;
        private System.Windows.Forms.Panel _viewPanel;
    }
}