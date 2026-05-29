using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    public class ChatbotEngine
    {
        private Dictionary<string, string> userMemory = new Dictionary<string, string>();
        private List<string> conversationHistory = new List<string>();
        private string lastTopic = "";

        public string UserName { get; private set; }
        public string CurrentTopic { get; private set; }

        private Dictionary<string, List<string>> keywordResponses;
        private Random random = new Random();

        // Sentiment detection keywords
        private Dictionary<string, string[]> sentimentKeywords = new Dictionary<string, string[]>
        {
            { "worried", new[] { "worried", "nervous", "anxious", "scared", "concerned", "afraid" } },
            { "curious", new[] { "curious", "interested", "tell me more", "want to learn", "how does" } },
            { "frustrated", new[] { "frustrated", "annoyed", "angry", "upset", "confused" } },
            { "grateful", new[] { "thanks", "thank you", "helpful", "appreciate", "great" } },
            { "happy", new[] { "happy", "excited", "great", "awesome", "wonderful" } },
            { "scared", new[] { "scared", "terrified", "panic", "hacked", "breached" } }
        };

        public ChatbotEngine()
        {
            InitializeResponses();
        }

        private void InitializeResponses()
        {
            keywordResponses = new Dictionary<string, List<string>>
            {
                ["password"] = new List<string>
                {
                    "🔐 Use strong passwords with 12+ characters, including numbers, symbols, and both cases!",
                    "🔑 Never reuse passwords across different accounts. Use a password manager!",
                    "🛡️ Enable Two-Factor Authentication (2FA) whenever possible for extra security.",
                    "💡 Tip: Use passphrases like 'PurpleDinosaurEatsPizza!' - easy to remember, hard to crack!"
                },

                ["scam"] = new List<string>
                {
                    "⚠️ Never share personal info with unsolicited callers/emails. Hang up and verify independently!",
                    "📧 Check email sender addresses carefully - scammers use fake addresses like @paypa1.com",
                    "🔍 If it sounds too good to be true (lottery wins, huge discounts), it's likely a scam!",
                    "📞 The IRS, police, or tech support will NEVER demand gift cards or immediate payment!"
                },

                ["privacy"] = new List<string>
                {
                    "👁️ Review app permissions regularly - many apps access more data than they need!",
                    "🔒 Use VPN on public WiFi to encrypt your browsing activity.",
                    "📱 Turn off location sharing for apps that don't need it to function.",
                    "🌐 Use privacy-focused browsers like Firefox or Brave with tracking protection enabled!"
                },

                ["phishing"] = new List<string>
                {
                    "🎣 Don't click links in suspicious emails. Hover to see the real URL first!",
                    "📞 Legitimate companies never ask for passwords or OTPs via email/call.",
                    "🔗 Check for spelling errors and generic greetings like 'Dear Customer'!",
                    "⚠️ When in doubt, type the website URL manually instead of clicking email links!"
                },

                ["malware"] = new List<string>
                {
                    "🦠 Keep antivirus software updated and run regular system scans!",
                    "💾 Don't download software from untrusted websites or pop-up ads.",
                    "📎 Never open email attachments from unknown senders!",
                    "🛡️ Windows Defender is actually very good - just make sure it's enabled!"
                },

                ["2fa"] = new List<string>
                {
                    "🔐 Two-Factor Authentication adds an extra layer of security beyond just your password!",
                    "📱 Use authenticator apps like Google Authenticator instead of SMS when possible!",
                    "✅ Always enable 2FA on important accounts like email, banking, and social media!",
                    "🔑 Without 2FA, a stolen password is all a hacker needs to access your account!"
                },

                ["two factor"] = new List<string>
                {
                    "🔐 Two-Factor Authentication adds an extra layer of security beyond just your password!",
                    "📱 Use authenticator apps like Google Authenticator instead of SMS when possible!",
                    "✅ Always enable 2FA on important accounts like email, banking, and social media!",
                    "🔑 Without 2FA, a stolen password is all a hacker needs to access your account!"
                },

                ["two-factor"] = new List<string>
                {
                    "🔐 Two-Factor Authentication adds an extra layer of security beyond just your password!",
                    "📱 Use authenticator apps like Google Authenticator instead of SMS when possible!",
                    "✅ Always enable 2FA on important accounts like email, banking, and social media!",
                    "🔑 Without 2FA, a stolen password is all a hacker needs to access your account!"
                },

                ["vpn"] = new List<string>
                {
                    "🌐 A VPN encrypts your internet traffic, hiding your activity from hackers on public WiFi!",
                    "🔒 Always use a VPN when connecting to public networks like coffee shops or airports!",
                    "🛡️ VPNs also help bypass geo-restrictions and protect your privacy from your ISP!",
                    "💰 There are free VPNs, but paid ones (like NordVPN, ProtonVPN) are more reliable!"
                },

                ["firewall"] = new List<string>
                {
                    "🔥 A firewall monitors incoming/outgoing network traffic based on security rules!",
                    "🛡️ Windows has a built-in firewall - make sure it's always turned on!",
                    "🔒 Firewalls block unauthorized access while allowing legitimate traffic through!",
                    "🏠 Your router also has a firewall - keep its firmware updated for maximum protection!"
                },

                ["help"] = new List<string>
                {
                    "📚 I can help with these cybersecurity topics:\n\n• passwords - tips for strong passwords\n• scams - how to spot scams\n• privacy - protecting your data\n• phishing - avoiding fake emails\n• malware - protecting from viruses\n• 2FA - two-factor authentication\n• VPN - virtual private networks\n• firewall - network protection\n\nJust type any keyword or ask a question!"
                }
            };
        }

        public string ProcessInput(string userInput)
        {
            string lowerInput = userInput.ToLower();
            conversationHistory.Add(userInput);

            // Help command
            if (lowerInput == "help" || lowerInput == "what can you do" || lowerInput == "menu")
            {
                return keywordResponses["help"][0];
            }

            // Name capture (first interaction)
            if (string.IsNullOrEmpty(UserName) && conversationHistory.Count <= 3 && !lowerInput.Contains("my name is"))
            {
                UserName = userInput;
                userMemory["name"] = UserName;
                return $"Nice to meet you, {UserName}! 🎉\n\nI'm your Cybersecurity Assistant. Try asking me about:\n• passwords 🔐\n• scams ⚠️\n• privacy 🔒\n• phishing 🎣\n• malware 🦠\n• 2FA 🔑\n• VPN 🌐\n• firewall 🔥\n\nType 'help' for more options!";
            }

            // "My name is" capture
            if (lowerInput.Contains("my name is"))
            {
                string[] parts = userInput.Split(new[] { "my name is" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    UserName = parts[1].Trim();
                    userMemory["name"] = UserName;
                    return $"Hello {UserName}! Great to meet you properly! 🎉\n\nWhat cybersecurity topic would you like to learn about today?";
                }
            }

            // Sentiment detection
            string sentiment = DetectSentiment(lowerInput);
            if (!string.IsNullOrEmpty(sentiment))
            {
                string empathyResponse = GetEmpathyResponse(sentiment);
                string tip = GetCybersecurityTip(lastTopic);
                return $"{empathyResponse}\n\n{tip}";
            }

            // Follow-up detection
            if (IsFollowUpRequest(lowerInput) && !string.IsNullOrEmpty(lastTopic))
            {
                string additionalTip = GetRandomResponseForTopic(lastTopic);
                return $"Sure! Here's another tip about {lastTopic.ToUpper()}:\n\n{additionalTip}\n\n💡 Would you like to know more about this or try another topic?";
            }

            // Memory recall
            if (lowerInput.Contains("remember") || lowerInput.Contains("recall") || lowerInput.Contains("what do you know about me"))
            {
                return RecallUserInfo();
            }

            // Keyword detection
            string response = CheckKeywords(lowerInput);
            if (response != null)
            {
                lastTopic = GetTopicFromInput(lowerInput);
                CurrentTopic = lastTopic;
                userMemory["last_topic"] = lastTopic;
                return response;
            }

            // Default response
            return GetDefaultResponse();
        }

        private string DetectSentiment(string input)
        {
            foreach (var sentiment in sentimentKeywords)
            {
                if (sentiment.Value.Any(keyword => input.Contains(keyword)))
                {
                    return sentiment.Key;
                }
            }
            return null;
        }

        private string GetEmpathyResponse(string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    return "😟 I understand your concern. Cybersecurity can feel overwhelming, but let me help you stay safe step by step.";
                case "curious":
                    return "🤔 That's a great question! It's wonderful that you want to learn about cybersecurity.";
                case "frustrated":
                    return "😤 I hear your frustration. Let me simplify this for you with some clear, actionable advice.";
                case "grateful":
                    return "🙏 You're very welcome! I'm glad I could help. Here's something else you might find useful:";
                case "happy":
                    return "😊 That's great to hear! Keeping positive helps with learning cybersecurity!";
                case "scared":
                    return "😨 I understand being scared about security. Let me give you simple steps to feel safer right now.";
                default:
                    return "💡 Thanks for sharing. Here's something that might help protect you online:";
            }
        }

        private string GetCybersecurityTip(string topic)
        {
            if (!string.IsNullOrEmpty(topic) && keywordResponses.ContainsKey(topic))
            {
                return keywordResponses[topic][random.Next(keywordResponses[topic].Count)];
            }
            return keywordResponses["password"][random.Next(keywordResponses["password"].Count)];
        }

        private bool IsFollowUpRequest(string input)
        {
            string[] followPhrases = {
                "another", "more", "tell me more", "explain more",
                "elaborate", "continue", "next tip", "additional",
                "what else", "go on", "keep going", "another tip"
            };
            return followPhrases.Any(phrase => input.Contains(phrase));
        }

        private string GetRandomResponseForTopic(string topic)
        {
            if (keywordResponses.ContainsKey(topic))
            {
                return keywordResponses[topic][random.Next(keywordResponses[topic].Count)];
            }
            return "Try asking about passwords, scams, privacy, phishing, malware, 2FA, VPN, or firewalls!";
        }

        private string CheckKeywords(string input)
        {
            foreach (var keyword in keywordResponses.Keys)
            {
                if (input.Contains(keyword))
                {
                    var responses = keywordResponses[keyword];
                    string response = responses[random.Next(responses.Count)];

                    if (keyword != "help")
                    {
                        response += "\n\n💡 Would you like another tip? Just say 'another tip' or 'tell me more'!";
                    }
                    return response;
                }
            }
            return null;
        }

        private string GetTopicFromInput(string input)
        {
            foreach (var keyword in keywordResponses.Keys)
            {
                if (input.Contains(keyword))
                    return keyword;
            }
            return "cybersecurity";
        }

        private string GetDefaultResponse()
        {
            string[] defaultResponses =
            {
                "🤔 I'm not sure I understand. Try asking about:\n• passwords\n• scams\n• privacy\n• phishing\n• malware\n• 2FA\n• VPN\n• firewall\n\nOr type 'help' for assistance!",
                "💭 I specialize in cybersecurity! Would you like tips about passwords, scams, privacy, phishing, or malware?",
                "🛡️ What would you like to know about - passwords, scams, privacy, phishing, malware, 2FA, VPN, or firewalls?\n\nType 'help' to see all options!"
            };
            return defaultResponses[random.Next(defaultResponses.Length)];
        }

        private string RecallUserInfo()
        {
            if (userMemory.ContainsKey("name") && userMemory.ContainsKey("last_topic"))
            {
                return $"📝 Of course! Your name is {userMemory["name"]}, and you're interested in {userMemory["last_topic"]}.\n\n" +
                       $"Based on that interest, remember this: {GetCybersecurityTip(userMemory["last_topic"])}";
            }
            else if (userMemory.ContainsKey("name"))
            {
                return $"📝 I remember! Your name is {userMemory["name"]}. What cybersecurity topic would you like to explore today?\n\n" +
                       $"Try: passwords, scams, privacy, phishing, malware, 2FA, VPN, or firewalls!";
            }
            return "I don't know much about you yet! Tell me your name and ask about a cybersecurity topic, and I'll remember!";
        }
    }
}