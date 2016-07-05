using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace CAF.WebSite.Application.WebUI.DynamicCaptcha
{
    /// <summary>
    /// VisualCaptcha object
    /// </summary>
    [Serializable]
    public sealed class Captcha
    {
        /// <summary>
        /// Dictionary of possible images to be displayed client-side
        /// Key: The image name (e.g. "Cloud")
        /// Value: The hashed value of the image path (e.g. "Cloud.png" --> "198723lkdjlfiwoekjdlsd")
        /// </summary>
        public readonly ImmutableDictionary<string, string> PossibleImageOptions;

        /// <summary>
        /// The correct selection out of the options contained in PossibleImageOptions
        /// </summary>
        public readonly KeyValuePair<string, string> ValidImageOption;

        /// <summary>
        /// The correct question/answer pair for audio accessibility option
        /// </summary>
        public readonly KeyValuePair<string, string> ValidAudioOption;

        /// <summary>
        /// Instantiate a Captcha session with an assortment of possible image selections
        /// and an audio option for accessibility
        /// </summary>
        public Captcha(int numberOfImageOptions)
        {
            PossibleImageOptions = GetRandomImageOptions(numberOfImageOptions);
            ValidImageOption = GetRandomOption(PossibleImageOptions);
            ValidAudioOption = GetRandomOption(Assets.Audios);
        }

        /// <summary>
        /// Retrieve object containing information needed by client-side library
        /// </summary>
        public FrontEndData GetFrontEndData()
        {
            var crypto = new CryptoHelper();
            return new FrontEndData
            {
                Values = PossibleImageOptions.Select(option => option.Value).ToList(),
                ImageName = ValidImageOption.Key,
                ImageFieldName = crypto.GetRandomString(20),
                AudioFieldName = crypto.GetRandomString(20)
            };
        }

        /// <summary>
        /// Get file content for image Captcha option
        /// </summary>
        /// <param name="index">Image index</param>
        /// <param name="isRetina">Uses Retina display</param>
        public byte[] GetImage(int index, bool isRetina)
        {
            var key = PossibleImageOptions.ToList()[index].Key;
            var imageName = Assets.Images[key];

            if (isRetina) { imageName = imageName.Replace(".png", "2x.png"); }

            return ReadResource("images." + imageName);
        }

        /// <summary>
        /// Get file content for audio Captcha option
        /// </summary>
        /// <param name="type">Either mp3 or ogg</param>
        public byte[] GetAudio(string type)
        {
            var audioName = ValidAudioOption.Key;
            if (type == "ogg") { audioName = audioName.Replace(".mp3", ".ogg"); }

            return ReadResource("audios." + audioName);
        }

        /// <summary>
        /// Answer value is valid for either image or audio option
        /// </summary>
        /// <param name="answerValue">This could be the hashed value of the image path or the answer to an audio question</param>
        public bool ValidateAnswer(string answerValue)
        {
            return IsValidImage(answerValue) || IsValidAudio(answerValue);
        }

        private static ImmutableDictionary<string, string> GetRandomImageOptions(int numberOfOptions)
        {
            var randomOptions = ImmutableDictionary.CreateBuilder<string, string>();
            var availableOptions = Assets.Images.ToList();

            var crypto = new CryptoHelper();
            for (var i = 0; i < numberOfOptions; i++)
            {
                var randomItem = availableOptions[crypto.GetRandomIndex(availableOptions.Count)];
                randomOptions.Add(randomItem.Key, crypto.GetRandomString(20));

                availableOptions.Remove(randomItem); // We don't want duplicate entries
            }

            return randomOptions.ToImmutable();
        }

        private static KeyValuePair<string, string> GetRandomOption(ICollection<KeyValuePair<string, string>> options)
        {
            return options.ToList()[new CryptoHelper().GetRandomIndex(options.Count)];
        }

        private bool IsValidImage(string hashedPath)
        {
            return ValidImageOption.Value == hashedPath;
        }

        private bool IsValidAudio(string value)
        {
            return ValidAudioOption.Value.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <param name="optionPath"> e.g. image.mypicture.png </param>
        private static byte[] ReadResource(string optionPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceLocation = string.Format("VisualCaptcha.Assets.{0}", optionPath);
            using (var stream = assembly.GetManifestResourceStream(resourceLocation))
            {
                if (stream == null) { return null; }
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }
    }
}
