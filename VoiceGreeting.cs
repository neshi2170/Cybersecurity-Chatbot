using System;
using System.Media;
using System.IO;

namespace CybersecurityChatbot
{
    public static class VoiceGreeting
    {
        public static void PlayGreeting()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

                if (File.Exists(filePath))
                {
                    using (SoundPlayer player = new SoundPlayer(filePath))
                    {
                        player.Play(); // Play without freezing UI
                    }
                }
                // Silent fail if file doesn't exist - app continues normally
            }
            catch (Exception ex)
            {
                // Silent fail - app continues without voice
                System.Diagnostics.Debug.WriteLine($"Voice greeting error: {ex.Message}");
            }
        }
    }
}