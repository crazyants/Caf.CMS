

namespace CAF.Mvc.JQuery.Datatables.Core
{
    public static class GridExtensions
    {
        public static GridTable<TModel> DataGrid<TModel>(this GridFacotory<TModel> factory,string gridId)
        {
            return new GridTable<TModel>(gridId);
        }

        public static GridTable<TModel> DataGrid<TModel>(this GridFacotory<TModel> factory, string gridId, string gridKey)
        {
            return new GridTable<TModel>(gridId, new GridConfiguration {GridKey = gridKey});
        }

        public static GridTable<TModel> DataGrid<TModel>(this GridFacotory factory, string gridId, GridConfiguration jqGridConfiguration)
        {
            return new GridTable<TModel>(gridId, jqGridConfiguration);
        }

    }
}