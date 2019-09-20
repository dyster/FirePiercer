namespace FirePiercer.Piercer
{
    partial class Piercer
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            sonesson_tools.TCP.Stats stats1 = new sonesson_tools.TCP.Stats();
            this._tcpServer = new FirePiercer.PierceServer();
            // 
            // _tcpServer
            // 
            this._tcpServer.InitialBufferSize = 8;
            this._tcpServer.Port = 443;
            this._tcpServer.Stats = stats1;
            this._tcpServer.UseSSL = true;

        }

        #endregion

        private PierceServer _tcpServer;
    }
}
