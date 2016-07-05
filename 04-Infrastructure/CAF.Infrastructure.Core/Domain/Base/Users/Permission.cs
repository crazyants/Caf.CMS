using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace CAF.Infrastructure.Core.Domain.Users
{
    /// <summary>
    /// 权限
    /// </summary>
    public class Permission
    {
        public Permission()
        {
            Functions = new List<PermissionButton>();
        }

        #region 属性

        public string Id { get; set; }
        /// <summary>
        /// 模块编码
        /// </summary>
        public string ModuleCode{ get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 模块地址
        /// </summary>
        public string ModuleUrl { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int ModuleIndex { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string ModuleDesc { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public string ModuleParent { get; set; }

        /// <summary>
        /// 系统ID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 操作集合
        /// </summary>
        public List<PermissionButton> Functions { get; set; }
        #endregion
    }
    /// <summary>
    /// 权限操作
    /// </summary>
    public class PermissionButton
    {
        public PermissionButton() { }


        /// <summary>
        /// 按钮编码
        /// </summary>
        public string ButtonCode { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string ButtonName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int ModuleIndex { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string ButtonDesc { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string ButtonIcon { get; set; }
    }
}
