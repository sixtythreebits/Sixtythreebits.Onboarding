using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Admin;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Admin;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    public class AdminControllerBase<T> : ControllerBase63<T> where T : AdminModelBase, new()
    {
        #region Methods
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, async () =>
            {                                
                if (Model.IsUserLoggedIn)
                {
                    var hasPermission = hasUserPermission();
                    if (hasPermission)
                    {
                        if (!Model.IsAjaxRequest)
                        {
                            setInitialProperties();
                            initClientPlugins();
                            initMenu();
                            initBreadCrumbs();
                            initTabs();
                            initPageTitle();
                            initSuccessErrorToast();
                            Model.ViewData[nameof(LayoutAdminViewModel)] = Model.LayoutViewModel;
                        }
                        return await next();
                    }
                    else
                    {
                        context.Result = Model.GetNotFoundAdminViewResult();
                        return new ActionExecutedContext(context, context.Filters, context.Controller) { Result = context.Result, Canceled = true };
                    }
                }
                else
                {
                    var urlLogin = Model.UrlFactory.Admin.CreateUrlLogin();
                    context.Result = new RedirectResult(urlLogin);
                    return new ActionExecutedContext(context, context.Filters, context.Controller) { Result = context.Result, Canceled = true };
                }
            });
        }        
        bool hasUserPermission()
        {
            var hasPermission = Model.User.HasPermission(Model.UrlCurrentPageWithoutDomain);
            return hasPermission;
        }
        void setInitialProperties()
        {
            Model.LayoutViewModel.ProjectName = Model.SystemProperties.ProjectName;
            Model.LayoutViewModel.UserFullname = Model.User.UserFullname;
            Model.LayoutViewModel.UserEmail = Model.User.UserEmail;
        }
        void initClientPlugins()
        {
            Model.PluginsClient
            .EnableGoogleFonts(true)
            .Enable63BitsFonts(true)
            .EnableFontAwesome(true)
            .EnableAdminTheme(true)
            .EnableJQuery(true)
            .EnableJQueryConfirm(true)
            .EnablePreloader(true)
            .Enable63BitsComponents(true)
            .Enable63BitsAnalogClock(true)
            .EnableMetisMenu(true)
            .EnableUtils(true);

            Model.LayoutViewModel.PluginsClient = Model.PluginsClient;
        }
        void initMenu()
        {
            if (Model.User.Permissions?.Count > 0)
            {
                Model.LayoutViewModel.Menu = Model.User.Permissions
                .Where(item => item.PermissionIsMenuItem && item.PermissionParentID == null)
                .Select(item => new Menu63ViewItem
                {
                    Title = item.PermissionMenuTitleOrCaption,
                    NavigateUrl = string.IsNullOrWhiteSpace(item.PermissionPagePath) ? item.PermissionGuid : item.PermissionPagePath,
                    Icon = item.PermissionMenuIcon,
                    IsSelected = item.PermissionPagePath == Model.UrlCurrentPageWithoutDomain,
                    Children = Model.User.Permissions.Where(subItem => subItem.PermissionIsMenuItem && subItem.PermissionParentID == item.PermissionID).Select(subItem => new Menu63ViewItem
                    {
                        Title = subItem.PermissionMenuTitleOrCaption,
                        NavigateUrl = subItem.PermissionPagePath,
                        Icon = subItem.PermissionMenuIcon,
                        IsSelected = subItem.PermissionPagePath == Model.UrlCurrentPageWithoutDomain
                    }).ToList()
                }).ToList();

                Model.LayoutViewModel.Menu.ForEach(item =>
                {
                    if (item.HasChildren)
                    {
                        item.IsSelected = item.Children.Any(subItem => subItem.IsSelected);
                    }
                });
            }

            Model.LayoutViewModel.UrlRelogin = Model.UrlFactory.Website.CreateUrlRelogin();
            Model.LayoutViewModel.UrlLogout = Model.UrlFactory.Website.CreateUrlLogout();
        }
        void initBreadCrumbs()
        {
            var pageHierarchy = Model.User.Permissions?.Select(item => new Breadcrumbs63.HierarchyItem<int?>
            {
                ID = item.PermissionID,
                ParentID = item.PermissionParentID,
                PageHttpPath = item.PermissionPagePath,
                PageTitle = item.PermissionMenuTitleOrCaption
            }).ToList();

            Model.LayoutViewModel.Breadcrumbs = Model.Breadcrumbs = new Breadcrumbs63();
            Model.LayoutViewModel.Breadcrumbs.InitBreadcrumbsByPageUrl(
                pageHierarchy: pageHierarchy,
                urlCurrentPage: Model.UrlCurrentPageWithDomain
            );
            Model.LayoutViewModel.ShowBreadCrumbs = Model.LayoutViewModel.Breadcrumbs.ItemsCount > 2;
        }
        void initTabs()
        {
            Model.LayoutViewModel.Tabs = Model.Tabs;
        }
        void initPageTitle()
        {
            Model.PageTitle = Model.LayoutViewModel.PageTitle = new PageTitle63(Model.SystemProperties.ProjectName);
            var p = Model.User.GetPermission(Model.UrlCurrentPageWithoutDomain);
            if (p != null)
            {
                Model.PageTitle.Set(p.PermissionName);
            }
        }
        void initSuccessErrorToast()
        {
            Model.ToastNotificationManager = new ToastNotificationManager63(
                sessionAssistance: Model.SessionAssistance,
                pluginsClient: Model.LayoutViewModel.PluginsClient
            );
            Model.ToastNotificationManager.InitNotificationFromSession();
            Model.LayoutViewModel.SuccessErrorPartialViewModel = Model.ToastNotificationManager.SuccessErrorToastPartialViewModel;
        }

        [NonAction]
        public IActionResult DevExtremeGridResult(AjaxResponse viewModel)
        {
            if (viewModel.IsSuccess)
            {
                return Json(viewModel.Data);
            }
            else
            {
                throw new System.Exception(viewModel.Data.ToString());
            }
        }

        [NonAction]
        public IActionResult DevExtremeGridActionResult(AjaxResponse viewModel)
        {
            if (viewModel.IsSuccess)
            {
                return Json("OK");
            }
            else
            {
                return new ContentResult { Content = viewModel.Data.ToString(), StatusCode = 500 };
            }
        }        
        #endregion
    }
}