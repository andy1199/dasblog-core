namespace newtelligence.DasBlog.Web
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
    using System.IO;
    using System.Xml.Serialization;
    using System.Xml;
    using newtelligence.DasBlog.Web.Core;
    

	/// <summary>
	///		Summary description for EditBlogRollBox.
	/// </summary>
	public partial class EditBlogRollBox : System.Web.UI.UserControl
	{
        protected Opml opmlTree;
        private string baseFileName="blogroll.opml";
        protected System.Resources.ResourceManager resmgr;
        

        public string BaseFileName 
        {
            get
            {
                return baseFileName;
            }
            set
            {
                baseFileName = value;
            }
        }

        private void SaveOutline( string fileName )
        {
            using (StreamWriter sw = new StreamWriter( fileName, false,System.Text.Encoding.UTF8))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Opml));
                ser.Serialize(sw, opmlTree);
            }
        }

        private void LoadOutline( string fileName )
        {
            if ( File.Exists( fileName ) )
            {
                using (Stream s = File.OpenRead(fileName))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Opml));
                    opmlTree = ser.Deserialize(s) as Opml;
                }

            }
            if ( opmlTree == null )
            {
                opmlTree = new Opml("Generated by newtelligence dasBlog 1.0");
            }            
            Session["newtelligence.DasBlog.Web.EditBlogRollBox.OpmlTree"] = opmlTree;
        }

        private void BindGrid()
        {
            blogRollGrid.DataSource = opmlTree.body.outline;
            blogRollGrid.DataKeyField = "xmlUrl";
            DataBind();
            
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (SiteSecurity.IsInRole("admin") == false) 
            {
                Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
            }

            

            if ( !IsPostBack || 
                Session["newtelligence.DasBlog.Web.EditBlogRollBox.OpmlTree"] == null )
            {
                SharedBasePage requestPage = Page as SharedBasePage;
                foreach( string file in Directory.GetFiles(SiteConfig.GetConfigPathFromCurrentContext(),"*.opml"))
                {
                    listFiles.Items.Add( Path.GetFileName(file) );
                }
                if ( listFiles.Items.Count == 0 )
                {
                    listFiles.Items.Add("blogroll.opml");
                }
                Session["newtelligence.DasBlog.Web.EditBlogRollBox.baseFileName"] = baseFileName = listFiles.Items[0].Text;
                string fileName = Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName);
                LoadOutline( fileName );
            }
            else
            {
                baseFileName = Session["newtelligence.DasBlog.Web.EditBlogRollBox.baseFileName"] as string;
                opmlTree = Session["newtelligence.DasBlog.Web.EditBlogRollBox.OpmlTree"] as Opml;
            }
            BindGrid();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.blogRollGrid.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.blogRollGrid_ItemCommand);
			this.blogRollGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.blogRollGrid_PageIndexChanged);
			this.blogRollGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.blogRollGrid_CancelCommand);
			this.blogRollGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.blogRollGrid_EditCommand);
			this.blogRollGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.blogRollGrid_UpdateCommand);
			this.blogRollGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.blogRollGrid_DeleteCommand);
			this.Init += new System.EventHandler(this.EditBlogRollBox_Init);

		}
		#endregion

        private void blogRollGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            blogRollGrid.EditItemIndex = e.Item.ItemIndex;
            blogRollGrid.DataBind();
        }

        private void blogRollGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            opmlTree.body.outline.RemoveAt(e.Item.DataSetIndex);
            SaveOutline( Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName));

            blogRollGrid.EditItemIndex = -1;
            blogRollGrid.SelectedIndex = -1;
            LoadOutline( Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName ));
            BindGrid();
        }

        private void blogRollGrid_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            blogRollGrid.EditItemIndex = -1;
            blogRollGrid.DataBind();
        }

        private void blogRollGrid_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            OpmlOutline row = opmlTree.body.outline[e.Item.DataSetIndex];
            EditBlogRollEditItem item = ((EditBlogRollEditItem)e.Item.FindControl("editBlogRollEditItem"));
            row.description = item.Description;
            row.title = item.Title;
            row.htmlUrl = item.HtmlUrl;
            row.xmlUrl = item.XmlUrl;
            SaveOutline( Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName));

            blogRollGrid.EditItemIndex = -1;
            blogRollGrid.SelectedIndex = -1;
            LoadOutline( Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName ));
            BindGrid();
        }                                          

        private void blogRollGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            blogRollGrid.CurrentPageIndex = e.NewPageIndex;
            blogRollGrid.DataBind();
        }

       
        protected void buttonSelect_Click(object sender, System.EventArgs e)
        {
            Session["newtelligence.DasBlog.Web.EditBlogRollBox.baseFileName"] = baseFileName = listFiles.Items[listFiles.SelectedIndex].Text;
            Session["newtelligence.DasBlog.Web.EditBlogRollBox.OpmlTree"] = null;
            blogRollGrid.EditItemIndex = -1;
            blogRollGrid.SelectedIndex = -1;
            LoadOutline( Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName ));
            BindGrid();
        }

        protected void buttonCreate_Click(object sender, System.EventArgs e)
        {
            if ( textNewFileName.Text.Length > 0 )
            {
                // Get the requested file name to create and stip off any extra directories
                string fileName = textNewFileName.Text;
                fileName = Path.GetFileName( fileName );
                
                // Double check that there is an extension.  If not, tag on opml
                if ( Path.GetExtension( fileName ) == String.Empty )
                    fileName = fileName + ".opml";
                
                // Add this to the list of current file names and select it as active
                listFiles.Items.Add( fileName );
                listFiles.SelectedValue = fileName;
                Session["newtelligence.DasBlog.Web.EditBlogRollBox.baseFileName"] = baseFileName = fileName;

                // This will created during LoadOutline, but have to clear it out first, otherwise this new blogroll
                // will get a copy of the currently selected one, instead of starting fresh
                opmlTree = null;

                LoadOutline( Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName ));
                BindGrid();

                textNewFileName.Text = "";
            }
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState (savedState);
        }

        protected override object SaveViewState()
        {
            return base.SaveViewState ();
        }

        private void blogRollGrid_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if ( e.CommandName == "AddItem" )
            {
                OpmlOutline newEntry = new OpmlOutline();
                newEntry.title = resmgr.GetString("text_new_entry");
                opmlTree.body.outline.Insert(0,newEntry);
                blogRollGrid.EditItemIndex = 0;
                blogRollGrid.CurrentPageIndex = 0;
                blogRollGrid.DataBind();
            }
        }

        protected void EditBlogRollBox_Init(object sender, System.EventArgs e)
        {
            resmgr = ((System.Resources.ResourceManager)ApplicationResourceTable.Get());
        }
	}
}
