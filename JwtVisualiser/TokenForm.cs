using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JwtVisualiser
{
    public class TokenForm : Form
    {
        private TreeView treeToken;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private Panel panel1;
        private Label label1;
        private TabPage tabPage2;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label3;
        private TextBox txtBody;
        private ComboBox comboMethod;
        private TextBox txtApiUrl;
        private Label label2;
        private Button btnExecute;
        private SplitContainer splitContainer1;
        private TextBox txtResultHeaders;
        private TextBox txtResultBody;
        private TextBox txtJwtResult;

        public TokenForm(string content)
        {
            InitializeComponent();
            ParseToken(content);
            this.txtJwtResult.Text = content;
            this.comboMethod.SelectedIndex = 0;
        }

        private void ParseToken(string jwtInput)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            //Check if readable token (string is in a JWT format)
            var readableToken = jwtHandler.CanReadToken(jwtInput);

            if (readableToken == true)
            {
                try
                {
                    var token = jwtHandler.ReadJwtToken(jwtInput);
                    TreeNode tHeader = new TreeNode("Header");
                    //Extract the headers of the JWT
                    var headers = token.Header;
                    foreach (var h in headers)
                    {
                        var hClaim = new TreeNode(h.Key);
                        var hValue = new TreeNode(h.Value.ToString());
                        hClaim.Nodes.Add(hValue);
                        tHeader.Nodes.Add(hClaim);
                    }
                    tHeader.Expand();
                    this.treeToken.Nodes.Add(tHeader);

                    var tToken = new TreeNode("Token");
                    //Extract the payload of the JWT
                    var claims = token.Claims;
                    foreach (Claim c in claims)
                    {
                        var hClaim = new TreeNode(c.Type);
                        var hValue = new TreeNode(c.Value);
                        hClaim.Nodes.Add(hValue);
                        tToken.Nodes.Add(hClaim);
                    }
                    tToken.ExpandAll();
                    this.treeToken.Nodes.Add(tToken);
                }
                catch(ArgumentException aex)
                {
                    this.treeToken.Nodes.Add(aex.Message);
                }
            }
            else
            {
                this.treeToken.Nodes.Add("String does not appear to be a valid JWT Token");
                this.treeToken.ExpandAll();
            }
        }


        private async void btnExecute_Click(object sender, EventArgs e)
        {
            string result = string.Empty;
            try
            {
                if (this.comboMethod.SelectedItem.ToString().Equals("GET", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = await SendGraphGetRequest(this.txtApiUrl.Text, this.txtJwtResult.Text);
                }
                if (this.comboMethod.SelectedItem.ToString().Equals("POST", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = await SendGraphPostRequest(this.txtApiUrl.Text, this.txtJwtResult.Text, this.txtBody.Text);
                }
            }
            catch(WebException ex)
            {
                result = ex.Message;
                if (ex.InnerException != null)
                {
                    result += $"{Environment.NewLine} {ex.InnerException.Message}";
                }
            }
            catch (HttpRequestException ex)
            {
                result = ex.Message;
                if (ex.InnerException != null)
                {
                    result += $"{Environment.NewLine} {ex.InnerException.Message}";
                }
            }
            this.txtResultBody.Clear();
            this.txtResultBody.AppendText(result);
        }

        private async Task UpdateHeaders(HttpResponseHeaders headers)
        {
            this.txtResultHeaders.Clear();
            foreach(var header in headers)
            {
                var values = string.Join(",", header.Value);
                this.txtResultHeaders.AppendText($"{header.Key}; {values} {Environment.NewLine}");
            }
        }

        public async Task<string> SendGraphGetRequest(string url, string accessToken)
        {
            HttpClient http = new HttpClient();
            
            // Append the access token for the Graph API to the Authorization header of the request, using the Bearer scheme.
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                await UpdateHeaders(response.Headers);
                return JsonConvert.SerializeObject(formatted, Formatting.Indented);
            }
            
            Trace.WriteLine("GET " + url);
            Trace.WriteLine("Authorization: Bearer " + accessToken.Substring(0, 80) + "...");
            Trace.WriteLine("Content-Type: application/json");

            await UpdateHeaders(response.Headers);

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> SendGraphPostRequest(string url, string accessToken, string json)
        {
            HttpClient http = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                string friendlyErrorMessage =  JsonConvert.SerializeObject(formatted, Formatting.Indented);
                await UpdateHeaders(response.Headers);
                return friendlyErrorMessage;
            }

            Trace.WriteLine("POST " + url);
            Trace.WriteLine("Authorization: Bearer " + accessToken.Substring(0, 80) + "...");
            Trace.WriteLine("Content-Type: application/json");
            Trace.WriteLine("");
            Trace.WriteLine(json);
            Trace.WriteLine("");

            await UpdateHeaders(response.Headers);
            return await response.Content.ReadAsStringAsync();
        }
        private void InitializeComponent()
        {
            this.txtJwtResult = new System.Windows.Forms.TextBox();
            this.treeToken = new System.Windows.Forms.TreeView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBody = new System.Windows.Forms.TextBox();
            this.comboMethod = new System.Windows.Forms.ComboBox();
            this.txtApiUrl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtResultHeaders = new System.Windows.Forms.TextBox();
            this.txtResultBody = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtJwtResult
            // 
            this.txtJwtResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtJwtResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtJwtResult.Location = new System.Drawing.Point(0, 23);
            this.txtJwtResult.Name = "txtJwtResult";
            this.txtJwtResult.ReadOnly = true;
            this.txtJwtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtJwtResult.Size = new System.Drawing.Size(1024, 24);
            this.txtJwtResult.TabIndex = 1;
            // 
            // treeToken
            // 
            this.treeToken.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.treeToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeToken.Location = new System.Drawing.Point(3, 56);
            this.treeToken.Name = "treeToken";
            this.treeToken.Size = new System.Drawing.Size(1024, 570);
            this.treeToken.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1038, 655);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.treeToken);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1030, 629);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Token Inspector";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtJwtResult);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1024, 47);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Token";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1030, 629);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "API Test";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.23047F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 88.76953F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtBody, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboMethod, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtApiUrl, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnExecute, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.83117F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 81.16883F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 439F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1024, 623);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(118, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Result";
            // 
            // txtBody
            // 
            this.txtBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBody.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBody.Location = new System.Drawing.Point(118, 32);
            this.txtBody.Multiline = true;
            this.txtBody.Name = "txtBody";
            this.txtBody.Size = new System.Drawing.Size(903, 119);
            this.txtBody.TabIndex = 3;
            // 
            // comboMethod
            // 
            this.comboMethod.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboMethod.FormattingEnabled = true;
            this.comboMethod.Items.AddRange(new object[] {
            "GET",
            "POST"});
            this.comboMethod.Location = new System.Drawing.Point(3, 3);
            this.comboMethod.Name = "comboMethod";
            this.comboMethod.Size = new System.Drawing.Size(109, 21);
            this.comboMethod.TabIndex = 0;
            // 
            // txtApiUrl
            // 
            this.txtApiUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtApiUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtApiUrl.Location = new System.Drawing.Point(118, 3);
            this.txtApiUrl.Name = "txtApiUrl";
            this.txtApiUrl.Size = new System.Drawing.Size(903, 22);
            this.txtApiUrl.TabIndex = 1;
            this.txtApiUrl.Text = "https://test/api/values";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Body (Optional)";
            // 
            // btnExecute
            // 
            this.btnExecute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExecute.Location = new System.Drawing.Point(3, 157);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(109, 23);
            this.btnExecute.TabIndex = 4;
            this.btnExecute.Text = "excute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(118, 186);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtResultHeaders);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtResultBody);
            this.splitContainer1.Size = new System.Drawing.Size(903, 434);
            this.splitContainer1.SplitterDistance = 301;
            this.splitContainer1.TabIndex = 7;
            // 
            // txtResultHeaders
            // 
            this.txtResultHeaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResultHeaders.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResultHeaders.Location = new System.Drawing.Point(0, 0);
            this.txtResultHeaders.Multiline = true;
            this.txtResultHeaders.Name = "txtResultHeaders";
            this.txtResultHeaders.ReadOnly = true;
            this.txtResultHeaders.Size = new System.Drawing.Size(301, 434);
            this.txtResultHeaders.TabIndex = 7;
            // 
            // txtResultBody
            // 
            this.txtResultBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResultBody.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResultBody.Location = new System.Drawing.Point(0, 0);
            this.txtResultBody.Multiline = true;
            this.txtResultBody.Name = "txtResultBody";
            this.txtResultBody.ReadOnly = true;
            this.txtResultBody.Size = new System.Drawing.Size(598, 434);
            this.txtResultBody.TabIndex = 8;
            // 
            // TokenForm
            // 
            this.ClientSize = new System.Drawing.Size(1038, 655);
            this.Controls.Add(this.tabControl1);
            this.Name = "TokenForm";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
