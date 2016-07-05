using System.Web;
using System.Web.Mvc;

using CAF.Infrastructure.MvcHtml;
using System.Web.Routing;

namespace AwesomeMvcDemo.Helpers
{
    public static class CrudHelpers
    {
        /*beging*/
        public static IHtmlString InitCrudPopupsForGrid<T>(this HtmlHelper<T> html, string gridId, string crudController, string createAction, string editAction, string deleteAction, int createPopupHeight = 430)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            var result =
            html.Awe()
                .InitPopupForm()
                .Name("create" + gridId)
                .Group(gridId)
                .Height(createPopupHeight)
                .Url(url.Action(createAction, crudController))
                .Success("utils.itemCreated('" + gridId + "')")
                .ToString()

            + html.Awe()
                  .InitPopupForm()
                  .Name("edit" + gridId)
                  .Group(gridId)
                  .Height(createPopupHeight)
                  .Url(url.Action(editAction, crudController))
                  .Modal(true)
                  .Success("utils.itemEdited('" + gridId + "')")

            + html.Awe()
                  .InitPopupForm()
                  .Name("delete" + gridId)
                  .Group(gridId)
                  .Url(url.Action(deleteAction, crudController))
                  .Success("utils.itemDeleted('" + gridId + "')")
                  .Parameter("gridId", gridId) // used to call grid.api.select and emphasize the row
                  .Height(200)
                  .Modal(true);

            return new MvcHtmlString(result);
        }
        /*endg*/

        public static IHtmlString InitCrudForGridNest<T>(this HtmlHelper<T> html, string gridId, string crudController, string createAction, string editAction, string deleteAction, object routeValues)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            var result =
                html.Awe()
                    .InitPopupForm()
                    .Name("create" + gridId)
                    .Group(gridId)
                    .Url(url.Action(createAction, crudController, routeValues))
                    .Tag(new { Inline = true, NoTitle = true })
                    .Success("utils.itemCreated('" + gridId + "')")
                    .ToString()
                + html.Awe()
                      .InitPopupForm()
                      .Name("edit" + gridId)
                      .Group(gridId)
                      .Url(url.Action(editAction, crudController))
                      .Tag(new { Inline = true, NoTitle = true })
                      .Success("utils.itemEdited('" + gridId + "')")
                + html.Awe()
                      .InitPopupForm()
                      .Name("delete" + gridId)
                      .Group(gridId)
                      .Url(url.Action(deleteAction, crudController))
                      .Tag(new { Inline = true, NoTitle = true })
                      .Parameter("gridId", gridId)
                      .Success("utils.itemDeleted('" + gridId + "')");

            return new MvcHtmlString(result);
        }


        /*beginal*/
        public static IHtmlString InitCrudPopupsForAjaxList<T>(
            this HtmlHelper<T> html, string ajaxListId, string controller, string key, string popupName)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);
            var result =
                html.Awe()
                    .InitPopupForm()
                    .Name("create" + popupName)
                    .Url(url.Action("Create", controller))
                    .Height(430)
                    .Success("utils.itemCreatedAlTbl('" + ajaxListId + "')")
                    .Group(ajaxListId)
                    .Title("create item")
                    .ToString()

                + html.Awe()
                      .InitPopupForm()
                      .Name("edit" + popupName)
                      .Url(url.Action("Edit", controller))
                      .Height(430)
                      .Success("utils.itemEditedAl('" + ajaxListId + "', '" + key + "')")
                      .Group(ajaxListId)
                      .Title("edit item")

                + html.Awe()
                      .InitPopupForm()
                      .Name("delete" + popupName)
                      .Url(url.Action("Delete", controller))
                      .Success("utils.itemDeletedAl('" + ajaxListId + "', '" + key + "')")
                      .Group(ajaxListId)
                      .OkText("Yes")
                      .CancelText("No")
                      .Height(200)
                      .Title("delete item");

            return new MvcHtmlString(result);
        }
        /*endal*/
    }
}