using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.ExperienceExplorer.Core;
using Sitecore.ExperienceExplorer.Core.State;
using Sitecore.ExperienceExplorer.WebControls;
using Sitecore.Pipelines.RenderLayout;
using Sitecore.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Sitecore.Support.ExperienceExplorer.Pipelines.RenderLayout
{
	public class InjectExplorerControl : RenderLayoutProcessor
	{

		private readonly IExplorerContext _context;

		public InjectExplorerControl(IExplorerContext context)
		{
			Assert.ArgumentNotNull(context, "context");
			this._context = context;
		}

		public override void Process(RenderLayoutArgs args)
		{
			if (this._context.IsExplorerMode())
			{
				Control control = null;
				if (Sitecore.Context.Page.Page.Master != null)
					control = Sitecore.Web.WebUtil.FindControlOfType(Sitecore.Context.Page.Page.Master, typeof(HtmlForm));
				else
					control = Sitecore.Web.WebUtil.FindControlOfType(Sitecore.Context.Page.Page, typeof(HtmlForm));
				if (control == null)
				{
					return;
				}
				if (control != null)
				{
					Control child = Context.Page.Page.LoadControl(ComponentSettings.Paths.GlobalHeaderPath);
					control.Controls.AddAt(0, child);
					Sitecore.ExperienceExplorer.WebControls.ExperienceExplorer child2 = new Sitecore.ExperienceExplorer.WebControls.ExperienceExplorer();
					control.Controls.Add(child2);
				}
			}
		}
	}
}