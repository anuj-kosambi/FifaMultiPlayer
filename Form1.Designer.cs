namespace FifaMulti_v1
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
            this.makeserver_button = new System.Windows.Forms.Button();
            this.Disconnect = new System.Windows.Forms.Button();
            this.ipaddress_textbox = new System.Windows.Forms.TextBox();
            this.connect_button = new System.Windows.Forms.Button();
            this.serverClose_button = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.StatusBarLabel = new System.Windows.Forms.ToolStripLabel();
            this.IconLabel = new System.Windows.Forms.ToolStripLabel();
            this.TabController = new System.Windows.Forms.TabControl();
            this.server_tab = new System.Windows.Forms.TabPage();
            this.keymapping_tab = new System.Windows.Forms.TabPage();
            this.grid = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStrip1.SuspendLayout();
            this.TabController.SuspendLayout();
            this.server_tab.SuspendLayout();
            this.keymapping_tab.SuspendLayout();
            this.SuspendLayout();
            // 
            // makeserver_button
            // 
            this.makeserver_button.Location = new System.Drawing.Point(6, 42);
            this.makeserver_button.Name = "makeserver_button";
            this.makeserver_button.Size = new System.Drawing.Size(103, 23);
            this.makeserver_button.TabIndex = 5;
            this.makeserver_button.Text = "Make Server";
            this.makeserver_button.UseVisualStyleBackColor = true;
            this.makeserver_button.Click += new System.EventHandler(this.makeserver_button_Click);
            // 
            // Disconnect
            // 
            this.Disconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Disconnect.Location = new System.Drawing.Point(991, 69);
            this.Disconnect.Name = "Disconnect";
            this.Disconnect.Size = new System.Drawing.Size(98, 23);
            this.Disconnect.TabIndex = 4;
            this.Disconnect.Text = "Disconnect";
            this.Disconnect.UseVisualStyleBackColor = true;
            this.Disconnect.Click += new System.EventHandler(this.Disconnect_Click);
            // 
            // ipaddress_textbox
            // 
            this.ipaddress_textbox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ipaddress_textbox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.ipaddress_textbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.ipaddress_textbox.Location = new System.Drawing.Point(3, 3);
            this.ipaddress_textbox.Margin = new System.Windows.Forms.Padding(3, 30, 3, 3);
            this.ipaddress_textbox.Name = "ipaddress_textbox";
            this.ipaddress_textbox.Size = new System.Drawing.Size(1099, 20);
            this.ipaddress_textbox.TabIndex = 0;
            // 
            // connect_button
            // 
            this.connect_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.connect_button.Location = new System.Drawing.Point(990, 42);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(97, 23);
            this.connect_button.TabIndex = 1;
            this.connect_button.Text = "Connect";
            this.connect_button.UseVisualStyleBackColor = true;
            this.connect_button.Click += new System.EventHandler(this.connect_button_Click);
            // 
            // serverClose_button
            // 
            this.serverClose_button.Location = new System.Drawing.Point(6, 71);
            this.serverClose_button.Name = "serverClose_button";
            this.serverClose_button.Size = new System.Drawing.Size(103, 23);
            this.serverClose_button.TabIndex = 6;
            this.serverClose_button.Text = "Close Server";
            this.serverClose_button.UseVisualStyleBackColor = true;
            this.serverClose_button.Click += new System.EventHandler(this.serverClose_button_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.toolStrip1.Font = new System.Drawing.Font("Georgia", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusBarLabel,
            this.IconLabel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1113, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // StatusBarLabel
            // 
            this.StatusBarLabel.ForeColor = System.Drawing.Color.White;
            this.StatusBarLabel.Name = "StatusBarLabel";
            this.StatusBarLabel.Size = new System.Drawing.Size(65, 22);
            this.StatusBarLabel.Text = "Welcome";
            // 
            // IconLabel
            // 
            this.IconLabel.Font = new System.Drawing.Font("Wingdings", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.IconLabel.Name = "IconLabel";
            this.IconLabel.Size = new System.Drawing.Size(21, 22);
            this.IconLabel.Text = "n";
            // 
            // TabController
            // 
            this.TabController.Controls.Add(this.server_tab);
            this.TabController.Controls.Add(this.keymapping_tab);
            this.TabController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabController.Location = new System.Drawing.Point(0, 25);
            this.TabController.Name = "TabController";
            this.TabController.SelectedIndex = 0;
            this.TabController.Size = new System.Drawing.Size(1113, 470);
            this.TabController.TabIndex = 10;
            // 
            // server_tab
            // 
            this.server_tab.Controls.Add(this.ipaddress_textbox);
            this.server_tab.Controls.Add(this.makeserver_button);
            this.server_tab.Controls.Add(this.Disconnect);
            this.server_tab.Controls.Add(this.connect_button);
            this.server_tab.Controls.Add(this.serverClose_button);
            this.server_tab.Location = new System.Drawing.Point(4, 22);
            this.server_tab.Name = "server_tab";
            this.server_tab.Padding = new System.Windows.Forms.Padding(3);
            this.server_tab.Size = new System.Drawing.Size(1105, 444);
            this.server_tab.TabIndex = 0;
            this.server_tab.Text = "Server Settings";
            this.server_tab.UseVisualStyleBackColor = true;
            this.server_tab.Click += new System.EventHandler(this.server_tab_Click);
            // 
            // keymapping_tab
            // 
            this.keymapping_tab.Controls.Add(this.grid);
            this.keymapping_tab.Location = new System.Drawing.Point(4, 22);
            this.keymapping_tab.Name = "keymapping_tab";
            this.keymapping_tab.Padding = new System.Windows.Forms.Padding(3);
            this.keymapping_tab.Size = new System.Drawing.Size(341, 145);
            this.keymapping_tab.TabIndex = 1;
            this.keymapping_tab.Text = "KeyBoard Mapping";
            this.keymapping_tab.UseVisualStyleBackColor = true;
            // 
            // grid
            // 
            this.grid.AutoScroll = true;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(3, 3);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(335, 139);
            this.grid.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1113, 495);
            this.Controls.Add(this.TabController);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "Fifa MultiPlayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.TabController.ResumeLayout(false);
            this.server_tab.ResumeLayout(false);
            this.server_tab.PerformLayout();
            this.keymapping_tab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button makeserver_button;
        private System.Windows.Forms.Button Disconnect;
        private System.Windows.Forms.TextBox ipaddress_textbox;
        private System.Windows.Forms.Button connect_button;
        private System.Windows.Forms.Button serverClose_button;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TabControl TabController;
        private System.Windows.Forms.TabPage server_tab;
        private System.Windows.Forms.TabPage keymapping_tab;
        private System.Windows.Forms.FlowLayoutPanel grid;
        public System.Windows.Forms.ToolStripLabel IconLabel;
        public System.Windows.Forms.ToolStripLabel StatusBarLabel;




    }
}

