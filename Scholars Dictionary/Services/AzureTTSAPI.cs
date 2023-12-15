using System.Diagnostics;
using Microsoft.CognitiveServices.Speech;
using Scholars_Dictionary.Enums;

namespace Scholars_Dictionary.Services
{
    public static class AzureTTSAPI
    {
        private static readonly string key = "d3116a084f92432d9b63342eedd7dd9f";
        private static readonly string location = "westeurope";
        private static readonly string endpoint = $"https://{location}.api.cognitive.microsoft.com/";

        public async static void TextToSpeech(string word, SupportedLanguages language)
        {
            var config = SpeechConfig.FromSubscription(key, location);
            switch (language)
            {
                case SupportedLanguages.ENGLISH:
                    config.SpeechSynthesisVoiceName = "en-US-EmmaNeural";
                    break;
                case SupportedLanguages.RUSSIAN:
                    config.SpeechSynthesisVoiceName = "ru-RU-SvetlanaNeural";
                    break;
                case SupportedLanguages.SPANISH:
                    config.SpeechSynthesisVoiceName = "es-ES-AbrilNeural";
                    break;
            }

            // use the default speaker as audio output.
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                using (var result = await synthesizer.SpeakTextAsync(word))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Trace.WriteLine($"[Azure TTS API] Speech synthesized for text [{word}]");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Trace.WriteLine($"[Azure TTS API] CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Trace.WriteLine($"[Azure TTS API] CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Trace.WriteLine($"[Azure TTS API] CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            Trace.WriteLine($"[Azure TTS API] CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }

        }
    }
}
