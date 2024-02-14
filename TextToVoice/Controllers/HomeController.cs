using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Speech.Synthesis;

namespace TextToVoice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Get all available cultures
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

            // Create a list to store the SelectListItem objects
            List<SelectListItem> languageOptions = new()
            {
                // Add a default option (optional)
                new SelectListItem { Text = "Select Language", Value = "" }
            };

            // Iterate through each culture and add it to the list
            foreach (CultureInfo culture in cultures)
            {
                // Use the DisplayName property of the CultureInfo to get the culture's display name
                languageOptions.Add(new SelectListItem { Text = culture.DisplayName, Value = culture.Name });
            }

            // Pass the list of SelectListItem objects to the view
            ViewBag.LanguageOptions = languageOptions;

            return View();
        }

        public IActionResult SpeekTextToSpeech(string inputText, string language, string gender)
        {
            // Hard code text
            inputText ??= "Hello Dany, How are you?";

            // Create a new SpeechSynthesizer object
            using SpeechSynthesizer synth = new SpeechSynthesizer();
            // Set the culture based on the language parameter (Default to English if not specified)
            CultureInfo culture = new(language ?? "en-US");

            // Set the voice for the specific culture
            VoiceGender voiceGender = gender.ToLower() == "female" ? VoiceGender.Female : VoiceGender.Male;

            // Set the voice for the specific culture and gender
            synth.SelectVoiceByHints(voiceGender, VoiceAge.Senior, 0, culture);

            // Generate speech for the specified text
            using var ms = new MemoryStream();
            synth.SetOutputToWaveStream(ms);
            synth.Speak(inputText);

            // Convert the speech data to a base64 string
            byte[] audioBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(audioBytes);

            // Return the base64 string
            return Content(base64String);
        }


        public IActionResult DownloadTextToSpeech(string inputText, string language, string gender)  
        {
            //Hard code text
            inputText ??= "Hello Dany, How are you?";

            //Create a new SpeechSynthesizer object
            SpeechSynthesizer synth = new SpeechSynthesizer();

            //Set the culture based on the language parameter (Default to English id not specified)
            CultureInfo culture = new (language ?? "en-US");

            //Set the output format to 16-bit PCM WAV
            //SpeechAudioFormatInfo format = new SpeechAudioFormatInfo(16000, AudioBitsPerSample.Sixteen, AudioChannel.Mono);

            //Set the voice for the specific culture
            synth.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult, 0, culture);

            //Generate speech for the specified text
            using var ms = new MemoryStream();
            synth.SetOutputToWaveStream(ms);
            synth.Speak(inputText);
            return File(ms.ToArray(), "audio/wav");
        }
    }
}