namespace Win32MenuWinFormsDemo;

partial class MenuTestForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuTestForm));
        button1 = new System.Windows.Forms.Button();
        button2 = new System.Windows.Forms.Button();
        checkBox1 = new System.Windows.Forms.CheckBox();
        label1 = new System.Windows.Forms.Label();
        SuspendLayout();
        // 
        // button1
        // 
        button1.Location = new System.Drawing.Point(113, 152);
        button1.Name = "button1";
        button1.Size = new System.Drawing.Size(117, 23);
        button1.TabIndex = 0;
        button1.Text = "SetMenu";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // button2
        // 
        button2.Location = new System.Drawing.Point(236, 152);
        button2.Name = "button2";
        button2.Size = new System.Drawing.Size(117, 23);
        button2.TabIndex = 1;
        button2.Text = "RemoveMenu";
        button2.UseVisualStyleBackColor = true;
        button2.Click += button2_Click;
        // 
        // checkBox1
        // 
        checkBox1.Location = new System.Drawing.Point(113, 122);
        checkBox1.Name = "checkBox1";
        checkBox1.Size = new System.Drawing.Size(168, 24);
        checkBox1.TabIndex = 2;
        checkBox1.Text = "SetupAsSystemMenu";
        checkBox1.UseVisualStyleBackColor = true;
        // 
        // label1
        // 
        label1.Location = new System.Drawing.Point(113, 178);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(318, 77);
        label1.TabIndex = 3;
        label1.Text = resources.GetString("label1.Text");
        // 
        // MenuTestForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(538, 320);
        Controls.Add(label1);
        Controls.Add(checkBox1);
        Controls.Add(button2);
        Controls.Add(button1);
        Text = "Win32Menu - NativeMenu Test Demo";
        Load += MenuTestForm_Load;
        ResumeLayout(false);
    }

    private System.Windows.Forms.Label label1;

    private System.Windows.Forms.CheckBox checkBox1;

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
}