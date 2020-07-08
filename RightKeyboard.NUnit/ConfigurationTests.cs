using NUnit.Framework;

namespace RightKeyboard
{
    [TestFixture]
    class ConfigurationTests
    {
        [Test]
        public void LoadConfiguration_Works()
        {
            var configuration = Configuration.LoadConfiguration(new KeyboardDevicesCollection());
            Assert.That(configuration, Is.Not.Null, "Depends on what is installed on the machine, so manual human check with debugger is required.");
        }
    }
}
