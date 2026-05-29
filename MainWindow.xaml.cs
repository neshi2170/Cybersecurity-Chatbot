using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        private ChatbotEngine chatbot;

        public MainWindow()
        {
            InitializeComponent();
            InitializeChatbot();
            PlayVoiceGreeting();
            AskUserName();
        }

        private void InitializeChatbot()
        {
            chatbot = new ChatbotEngine();
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                VoiceGreeting.PlayGreeting();
            }
            catch { /* Silent fail */ }
        }

        private void AskUserName()
        {
            AddBotMessage("Hello! Welcome to the Cybersecurity Awareness Chatbot!");
            AddBotMessage("What's your name?");
        }

        private void AddUserMessage(string message)
        {
            string formattedMessage = $"👤 You: {message}";
            ChatListBox.Items.Add(formattedMessage);
            ChatListBox.ScrollIntoView(ChatListBox.Items[ChatListBox.Items.Count - 1]);
        }

        private void AddBotMessage(string message)
        {
            string formattedMessage = $"🤖 Bot: {message}";
            ChatListBox.Items.Add(formattedMessage);
            ChatListBox.ScrollIntoView(ChatListBox.Items[ChatListBox.Items.Count - 1]);

            // Update topic display
            if (chatbot != null && !string.IsNullOrEmpty(chatbot.CurrentTopic))
            {
                CurrentTopicText.Text = chatbot.CurrentTopic.ToUpper();
                CurrentTopicText.Foreground = new SolidColorBrush(Colors.LightGreen);
            }

            // Update memory status
            if (chatbot != null && !string.IsNullOrEmpty(chatbot.UserName))
            {
                MemoryStatus.Text = $"Name: {chatbot.UserName}";
                MemoryStatus.Foreground = new SolidColorBrush(Colors.LightGreen);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void UserInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserInput();
            }
        }

        private void ProcessUserInput()
        {
            string userInput = UserInputTextBox.Text.Trim();

            // Input validation
            if (string.IsNullOrWhiteSpace(userInput))
            {
                AddBotMessage("Please type something! I'm here to help with cybersecurity questions.");
                return;
            }

            if (userInput.Length > 500)
            {
                AddBotMessage("That's a long message! Could you please keep it shorter so I can help better?");
                return;
            }

            // Add user message to chat
            AddUserMessage(userInput);

            // Clear input box
            UserInputTextBox.Clear();

            // Process through chatbot engine
            string response = chatbot.ProcessInput(userInput);
            AddBotMessage(response);

            // Update user name display
            if (!string.IsNullOrEmpty(chatbot.UserName) && UserNameDisplay.Text == "Not logged in")
            {
                UserNameDisplay.Text = $"👤 User: {chatbot.UserName}";
                UserNameDisplay.Foreground = new SolidColorBrush(Colors.LightGreen);
                MemoryStatus.Text = $"Name: {chatbot.UserName}";
            }
        }

        private void VoiceButton_Click(object sender, RoutedEventArgs e)
        {
            AddBotMessage("🎤 Voice input feature coming soon! For now, please type your message.\n\nTip: Try asking about passwords, scams, privacy, or phishing!");
        }
    }
}