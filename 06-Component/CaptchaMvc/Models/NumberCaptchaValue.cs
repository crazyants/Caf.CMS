using System;
using System.Runtime.Serialization;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;

namespace CaptchaMvc.Models
{
    /// <summary>
    ///     Represents the base model for storing number ​​captcha values.
    /// </summary>
    [Serializable]
    [DataContract]
    public class NumberCaptchaValue : CaptchaValueBase
    {
        #region Fields

        [DataMember] private string _captchaText;

        [DataMember] private int _value;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="NumberCaptchaValue" /> class. This constructor used only for deserialize.
        /// </summary>
        protected NumberCaptchaValue()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NumberCaptchaValue" /> class.
        /// </summary>
        public NumberCaptchaValue(string captchaText, int value)
        {
            Validate.ArgumentNotNullOrEmpty(captchaText, "captchaText");
            _captchaText = captchaText;
            _value = value;
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
        /// <returns>
        ///     <c>True</c> if the value is equals; otherwise, <c>false</c> .
        /// </returns>
        public override bool IsEqual(string inputText)
        {
            int input;
            if (!int.TryParse(inputText, out input))
                return false;
            return input == _value;
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
            _value = int.Parse(value);
        }

        #endregion
    }
}