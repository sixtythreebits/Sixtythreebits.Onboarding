using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;

namespace SixtyThreeBits.Web.Domain.Libraries
{   
    public abstract class DevExtremeGridModelBase63<T> where T : class
    {
        #region Properties        
        public bool IsAddNewButtonVisible { get; set; }
        public bool IsEditButtonVisible { get; set; }
        public bool IsDeleteButtonVisible { get; set; }

        public string UrlLoad { get; set; }
        public object LoadParams { get; set; }
        public string UrlAddNew { get; set; }
        public string UrlUpdate { get; set; }
        public string UrlDelete { get; set; }        

        public bool IsError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; set; }
        public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
        #endregion

        #region Methods
        public DataGridBuilder<T> CreateGridWithStartupValues(IHtmlHelper html, string keyFieldName, string onBeforeSendJsFunction = null)
        {
            return html.DevExtreme().DataGrid<T>()
            .Width("100%")
            .ShowBorders(true)
            .ShowRowLines(true)
            .FocusedRowEnabled(true)
            .FocusedRowIndex(0)
            .SyncLookupFilterValues(false)
            .AllowColumnResizing(true)
            .Toolbar(options =>
            {
                options.Visible(false);
            })
            .Scrolling(options =>
            {
                options.Mode(GridScrollingMode.Standard);
                options.ShowScrollbar(ShowScrollbarMode.Always);
            })
            .FilterRow(options =>
            {
                options.Visible(true);
                options.ApplyFilter(GridApplyFilterMode.Auto);
                options.ShowAllText(Resources.TextAllDevexpressGridFilterRaw);
            })
            .HeaderFilter(options =>
            {
                options.Visible(true);
            })
            .DataSource(options =>
            {
                var optionsBuilder = options.RemoteController();
                optionsBuilder.Key(keyFieldName);
                optionsBuilder.LoadUrl(UrlLoad);
                optionsBuilder.InsertUrl(UrlAddNew);
                optionsBuilder.UpdateUrl(UrlUpdate);
                optionsBuilder.DeleteUrl(UrlDelete);

                if (!string.IsNullOrWhiteSpace(onBeforeSendJsFunction))
                {
                    optionsBuilder.OnBeforeSend(onBeforeSendJsFunction);
                }
                if (LoadParams != null)
                {
                    optionsBuilder.LoadParams(LoadParams);
                }

                return optionsBuilder;
            })
            .Editing(options =>
            {
                options.Mode(GridEditMode.Row);
                //options.Mode(GridEditMode.Cell);
                options.AllowAdding(IsAddNewButtonVisible);
                options.AllowUpdating(IsEditButtonVisible);
                options.AllowDeleting(IsDeleteButtonVisible);
                options.Texts(optionsTexts =>
                {
                    optionsTexts.ConfirmDeleteMessage(TextConfirmDelete);

                });

            })
            .Pager(options =>
            {
                options.AllowedPageSizes([30, 50, 100 ]);
                options.ShowInfo(true);
                options.ShowPageSizeSelector(true);
                options.Visible(true);
            })
            .Paging(options =>
            {
                options.Enabled(true);
                options.PageSize(30);
            })
            .Columns(Columns =>
            {
                if (IsAddNewButtonVisible || IsEditButtonVisible || IsDeleteButtonVisible)
                {
                    var isAllowedAddOrUpdate = IsAddNewButtonVisible || IsEditButtonVisible;
                    var width = isAllowedAddOrUpdate ? 60 : 30;
                    var commandColumn = Columns.Add();
                    commandColumn
                        .Width(width)
                        .Type(GridCommandColumnType.Buttons)
                        .Alignment(HorizontalAlignment.Center)
                        .Buttons(b =>
                        {
                            if (IsEditButtonVisible)
                            {
                                b.Add().Name(GridColumnButtonName.Edit).Icon("fa-solid fa-pencil").Text(Resources.TextUpdate);
                            }
                            if (IsDeleteButtonVisible)
                            {
                                b.Add().Name(GridColumnButtonName.Delete).Icon("fa-light fa-trash-can").Text(Resources.TextDelete);
                            }
                            if (isAllowedAddOrUpdate)
                            {
                                b.Add().Name(GridColumnButtonName.Save).Icon("fa-solid fa-check").Text(Resources.TextSave);
                                b.Add().Name(GridColumnButtonName.Cancel).Icon("fa-solid fa-circle-minus").Text(Resources.TextCancel);
                            }

                        });
                }
            });
        }

        public abstract DataGridBuilder<T> Render(IHtmlHelper Html);
        #endregion
    }    
}