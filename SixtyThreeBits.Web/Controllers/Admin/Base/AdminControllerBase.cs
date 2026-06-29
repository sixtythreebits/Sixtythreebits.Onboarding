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
        #region Properties
        AdminModelBase _model;
        LayoutAdminViewModel _viewModel;
        #endregion

        #region Methods
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, async () =>
            {
                _viewModel = new LayoutAdminViewModel();
                _model = WebUtilities.GetModelFromController<AdminModelBase>(context.Controller);

                var isAuhenticated = isUserAuhenticated();
                if (isAuhenticated)
                {
                    var hasPermission = hasUserPermission();
                    if (hasPermission)
                    {
                        if (!_model.IsAjaxRequest)
                        {
                            setInitialProperties();
                            initClientPlugins();
                            initMenu();
                            initBreadCrumbs();
                            initTabs();
                            initPageTitle();
                            initSuccessErrorToast();
                            _model.ViewData[nameof(LayoutAdminViewModel)] = _viewModel;
                        }
                        return await next();
                    }
                    else
                    {
                        context.Result = _model.GetNotFoundAdminViewResult();
                        return new ActionExecutedContext(context, context.Filters, context.Controller) { Result = context.Result, Canceled = true };
                    }
                }
                else
                {
                    var urlLogin = _model.UrlFactory.Admin.CreateUrlLogin();
                    context.Result = new RedirectResult(urlLogin);
                    return new ActionExecutedContext(context, context.Filters, context.Controller) { Result = context.Result, Canceled = true };
                }
            });
        }
        bool isUserAuhenticated()
        {
            return _model.User != null;
        }
        bool hasUserPermission()
        {
            var hasPermission = _model.User.HasPermission(_model.UrlCurrentPageWithoutDomain);
            return hasPermission;
        }
        void setInitialProperties()
        {
            _viewModel.ProjectName = _model.SystemProperties.ProjectName;
            _viewModel.UserFullname = _model.User.UserFullname;
            _viewModel.UserEmail = _model.User.UserEmail;
        }
        void initClientPlugins()
        {
            _model.PluginsClient
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

            _viewModel.PluginsClient = _model.PluginsClient;
        }
        void initMenu()
        {
            if (_model.User.Permissions?.Count > 0)
            {
                _viewModel.Menu = _model.User.Permissions
                .Where(item => item.PermissionIsMenuItem && item.PermissionParentID == null)
                .Select(item => new Menu63ViewItem
                {
                    Title = item.PermissionMenuTitleOrCaption,
                    NavigateUrl = string.IsNullOrWhiteSpace(item.PermissionPagePath) ? item.PermissionGuid : item.PermissionPagePath,
                    Icon = item.PermissionMenuIcon,
                    IsSelected = item.PermissionPagePath == _model.UrlCurrentPageWithoutDomain,
                    Children = _model.User.Permissions.Where(subItem => subItem.PermissionIsMenuItem && subItem.PermissionParentID == item.PermissionID).Select(subItem => new Menu63ViewItem
                    {
                        Title = subItem.PermissionMenuTitleOrCaption,
                        NavigateUrl = subItem.PermissionPagePath,
                        Icon = subItem.PermissionMenuIcon,
                        IsSelected = subItem.PermissionPagePath == _model.UrlCurrentPageWithoutDomain
                    }).ToList()
                }).ToList();

                _viewModel.Menu.ForEach(item =>
                {
                    if (item.HasChildren)
                    {
                        item.IsSelected = item.Children.Any(subItem => subItem.IsSelected);
                    }
                });
            }

            _viewModel.UrlRelogin = _model.UrlFactory.Website.CreateUrlRelogin();
            _viewModel.UrlLogout = _model.UrlFactory.Website.CreateUrlLogout();
        }
        void initBreadCrumbs()
        {
            var pageHierarchy = _model.User.Permissions?.Select(item => new Breadcrumbs63.HierarchyItem<int?>
            {
                ID = item.PermissionID,
                ParentID = item.PermissionParentID,
                PageHttpPath = item.PermissionPagePath,
                PageTitle = item.PermissionMenuTitleOrCaption
            }).ToList();

            _viewModel.Breadcrumbs = _model.Breadcrumbs = new Breadcrumbs63();
            _viewModel.Breadcrumbs.InitBreadcrumbsByPageUrl(
                pageHierarchy: pageHierarchy,
                urlCurrentPage: _model.UrlCurrentPageWithDomain
            );
            _viewModel.ShowBreadCrumbs = _viewModel.Breadcrumbs.ItemsCount > 2;
        }
        void initTabs()
        {
            _viewModel.Tabs = _model.Tabs;
        }
        void initPageTitle()
        {
            _model.PageTitle = _viewModel.PageTitle = new PageTitle63(_model.SystemProperties.ProjectName);
            var p = _model.User.GetPermission(_model.UrlCurrentPageWithoutDomain);
            if (p != null)
            {
                _model.PageTitle.Set(p.PermissionName);
            }
        }        
        void initSuccessErrorToast()
        {
            _model.InitSuccessErrorToastNotificationPartialViewModel();
            _viewModel.SuccessErrorPartialViewModel = _model.SuccessErrorPartialViewModel;
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