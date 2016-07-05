namespace AwesomeMvcDemo.Utils
{
    public static class GridUtils
    {
        public static string EditFormat(string popupName, string key = "Id")
        {
            return string.Format("<button type='button' class='awe-btn' onclick=\"awe.open('{0}', {{ params:{{ {1}: .{1} }} }})\"><span class='ico-edit'></span></button>", 
                popupName, key);
        }

        public static string DeleteFormat(string popupName, string key = "Id")
        {
            return string.Format("<button type='button' class='awe-btn' onclick=\"awe.open('{0}', {{ params:{{ {1}: .{1} }} }})\"><span class='ico-del'></span></button>",
                popupName, key);
        }

        public static string EditFormatForGrid(string gridId, string key = "Id")
        {
            return EditFormat("edit" + gridId, key);
        }

        public static string DeleteFormatForGrid(string gridId, string key = "Id")
        {
            return DeleteFormat("delete" + gridId, key);
        }

        public static string EditGridNestFormat()
        {
            return "<button type='button' class='awe-btn editnst'><span class='ico-edit'></span></button>";
        }

        public static string DeleteGridNestFormat()
        {
            return "<button type='button' class='awe-btn delnst'><span class='ico-del'></span></button>";
        }

        public static string AddChildFormat()
        {
            return "<button type='button' class='awe-btn' onclick=\"awe.open('createNode', { params:{ parentId: .Id } })\">add child</button>";
        }
    }
}