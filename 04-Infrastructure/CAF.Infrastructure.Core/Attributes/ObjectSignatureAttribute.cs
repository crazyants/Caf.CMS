using System;


namespace CAF.Infrastructure.Core
{
    /// <summary>
    /// 对象签名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)] 
    public sealed class ObjectSignatureAttribute : Attribute
    {
    }

}
