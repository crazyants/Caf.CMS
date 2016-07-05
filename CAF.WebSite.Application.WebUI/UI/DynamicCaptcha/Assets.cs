using System.Collections.Generic;
using System.Collections.Immutable;

namespace CAF.WebSite.Application.WebUI.DynamicCaptcha
{
    /// <summary>
    /// Static class containing image/audio option key/values
    /// </summary>
    internal static class Assets
    {
        internal static readonly ImmutableDictionary<string, string> Images = new Dictionary<string, string>
        {
            { "Airplane", "airplane.png" },
            { "Balloons", "balloons.png" },
            { "Camera", "camera.png" },
            { "Car", "car.png" },
            { "Cat", "cat.png" },
            { "Chair", "chair.png" },
            { "Clip", "clip.png" },
            { "Clock", "clock.png" },
            { "Cloud", "cloud.png" },
            { "Computer", "computer.png" },
            { "Envelope", "envelope.png" },
            { "Eye", "eye.png" },
            { "Flag", "flag.png" },
            { "Folder", "folder.png" },
            { "Foot", "foot.png" },
            { "Graph", "graph.png" },
            { "House", "house.png" },
            { "Key", "key.png" },
            { "Leaf", "leaf.png" },
            { "Light Bulb", "light-bulb.png" },
            { "Lock", "lock.png" },
            { "Magnifying Glass", "magnifying-glass.png" },
            { "Man", "man.png" },
            { "Music Note", "music-note.png" },
            { "Pants", "pants.png" },
            { "Pencil", "pencil.png" },
            { "Printer", "printer.png" },
            { "Robot", "robot.png" },
            { "Scissors", "scissors.png" },
            { "Sunglasses", "sunglasses.png" },
            { "Tag", "tag.png" },
            { "Tree", "tree.png" },
            { "Truck", "truck.png" },
            { "T-Shirt", "t-shirt.png" },
            { "Umbrella", "umbrella.png" },
            { "Woman", "woman.png" },
            { "World", "world.png" }
        }.ToImmutableDictionary();

        internal static readonly ImmutableDictionary<string, string> Audios = new Dictionary<string, string>
        {
            { "5times2.mp3", "10" },
            { "2times10.mp3", "20" },
            { "5plus1.mp3", "6" },
            { "4plus1.mp3", "5" },
            { "4plus3.mp3", "7" },
            { "6plus6.mp3", "12" },
            { "12times2.mp3", "24" },
            { "99plus1.mp3", "100" },
            { "add3to1.mp3", "4" },
            { "addblueandyellow.mp3", "green" },
            { "after2.mp3", "3" },
            { "divide4by2.mp3", "2" },
            { "milkcolor.mp3", "white" },
            { "skycolor.mp3", "blue" },
            { "sunastar.mp3", "yes" },
            { "yourobot.mp3", "no" },
            { "capitaloffrance.mp3", "paris" },
            { "skynight.mp3", "black" },
            { "thirdmonth.mp3", "march" },
            { "firstletteralphabet.mp3", "a" }
        }.ToImmutableDictionary();
    }
}
