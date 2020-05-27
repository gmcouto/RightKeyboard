using NUnit.Framework;
using RightKeyboard.Win32;

namespace RightKeyboard.NUnit
{
    public class Tests
    {
        [Test]
        public void GetKeyboardLayoutName_MustReturnLanguageNameAndKeyboardName()
        {
            var list = API.GetKeyboardLayoutList();
            foreach (var layout in list)
            {
                var name = API.GetKeyboardLayoutName(layout);
                Assert.That(name, Is.Not.Empty, "Depends on what is installed on the machine, so manual human check with debugger is required.");
            }
        }
    }
}