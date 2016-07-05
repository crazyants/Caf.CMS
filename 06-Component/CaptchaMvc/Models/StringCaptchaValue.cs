using System;
using System.Runtime.Serialization;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    ///     Represents the base model for storing ​string ​captcha values.
    /// </summary>
    [Serializable]
    [DataContract]
    public class StringCaptchaValue : CaptchaValueBase
    {
        #region Fields

        [DataMember]
        private string _captchaText;

        [DataMember]
        private StringComparison _stringComparison;

        [DataMember]
        private string _value;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="NumberCaptchaValue" /> class. This constructor used only for deserialize.
        /// </summary>
        protected StringCaptchaValue()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringCaptchaValue" /> class.
        /// </summary>
        public StringCaptchaValue(string captchaText, string value, bool ignoreCase)
        {
            Validate.ArgumentNotNullOrEmpty(captchaText, "captchaText");
            Validate.ArgumentNotNull(value, "value");
            _captchaText = captchaText;
            _value = value;
            _stringComparison = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
        }

        #endregion

        #region Implementation of ICaptchaValue

        /// <summary>
        ///     Gets the specified captcha text.
        /// </summary>
        public override string CaptchaText
        {
            get { return _captchaText; }
        }

        /// <summary>
        ///     Gets the specified captcha value.
        /// </summary>
        public override object Value
        {
            get { return _value; }
        }

        /// <summary>
        ///     Determines whether the current captcha value is equal for the <c>inputText</c>.
        /// </summary>
        /// <param name="inputText"> The specified input text. </param>
        /// <returns> <c>True</c> if the value is equals; otherwise, <c>false</c> . </returns>
        public override bool IsEqual(string inputText)
        {
            return _value.Equals(inputText, _stringComparison);
        }

        /// <summary>
        ///     Deserializes the specified values into an <see cref="ICaptchaValue" />.
        /// </summary>
        /// <param name="captchaText"> The specified captcha text. </param>
        /// <param name="value"> The specified captcha value. </param>
        /// <param name="source"> The specified values. </param>
        protected override void DeserializeInternal(string captchaText, string value, string[] source)
        {
            _captchaText = captchaText;
            _value = value;
            if (source.Length < 3) return;
            _stringComparison = (StringComparison)int.Parse(source[2].Replace("~[", string.Empty).Replace("]~", string.Empty));
        }

        /// <summary>
        ///     Gets the optional values to serialize.
        /// </summary>
        /// <returns> An instance of <see cref="string" /> . </returns>
        protected override string GetOptionalValue()
        {
            return ((int) _stringComparison).ToString();
        }

        #endregion
    }
}