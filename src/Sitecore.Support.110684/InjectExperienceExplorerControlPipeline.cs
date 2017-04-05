using Sitecore.ExperienceExplorer.Business.Constants;
using Sitecore.ExperienceExplorer.Business.Helpers;
using Sitecore.ExperienceExplorer.Business.Managers;
using Sitecore.ExperienceExplorer.Business.Utilities;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Sitecore.Support.ExperienceExplorer.Business.Pipelines.RenderLayout
{
    public class InjectExperienceExplorerControlPipeline
    {
        public void Process(Sitecore.Pipelines.RenderLayout.RenderLayoutArgs args)
        {
            if (!SettingsHelper.ExperienceModePipelineEnabled || Sitecore.Context.Item == null)
            {
                return;
            }
            bool flag = ModuleManager.IsExpButtonClicked || PageModeHelper.IsExperienceMode;
            if (flag && !ExperienceExplorerUtil.CurrentTicketIsValid())
            {
                PageModeHelper.RedirectToLoginPage();
            }
            if (!Sitecore.Context.IsLoggedIn)
            {
                Sitecore.Publishing.PreviewManager.RestoreUser();
            }
            if (!Sitecore.Context.IsLoggedIn && Sitecore.Web.WebUtil.GetQueryStringOrCookie(SettingsHelper.AddOnQueryStringKey) == "1")
            {
                Sitecore.Sites.SiteContext site = Sitecore.Configuration.Factory.GetSite("login");
                Sitecore.Web.WebUtil.Redirect(site.VirtualFolder);
                return;
            }
            if (!SettingsHelper.IsEnabledForCurrentSite)
            {
                return;
            }
            if (Sitecore.Context.Site.DisplayMode != Sitecore.Sites.DisplayMode.Normal)
            {
                return;
            }
            try
            {
                Sitecore.Web.WebUtil.SetCookieValue(SettingsHelper.AddOnQueryStringKey, flag ? "1" : "0");
                if (flag)
                {
                    SettingsHelper.ExplorerWasAccessed = true;
                    Control control = null;
                    if (Sitecore.Context.Page.Page.Master != null)
                        control = Sitecore.Web.WebUtil.FindControlOfType(Sitecore.Context.Page.Page.Master, typeof(HtmlForm));
                    else
                        control = Sitecore.Web.WebUtil.FindControlOfType(Sitecore.Context.Page.Page, typeof(HtmlForm));
                    if (control == null)
                    {
                        return;
                    }
                    Control child = Sitecore.Context.Page.Page.LoadControl(Paths.Module.Controls.GlobalHeaderPath);
                    control.Controls.AddAt(0, child);
                    Sitecore.ExperienceExplorer.Business.WebControls.ExperienceExplorer child2 = new Sitecore.ExperienceExplorer.Business.WebControls.ExperienceExplorer();
                    control.Controls.Add(child2);
                    ModuleManager.IsExpButtonClicked = false;
                    HttpContext.Current.Items["IsExperienceMode"] = null;
                    this.EnsureFirstLoad();
                    if (!UserHelper.IsVirtualUser(Sitecore.Context.User.Name))
                    {
                        UserHelper.AuthentificateVirtualUser(Sitecore.Context.User.Name);
                    }
                }
                if (Sitecore.Context.PageMode.IsPreview || Sitecore.Context.PageMode.IsExperienceEditor || Sitecore.Context.PageMode.IsDebugging)
                {
                    UserHelper.AuthentificateRealUser();
                }
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("code blocks"))
                {
                    Sitecore.Diagnostics.Log.Error("Inject Experience Explorer Control: ", ex, this);
                }
            }
        }

        private void EnsureFirstLoad()
        {
            if (HttpContext.Current.Session["IsFirstTime"] == null && !UserHelper.IsVirtualUser(Sitecore.Context.User.Name))
            {
                HttpContext.Current.Session["IsFirstTime"] = true;
                return;
            }
            HttpContext.Current.Session["IsFirstTime"] = false;
        }
    }
}